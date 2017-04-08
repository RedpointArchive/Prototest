﻿namespace Prototest.Library
{
    public class PrototestReferenceEqualityFailureException : PrototestFailureException
    {
        /// <summary>
        /// The first object that was compared.
        /// </summary>
        public object Object1 { get; set; }

        /// <summary>
        /// The second object that was compared.
        /// </summary>
        public object Object2 { get; set; }

        public PrototestReferenceEqualityFailureException(object object1, object object2)
            : base("Object " + PrototestValueFormatter.Format(object1) + " is not the same instance as " + PrototestValueFormatter.Format(object2))
        {
            Object1 = object1;
            Object2 = object2;
        }
    }
}
