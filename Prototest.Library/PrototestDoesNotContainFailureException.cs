using System;
using System.Collections.Generic;

namespace Prototest.Library
{
    public class PrototestDoesNotContainFailureException : PrototestFailureException
    {
        public PrototestDoesNotContainFailureException(string message) : base(message)
        {
        }
    }

    public class PrototestStringDoesNotContainFailureException : PrototestDoesNotContainFailureException
    {
        /// <summary>
        /// The string that was searched in.
        /// </summary>
        public string Container { get; set; }

        /// <summary>
        /// The substring that was searched for.
        /// </summary>
        public string Substring { get; set; }

        public PrototestStringDoesNotContainFailureException(string substr, string container) : base(
            "The substring " + PrototestValueFormatter.Format(substr) + " was found in the string " + PrototestValueFormatter.Format(container))
        {
            Substring = substr;
            Container = container;
        }
    }

    public class PrototestDoesNotContainFailureException<T> : PrototestDoesNotContainFailureException
    {
        /// <summary>
        /// The collection of elements that was checked.
        /// </summary>
        public IEnumerable<T> Collection { get; set; }

        /// <summary>
        /// The value that was expected to not appear in the collection, but did.
        /// </summary>
        public T Value { get; set; }

        /// <summary>
        /// The predicate that an element in the collection unexpectedly matched.
        /// </summary>
        public Predicate<T> Predicate { get; set; }

        public PrototestDoesNotContainFailureException(IEnumerable<T> collection, T value)
            : base("The collection " + PrototestValueFormatter.Format(collection) + " contains " + PrototestValueFormatter.Format(value))
        {
            Collection = collection;
            Value = value;
        }

        public PrototestDoesNotContainFailureException(IEnumerable<T> collection, Predicate<T> predicate)
            : base("The collection " + PrototestValueFormatter.Format(collection) + " had an element matching the predicate")
        {
            Collection = collection;
            Predicate = predicate;
        }
    }
}
