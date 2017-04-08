#if !PLATFORM_UNITY

using System.Collections.Generic;

namespace Prototest.Library.Version14
{
    public interface ITestSummaryConsumer
    {
        void HandleResults(List<TestResult> results);
    }
}

#endif