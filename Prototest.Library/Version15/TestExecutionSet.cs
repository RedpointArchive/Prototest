#if !PLATFORM_UNITY && !PLATFORM_PCL

using Prototest.Library.Version1;
using Prototest.Library.Version13;
using System;
using System.Collections.Generic;

namespace Prototest.Library.Version15
{
    public class TestExecutionSet
    {
        public TestSettings Settings { get; set; }

        public List<Version11.TestSet> Version11Sets { get; set; }
        
        public List<Version14.TestSet> Version14Sets { get; set; }

        public IAssert Assert { get; set; }

        public IThreadControl ThreadControl { get; set; }

        public Version1.ICategorize Categorize { get; set; }

        public Dictionary<Type, Func<object>> AssertTypes { get; set; }

        public Type[] AllTypes { get; set; }

        public List<Type> TestRunContextTypes { get; set; }

        public ITestRunContextApi TestRunContextApi { get; set; }
    }
}

#endif