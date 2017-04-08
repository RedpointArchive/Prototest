using System;
using System.Linq.Expressions;

namespace Prototest.Library
{
    public class PrototestTruthFailureException : PrototestFailureException
    {
        /// <summary>
        /// The user message if one was set, otherwise null.
        /// </summary>
        public string UserMessage { get; set; }

        /// <summary>
        /// The associated expression that failed if available, otherwise null.
        /// </summary>
        public Expression<Func<bool>> Expression { get; set; }

        public PrototestTruthFailureException() : base("Expected true, got false")
        {
        }

        public PrototestTruthFailureException(string message) : base("Expected true, got false: " + message)
        {
            UserMessage = message;
        }

        public PrototestTruthFailureException(Expression<Func<bool>> expression) : base(PrototestValueFormatter.Format(expression) + " was not true")
        {
            Expression = expression;
        }

        public PrototestTruthFailureException(Expression<Func<bool>> expression, string message) : base(PrototestValueFormatter.Format(expression) + " was not true: " + message)
        {
            Expression = expression;
            UserMessage = message;
        }
    }
}
