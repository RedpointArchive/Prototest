#if !PLATFORM_UNITY

using System.Collections.Generic;

namespace Prototest.Library.Version14
{
    public class TestSet
    {
        public string Name { get; set; }

        public List<TestSetEntry> Entries { get; set; }

        public bool RunSingleThreadedOnMainThread { get; set; }
    }
}

#endif