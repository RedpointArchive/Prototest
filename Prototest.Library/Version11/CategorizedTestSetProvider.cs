using System;
using System.Collections.Generic;
using System.Linq;
using Prototest.Library.Version1;

namespace Prototest.Library.Version11
{
    public class CategorizedTestSetProvider : ITestSetProvider
    {
        public List<TestSet> GetTestSets(List<TestInputEntry> entries, Dictionary<Type, object> assertTypes)
        {
            return new List<TestSet>
            {
                new TestSet
                {
                    Name = "categorized",
                    Entries = (from cls in entries
                              let obj = cls.Constructor.Invoke(
                                cls.Constructor.GetParameters()
                                    .Select(z => assertTypes[z.ParameterType])
                                    .ToArray())
                              from method in
                                  ((Categorize)assertTypes[typeof(ICategorize)]).GetAndClearRegisteredMethods()
                              select new TestSetEntry
                              {
                                  TestClass = cls.Type,
                                  TestConstructor = cls.Constructor,
                                  TestMethod = method.Method,
                                  RunTestMethod = z => method()
                              }).ToList()
                }
            };
        }
    }
}
