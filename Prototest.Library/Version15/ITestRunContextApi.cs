using System;
using System.Collections.Generic;
using System.Text;

namespace Prototest.Library.Version15
{
    public interface ITestRunContextApi
    {
        bool IsVsHostedTestRun { get; }

        int RunProcess(string filePath, string workingDirectory, string arguments, IDictionary<string, string> environmentVariables);

        void LogMessage(string message);

        string SolutionDirectory { get; }

        string TestRunDirectory { get; }
    }
}
