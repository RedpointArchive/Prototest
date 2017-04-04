#if !PLATFORM_IOS && !PLATFORM_ANDROID

using System;
#if !PLATFORM_UNITY && !PLATFORM_PCL
using System.Collections.Concurrent;
#endif
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using Prototest.Library.Version1;
using Prototest.Library.Version11;

namespace Prototest.Library
{
    public static class Runner
    {
        public static bool Run(
            Assembly assembly,
            string[] args)
        {
            // Sets the default in this version.
            var version = "12";

#if !PLATFORM_UNITY
            var options = new OptionSet
            {
                {"version", s => { version = s; }}
            };

            var extra = options.Parse(args).ToArray();
#else
            var extra = new string[0];
#endif
            
            var versionRunner = (from type in GetTypesFromAssembly(GetTypeInfo(typeof(Runner)).Assembly)
                where GetTypeInfo(typeof(IVersionedTestRunner)).IsAssignableFrom(GetTypeInfo(type))
                where !GetTypeInfo(type).IsAbstract && !GetTypeInfo(type).IsInterface
                let inst = (IVersionedTestRunner) Activator.CreateInstance(type)
                where inst.Version == version
                select inst).FirstOrDefault();

            if (versionRunner == null)
            {
#if PLATFORM_PCL
                Debug.WriteLine("No such versioned runner.");
#else
                Console.Error.WriteLine("No such versioned runner.");
#endif
                return false;
            }

            return versionRunner.Run(assembly, extra);
        }

#if PLATFORM_PCL
        private static TypeInfo GetTypeInfo(Type t)
        {
            return t.GetTypeInfo();
        }

        private static Type[] GetTypesFromAssembly(Assembly a)
        {
            return a.DefinedTypes.Select(x => x.AsType()).ToArray();
        }
#else
        private static Type GetTypeInfo(Type t)
        {
            return t;
        }

        private static Type[] GetTypesFromAssembly(Assembly a)
        {
            return a.GetTypes().ToArray();
        }
#endif
    }
}

#endif