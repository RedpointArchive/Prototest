using System;
using System.Linq.Expressions;

namespace Prototest.Library
{
    public class PrototestFalsityFailureException : PrototestFailureException
    {
        /// <summary>
        /// The user message if one was set, otherwise null.
        /// </summary>
        public string UserMessage { get; set; }

        /// <summary>
        /// The associated expression that failed if available, otherwise null.
        /// </summary>
        public Expression<Func<bool>> Expression { get; set; }

        public PrototestFalsityFailureException() : base("Expected false, got true")
        {
        }

        public PrototestFalsityFailureException(string message): base("Expected false, got true: " + message)
        {
            UserMessage = message;
        }

        public PrototestFalsityFailureException(Expression<Func<bool>> expression) : base(PrototestValueFormatter.Format(expression) + " was not false")
        {
            Expression = expression;
        }

        public PrototestFalsityFailureException(Expression<Func<bool>> expression, string message) : base(PrototestValueFormatter.Format(expression) + " was not false: " + message)
        {
            Expression = expression;
            UserMessage = message;
        }
    }
}
