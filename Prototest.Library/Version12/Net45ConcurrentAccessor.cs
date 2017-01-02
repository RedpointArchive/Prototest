#if !PLATFORM_UNITY

using System.Collections;
using System.Collections.Generic;
using System.Collections.Concurrent;

namespace Prototest.Library.Version12
{
    public class Net45ConcurrentCollection<T> : IConcurrentCollection<T>
    {
        private readonly ConcurrentBag<T> _collection;

        public Net45ConcurrentCollection(ConcurrentBag<T> collection)
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