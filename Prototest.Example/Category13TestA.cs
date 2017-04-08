using Prototest.Library.Version1;

namespace Prototest.Example
{
    public class Category13TestA
    {
        private readonly IAssert _assert;

        public Category13TestA(IAssert assert, Prototest.Library.Version13.ICategorize categorize)
        {
            _assert = assert;

            categorize.Method<Category13TestA>("Functional", x => x.TestMethodA());
            categorize.Method<Category13TestA>("Functional", x => x.TestMethodB());
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