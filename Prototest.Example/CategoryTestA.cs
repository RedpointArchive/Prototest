using Prototest.Library.Version1;

namespace Prototest.Example
{
    public class CategoryTestA
    {
        private readonly IAssert _assert;

        public CategoryTestA(IAssert assert, ICategorize categorize)
        {
            _assert = assert;

            categorize.Method("Functional", TestMethodA);
            categorize.Method("Functional", TestMethodB);
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