using Microsoft.VisualStudio.TestPlatform.ObjectModel;
using Microsoft.VisualStudio.TestPlatform.ObjectModel.Adapter;
using Microsoft.VisualStudio.TestPlatform.ObjectModel.Logging;
using Prototest.Library.Version11;
using Prototest.Library.Version12;
using Prototest.Library.Version14;
using Prototest.Library.Version15;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using DefaultTestRunner = Prototest.Library.Version15.DefaultTestRunner;

namespace Prototest.TestAdapter
{
    [FileExtension(".dll")]
    [DefaultExecutorUri("executor://prototesttestexecutor/")]
    [ExtensionUri("executor://prototesttestexecutor/")]
    [Category("managed")]
    public class PrototestTestDiscoverer : Microsoft.VisualStudio.TestPlatform.ObjectModel.Adapter.ITestDiscoverer, ITestExecutor
    {
        public void Cancel()
        {
            Debug.WriteLine("Cancel");
        }

        private List<TestCase> DiscoverTests(IEnumerable<string> sources)
        {
            var discoverer = new DefaultTestDiscoverer();
            var results = new List<TestCase>();

            foreach (var source in sources)
            {
                var discoveredResults = discoverer.Discover(
                    Assembly.LoadFile(source),
                    null,
                    new TestSettings());
                foreach (var set in discoveredResults.Version11Sets)
                {
                    foreach (var entry in set.Entries)
                    {
                        results.Add(new TestCase(set.Name + ":" + entry.TestClass.FullName + "." + entry.TestMethod.Name, new Uri("executor://prototesttestexecutor/"), source)
                        {
                            LocalExtensionData = new TestMetadata
                            {
                                Version11Set = set,
                                TestExecutionSet = discoveredResults,
                                SetName = set.Name,
                                TestClass = entry.TestClass,
                                TestMethod = entry.TestMethod,
                            },
                        });
                    }
                }
                foreach (var set in discoveredResults.Version14Sets)
                {
                    foreach (var entry in set.Entries)
                    {
                        results.Add(new TestCase(set.Name + ":" + entry.TestClass.FullName + "." + entry.TestMethod.Name, new Uri("executor://prototesttestexecutor/"), source)
                        {
                            LocalExtensionData = new TestMetadata
                            {
                                Version14Set = set,
                                TestExecutionSet = discoveredResults,
                                SetName = set.Name,
                                TestClass = entry.TestClass,
                                TestMethod = entry.TestMethod,
                            },
                        });
                    }
                }
            }

            return results;
        }

        public void DiscoverTests(IEnumerable<string> sources, IDiscoveryContext discoveryContext, IMessageLogger logger, ITestCaseDiscoverySink discoverySink)
        {
            var results = DiscoverTests(sources);
            foreach (var result in results)
            {
                discoverySink.SendTestCase(result);
            }
        }

        public void RunTests(IEnumerable<string> sources, IRunContext runContext, IFrameworkHandle frameworkHandle)
        {
            var results = DiscoverTests(sources);
            RunTests(results, runContext, frameworkHandle);
        }
        
