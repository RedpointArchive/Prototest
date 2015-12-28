using System;
using System.Collections.Generic;
using System.Reflection;

namespace Prototest.Library.Version11
{
    public class TestInputEntry
    {
        public ConstructorInfo Constructor;

        public List<MethodInfo> TestMethods;

        public Type Type;
    }
}