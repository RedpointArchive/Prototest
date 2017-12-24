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

        public async Task TestThrowsAsync1()
        {
            await _assert.ThrowsAsync(async () =>
            {
                throw new System.NotSupportedException();
            });
        }

        public async Task TestThrowsAsync2()
        {
            await _assert.ThrowsAsync<System.NotSupportedException>(async () =>
            {
                throw new System.NotSupportedException();
            });
        }

        public async Task TestThrowsAsync3()
        {
            await _assert.ThrowsAsync(() =>
            {
                throw new System.NotSupportedException();
            });
        }

        public async Task TestThrowsAsync4()
        {
            await _assert.ThrowsAsync<System.NotSupportedException>(() =>
            {
                throw new System.NotSupportedException();
            });
        }

        public async Task TestThrowsAsync6()
        {
            await _assert.DoesNotThrowAsync(async () =>
            {
                // Do nothing.
                var a = 1;
            });
        }

        public async Task TestThrowsAsync7()
        {
            await _assert.DoesNotThrowAsync<System.NotSupportedException>(async () =>
            {
                throw new System.NotImplementedException();
            });
        }
    }
}

#endif