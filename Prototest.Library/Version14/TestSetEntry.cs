#if !PLATFORM_UNITY

using System;
using System.Reflection;
using System.Threading.Tasks;

namespace Prototest.Library.Version14
{
    public class TestSetEntry
    {
        public Type TestClass { get; set; }

        public ConstructorInfo TestConstructor { get; set; }

        public MethodInfo TestMethod { get; set; }

        public Func<object, Task> RunTestMethod { get; set; }

        public bool AllowFail { get; set; }
    }
}

#endif