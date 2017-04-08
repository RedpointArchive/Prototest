#if !PLATFORM_UNITY

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Prototest.Library.Version1;
using Prototest.Library.Version13;

namespace Prototest.Library.Version14
{
    public class DefaultTestSetProvider : ITestSetProvider
    {
        private Func<object, Task> CreateRunTestMethod(MethodInfo method)
        {
            if (method.ReturnType == typeof(Task))
            {
#if PLATFORM_PCL
                return (Func<object, Task>)(objj => ((Func<Task>)method.CreateDelegate(typeof(Func<Task>), objj))());
#else
                return (Func<object, Task>)(objj => ((Func<Task>)Delegate.CreateDelegate(typeof(Func<Task>), objj, method))());
#endif
            }
            else
            {
#if PLATFORM_PCL
                var action = (Action<object>)(objj => ((Action)method.CreateDelegate(typeof(Action), objj))());
#else
                var action = (Action<object>) (objj => ((Action) Delegate.CreateDelegate(typeof(Action), objj, method))());
#endif

#pragma warning disable CS1998 // Async method lacks 'await' operators and will run synchronously
                return async o =>
                {
                    action(o);
                };
#pragma warning restore CS1998 // Async method lacks 'await' operators and will run synchronously
            }
        }

        public List<TestSet> GetTestSets(List<Version11.TestInputEntry> entries, Dictionary<Type, Func<object>> assertTypes)
        {
            var e = (from cls in entries
                     where (cls.Constructor.GetParameters().All(z => z.ParameterType != typeof(Version1.ICategorize) && z.ParameterType != typeof(Version13.ICategorize) && z.ParameterType != typeof(Version14.ICategorize)))
                     let obj = cls.Constructor.Invoke(
                         cls.Constructor.GetParameters()
                             .Select(z => assertTypes[z.ParameterType]())
                             .ToArray())
                     let threadControlState = ((ThreadControl)assertTypes[typeof(IThreadControl)]()).GetAndClearThreadControlMarked()
                     from method in cls.TestMethods
                     where method.GetParameters().Length == 0
                     where method.ReturnType == typeof(Task)
                     select new
                     {
                         TestClass = cls.Type,
                         TestConstructor = cls.Constructor,
                         TestMethod = method,
                         RunTestMethod = CreateRunTestMethod(method),
                         RunOnSingleThread = threadControlState
                     }).ToList();

            return new List<TestSet>
            {
                new TestSet
                {
                    Name = "default",
                    RunSingleThreadedOnMainThread = false,
                    Entries = e.Where(x => !x.RunOnSingleThread).Select(x => new TestSetEntry
                    {
                        TestClass = x.TestClass,
                        TestConstructor = x.TestConstructor,
                        TestMethod = x.TestMethod,
                        RunTestMethod = x.RunTestMethod
                    }).ToList()
                },
                new TestSet
                {
                    Name = "default-singlethreaded",
                    RunSingleThreadedOnMainThread = true,
                    Entries = e.Where(x => x.RunOnSingleThread).Select(x => new TestSetEntry
                    {
                        TestClass = x.TestClass,
                        TestConstructor = x.TestConstructor,
                        TestMethod = x.TestMethod,
                        RunTestMethod = x.RunTestMethod
                    }).ToList()
                },
            };
        }
    }
}

#endif