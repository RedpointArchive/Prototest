using System;

namespace Prototest.Library
{
    public class PrototestIsTypeFailureException : PrototestFailureException
    {
        /// <summary>
        /// The type that was checked.
        /// </summary>
        public Type Type { get; set; }

        /// <summary>
        /// The object that was not of the specified type.
        /// </summary>
        public object Object { get; set; }

        public PrototestIsTypeFailureException(Type t, object o)
            : base("Object '" + o + "' is not of type " + t.FullName)
        {
            Type = t;
            Object = o;
        }
    }
}
