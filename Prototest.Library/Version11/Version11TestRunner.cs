#if !PLATFORM_UNITY && !PLATFORM_PCL

using System.Reflection;

namespace Prototest.Library.Version11
{
    public class Version11TestRunner : IVersionedTestRunner
    {
        public string Version
        {
            get
            {
                return "11";
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

#if !PLATFORM_IOS && !PLATFORM_ANDROID
        private ITestConnector GetTestConnector()
        {
            return new ConsoleTestConnector();
        }
#elif PLATFORM_IOS
        private ITestConnector GetTestConnector()
        {
            return null;
        }
#elif PLATFORM_ANDROID
        private ITestConnector GetTestConnector()
        {
            return null;
        }
#endif
    }
}

#endif