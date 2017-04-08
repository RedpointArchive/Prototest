using System;
using System.Collections;
using System.Linq;
using System.Linq.Expressions;

namespace Prototest.Library
{
    public static class PrototestValueFormatter
    {
        public static string Format(object value)
        {
            if (value == null)
            {
                return "(null)";
            }

            if (value is string)
            {
                return "'" + value + "'";
            }

            if (value is ValueType)
            {
                return value.ToString();
            }

            if (value is Array)
            {
                var arr = (Array) value;
                if (arr.Length < 10)
                {
                    return "[" + arr.OfType<object>().Select(Format).Aggregate((a, b) => a + ", " + b) + "]";
                }

                return "[" + arr.OfType<object>().Take(3).Select(Format).Aggregate((a, b) => a + ", " + b) + ", ...]";
            }

            if (value is ICollection)
            {
                var coll = (ICollection)value;
                if (coll.Count < 10)
                {
                    return "[" + coll.OfType<object>().Select(Format).Aggregate((a, b) => a + ", " + b) + "]";
                }

                return "[" + coll.OfType<object>().Take(3).Select(Format).Aggregate((a, b) => a + ", " + b) + ", ...]";
            }

            if (value is IEnumerable)
            {
                var en = (IEnumerable)value;
                return "[" + en.OfType<object>().Take(3).Select(Format).Aggregate((a, b) => a + ", " + b) + ", ...]";
            }

            if (value is Expression)
            {
                return "{ " + value + " }";
            }

            return value.ToString();
        }
    }
}
