using System.Reflection;
using Prototest.Library.Version12;

namespace Prototest.Library.Version14
{
    public class Version14TestRunner : IVersionedTestRunner
    {
        public string Version
        {
            get
            {
                return "14";
            }
        }

        public bool Run(Assembly assembly, string[] args)
        {
            ITestRunner runner = new DefaultTestRunner();
            return runner.Run(
                assembly,
                GetTestConnector(),
                args);
        }

#if PLATFORM_IOS
        private ITestConnector GetTestConnector()
        {
            return null;
        }
#elif PLATFORM_ANDROID
        private ITestConnector GetTestConnector()
        {
            return null;
        }
#else
        private ITestConnector GetTestConnector()
        {
            return new ConsoleTestConnector();
        }
#endif
    }
}
