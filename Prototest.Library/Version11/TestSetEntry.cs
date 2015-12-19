using System;
using System.Reflection;

namespace Prototest.Library.Version11
{
    public class TestSetEntry
    {
        public Type TestClass { get; set; }

        public ConstructorInfo TestConstructor { get; set; }

        public MethodInfo TestMethod { get; set; }

        public Action<object> RunTestMethod { get; set; }

        public bool AllowFail { get; set; }
    }
}