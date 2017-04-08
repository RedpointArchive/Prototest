namespace Prototest.Library
{
    public class PrototestNotNullFailureException : PrototestFailureException
    {
        /// <summary>
        /// The user message if one was set, otherwise null.
        /// </summary>
        public string UserMessage { get; set; }

        public PrototestNotNullFailureException() : base("The object was null")
        {
        }

        public PrototestNotNullFailureException(string message) : base("The object was null: " + message)
        {
            UserMessage = message;
        }
    }
}
