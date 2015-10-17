using System;

namespace Prototest.Library
{
    public class PrototestFailureException : Exception
    {
        public PrototestFailureException(string message) : base(message)
        {
        }
    }
}