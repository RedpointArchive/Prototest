using System;
using System.Collections.Generic;
using System.Text;

namespace Prototest.Library.Version15
{
    public interface ICategorizeClearable
    {
        List<Action<object>> GetAndClearRegisteredMethods();
#if !PLATFORM_UNITY
        List<Func<object, System.Threading.Tasks.Task>> GetAndClearRegisteredAsyncMethods();
#endif
    }
}
