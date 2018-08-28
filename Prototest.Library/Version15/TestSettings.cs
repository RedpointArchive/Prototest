#if !PLATFORM_UNITY && !PLATFORM_PCL

using System.Collections.Generic;

namespace Prototest.Library.Version15
{
    public class TestSettings
    {
        public bool NoDefaultSet { get; set; } = false;

        public List<string> Categories { get; set; } = new List<string>();
    }
}

#endif