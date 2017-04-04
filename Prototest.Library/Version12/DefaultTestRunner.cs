using System;
#if !PLATFORM_UNITY && !PLATFORM_PCL
using System.Collections.Concurrent;
#endif
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
#if !PLATFORM_UNITY
using System.Threading.Tasks;
#endif
using Prototest.Library.Version1;
using Prototest.Library.Version13;

namespace Prototest.Library.Version12
{
    public class DefaultTestRunner : ITestRunner
    {
        public bool Run(Assembly assembly, ITestConnector connector, string[] args)
        {
            var noDefaultSet = false;
            var categories = new List<string>();

#if !PLATFORM_IOS && !PLATFORM_ANDROID && !PLATFORM_UNITY

            var options = new OptionSet
            {
                {"no-default-set", x => noDefaultSet = true},
                {"c|category=", x => categories.Add(x) }
            };

            options.Parse(args);

#endif

            var assert = new Assert();
            var threadControl = new ThreadControl();
            var categorize = new Version11.Categorize(categories);
            var assertTypes = new Dictionary<Type, Func<object>>
            {
                {typeof(IAssert), () => assert},
                {typeof(Version1.ICategorize), () => categorize},
                {typeof(Version13.ICategorize), () => categorize},
                {typeof(IThreadControl), () => threadControl},
                {typeof(ITestAttachment), () => new TestAttachment()}
            };

            var testClasses = new List<Version11.TestInputEntry>();

#if PLATFORM_PCL
            var types = assembly.ExportedTypes.ToList();
#else
            var types = assembly.GetTypes();
#endif
            foreach (var type in types)
            {
#if PLATFORM_PCL
                var typeInfo = type.GetTypeInfo();
#else
                var typeInfo = type;
#endif

                if (typeInfo.IsAbstract || typeInfo.IsInterface)
                {
                    continue;
                }

#if PLATFORM_PCL
                var constructors = typeInfo.DeclaredConstructors.Where(x => !x.IsStatic).ToArray();
#else
                var constructors = typeInfo.GetConstructors();
#endif
                if (constructors.Length != 1)
                {
                    continue;
                }

                var constructor = constructors.First();
                var parameters = constructor.GetParameters();
                if (parameters.Length > 0 && parameters.All(x => assertTypes.Keys.Contains(x.ParameterType)))
                {
                    var testClass = new Version11.TestInputEntry
                    {
                        Constructor = constructor,
                        TestMethods = new List<MethodInfo>(),
                        Type = type
                    };

#if PLATFORM_PCL
                    testClass.TestMethods = typeInfo.DeclaredMethods
                        .Where(x => x.IsPublic && !x.IsStatic)
                        .ToList();
#else
                    testClass.TestMethods =
                        type.GetMethods(BindingFlags.Instance | BindingFlags.Public | BindingFlags.DeclaredOnly)
                            .ToList();
#endif

                    testClasses.Add(testClass);
                }
            }

            if (connector != null)
            {
                connector.InitTestClassesFound(testClasses.Count);
                connector.InitTestMethodsFound(testClasses.Sum(x => x.TestMethods.Count));
            }

            var lockObject = new object();
            var ran = 0;
            var pass = 0;
            var fail = 0;
#if !PLATFORM_UNITY && !PLATFORM_PCL
            var bagConcurrent = new ConcurrentBag<string>();
            var resultsConcurrent = new ConcurrentBag<Version11.TestResult>();
            var bag = new Net45ConcurrentCollection<string>(bagConcurrent);
            var results = new Net45ConcurrentCollection<Version11.TestResult>(resultsConcurrent);
#else
            var bagList = new List<string>();
            var resultsList = new List<Version11.TestResult>();
            var bag = new Net35ConcurrentCollection<string>(bagList);
            var results = new Net35ConcurrentCollection<Version11.TestResult>(resultsList);
#endif
            var anyFail = false;

            IEnumerable<Type> setTypes = types;

            if (!noDefaultSet)
            {
				setTypes = types.Concat(GetTypesFromAssembly(GetTypeInfo(typeof(DefaultTestRunner)).Assembly));
            }

            var sets = new List<Version11.TestSet>();
            foreach (
                var type in
                    setTypes
                        .Where(x => GetTypeInfo(typeof(Version11.ITestSetProvider)).IsAssignableFrom(GetTypeInfo(x)) && !GetTypeInfo(x).IsInterface && !GetTypeInfo(x).IsAbstract))
            {
                var provider = (Version11.ITestSetProvider)Activator.CreateInstance(type);
                sets.AddRange(provider.GetTestSets(testClasses, assertTypes.ToDictionary(k => k.Key, v => v.Value())));
            }
            foreach (
                var type in
                    setTypes
                        .Where(x => GetTypeInfo(typeof(Version13.ITestSetProvider)).IsAssignableFrom(GetTypeInfo(x)) && !GetTypeInfo(x).IsInterface && !GetTypeInfo(x).IsAbstract))
            {
                var provider = (Version13.ITestSetProvider)Activator.CreateInstance(type);
                sets.AddRange(provider.GetTestSets(testClasses, assertTypes));
            }

            var summaryConsumers = new List<Version11.ITestSummaryConsumer>();
            foreach (
                var type in
                    types.Where(
                        x => GetTypeInfo(typeof(Version11.ITestSummaryConsumer)).IsAssignableFrom(GetTypeInfo(x)) && !GetTypeInfo(x).IsInterface && !GetTypeInfo(x).IsAbstract))
            {
                summaryConsumers.Add((Version11.ITestSummaryConsumer)Activator.CreateInstance(type));
            }

            if (connector != null)
            {
                connector.InitTestEntriesFound(sets.Sum(x => x.Entries.Count));
            }

            foreach (var set in sets)
            {
                var tasks = set.Entries
                    .Select(x => new Action(
                        () =>
                        {
                            var constructorParameters = x.TestConstructor.GetParameters()
                                .Select(z => assertTypes[z.ParameterType]())
                                .ToArray();
                            var testAttachment = constructorParameters.OfType<ITestAttachment>().FirstOrDefault();

                            var obj = x.TestConstructor.Invoke(constructorParameters);
                            if (connector != null)
                            {
                                connector.TestStarted(set.Name, x.TestClass, x.TestMethod);
                            }
                            lock (lockObject) ran++;
                            if (Debugger.IsAttached && !x.AllowFail)
                            {
                                x.RunTestMethod(obj);
                                lock (lockObject) pass++;
                                if (connector != null)
                                {
                                    connector.TestPassed(set.Name, x.TestClass, x.TestMethod, pass);
                                }
                                results.Add(new Version11.TestResult
                                {
                                    Set = set,
                                    Entry = x,
                                    Exception = null,
                                    Passed = true,
                                    Attachments = testAttachment != null ? testAttachment.GetAttachments() : new Dictionary<string, object>()
                                });
                            }
                            else
                            {
                                try
                                {
                                    x.RunTestMethod(obj);
                                    lock (lockObject) pass++;
                                    if (connector != null)
                                    {
                                        connector.TestPassed(set.Name, x.TestClass, x.TestMethod, pass);
                                    }
                                    results.Add(new Version11.TestResult
                                    {
                                        Set = set,
                                        Entry = x,
                                        Exception = null,
                                        Passed = true,
                                        Attachments = testAttachment != null ? testAttachment.GetAttachments() : new Dictionary<string, object>()
                                    });
                                }
                                catch (Exception ex)
                                {
                                    lock (lockObject) fail++;
                                    if (connector != null)
                                    {
                                        connector.TestFailed(set.Name, x.TestClass, x.TestMethod, bag, ex);
                                    }
                                    results.Add(new Version11.TestResult
                                    {
                                        Set = set,
                                        Entry = x,
                                        Exception = ex,
                                        Passed = false,
                                        Attachments = testAttachment != null ? testAttachment.GetAttachments() : new Dictionary<string, object>()
                                    });
                                    if (!x.AllowFail)
                                    {
                                        anyFail = true;
                                    }
                                }
                            }
                        })).ToArray();

#if !PLATFORM_UNITY && !PLATFORM_PCL
                if (Debugger.IsAttached || set.RunSingleThreadedOnMainThread)
                {
                    // Run in single thread when a debugger is attached to make
                    // diagnosing issues easier.
#endif
                    foreach (var task in tasks)
                    {
                        task();
                    }
#if !PLATFORM_UNITY && !PLATFORM_PCL
                }
                else
                {
                    Parallel.Invoke(tasks);
                }
#endif
            }

            if (connector != null)
            {
                connector.Summary(anyFail, ran, fail, pass);
                connector.Details(anyFail, bag);
            }

#if !PLATFORM_UNITY && !PLATFORM_PCL
            var resultsList = results.ToList();
#endif
            foreach (var consumer in summaryConsumers)
            {
                consumer.HandleResults(resultsList);
            }

            return !anyFail;
        }

#if PLATFORM_PCL
        private static TypeInfo GetTypeInfo(Type t)
        {
            return t.GetTypeInfo();
        }

        private static Type[] GetTypesFromAssembly(Assembly a)
        {
            return a.DefinedTypes.Select(x => x.AsType()).ToArray();
        }
#else
        private static Type GetTypeInfo(Type t)
        {
            return t;
        }

        private static Type[] GetTypesFromAssembly(Assembly a)
        {
            return a.GetTypes().ToArray();
        }
#endif
    }
}