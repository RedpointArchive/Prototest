using System.Collections;

namespace Prototest.Library
{
    public class PrototestNotEmptyFailureException : PrototestFailureException
    {
        /// <summary>
        /// The collection of elements that was checked.
        /// </summary>
        public IEnumerable Collection { get; set; }

        public PrototestNotEmptyFailureException(IEnumerable collection)
            : base("The collection " + PrototestValueFormatter.Format(collection) + " was empty")
        {
            Collection = collection;
        }
    }
}
