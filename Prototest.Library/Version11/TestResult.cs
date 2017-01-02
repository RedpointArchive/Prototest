using System;

namespace Prototest.Library.Version11
{
    public class TestResult
    {
        public TestSet Set { get; set; }

        public TestSetEntry Entry { get; set; }

        public bool Passed { get; set; }

        public Exception Exception { get; set; }
    }
}
