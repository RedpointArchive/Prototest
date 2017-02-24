using Prototest.Library.Version1;
using Prototest.Library.Version13;

namespace Prototest.Example
{
    public class SingleThreadCategoryTestA
    {
        private readonly IAssert _assert;

        public SingleThreadCategoryTestA(IAssert assert, ICategorize categorize, IThreadControl threadControl)
        {
            _assert = assert;

            categorize.Method("Functional", TestMethodA);
            categorize.Method("Functional", TestMethodB);

            threadControl.RequireTestsToRunOnMainThread();
        }

        private void TestMethodB()
        {
            _assert.Equal(5, 5);
        }

        private void TestMethodA()
        {
            _assert.Equal(true, true);
        }
    }
}