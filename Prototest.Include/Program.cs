using System;
using System.Reflection;

namespace Prototest.Include
{
    public static class Program
    {
        public static void Main(string[] args)
        {
            if (Prototest.Library.Runner.Run(Assembly.GetExecutingAssembly()))
            {
                Environment.Exit(0);
            }

            Environment.Exit(1);
        }
    }
}

