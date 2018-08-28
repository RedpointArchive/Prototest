#if !PLATFORM_UNITY && !PLATFORM_PCL

using Prototest.Library.Version12;
using System.Reflection;

namespace Prototest.Library.Version15
{
    public interface ITestRunner
    {
        bool Run(ITestConnector connector, TestExecutionSet testExecutionSet);
    }
}

#endif