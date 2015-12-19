using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Prototest.Library.Version1;
using Prototest.Library.Version11;

namespace Prototest.Library
{
    public static class Runner
    {
        public static bool Run(
            Assembly assembly,
            Action<int> runStateInitTestClassesFound, 
            Action<int> runStateInitTestMethodsFound, 
            Action<int> runStateInitTestEntriesFound, 
            Action<string, Type, MethodInfo> runStateStartTest, 
            Action<string, Type, MethodInfo, int> runStatePassTest, 
            Action<string, Type, MethodInfo, ConcurrentBag<string>, Exception> runStateFailTest,
            Action<bool, int, int, int> runStateSummary,
            Action<bool, ConcurrentBag<string>> runStateDetail,
            string[] args)
        {
            var assertTypes = new Dictionary<Type, Type>
            {
                {typeof (IAssert), typeof (Assert)}
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

            runStateInitTestClassesFound(testClasses.Count);
            runStateInitTestMethodsFound(testClasses.Sum(x => x.TestMethods.Count));

            var lockObject = new object();
            var ran = 0;
            var pass = 0;
            var fail = 0;
            var bag = new ConcurrentBag<string>();
            var results = new ConcurrentBag<TestResult>();
            var anyFail = false;

            IEnumerable<Type> setTypes = types;

            // TODO: Use a proper option parser, like the one in Protobuild.
            if (args.Length < 1 || args[0] != "--no-default-set")
            {
                setTypes = types.Concat(typeof (Runner).Assembly.GetTypes());
            }

            var sets = new List<TestSet>();
            foreach (
                var type in
                    setTypes
                        .Where(x => typeof (ITestSetProvider).IsAssignableFrom(x) && !x.IsInterface && !x.IsAbstract))
            {
                var provider = (ITestSetProvider) Activator.CreateInstance(type);
                sets.AddRange(provider.GetTestSets(testClasses));
            }

            var summaryConsumers = new List<ITestSummaryConsumer>();
            foreach (
                var type in
                    types.Where(
                        x => typeof (ITestSummaryConsumer).IsAssignableFrom(x) && !x.IsInterface && !x.IsAbstract))
            {
                summaryConsumers.Add((ITestSummaryConsumer)Activator.CreateInstance(type));
            }

            runStateInitTestEntriesFound(sets.Sum(x => x.Entries.Count));

            foreach (var set in sets)
            {
                var tasks = set.Entries
                    .Select(x => new Action(
                        () =>
                        {
                            var obj = x.TestConstructor.Invoke(
                                x.TestConstructor.GetParameters()
                                    .Select(z => Activator.CreateInstance(assertTypes[z.ParameterType]))
                                    .ToArray());
                            runStateStartTest(set.Name, x.TestClass, x.TestMethod);
                            lock (lockObject) ran++;
                            if (Debugger.IsAttached && !x.AllowFail && false)
                            {
                                x.RunTestMethod(obj);
                                lock (lockObject) pass++;
                                runStatePassTest(set.Name, x.TestClass, x.TestMethod, pass);
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
                                    runStatePassTest(set.Name, x.TestClass, x.TestMethod, pass);
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
                                    runStateFailTest(set.Name, x.TestClass, x.TestMethod, bag, ex);
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

            runStateSummary(anyFail, ran, fail, pass);
            runStateDetail(anyFail, bag);

            var resultsList = results.ToList();
            foreach (var consumer in summaryConsumers)
            {
                consumer.HandleResults(resultsList);
            }

            return !anyFail;
        }
        
    }
}
