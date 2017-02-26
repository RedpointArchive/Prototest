using System;
using System.Collections.Generic;
using Prototest.Library.Version11;

namespace Prototest.Library.Version13
{
    public interface ITestSetProvider
    {
        List<TestSet> GetTestSets(List<TestInputEntry> classes, Dictionary<Type, Func<object>> assertTypes);
    }
}
