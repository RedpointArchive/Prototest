using System;
using System.Collections.Generic;
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
                if (parameters.All(x => assertTypes.Keys.Contains(x.ParameterType)))
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

            Console.WriteLine("## init " + testClasses.Count + " test classes found");
            Console.WriteLine("## init " + testClasses.Sum(x => x.TestMethods.Count) + " test methods found");

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
                            Console.WriteLine("## start " + x.Type.FullName + "." + x.TestMethod.Name);
                            try
                            {
                                x.TestMethod.Invoke(obj, null);
                                Console.WriteLine("## pass " + x.Type.FullName + "." + x.TestMethod.Name);
                            }
                            catch (Exception ex)
                            {
                                Console.WriteLine("## fail " + x.Type.FullName + "." + x.TestMethod.Name + ": " + ex);
                                anyFail = true;
                            }
                        })).ToArray());

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
