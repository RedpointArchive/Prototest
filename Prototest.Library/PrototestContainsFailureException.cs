using System;
using System.Collections.Generic;

namespace Prototest.Library
{
    public class PrototestContainsFailureException : PrototestFailureException
    {
        public PrototestContainsFailureException(string message) : base(message)
        {
        }
    }

    public class PrototestStringContainsFailureException : PrototestContainsFailureException
    {
        /// <summary>
        /// The string that was searched in.
        /// </summary>
        public string Container { get; set; }

        /// <summary>
        /// The substring that was searched for.
        /// </summary>
        public string Substring { get; set; }

        public PrototestStringContainsFailureException(string substr, string container) : base(
            "The substring " + PrototestValueFormatter.Format(substr) + " was not found in the string " + PrototestValueFormatter.Format(container))
        {
            Substring = substr;
            Container = container;
        }
    }

    public class PrototestContainsFailureException<T> : PrototestContainsFailureException
    {
        /// <summary>
        /// The collection of elements that was checked.
        /// </summary>
        public IEnumerable<T> Collection { get; set; }

        /// <summary>
        /// The value that was expected to appear in the collection, but did not.
        /// </summary>
        public T Value { get; set; }

        /// <summary>
        /// The predicate that an element in the collection was expected to match, but did not.
        /// </summary>
        public Predicate<T> Predicate { get; set; }

        public PrototestContainsFailureException(IEnumerable<T> collection, T value)
            : base("The collection " + PrototestValueFormatter.Format(collection) + " did not contain " + PrototestValueFormatter.Format(value))
        {
            Collection = collection;
            Value = value;
        }

        public PrototestContainsFailureException(IEnumerable<T> collection, Predicate<T> predicate)
            : base("The collection " + PrototestValueFormatter.Format(collection) + " did not have any element matching the predicate")
        {
            Collection = collection;
            Predicate = predicate;
        }
    }
}
