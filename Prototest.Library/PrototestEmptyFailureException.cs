using System.Collections;

namespace Prototest.Library
{
    public class PrototestEmptyFailureException : PrototestFailureException
    {
        /// <summary>
        /// The collection of elements that was checked.
        /// </summary>
        public IEnumerable Collection { get; set; }

        public PrototestEmptyFailureException(IEnumerable collection)
            : base("The collection " + PrototestValueFormatter.Format(collection) + " was not empty")
        {
            Collection = collection;
        }
    }
}
