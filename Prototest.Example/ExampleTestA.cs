using System.Threading;
using Prototest.Library.Version1;

namespace Prototest.Example
{
    public class ExampleTestA
    {
        private readonly IAssert _assert;

        public ExampleTestA(IAssert assert)
        {
            _assert = assert;
        }

        public void Test1()
        {
            _assert.True(true);
            _assert.False(false);
        }

        public void Test2()
        {
            _assert.Equal(5, 5);
            _assert.Equal("hello", "hello");
        }

        public void SleepyTest()
        {
            Thread.Sleep(1500);
            _assert.NotEqual("apples", "oranges");
        }
    }
}