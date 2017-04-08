#if !PLATFORM_UNITY

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Prototest.Library.Version13;

namespace Prototest.Library.Version14
{
    public class CategorizedTestSetProvider : Version14.ITestSetProvider
    {
        public List<TestSet> GetTestSets(List<Version11.TestInputEntry> entries, Dictionary<Type, Func<object>> assertTypes)
        {
            var e1 = (from cls in entries
                     where (cls.Constructor.GetParameters().Any(z => z.ParameterType == typeof(Version1.ICategorize) || z.ParameterType == typeof(Version13.ICategorize) || z.ParameterType == typeof(Version14.ICategorize)))
                     let obj = cls.Constructor.Invoke(
                    cls.Constructor.GetParameters()
                        .Select(z => assertTypes[z.ParameterType]())
                        .ToArray())
                let threadControlState = ((ThreadControl)assertTypes[typeof(IThreadControl)]()).GetAndClearThreadControlMarked()
                from method in
                    ((Version11.Categorize) assertTypes[typeof(Version1.ICategorize)]()).GetAndClearRegisteredAsyncMethods()
                select new
                {
                    TestClass = cls.Type,
                    TestConstructor = cls.Constructor,
#if PLATFORM_PCL
                    TestMethod = method.GetMethodInfo(),
                    RunTestMethod = method,
#else
                    TestMethod = method.Method,
                    RunTestMethod = method,
#endif
                    RunOnSingleThread = threadControlState
                }).ToList();
            var e2 = (from cls in entries
                      where (cls.Constructor.GetParameters().Any(z => z.ParameterType == typeof(Version1.ICategorize) || z.ParameterType == typeof(Version13.ICategorize) || z.ParameterType == typeof(Version14.ICategorize)))
                      let obj = cls.Constructor.Invoke(
                     cls.Constructor.GetParameters()
                         .Select(z => assertTypes[z.ParameterType]())
                         .ToArray())
                      let threadControlState = ((ThreadControl)assertTypes[typeof(IThreadControl)]()).GetAndClearThreadControlMarked()
                      from method in
                          ((Version11.Categorize)assertTypes[typeof(Version1.ICategorize)]()).GetAndClearRegisteredMethods()
                      select new
                      {
                          TestClass = cls.Type,
                          TestConstructor = cls.Constructor,
#if PLATFORM_PCL
                          TestMethod = method.GetMethodInfo(),
                          RunTestMethod = method,
#else
                          TestMethod = method.Method,
                          RunTestMethod = method,
#endif
                          RunOnSingleThread = threadControlState
                      }).ToList();

            return new List<TestSet>
            {
                new TestSet
                {
                    Name = "categorized",
                    RunSingleThreadedOnMainThread = false,
                    Entries = e1.Where(x => !x.RunOnSingleThread).Select(x => new TestSetEntry
                    {
                        TestClass = x.TestClass,
                        TestConstructor = x.TestConstructor,
                        TestMethod = x.TestMethod,
                        RunTestMethod = x.RunTestMethod
                    }).Concat(e2.Where(x => !x.RunOnSingleThread).Select(x => new TestSetEntry
                    {
                        TestClass = x.TestClass,
                        TestConstructor = x.TestConstructor,
                        TestMethod = x.TestMethod,
                        RunTestMethod = async o => x.RunTestMethod(o)
                    })).ToList()
                },
                new TestSet
                {
                    Name = "categorized-singlethreaded",
                    RunSingleThreadedOnMainThread = true,
                    Entries = e1.Where(x => x.RunOnSingleThread).Select(x => new TestSetEntry
                    {
                        TestClass = x.TestClass,
                        TestConstructor = x.TestConstructor,
                        TestMethod = x.TestMethod,
                        RunTestMethod = x.RunTestMethod
                    }).Concat(e2.Where(x => x.RunOnSingleThread).Select(x => new TestSetEntry
                    {
                        TestClass = x.TestClass,
                        TestConstructor = x.TestConstructor,
                        TestMethod = x.TestMethod,
                        RunTestMethod = async o => x.RunTestMethod(o)
                    })).ToList()
                },
            };
        }
    }
}

#endif