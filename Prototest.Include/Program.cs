#if PLATFORM_IOS
using UIKit;
#elif PLATFORM_ANDROID
#else
using System;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.Reflection;
#endif

namespace Prototest.Include
{
    public static class Program
    {
        public static void Main(string[] args)
        {
#if PLATFORM_IOS
            UIApplication.Main(args, null, "AppDelegate");
#elif PLATFORM_ANDROID
#else
            if (Prototest.Library.Runner.Run(
                Assembly.GetExecutingAssembly(),
                args))
            {
                Environment.Exit(0);
            }

            Environment.Exit(1);
#endif
        }
    }
}

