#if !NO_PROTOTEST_ENTRY_POINT
#if PLATFORM_IOS
using UIKit;
#elif PLATFORM_ANDROID
#else
using System;
#if !PLATFORM_UNITY && !PLATFORM_PCL
using System.Collections.Concurrent;
#endif
using System.Diagnostics;
using System.Reflection;
#endif
#if PLATFORM_MACOS
#if PLATFORM_MACOS_LEGACY
using MonoMac.AppKit;
using MonoMac.Foundation;
#else
using AppKit;
using Foundation;
#endif
#endif

namespace Prototest.Include
{
    public static class Program
    {
#if PLATFORM_PCL
        public static int Main(string[] args)
#else
        public static void Main(string[] args)
#endif
        {
#if PLATFORM_IOS
            UIApplication.Main(args, null, "AppDelegate");
#elif PLATFORM_ANDROID
#elif PLATFORM_MACOS
            NSApplication.Init();

            using (var p = new NSAutoreleasePool())
            {
                NSApplication.SharedApplication.Delegate = new AppDelegate();
                AppDelegate.Args = args;

                NSApplication.Main(args);
            }
        }
    }
}

public class AppDelegate : NSApplicationDelegate
{
    public static string[] Args;

    public override bool ApplicationShouldTerminateAfterLastWindowClosed(NSApplication sender)
    {
        return true;
    }

#if PLATFORM_MACOS_LEGACY
    public override void FinishedLaunching(NSObject notification)
#else
    public override void DidFinishLaunching(NSNotification notification)
#endif
    {
        var args = Args;
#else
#endif
#if !PLATFORM_IOS && !PLATFORM_ANDROID
#if PLATFORM_PCL
            if (Prototest.Library.Runner.Run(
                typeof(Program).GetTypeInfo().Assembly,
                args))
            {
                return 0;
            }

            return 1;
#else
            if (Prototest.Library.Runner.Run(
                Assembly.GetExecutingAssembly(),
                args))
            {
                Environment.Exit(0);
            }

            Environment.Exit(1);
#endif
#endif
        }
    }
#if !PLATFORM_MACOS
}
#endif
#endif