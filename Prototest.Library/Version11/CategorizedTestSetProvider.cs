using System;
using System.Collections.Generic;
using System.Linq;
using Prototest.Library.Version1;
using Prototest.Library.Version13;

namespace Prototest.Library.Version11
{
    public class CategorizedTestSetProvider : ITestSetProvider
    {
        public List<TestSet> GetTestSets(List<TestInputEntry> entries, Dictionary<Type, object> assertTypes)
        {
            var e = (from cls in entries
                     where (cls.Constructor.GetParameters().Any(z => z.ParameterType == typeof(ICategorize)))
                     let obj = cls.Constructor.Invoke(
                    cls.Constructor.GetParameters()
                        .Select(z => assertTypes[z.ParameterType])
                        .ToArray())
                let threadControlState = ((ThreadControl)assertTypes[typeof(IThreadControl)]).GetAndClearThreadControlMarked()
                from method in
                    ((Categorize) assertTypes[typeof(ICategorize)]).GetAndClearRegisteredMethods()
                select new
                {
                    TestClass = cls.Type,
                    TestConstructor = cls.Constructor,
                    TestMethod = method.Method,
                    RunTestMethod = (Action<object>)(z => method()),
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
