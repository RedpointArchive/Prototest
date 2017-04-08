#if !PLATFORM_UNITY

using System.Threading.Tasks;
using Prototest.Library.Version1;

namespace Prototest.Example
{
    public class AsyncTestA
    {
        private readonly IAssert _assert;

        public AsyncTestA(IAssert assert)
        {
            _assert = assert;
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