using Microsoft.VisualStudio.TestPlatform.ObjectModel.Adapter;
using Prototest.Library.Version15;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace Prototest.TestAdapter
{
    public class VsTestRunContextApi : ITestRunContextApi
    {
        private readonly IFrameworkHandle _frameworkHandle;

        public VsTestRunContextApi(IFrameworkHandle frameworkHandle)
        {
            _frameworkHandle = frameworkHandle;
        }

        public bool IsVsHostedTestRun => true;
        
        public int RunProcess(string filePath, string workingDirectory, string arguments, IDictionary<string, string> environmentVariables)
        {
            try
            {
                try
                {
                    return _frameworkHandle.LaunchProcessWithDebuggerAttached(
                        filePath,
                        workingDirectory,
                        arguments,
                        environmentVariables);
                }
                catch (InvalidOperationException)
                {
                    var startInfo = new ProcessStartInfo
                    {
                        FileName = filePath,
                        WorkingDirectory = workingDirectory,
                        Arguments = arguments,
                        CreateNoWindow = true,
                        UseShellExecute = false,
                    };
                    foreach (var kv in environmentVariables)
                    {
                        startInfo.Environment.Add(kv);
                    }
                    var process = Process.Start(startInfo);
                    return process.Id;
                }
            }
            catch (Exception ex)
            {
                _frameworkHandle.SendMessage(Microsoft.VisualStudio.TestPlatform.ObjectModel.Logging.TestMessageLevel.Error, ex.Message + Environment.NewLine + ex.StackTrace);
                throw;
            }
        }

        public void LogMessage(string message)
        {
            _frameworkHandle.SendMessage(Microsoft.VisualStudio.TestPlatform.ObjectModel.Logging.TestMessageLevel.Informational, message);
        }
    }
}
