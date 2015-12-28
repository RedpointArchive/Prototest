using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
#if !PLATFORM_IOS && !PLATFORM_ANDROID
using NDesk.Options;
#endif
using Prototest.Library.Version1;

namespace Prototest.Library.Version11
{
    public class DefaultTestRunner : ITestRunner
    {
        public bool Run(Assembly assembly, ITestConnector connector, string[] args)
        {
            var noDefaultSet = false;
            var categories = new List<string>();

#if !PLATFORM_IOS && !PLATFORM_ANDROID

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
                {typeof(ICategorize), new Categorize(categories) }
            };

            var testClasses = new List<TestInputEntry>();

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
                    var testClass = new TestInputEntry
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
            var bag = new ConcurrentBag<string>();
            var results = new ConcurrentBag<TestResult>();
            var anyFail = false;

            IEnumerable<Type> setTypes = types;

            if (!noDefaultSet)
            {
				setTypes = types.Concat(typeof(DefaultTestRunner).Assembly.GetTypes());
            }

            var sets = new List<TestSet>();
            foreach (
                var type in
                    setTypes
                        .Where(x => typeof(ITestSetProvider).IsAssignableFrom(x) && !x.IsInterface && !x.IsAbstract))
            {
                var provider = (ITestSetProvider)Activator.CreateInstance(type);
                sets.AddRange(provider.GetTestSets(testClasses, assertTypes));
            }

            var summaryConsumers = new List<ITestSummaryConsumer>();
            foreach (
                var type in
                    types.Where(
                        x => typeof(ITestSummaryConsumer).IsAssignableFrom(x) && !x.IsInterface && !x.IsAbstract))
            {
                summaryConsumers.Add((ITestSummaryConsumer)Activator.CreateInstance(type));
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
                                results.Add(new TestResult
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
                                    results.Add(new TestResult
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
                                    results.Add(new TestResult
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

                if (Debugger.IsAttached)
                {
                    // Run in single thread when a debugger is attached to make
                    // diagnosing issues easier.
                    foreach (var task in tasks)
                    {
                        task();
                    }
                }
                else
                {
                    Parallel.Invoke(tasks);
                }
            }

            connector.Summary(anyFail, ran, fail, pass);
            connector.Details(anyFail, bag);

            var resultsList = results.ToList();
            foreach (var consumer in summaryConsumers)
            {
                consumer.HandleResults(resultsList);
            }

            return !anyFail;
        }
    }
}
