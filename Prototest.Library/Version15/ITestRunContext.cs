#if !PLATFORM_UNITY && !PLATFORM_PCL

using System;
using System.Collections.Generic;
using System.Text;

namespace Prototest.Library.Version15
{
    public interface ITestRunContext
    {
        IDisposable AcquireContext(ITestRunContextApi contextApi);
    }
}

#endif
