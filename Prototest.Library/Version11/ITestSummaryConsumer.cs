using System.Collections.Generic;

namespace Prototest.Library.Version11
{
    public interface ITestSummaryConsumer
    {
        void HandleResults(List<TestResult> results);
    }
}
