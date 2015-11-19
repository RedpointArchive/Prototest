#if PLATFORM_IOS
using UIKit;
#else
using System;
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
#else
            if (Prototest.Library.Runner.Run(Assembly.GetExecutingAssembly()))
            {
                Environment.Exit(0);
            }

            Environment.Exit(1);
#endif
        }
    }
}

