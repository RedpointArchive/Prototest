using Prototest.Library.Version15;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;

namespace Prototest.Example
{
    public class TestTestRunContext : ITestRunContext
    {
        public IDisposable AcquireContext(ITestRunContextApi contextApi)
        {
            contextApi.LogMessage("Test context spinning up...");

            var pid = contextApi.RunProcess(
                Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.System), "cmd.exe"),
                Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),
                "",
                new Dictionary<string, string>());

            return new TeardownDisposable(contextApi, pid);
        }

        public class TeardownDisposable : IDisposable
        {
            private readonly ITestRunContextApi _contextApi;
            private readonly int _pid;

            public TeardownDisposable(ITestRunContextApi contextApi, int pid)
            {
                _contextApi = contextApi;
                _pid = pid;
            }

            public void Dispose()
            {
                _contextApi.LogMessage("Test context tearing down...");
                Process.GetProcessById(_pid).Kill();
            }
        }
    }
}
