using System;

namespace Prototest.Library
{
    public class PrototestDoesNotThrowFailureException : PrototestFailureException
    {
        /// <summary>
        /// The type of exception we expected to NOT be thrown, or null if we expected no exceptions.
        /// </summary>
        public Type UnexpectedExceptionType { get; set; }

        /// <summary>
        /// The exception that was thrown in cases where the exception type matches <see cref="UnexpectedExceptionType"/>.
        /// </summary>
        public Exception ThrownException { get; set; }

        /// <summary>
        /// The user message if one was set, otherwise null.
        /// </summary>
        public string UserMessage { get; set; }
        
        public PrototestDoesNotThrowFailureException(Exception thrownException)
            : base("Unexpected exception " + PrototestValueFormatter.Format(thrownException) + " was thrown")
        {
            ThrownException = thrownException;
        }

        public PrototestDoesNotThrowFailureException(Exception thrownException, string message)
            : base("Unexpected exception " + PrototestValueFormatter.Format(thrownException) + " was thrown: " + message)
        {
            ThrownException = thrownException;
            UserMessage = message;
        }

        public PrototestDoesNotThrowFailureException(Type unexpectedExceptionType, Exception thrownException)
            : base("Unexpected exception " + PrototestValueFormatter.Format(thrownException) + " of type " + PrototestValueFormatter.Format(unexpectedExceptionType) + " was thrown")
        {
            UnexpectedExceptionType = unexpectedExceptionType;
            ThrownException = thrownException;
        }

        public PrototestDoesNotThrowFailureException(Type unexpectedExceptionType, Exception thrownException, string message)
            : base("Unexpected exception " + PrototestValueFormatter.Format(thrownException) + " of type " + PrototestValueFormatter.Format(unexpectedExceptionType) + " was thrown: " + message)
        {
            UnexpectedExceptionType = unexpectedExceptionType;
            ThrownException = thrownException;
            UserMessage = message;
        }
    }
}
