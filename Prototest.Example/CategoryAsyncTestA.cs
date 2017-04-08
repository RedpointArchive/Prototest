#if !PLATFORM_UNITY

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Prototest.Library.Version1;

namespace Prototest.Example
{
    public class CategoryAsyncTestA
    {
        private readonly IAssert _assert;

        public CategoryAsyncTestA(IAssert assert, Prototest.Library.Version14.ICategorize categorize)
        {
            _assert = assert;

            categorize.MethodAsync<CategoryAsyncTestA>("Functional", x => x.TestDelayed());
            categorize.MethodAsync<CategoryAsyncTestA>("Functional", x => x.TestYield());
        }

        private async Task TestDelayed()
        {
            await Task.Delay(1500);
        }

        private async Task TestYield()
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