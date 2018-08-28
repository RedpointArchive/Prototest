#if !PLATFORM_UNITY && !PLATFORM_PCL

using Prototest.Library.Version12;
using System.Reflection;
using System.Threading.Tasks;

namespace Prototest.Library.Version15
{
    public interface ITestDiscoverer
    {
        TestExecutionSet Discover(Assembly assembly, ITestConnector connector, TestSettings setting);
    }
}

#endif