        public void RunTests(IEnumerable<TestCase> tests, IRunContext runContext, IFrameworkHandle frameworkHandle)
        {
            var runner = new DefaultTestRunner();
            var testCases = tests.ToList();

            // Are there any test cases which don't have local extension data? If so, we need to re-run discovery and
            // attach local extension data based on ID.
            if (testCases.Any(x => x.LocalExtensionData == null))
            {
                var discoveredTests = DiscoverTests(testCases.Select(x => x.Source).Distinct());
                var extensionDataById = discoveredTests.ToDictionary(k => k.Id, v => v.LocalExtensionData);
                foreach (var testCaseWithNullData in testCases.Where(x => x.LocalExtensionData == null))
                {
                    testCaseWithNullData.LocalExtensionData = extensionDataById[testCaseWithNullData.Id];
                }
            }

            var connector = new TestConnector(testCases, runContext, frameworkHandle);
            
            var groupedTestCases = testCases.GroupBy(x => ((TestMetadata)x.LocalExtensionData).TestExecutionSet);
            foreach (var groupedTestCase in groupedTestCases)
            {
                runner.Run(connector, new TestExecutionSet
                {
                    AllTypes = groupedTestCase.Key.AllTypes,
                    AssertTypes = groupedTestCase.Key.AssertTypes,
                    Assert = groupedTestCase.Key.Assert,
                    Categorize = groupedTestCase.Key.Categorize,
                    Settings = groupedTestCase.Key.Settings,
                    ThreadControl = groupedTestCase.Key.ThreadControl,
                    Version11Sets = groupedTestCase.Select(x => ((TestMetadata)x.LocalExtensionData).Version11Set).Where(x => x != null).Distinct().ToList(),
                    Version14Sets = groupedTestCase.Select(x => ((TestMetadata)x.LocalExtensionData).Version14Set).Where(x => x != null).Distinct().ToList(),
                    TestRunContextTypes = groupedTestCase.Key.TestRunContextTypes,
                    TestRunContextApi = new VsTestRunContextApi(runContext, frameworkHandle),
                });
            }
        }

        private class TestMetadata
        {
            public string SetName { get; set; }

            public Type TestClass { get; set; }

            public MethodInfo TestMethod { get; set; }

            public Library.Version11.TestSet Version11Set { get; set; }

            public Library.Version14.TestSet Version14Set { get; set; }

            public TestExecutionSet TestExecutionSet { get; set; }
        }

        private class TestConnector : Library.Version12.ITestConnector
        {
            private readonly List<TestCase> _testCases;
            private readonly IRunContext _runContext;
            private readonly IFrameworkHandle _frameworkHandle;

            public TestConnector(List<TestCase> testCases, IRunContext runContext, IFrameworkHandle frameworkHandle)
            {
                _testCases = testCases;
                _runContext = runContext;
                _frameworkHandle = frameworkHandle;
            }

            public void Details(bool anyFail, IConcurrentCollection<string> details)
            {
            }

            public void InitTestClassesFound(int testClasses)
            {
            }

            public void InitTestEntriesFound(int testEntries)
            {
            }

            public void InitTestMethodsFound(int testMethods)
            {
            }

            public void Summary(bool anyFail, int ran, int fail, int pass)
            {
            }

            public void TestFailed(string setName, Type testClass, MethodInfo testMethod, IConcurrentCollection<string> errors, Exception testFailure)
            {
                foreach (var testCase in _testCases)
                {
                    var metadata = (TestMetadata)testCase.LocalExtensionData;
                    if (metadata.SetName == setName && metadata.TestClass == testClass && metadata.TestMethod == testMethod)
                    {
                        _frameworkHandle.RecordResult(new Microsoft.VisualStudio.TestPlatform.ObjectModel.TestResult(testCase)
                        {
                            Outcome = TestOutcome.Failed,
                            ErrorMessage = testFailure?.Message,
                            ErrorStackTrace = testFailure?.StackTrace
                        });
                    }
                }
            }

            public void TestPassed(string setName, Type testClass, MethodInfo testMethod, int testsPassed)
            {
                foreach (var testCase in _testCases)
                {
                    var metadata = (TestMetadata)testCase.LocalExtensionData;
                    if (metadata.SetName == setName && metadata.TestClass == testClass && metadata.TestMethod == testMethod)
                    {
                        _frameworkHandle.RecordResult(new Microsoft.VisualStudio.TestPlatform.ObjectModel.TestResult(testCase)
                        {
                            Outcome = TestOutcome.Passed,
                        });
                    }
                }
            }

            public void TestStarted(string setName, Type testClass, MethodInfo testMethod)
            {
                foreach (var testCase in _testCases)
                {
                    var metadata = (TestMetadata)testCase.LocalExtensionData;
                    if (metadata.SetName == setName && metadata.TestClass == testClass && metadata.TestMethod == testMethod)
                    {
                        _frameworkHandle.RecordStart(testCase);
                    }
                }
            }
        }
    }
}
