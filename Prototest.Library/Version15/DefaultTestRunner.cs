#if !PLATFORM_UNITY && !PLATFORM_PCL

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Prototest.Library.Version12;
using Prototest.Library.Version13;

namespace Prototest.Library.Version15
{
    public class DefaultTestRunner : ITestRunner
    {
        public bool Run(ITestConnector connector, TestExecutionSet testExecutionSet)
        {
            var ran = 0;
            var pass = 0;
            var fail = 0;
            var anyFail = false;

            var lockObject = new object();
            var bagConcurrent = new ConcurrentBag<string>();
            var resultsConcurrent11 = new ConcurrentBag<Version11.TestResult>();
            var resultsConcurrent14 = new ConcurrentBag<Version14.TestResult>();
            var bag = new Net45ConcurrentCollection<string>(bagConcurrent);
            var results11 = new Net45ConcurrentCollection<Version11.TestResult>(resultsConcurrent11);
            var results14 = new Net45ConcurrentCollection<Version14.TestResult>(resultsConcurrent14);
            var contexts = new List<IDisposable>();

            try
            {
                foreach (var contextType in testExecutionSet.TestRunContextTypes)
                {
                    var contextProvider = (ITestRunContext)Activator.CreateInstance(contextType);
                    contexts.Add(contextProvider.AcquireContext(testExecutionSet.TestRunContextApi));
                }

                var summaryConsumers11 = new List<Version11.ITestSummaryConsumer>();
                var summaryConsumers14 = new List<Version14.ITestSummaryConsumer>();
                foreach (
                    var type in
                        testExecutionSet.AllTypes.Where(
                            x => GetTypeInfo(typeof(Version11.ITestSummaryConsumer)).IsAssignableFrom(GetTypeInfo(x)) && !GetTypeInfo(x).IsInterface && !GetTypeInfo(x).IsAbstract))
                {
                    summaryConsumers11.Add((Version11.ITestSummaryConsumer)Activator.CreateInstance(type));
                }
                foreach (
                    var type in
                        testExecutionSet.AllTypes.Where(
                            x => GetTypeInfo(typeof(Version14.ITestSummaryConsumer)).IsAssignableFrom(GetTypeInfo(x)) && !GetTypeInfo(x).IsInterface && !GetTypeInfo(x).IsAbstract))
                {
                    summaryConsumers14.Add((Version14.ITestSummaryConsumer)Activator.CreateInstance(type));
                }

                foreach (var set in testExecutionSet.Version11Sets)
                {
                    var tasks = set.Entries
                        .Select(x => new Action(
                            () =>
                            {
                                var constructorParameters = x.TestConstructor.GetParameters()
                                    .Select(z => testExecutionSet.AssertTypes[z.ParameterType]())
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
                                    results11.Add(new Version11.TestResult
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
                                        results11.Add(new Version11.TestResult
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
                                        results11.Add(new Version11.TestResult
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

                    if (Debugger.IsAttached || set.RunSingleThreadedOnMainThread)
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

                foreach (var set in testExecutionSet.Version14Sets)
                {
                    var tasks = set.Entries
                        .Select(x => new Func<Task>(
                            async () =>
                            {
                                var constructorParameters = x.TestConstructor.GetParameters()
                                    .Select(z => testExecutionSet.AssertTypes[z.ParameterType]())
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
                                    await x.RunTestMethod(obj);
                                    lock (lockObject) pass++;
                                    if (connector != null)
                                    {
                                        connector.TestPassed(set.Name, x.TestClass, x.TestMethod, pass);
                                    }
                                    results14.Add(new Version14.TestResult
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
                                        await x.RunTestMethod(obj);
                                        lock (lockObject) pass++;
                                        if (connector != null)
                                        {
                                            connector.TestPassed(set.Name, x.TestClass, x.TestMethod, pass);
                                        }
                                        results14.Add(new Version14.TestResult
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
                                        results14.Add(new Version14.TestResult
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

                    if (Debugger.IsAttached || set.RunSingleThreadedOnMainThread)
                    {
                        // Run in single thread when a debugger is attached to make
                        // diagnosing issues easier.
                        foreach (var task in tasks)
                        {
                            Task.WaitAll(task());
                        }
                    }
                    else
                    {
                        Task.WaitAll(tasks.Select(x => x()).ToArray());
                    }
                }

                if (connector != null)
                {
                    connector.Summary(anyFail, ran, fail, pass);
                    connector.Details(anyFail, bag);
                }

                var resultsList11 = results11.ToList();
                var resultsList14 = results14.ToList();
                foreach (var consumer in summaryConsumers11)
                {
                    consumer.HandleResults(resultsList11);
                }
                foreach (var consumer in summaryConsumers14)
                {
                    consumer.HandleResults(resultsList14);
                }

                return !anyFail;
            }
            finally
            {
                foreach (var context in contexts)
                {
                    try
                    {
                        context.Dispose();
                    }
                    catch
                    {
                        // Ignore exception from shutting down contexts.
                    }
                }
            }
        }
        
        private static Type GetTypeInfo(Type t)
        {
            return t;
        }
    }
}

#endif