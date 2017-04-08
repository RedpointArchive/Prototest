#if !PLATFORM_UNITY

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Prototest.Library.Version1;

namespace Prototest.Example
{
    public class CategoryMixedTestA
    {
        private readonly IAssert _assert;

        public CategoryMixedTestA(IAssert assert, Prototest.Library.Version14.ICategorize categorize, Prototest.Library.Version1.ICategorize categorize1)
        {
            _assert = assert;

            categorize.MethodAsync<CategoryMixedTestA>("Functional", x => x.TestDelayed());
            categorize.MethodAsync<CategoryMixedTestA>("Functional", x => x.TestYield());
            categorize.Method<CategoryMixedTestA>("Functional", x => x.TestMethodA());
            categorize.Method<CategoryMixedTestA>("Functional", x => x.TestMethodB());
            categorize1.Method("Functional", TestMethodC);
            categorize1.Method("Functional", TestMethodD);
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

        private void TestMethodB()
        {
            _assert.Equal(5, 5);
        }

        private void TestMethodA()
        {
            _assert.Equal(true, true);
        }

        private void TestMethodC()
        {
            _assert.Equal(5, 5);
        }

        private void TestMethodD()
        {
            _assert.Equal(true, true);
        }
    }
}

#endif