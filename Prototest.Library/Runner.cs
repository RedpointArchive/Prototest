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
        public static bool Run(Assembly assembly)
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

            WriteOutput("## init " + testClasses.Count + " test classes found");
            WriteOutput("## init " + testClasses.Sum(x => x.TestMethods.Count) + " test methods found");

            var lockObject = new object();
            var ran = 0;
            var pass = 0;
            var fail = 0;
            var bag = new ConcurrentBag<string>();
            var anyFail = false;
            Task.WaitAll(
                testClasses.SelectMany(
                    x => x.TestMethods.Select(
                        y => new {x.Type, x.Constructor, TestMethod = y}))
                    .Select(x => Task.Run(
                        () =>
                        {
                            var obj = x.Constructor.Invoke(
                                x.Constructor.GetParameters()
                                    .Select(z => Activator.CreateInstance(assertTypes[z.ParameterType]))
                                    .ToArray());
                            WriteOutput("## start " + x.Type.FullName + "." + x.TestMethod.Name);
                            lock (lockObject) ran++;
                            try
                            {
                                x.TestMethod.Invoke(obj, null);
                                WriteOutput("## pass " + x.Type.FullName + "." + x.TestMethod.Name);
                                lock (lockObject) pass++;
                            }
                            catch (TargetInvocationException ex)
                            {
                                WriteOutput("## fail " + x.Type.FullName + "." + x.TestMethod.Name + ": " + ex.InnerException);
                                lock (lockObject) fail++;
                                anyFail = true;
                                bag.Add("fail " + x.Type.FullName + "." + x.TestMethod.Name + ": " + ex.InnerException);
                            }
                            catch (Exception ex)
                            {
                                WriteOutput("## fail " + x.Type.FullName + "." + x.TestMethod.Name + ": " + ex);
                                lock (lockObject) fail++;
                                anyFail = true;
                                bag.Add("fail " + x.Type.FullName + "." + x.TestMethod.Name + ": " + ex);
                            }
                        })).ToArray());

            var end = "## summary ";
            if (anyFail) end += "fail";
            else end += "pass";
            end += ": " + ran + " ran " + fail + " fail " + pass + " pass";
            WriteOutput(end);

            if (anyFail)
            {
                foreach (var b in bag)
                {
                    WriteOutput("## detail " + b);
                }
            }

            return !anyFail;
        }

        private static void WriteOutput(string msg)
        {
            Console.WriteLine(msg);
            Debug.WriteLine(msg);
        }
    }

    internal class TestClass
    {
        public ConstructorInfo Constructor;

        public List<MethodInfo> TestMethods;
        public Type Type;
    }
}
