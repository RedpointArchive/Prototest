using System.Collections.Generic;

namespace Prototest.Library.Version11
{
    public interface ITestSetProvider
    {
        List<TestSet> GetTestSets(List<TestInputEntry> classes);
    }
}
