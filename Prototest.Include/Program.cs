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
                RunStateInitTestClassesFound,
                RunStateInitTestMethodsFound,
                RunStateInitTestEntriesFound,
                RunStateStartTest,
                RunStatePassTest,
                RunStateFailTest,
                RunStateSummary,
                RunStateDetail,
                args))
            {
                Environment.Exit(0);
            }

            Environment.Exit(1);
#endif
        }

#if !PLATFORM_IOS && !PLATFORM_ANDROID
        private static void RunStateSummary(bool anyFail, int ran, int fail, int pass)
        {
            var end = "## summary ";
            if (anyFail) end += "fail";
            else end += "pass";
            end += ": " + ran + " ran " + fail + " fail " + pass + " pass";
            WriteOutput(end);
        }

        private static void RunStateDetail(bool anyFail, ConcurrentBag<string> bag)
        {
            if (anyFail)
            {
                foreach (var b in bag)
                {
                    WriteOutput("## detail " + b);
                }
            }
        }

        private static void RunStateFailTest(string setName, Type type, MethodInfo testMethod, ConcurrentBag<string> bag, Exception ex)
        {
            WriteOutput("## fail " + setName + ":" + type.FullName + "." + testMethod.Name + ": " + ex);
            bag.Add("fail " + setName + ":" + type.FullName + "." + testMethod.Name + ": " + ex);
        }

        private static void RunStatePassTest(string setName, Type type, MethodInfo testMethod, int pass)
        {
            WriteOutput("## pass " + setName + ":" + type.FullName + "." + testMethod.Name);
        }

        private static void RunStateStartTest(string setName, Type type, MethodInfo testMethod)
        {
            WriteOutput("## start " + setName + ":" + type.FullName + "." + testMethod.Name);
        }

        private static void RunStateInitTestMethodsFound(int count)
        {
            WriteOutput("## init " + count + " test methods found");
        }

        private static void RunStateInitTestClassesFound(int count)
        {
            WriteOutput("## init " + count + " test classes found");
        }

        private static void RunStateInitTestEntriesFound(int count)
        {
            WriteOutput("## init " + count + " test entries found");
        }

        private static void WriteOutput(string msg)
        {
            Console.WriteLine(msg);
            Debug.WriteLine(msg);
        }
#endif
    }
}

