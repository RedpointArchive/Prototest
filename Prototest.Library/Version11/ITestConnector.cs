#if !PLATFORM_UNITY && !PLATFORM_PCL

using System;
using System.Collections.Concurrent;
using System.Reflection;

namespace Prototest.Library.Version11
{
    public interface ITestConnector
    {
        void InitTestClassesFound(int testClasses);

        void InitTestMethodsFound(int testMethods);

        void InitTestEntriesFound(int testEntries);

        void TestStarted(string setName, Type testClass, MethodInfo testMethod);

        void TestPassed(string setName, Type testClass, MethodInfo testMethod, int testsPassed);

        void TestFailed(string setName, Type testClass, MethodInfo testMethod, ConcurrentBag<string> errors,
            Exception testFailure);

        void Summary(bool anyFail, int ran, int fail, int pass);

        void Details(bool anyFail, ConcurrentBag<string> details);
    }
}

#endif