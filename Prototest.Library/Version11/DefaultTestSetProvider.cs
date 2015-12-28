using System;
using System.Collections.Generic;
using System.Linq;
using Prototest.Library.Version1;

namespace Prototest.Library.Version11
{
    public class DefaultTestSetProvider : ITestSetProvider
    {
        public List<TestSet> GetTestSets(List<TestInputEntry> classes, Dictionary<Type, object> assertTypes)
        {
            return new List<TestSet>
            {
                new TestSet
                {
                    Name = "default",
                    Entries = classes.SelectMany(x => x.TestMethods.Select(
                        y => new {x.Type, x.Constructor, TestMethod = y}))
                        .Where(x => x.Constructor.GetParameters().All(z => z.ParameterType != typeof(ICategorize)))
                        .Where(x => x.TestMethod.GetParameters().Length == 0)
                        .Select(x => new TestSetEntry
                        {
                            TestClass = x.Type,
                            TestConstructor = x.Constructor,
                            TestMethod = x.TestMethod,
                            RunTestMethod = obj => ((Action) Delegate.CreateDelegate(typeof (Action), obj, x.TestMethod))()
                        }).ToList()
                }
            };
        }
    }
}
