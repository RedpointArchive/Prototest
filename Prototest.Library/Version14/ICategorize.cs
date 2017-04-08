#if !PLATFORM_UNITY

using System;
using System.Threading.Tasks;

namespace Prototest.Library.Version14
{
    public interface ICategorize : Version13.ICategorize
    {
        void MethodAsync<T>(string category, Func<T, Task> method);
    }
}

#endif