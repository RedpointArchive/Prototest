using System;

namespace Prototest.Library
{
    public class PrototestThrowsFailureException : PrototestFailureException
    {
        /// <summary>
        /// The type of exception we expected to be thrown, or null if we expected any exception.
        /// </summary>
        public Type ExpectedExceptionType { get; set; }

        /// <summary>
        /// The actual exception that was thrown in cases where the exception type does not match <see cref="ExpectedExceptionType"/>.
        /// </summary>
        public Exception ActuallyThrownException { get; set; }

        /// <summary>
        /// The user message if one was set, otherwise null.
        /// </summary>
        public string UserMessage { get; set; }

        public PrototestThrowsFailureException()
            : base("Expected an exception to be thrown")
        {
        }

        public PrototestThrowsFailureException(string message)
            : base("Expected an exception to be thrown: " + message)
        {
            UserMessage = message;
        }

        public PrototestThrowsFailureException(Type expectedExceptionType)
            : base("Expected an exception of type " + PrototestValueFormatter.Format(expectedExceptionType) + " to be thrown")
        {
            ExpectedExceptionType = expectedExceptionType;
        }

        public PrototestThrowsFailureException(Type expectedExceptionType, string message)
            : base("Expected an exception of type " + PrototestValueFormatter.Format(expectedExceptionType) + " to be thrown: " + message)
        {
            ExpectedExceptionType = expectedExceptionType;
            UserMessage = message;
        }

        public PrototestThrowsFailureException(Type expectedExceptionType, Exception actuallyThrownException)
            : base("Expected an exception of type " + PrototestValueFormatter.Format(expectedExceptionType) + " to be thrown, but " + PrototestValueFormatter.Format(actuallyThrownException) + " was thrown instead")
        {
            ExpectedExceptionType = expectedExceptionType;
            ActuallyThrownException = actuallyThrownException;
        }

        public PrototestThrowsFailureException(Type expectedExceptionType, Exception actuallyThrownException, string message)
            : base("Expected an exception of type " + PrototestValueFormatter.Format(expectedExceptionType) + " to be thrown, but " + PrototestValueFormatter.Format(actuallyThrownException) + " was thrown instead: " + message)
        {
            ExpectedExceptionType = expectedExceptionType;
            ActuallyThrownException = actuallyThrownException;
            UserMessage = message;
        }
    }
}
