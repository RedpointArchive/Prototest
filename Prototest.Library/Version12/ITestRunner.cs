using System.Reflection;

namespace Prototest.Library.Version12
{
    public interface ITestRunner
    {
        bool Run(
            Assembly assembly,
            ITestConnector connector,
            string[] args);
    }
}