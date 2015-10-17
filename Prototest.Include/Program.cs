using System;
using System.Reflection;

namespace Prototest.Include
{
    public static class Program
    {
        public static void Main(string[] args)
        {
            Prototest.Library.Runner.Run(Assembly.GetExecutingAssembly());
        }
    }
}

