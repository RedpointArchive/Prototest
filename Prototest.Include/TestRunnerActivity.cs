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
using Resource = Prototest.Library.Resource;

namespace Prototest.Include
{
    [Activity(MainLauncher = true)]
    public class TestRunnerActivity : Activity
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
            Prototest.Library.Runner.Run(
                Assembly.GetExecutingAssembly(),
                RunStateInitTestClassesFound,
                RunStateInitTestMethodsFound,
                RunStateInitTestEntriesFound,
                RunStateStartTest,
                RunStatePassTest,
                RunStateFailTest,
                RunStateSummary,
                RunStateDetail,
                new string[0]);
        }

        private void RunStateSummary(bool anyFail, int ran, int fail, int pass)
        {
            runStateTextView.Text = pass + " Pass, " + fail + " Fail, " + ran + " Ran";
        }

        private void RunStateDetail(bool anyFail, ConcurrentBag<string> bag)
        {
        }

        private void RunStateFailTest(string setName, Type type, MethodInfo testMethod, ConcurrentBag<string> bag, Exception ex)
        {
        }

        private void RunStatePassTest(string setName, Type type, MethodInfo testMethod, int pass)
        {
        }

        private void RunStateStartTest(string setName, Type type, MethodInfo testMethod)
        {
            runStateTextView.Text = "... " + testMethod.Name;
        }

        private void RunStateInitTestMethodsFound(int count)
        {
        }

        private void RunStateInitTestClassesFound(int count)
        {
        }

        private void RunStateInitTestEntriesFound(int count)
        {
            runStateTextView.Text = count + " to run";
        }
    }
}