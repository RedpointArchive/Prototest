using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Prototest.Library.Version1;

namespace Prototest.Library
{
    public static class Runner
    {
        public static bool Run(
            Assembly assembly,
            Action<int> runStateInitTestClassesFound, 
            Action<int> runStateInitTestMethodsFound, 
            Action<Type, MethodInfo> runStateStartTest,
            Action<Type, MethodInfo, int> runStatePassTest, 
            Action<Type, MethodInfo, ConcurrentBag<string>, Exception> runStateFailTest, 
            Action<bool, int, int, int> runStateSummary, 
            Action<bool, ConcurrentBag<string>> runStateDetail)
        {
            var assertTypes = new Dictionary<Type, Type>
            {
                {typeof (IAssert), typeof (Assert)}
            };

            var testClasses = new List<TestClass>();

            var types = assembly.GetTypes();
            foreach (var type in types)
            {
                if (type.IsAbstract || type.IsInterface)
                {
                    continue;
                }

                var constructors = type.GetConstructors();
                if (constructors.Length != 1)
                {
                    continue;
                }

                var constructor = constructors.First();
                var parameters = constructor.GetParameters();
                if (parameters.Length > 0 && parameters.All(x => assertTypes.Keys.Contains(x.ParameterType)))
                {
                    var testClass = new TestClass
                    {
                        Constructor = constructor,
                        TestMethods = new List<MethodInfo>(),
                        Type = type
                    };

                    testClass.TestMethods =
                        type.GetMethods(BindingFlags.Instance | BindingFlags.Public | BindingFlags.DeclaredOnly)
                            .Where(x => x.GetParameters().Length == 0)
                            .ToList();

                    testClasses.Add(testClass);
                }
            }

            runStateInitTestClassesFound(testClasses.Count);
            runStateInitTestMethodsFound(testClasses.Sum(x => x.TestMethods.Count));

            var lockObject = new object();
            var ran = 0;
            var pass = 0;
            var fail = 0;
            var bag = new ConcurrentBag<string>();
            var anyFail = false;
            var tasks = testClasses.SelectMany(
                    x => x.TestMethods.Select(
                        y => new {x.Type, x.Constructor, TestMethod = y}))
                    .Select(x => new Action(
                        () =>
                        {
                            var obj = x.Constructor.Invoke(
                                x.Constructor.GetParameters()
                                    .Select(z => Activator.CreateInstance(assertTypes[z.ParameterType]))
                                    .ToArray());
                            runStateStartTest(x.Type, x.TestMethod);
                            lock (lockObject) ran++;
                            try
                            {
                                x.TestMethod.Invoke(obj, null);
                                lock (lockObject) pass++;
                                runStatePassTest(x.Type, x.TestMethod, pass);
                            }
                            catch (TargetInvocationException ex)
                            {
                                lock (lockObject) fail++;
                                runStateFailTest(x.Type, x.TestMethod, bag, ex.InnerException);
                                anyFail = true;
                            }
                            catch (Exception ex)
                            {
                                lock (lockObject) fail++;
                                runStateFailTest(x.Type, x.TestMethod, bag, ex);
                                anyFail = true;
                            }
                        })).ToArray();

            if (Debugger.IsAttached)
            {
                // Run in single thread when a debugger is attached to make
                // diagnosing issues easier.
                foreach (var task in tasks)
                {
                    task();
                }
            }
            else
            {
                Parallel.Invoke(tasks);
            }

            runStateSummary(anyFail, ran, fail, pass);
            runStateDetail(anyFail, bag);

            return !anyFail;
        }
        
    }

    internal class TestClass
    {
        public ConstructorInfo Constructor;

        public List<MethodInfo> TestMethods;
        public Type Type;
    }
}
