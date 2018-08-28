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
        private readonly IRunContext _runContext;
        private readonly IFrameworkHandle _frameworkHandle;

        public VsTestRunContextApi(IRunContext runContext, IFrameworkHandle frameworkHandle)
        {
            _runContext = runContext;
            _frameworkHandle = frameworkHandle;
        }

        public bool IsVsHostedTestRun => true;

        public string SolutionDirectory => _runContext.SolutionDirectory;

        public string TestRunDirectory => _runContext.TestRunDirectory;

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
                        RedirectStandardOutput = true,
                        RedirectStandardError = true,
                    };
                    foreach (var kv in environmentVariables)
                    {
                        startInfo.Environment.Add(kv);
                    }
                    var process = Process.Start(startInfo);
                    process.EnableRaisingEvents = true;
                    process.OutputDataReceived += (sender, e) =>
                    {
                        var data = e.Data.TrimEnd();
                        if (!string.IsNullOrEmpty(data))
                        {
                            _frameworkHandle.SendMessage(Microsoft.VisualStudio.TestPlatform.ObjectModel.Logging.TestMessageLevel.Informational, data);
                        }
                    };
                    process.ErrorDataReceived += (sender, e) =>
                    {
                        var data = e.Data.TrimEnd();
                        if (!string.IsNullOrEmpty(data))
                        {
                            _frameworkHandle.SendMessage(Microsoft.VisualStudio.TestPlatform.ObjectModel.Logging.TestMessageLevel.Warning, data);
                        }
                    };
                    process.BeginOutputReadLine();
                    process.BeginErrorReadLine();
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
