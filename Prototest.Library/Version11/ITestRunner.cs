#if !PLATFORM_UNITY && !PLATFORM_PCL

using System.Reflection;

namespace Prototest.Library.Version11
{
    public interface ITestRunner
    {
        bool Run(
            Assembly assembly,
            ITestConnector connector,
            string[] args);
    }
}

#endif