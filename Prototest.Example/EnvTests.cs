using Prototest.Library.Version1;
using System;

namespace Prototest.Example
{
    public class EnvTests
    {
        private readonly IAssert _assert;

        public EnvTests(IAssert assert)
        {
            _assert = assert;
        }

        public void TestEnvironmentVariableIsSet()
        {
            _assert.Equal(Environment.GetEnvironmentVariable("TEST_ENVIRONMENT"), "production");
        }
    }
}
