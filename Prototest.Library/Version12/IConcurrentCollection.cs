using System.Collections.Generic;

namespace Prototest.Library.Version12
{
    public interface IConcurrentCollection<T> : IEnumerable<T>
    {
        void Add(T t);
    }
}
