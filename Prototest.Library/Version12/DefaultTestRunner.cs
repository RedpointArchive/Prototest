using System;
#if !PLATFORM_UNITY
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
#if !PLATFORM_IOS && !PLATFORM_ANDROID && !PLATFORM_UNITY
using NDesk.Options;
#endif
using Prototest.Library.Version1;

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

            var assertTypes = new Dictionary<Type, object>
            {
                {typeof (IAssert), new Assert()},
                {typeof(ICategorize), new Version11.Categorize(categories) }
            };

            var testClasses = new List<Version11.TestInputEntry>();

            var types = assembly.GetTypes();
            foreach (var type in types)
            {
                if (type.IsAbstract || type.IsInterface)
                {
                    continue;
                }

                var constructors = type.GetConstructors();
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

                    testClass.TestMethods =
                        type.GetMethods(BindingFlags.Instance | BindingFlags.Public | BindingFlags.DeclaredOnly)
                            .ToList();

                    testClasses.Add(testClass);
                }
            }

            connector.InitTestClassesFound(testClasses.Count);
            connector.InitTestMethodsFound(testClasses.Sum(x => x.TestMethods.Count));

            var lockObject = new object();
            var ran = 0;
            var pass = 0;
            var fail = 0;
#if !PLATFORM_UNITY
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
				setTypes = types.Concat(typeof(DefaultTestRunner).Assembly.GetTypes());
            }

            var sets = new List<Version11.TestSet>();
            foreach (
                var type in
                    setTypes
                        .Where(x => typeof(Version11.ITestSetProvider).IsAssignableFrom(x) && !x.IsInterface && !x.IsAbstract))
            {
                var provider = (Version11.ITestSetProvider)Activator.CreateInstance(type);
                sets.AddRange(provider.GetTestSets(testClasses, assertTypes));
            }

            var summaryConsumers = new List<Version11.ITestSummaryConsumer>();
            foreach (
                var type in
                    types.Where(
                        x => typeof(Version11.ITestSummaryConsumer).IsAssignableFrom(x) && !x.IsInterface && !x.IsAbstract))
            {
                summaryConsumers.Add((Version11.ITestSummaryConsumer)Activator.CreateInstance(type));
            }

            connector.InitTestEntriesFound(sets.Sum(x => x.Entries.Count));

            foreach (var set in sets)
            {
                var tasks = set.Entries
                    .Select(x => new Action(
                        () =>
                        {
                            var obj = x.TestConstructor.Invoke(
                                x.TestConstructor.GetParameters()
                                    .Select(z => assertTypes[z.ParameterType])
                                    .ToArray());
                            connector.TestStarted(set.Name, x.TestClass, x.TestMethod);
                            lock (lockObject) ran++;
                            if (Debugger.IsAttached && !x.AllowFail)
                            {
                                x.RunTestMethod(obj);
                                lock (lockObject) pass++;
                                connector.TestPassed(set.Name, x.TestClass, x.TestMethod, pass);
                                results.Add(new Version11.TestResult
                                {
                                    Set = set,
                                    Entry = x,
                                    Exception = null,
                                    Passed = true
                                });
                            }
                            else
                            {
                                try
                                {
                                    x.RunTestMethod(obj);
                                    lock (lockObject) pass++;
                                    connector.TestPassed(set.Name, x.TestClass, x.TestMethod, pass);
                                    results.Add(new Version11.TestResult
                                    {
                                        Set = set,
                                        Entry = x,
                                        Exception = null,
                                        Passed = true
                                    });
                                }
                                catch (Exception ex)
                                {
                                    lock (lockObject) fail++;
                                    connector.TestFailed(set.Name, x.TestClass, x.TestMethod, bag, ex);
                                    results.Add(new Version11.TestResult
                                    {
                                        Set = set,
                                        Entry = x,
                                        Exception = ex,
                                        Passed = false
                                    });
                                    if (!x.AllowFail)
                                    {
                                        anyFail = true;
                                    }
                                }
                            }
                        })).ToArray();

#if !PLATFORM_UNITY
                if (Debugger.IsAttached)
                {
                    // Run in single thread when a debugger is attached to make
                    // diagnosing issues easier.
#endif
                    foreach (var task in tasks)
                    {
                        task();
                    }
#if !PLATFORM_UNITY
                }
                else
                {
                    Parallel.Invoke(tasks);
                }
#endif
            }

            connector.Summary(anyFail, ran, fail, pass);
            connector.Details(anyFail, bag);

#if !PLATFORM_UNITY
            var resultsList = results.ToList();
#endif
            foreach (var consumer in summaryConsumers)
            {
                consumer.HandleResults(resultsList);
            }

            return !anyFail;
        }
    }
}