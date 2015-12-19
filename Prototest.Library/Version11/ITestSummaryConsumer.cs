using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Prototest.Library.Version11
{
    public interface ITestSummaryConsumer
    {
        void HandleResults(List<TestResult> results);
    }
}
