using System;
using System.Collections.Generic;
using System.Diagnostics;
using Prototest.Library.Version11;

namespace Prototest.Example
{
    public class BasicAttachmentTestSummaryConsumer : ITestSummaryConsumer
    {
        public void HandleResults(List<TestResult> results)
        {
            foreach (var result in results)
            {
                foreach (var attachment in result.Attachments)
                {
#if PLATFORM_PCL
                    Debug.WriteLine("attachment " + result.Entry.TestClass.FullName + ":" + result.Entry.TestMethod.Name + " has " + attachment.Key + " = " + attachment.Value);
#else
                    Console.WriteLine("attachment " + result.Entry.TestClass.FullName + ":" + result.Entry.TestMethod.Name + " has " + attachment.Key + " = " + attachment.Value);
#endif
                }
            }
        }
    }
}
