using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace Prototest.Library
{
    public class PrototestAllFilterFailureException : PrototestFailureException
    {
        public PrototestAllFilterFailureException(string message) : base(message)
        {
        }
    }

    public class PrototestAllFilterFailureException<T> : PrototestAllFilterFailureException
    {
        /// <summary>
        /// The collection of elements that was filtered.
        /// </summary>
        public IEnumerable<T> Collection { get; set; }

        /// <summary>
        /// The expression which all elements did not pass, if available.
        /// </summary>
        public Expression<Func<T, bool>> FilterExpression { get; set; }

        public PrototestAllFilterFailureException(IEnumerable<T> collection)
            : base("All elements in " + PrototestValueFormatter.Format(collection) + " did not meet specified filter")
        {
            Collection = collection;
        }

        public PrototestAllFilterFailureException(IEnumerable<T> collection, Expression<Func<T, bool>> filter)
            : base("All elements in " + PrototestValueFormatter.Format(collection) + " did not meet filter " + PrototestValueFormatter.Format(filter))
        {
            Collection = collection;
            FilterExpression = filter;
        }
    }
}
