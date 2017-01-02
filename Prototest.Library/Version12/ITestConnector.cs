using System;
using System.Reflection;

namespace Prototest.Library.Version12
{
    public interface ITestConnector
    {
        void InitTestClassesFound(int testClasses);

        void InitTestMethodsFound(int testMethods);

        void InitTestEntriesFound(int testEntries);

        void TestStarted(string setName, Type testClass, MethodInfo testMethod);

        void TestPassed(string setName, Type testClass, MethodInfo testMethod, int testsPassed);

        void TestFailed(string setName, Type testClass, MethodInfo testMethod, IConcurrentCollection<string> errors,
            Exception testFailure);

        void Summary(bool anyFail, int ran, int fail, int pass);

        void Details(bool anyFail, IConcurrentCollection<string> details);
    }
}
