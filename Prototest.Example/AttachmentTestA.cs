using Prototest.Library.Version1;
using Prototest.Library.Version13;

namespace Prototest.Example
{
    public class AttachmentTestA
    {
        private readonly IAssert _assert;
        private readonly ITestAttachment _testAttachment;

        public AttachmentTestA(IAssert assert, ITestAttachment testAttachment)
        {
            _assert = assert;
            _testAttachment = testAttachment;
        }

        public void TestMethod()
        {
            _testAttachment.Attach("MY_KEY", "hello world!");
            _assert.True(true);
        }
    }
}