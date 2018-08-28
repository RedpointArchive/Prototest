using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Prototest.Library.Version1;
using Prototest.Library.Version13;

namespace Prototest.Library.Version11
{
    public class CategorizedTestSetProvider : Version13.ITestSetProvider
    {
        public List<TestSet> GetTestSets(List<TestInputEntry> entries, Dictionary<Type, Func<object>> assertTypes)
        {
            var e = (from cls in entries
                     where cls.Constructor.GetParameters().Any(z => z.ParameterType == typeof(Version1.ICategorize) || z.ParameterType == typeof(Version13.ICategorize))
#if !PLATFORM_UNITY
                     && cls.Constructor.GetParameters().All(z => z.ParameterType != typeof(Version14.ICategorize))
#endif
                     let obj = cls.Constructor.Invoke(
                    cls.Constructor.GetParameters()
                        .Select(z => assertTypes[z.ParameterType]())
                        .ToArray())
                let threadControlState = ((ThreadControl)assertTypes[typeof(IThreadControl)]()).GetAndClearThreadControlMarked()
                from method in
                    ((Version15.ICategorizeClearable) assertTypes[typeof(Version1.ICategorize)]()).GetAndClearRegisteredMethods() // The same object is shared between the v1.1 and v1.3 interfaces.
#if !PLATFORM_UNITY
#if PLATFORM_PCL
                where method.GetMethodInfo().ReturnType != typeof(System.Threading.Tasks.Task)
#else
                where method.Method.ReturnType != typeof(System.Threading.Tasks.Task)
#endif
#endif
                select new
                {
                    TestClass = cls.Type,
                    TestConstructor = cls.Constructor,
#if PLATFORM_PCL
                    TestMethod = method.GetMethodInfo(),
#else
                    TestMethod = method.Method,
#endif
                    RunTestMethod = method,
                    RunOnSingleThread = threadControlState
                }).ToList();

            return new List<TestSet>
            {
                new TestSet
                {
                    Name = "categorized",
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
                    Name = "categorized-singlethreaded",
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
