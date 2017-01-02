#if !PLATFORM_IOS && !PLATFORM_ANDROID && !PLATFORM_UNITY

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using NDesk.Options;
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

            var options = new OptionSet
            {
                {"version", s => { version = s; }}
            };

            var extra = options.Parse(args);

            var versionRunner = (from type in typeof (Runner).Assembly.GetTypes()
                where typeof (IVersionedTestRunner).IsAssignableFrom(type)
                where !type.IsAbstract && !type.IsInterface
                let inst = (IVersionedTestRunner) Activator.CreateInstance(type)
                where inst.Version == version
                select inst).FirstOrDefault();

            if (versionRunner == null)
            {
                Console.Error.WriteLine("No such versioned runner.");
                return false;
            }

            return versionRunner.Run(assembly, extra.ToArray());
        }
    }
}

#endif