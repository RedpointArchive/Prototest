using System;
using System.Collections.Generic;
using System.Linq;

namespace Prototest.Library.Version11
{
    public class DefaultTestSetProvider : ITestSetProvider
    {
        public List<TestSet> GetTestSets(List<TestInputEntry> classes)
        {
            return new List<TestSet>
            {
                new TestSet
                {
                    Name = "default",
                    Entries = classes.SelectMany(x => x.TestMethods.Select(
                        y => new {x.Type, x.Constructor, TestMethod = y}))
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
