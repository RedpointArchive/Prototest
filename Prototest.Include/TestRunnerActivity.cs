#if PLATFORM_ANDROID
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using Android;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Prototest.Library.Version11;
using Resource = Prototest.Library.Resource;

namespace Prototest.Include
{
    [Activity(MainLauncher = true)]
    public class TestRunnerActivity : Activity, ITestConnector
    {
        private TextView runStateTextView;

        private Button runTestsButton;

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);

            SetContentView(Prototest.Library.Resource.Layout.TestRunner);

            runStateTextView = FindViewById<TextView>(Resource.Id.runStatistics);
            runTestsButton = FindViewById<Button>(Resource.Id.runAllButton);
            runTestsButton.Click += (sender, args) => RunTests();
        }

        public void RunTests()
        {
            new DefaultTestRunner().Run(
                Assembly.GetExecutingAssembly(),
                this,
                new string[0]);
        }

        public void InitTestClassesFound(int testClasses)
        {
        }

        public void InitTestMethodsFound(int testMethods)
        {
        }

        public void InitTestEntriesFound(int testEntries)
        {
            runStateTextView.Text = testEntries + " to run";
        }

        public void TestStarted(string setName, Type testClass, MethodInfo testMethod)
        {
            runStateTextView.Text = "... " + testMethod.Name;
        }

        public void TestPassed(string setName, Type testClass, MethodInfo testMethod, int testsPassed)
        {
        }

        public void TestFailed(string setName, Type testClass, MethodInfo testMethod, ConcurrentBag<string> errors, Exception testFailure)
        {
        }

        public void Summary(bool anyFail, int ran, int fail, int pass)
        {
            runStateTextView.Text = pass + " Pass, " + fail + " Fail, " + ran + " Ran";
        }

        public void Details(bool anyFail, ConcurrentBag<string> details)
        {
        }
    }
}
#endif