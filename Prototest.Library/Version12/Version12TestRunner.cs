using System.Reflection;

namespace Prototest.Library.Version12
{
    public class Version12TestRunner : IVersionedTestRunner
    {
        public string Version
        {
            get
            {
                return "12";
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
