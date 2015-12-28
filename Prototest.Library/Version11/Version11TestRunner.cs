using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Prototest.Library.Version11
{
    public class Version11TestRunner : IVersionedTestRunner
    {
        public string Version => "11";

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
#endif
    }
}
