using System;
#if !PLATFORM_UNITY
using System.Threading.Tasks;
#endif
using Prototest.Library;
using Prototest.Library.Version1;

namespace Prototest.Example
{
    public class AssertionTests
    {
        private readonly IAssert _assert;

        public AssertionTests(IAssert assert)
        {
            _assert = assert;
        }

        public void TestStringEqualityFailure()
        {
            _assert.Throws<PrototestEqualityFailureException>(() =>
            {
                _assert.Equal("a", "b");
            });
        }

        public void TestObjectEqualityFailure()
        {
            _assert.Throws<PrototestEqualityFailureException>(() =>
            {
                _assert.Equal(4, 5);
            });
        }

        public void TestArrayEqualityFailure()
        {
            _assert.Throws<PrototestEqualityFailureException>(() =>
            {
                _assert.Equal(new int[] {4}, new int[]{5});
            });
        }

        public void TestArrayLengthEqualityFailure()
        {
            _assert.Throws<PrototestEqualityFailureException>(() =>
            {
                _assert.Equal(new int[0], new int[] { 5 });
            });
        }

        public void TestArrayNullEqualityFailure()
        {
            _assert.Throws<PrototestEqualityFailureException>(() =>
            {
                _assert.Equal(null, new int[] { 5 });
            });
        }

        public void TestStringInequalityFailure()
        {
            _assert.Throws<PrototestInequalityFailureException>(() =>
            {
                _assert.NotEqual("a", "a");
            });
        }

        public void TestObjectInequalityFailure()
        {
            _assert.Throws<PrototestInequalityFailureException>(() =>
            {
                _assert.NotEqual(5, 5);
            });
        }

        public void TestArrayInequalityFailure()
        {
            _assert.Throws<PrototestInequalityFailureException>(() =>
            {
                _assert.NotEqual(new int[] { 5 }, new int[] { 5 });
            });
        }

#if !PLATFORM_UNITY
        public void TestAsyncThrowsFailure()
        {
            _assert.Throws<PrototestThrowsFailureException>(() =>
            {
                _assert.Throws<InvalidOperationException>(async () =>
                {
                    await Task.Delay(100);
                });
            });
        }

        public void TestAsyncThrowsAnyFailure()
        {
            _assert.Throws<PrototestThrowsFailureException>(() =>
            {
                _assert.Throws(async () =>
                {
                    await Task.Delay(100);
                });
            });
        }

        public void TestAsyncDoesNotThrowFailure()
        {
            _assert.Throws<PrototestDoesNotThrowFailureException>(() =>
            {
                _assert.DoesNotThrow<InvalidOperationException>(async () =>
                {
                    await Task.Delay(100);
                    throw new InvalidOperationException();
                });
            });
        }

        public void TestAsyncDoesNotThrowAnyFailure()
        {
            _assert.Throws<PrototestDoesNotThrowFailureException>(() =>
            {
                _assert.DoesNotThrow(async () =>
                {
                    await Task.Delay(100);
                    throw new InvalidOperationException();
                });
            });
        }

#endif
    }
}