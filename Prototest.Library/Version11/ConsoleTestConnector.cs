﻿#if !PLATFORM_UNITY && !PLATFORM_PCL

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Prototest.Library.Version11
{
#if !PLATFORM_IOS && !PLATFORM_ANDROID
    public class ConsoleTestConnector : ITestConnector
    {
        public void InitTestClassesFound(int testClasses)
        {
            WriteOutput("## init " + testClasses + " test classes found");
        }

        public void InitTestMethodsFound(int testMethods)
        {
            WriteOutput("## init " + testMethods + " test methods found");
        }

        public void InitTestEntriesFound(int testEntries)
        {
            WriteOutput("## init " + testEntries + " test entries found");
        }

        public void TestStarted(string setName, Type testClass, MethodInfo testMethod)
        {
            WriteOutput("## start " + setName + ":" + testClass.FullName + "." + testMethod.Name);
        }

        public void TestPassed(string setName, Type testClass, MethodInfo testMethod, int testsPassed)
        {
            WriteOutput("## pass " + setName + ":" + testClass.FullName + "." + testMethod.Name);
        }

        public void TestFailed(string setName, Type testClass, MethodInfo testMethod, ConcurrentBag<string> errors, Exception testFailure)
        {
            WriteOutput("## fail " + setName + ":" + testClass.FullName + "." + testMethod.Name + ": " + testFailure);
            errors.Add("fail " + setName + ":" + testClass.FullName + "." + testMethod.Name + ": " + testFailure);
        }

        public void Summary(bool anyFail, int ran, int fail, int pass)
        {
            var end = "## summary ";
            if (anyFail) end += "fail";
            else end += "pass";
            end += ": " + ran + " ran " + fail + " fail " + pass + " pass";
            WriteOutput(end);
        }

        public void Details(bool anyFail, ConcurrentBag<string> details)
        {
            if (anyFail)
            {
                foreach (var b in details)
                {
                    WriteOutput("## detail " + b);
                }
            }
        }

        private void WriteOutput(string msg)
        {
            Console.WriteLine(msg);
            Debug.WriteLine(msg);
        }
    }
#endif
}

#endif