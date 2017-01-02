#if PLATFORM_UNITY

using System.Collections;
using System.Collections.Generic;

namespace Prototest.Library.Version12
{
    public class Net35ConcurrentCollection<T> : IConcurrentCollection<T>
    {
        private readonly List<T> _collection;

        public Net35ConcurrentCollection(List<T> collection)
        {
            _collection = collection;
        }

        public void Add(T t)
        {
            _collection.Add(t);
        }

        public IEnumerator<T> GetEnumerator()
        {
            return _collection.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((IEnumerable)_collection).GetEnumerator();
        }
    }
}

#endif