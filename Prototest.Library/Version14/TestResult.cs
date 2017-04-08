#if !PLATFORM_UNITY

using System;
using System.Collections.Generic;

namespace Prototest.Library.Version14
{
    public class TestResult
    {
        public TestSet Set { get; set; }

        public TestSetEntry Entry { get; set; }

        public bool Passed { get; set; }

        public Exception Exception { get; set; }
        
        public Dictionary<string, object> Attachments { get; set; }
    }
}

#endif