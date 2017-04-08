namespace Prototest.Library
{
    public class PrototestNullFailureException : PrototestFailureException
    {
        /// <summary>
        /// The user message if one was set, otherwise null.
        /// </summary>
        public string UserMessage { get; set; }

        /// <summary>
        /// The object that was not null.
        /// </summary>
        public object NonNullObject { get; set; }

        public PrototestNullFailureException(object obj) : base("The object " + PrototestValueFormatter.Format(obj) + " was not null")
        {
            NonNullObject = obj;
        }

        public PrototestNullFailureException(object obj, string message) : base("The object " + PrototestValueFormatter.Format(obj) + " was not null: " + message)
        {
            NonNullObject = obj;
            UserMessage = message;
        }
    }
}
