#if !PLATFORM_UNITY && !PLATFORM_PCL

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Prototest.Library.Version1;
using Prototest.Library.Version12;
using Prototest.Library.Version13;

namespace Prototest.Library.Version15
{
    public class DefaultTestDiscoverer : ITestDiscoverer
    {
        public TestExecutionSet Discover(Assembly assembly, ITestConnector connector, TestSettings settings)
        {
            var assert = new Assert();
            var threadControl = new ThreadControl();
            var categorize = new Categorize();
            var assertTypes = new Dictionary<Type, Func<object>>
            {
                {typeof(IAssert), () => assert},
                {typeof(Version1.ICategorize), () => categorize},
                {typeof(Version13.ICategorize), () => categorize},
                {typeof(Version14.ICategorize), () => categorize},
                {typeof(IThreadControl), () => threadControl},
                {typeof(ITestAttachment), () => new TestAttachment()}
            };

            var testClasses = new List<Version11.TestInputEntry>();
            var testContextTypes = new List<Type>();

            var types = assembly.GetTypes();
            foreach (var type in types)
            {
                var typeInfo = type;

                if (typeInfo.IsAbstract || typeInfo.IsInterface)
                {
                    continue;
                }

                var constructors = typeInfo.GetConstructors();
                if (constructors.Length != 1)
                {
                    continue;
                }
                
                var constructor = constructors.First();
                var parameters = constructor.GetParameters();
                if (parameters.Length > 0 && parameters.All(x => assertTypes.Keys.Contains(x.ParameterType)))
                {
                    var testClass = new Version11.TestInputEntry
                    {
                        Constructor = constructor,
                        TestMethods = new List<MethodInfo>(),
                        Type = type
                    };

                    testClass.TestMethods =
                        type.GetMethods(BindingFlags.Instance | BindingFlags.Public | BindingFlags.DeclaredOnly)
                            .ToList();

                    testClasses.Add(testClass);
                }
                else if (parameters.Length == 0 && typeof(ITestRunContext).IsAssignableFrom(type))
                {
                    testContextTypes.Add(type);
                }
            }

            // Search dependencies for test run context.
            foreach (var dependencyAssemblyName in assembly.GetReferencedAssemblies())
            {
                var dependencyAssembly = Assembly.Load(dependencyAssemblyName);
                foreach (var type in dependencyAssembly.GetTypes())
                {
                    var typeInfo = type;

                    if (typeInfo.IsAbstract || typeInfo.IsInterface)
                    {
                        continue;
                    }

                    var constructors = typeInfo.GetConstructors();
                    if (constructors.Length != 1)
                    {
                        continue;
                    }

                    var constructor = constructors.First();
                    var parameters = constructor.GetParameters();
                    if (parameters.Length == 0 && typeof(ITestRunContext).IsAssignableFrom(type))
                    {
                        testContextTypes.Add(type);
                    }
                }
            }

            if (connector != null)
            {
                connector.InitTestClassesFound(testClasses.Count);
                connector.InitTestMethodsFound(testClasses.Sum(x => x.TestMethods.Count));
            }

            IEnumerable<Type> setTypes = types;

            if (!settings.NoDefaultSet)
            {
                setTypes = types.Concat(GetTypesFromAssembly(GetTypeInfo(typeof(DefaultTestRunner)).Assembly));
            }

            var sets11 = new List<Version11.TestSet>();
            var sets14 = new List<Version14.TestSet>();
            foreach (
                var type in
                    setTypes
                        .Where(x => GetTypeInfo(typeof(Version11.ITestSetProvider)).IsAssignableFrom(GetTypeInfo(x)) && !GetTypeInfo(x).IsInterface && !GetTypeInfo(x).IsAbstract))
            {
                var provider = (Version11.ITestSetProvider)Activator.CreateInstance(type);
                sets11.AddRange(provider.GetTestSets(testClasses, assertTypes.ToDictionary(k => k.Key, v => v.Value())));
            }
            foreach (
                var type in
                    setTypes
                        .Where(x => GetTypeInfo(typeof(Version13.ITestSetProvider)).IsAssignableFrom(GetTypeInfo(x)) && !GetTypeInfo(x).IsInterface && !GetTypeInfo(x).IsAbstract))
            {
                var provider = (Version13.ITestSetProvider)Activator.CreateInstance(type);
                sets11.AddRange(provider.GetTestSets(testClasses, assertTypes));
            }
            foreach (
                var type in
                    setTypes
                        .Where(x => GetTypeInfo(typeof(Version14.ITestSetProvider)).IsAssignableFrom(GetTypeInfo(x)) && !GetTypeInfo(x).IsInterface && !GetTypeInfo(x).IsAbstract))
            {
                var provider = (Version14.ITestSetProvider)Activator.CreateInstance(type);
                sets14.AddRange(provider.GetTestSets(testClasses, assertTypes));
            }

            if (connector != null)
            {
                connector.InitTestEntriesFound(sets11.Sum(x => x.Entries.Count));
            }

            return new TestExecutionSet
            {
                Settings = settings,
                Version11Sets = sets11,
                Version14Sets = sets14,
                Assert = assert,
                ThreadControl = threadControl,
                Categorize = categorize,
                AssertTypes = assertTypes,
                AllTypes = types,
                TestRunContextTypes = testContextTypes,
            };
        }

        private static Type GetTypeInfo(Type t)
        {
            return t;
        }

        private static Type[] GetTypesFromAssembly(Assembly a)
        {
            return a.GetTypes().ToArray();
        }
    }
}

#endif