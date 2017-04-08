#if !PLATFORM_UNITY

using System.Threading.Tasks;
using Prototest.Library.Version1;

namespace Prototest.Example
{
    public class MixedTestA
    {
        private readonly IAssert _assert;

        public MixedTestA(IAssert assert)
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

        public async Task TestDelayed()
        {
            await Task.Delay(1500);
        }

        public async Task TestYield()
        {
            await Task.Yield();
            await Task.Yield();
            await Task.Yield();
            await Task.Yield();
            await Task.Yield();
        }
    }
}

#endif