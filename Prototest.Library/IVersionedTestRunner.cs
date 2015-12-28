using System.Reflection;

namespace Prototest.Library
{
    internal interface IVersionedTestRunner
    {
        string Version { get; }

        bool Run(Assembly assembly, string[] args);
    }
}