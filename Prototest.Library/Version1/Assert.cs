using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Prototest.Library.Version1
{
    internal class Assert : IAssert
    {
        public void True(bool expression)
        {
            if (!expression)
            {
                throw new PrototestFailureException("Expected true, got false");
            }
        }

        public void True(bool expression, string message)
        {
            if (!expression)
            {
                throw new PrototestFailureException("Expected true, got false: " + message);
            }
        }

        public void IsType<T>(object obj)
        {
            if (!(obj is T))
            {
                throw new PrototestFailureException("object is not of type " + typeof(T).FullName);
            }
        }

        public void NotSame(object a, object b)
        {
            if (object.ReferenceEquals(a, b))
            {
                throw new PrototestFailureException("A and B are the same object");
            }
        }

        public void Equal<T>(T[] a, T[] b) where T : IEquatable<T>
        {
            True((a == null && b == null) || (a != null && b != null));
            if (a == null || b == null)
            {
                return;
            }
            Equal(a.Length, b.Length);
            for (var i = 0; i < a.Length; i++)
            {
                Equal(a[i], b[i]);
            }
        }

        public void Equal<T>(T? a, T? b) where T : struct, IEquatable<T>
        {
            if (a == null)
            {
                if (b != null)
                {
                    throw new PrototestFailureException("A has value while B is null");
                }
            }
            else
            {
                if (b == null)
                {
                    throw new PrototestFailureException("A is null while B has value");
                }

                if (!a.Equals(b))
                {
                    throw new PrototestFailureException("A is not equal to B");
                }
            }
        }

        public void Equal<T>(T?[] a, T?[] b) where T : struct, IEquatable<T>
        {
            True((a == null && b == null) || (a != null && b != null));
            if (a == null || b == null)
            {
                return;
            }
            Equal(a.Length, b.Length);
            for (var i = 0; i < a.Length; i++)
            {
                Equal(a[i], b[i]);
            }
        }

        public void False(bool expression)
        {
            if (expression)
            {
                throw new PrototestFailureException("Expected false, got true");
            }
        }

        public void False(bool expression, string message)
        {
            if (expression)
            {
                throw new PrototestFailureException("Expected false, got true: " + message);
            }
        }

        public void All<T>(IEnumerable<T> collection, Func<T, bool> action)
        {
            if (!collection.All(action))
            {
                throw new PrototestFailureException("all elements in collection do not meet filter");
            }
        }

        public void Contains<T>(T expected, IEnumerable<T> collection)
        {
            foreach (var t in collection)
            {
                if (t == null)
                {
                    if (expected != null)
                    {
                        throw new PrototestFailureException("A has value while B is null");
                    }
                }
                else
                {
                    if (expected == null)
                    {
                        throw new PrototestFailureException("A is null while B has value");
                    }

                    if (t.Equals(expected))
                    {
                        return;
                    }
                }
            }

            throw new PrototestFailureException("expected value not found in collection");
        }

        public void Contains<T>(IEnumerable<T> collection, Predicate<T> condition)
        {
            foreach (var t in collection)
            {
                if (condition(t))
                {
                    return;
                }
            }

            throw new PrototestFailureException("expected value not found in collection");
        }

        public void Contains(string substr, string container)
        {
            if (substr == null || container == null)
            {
                throw new PrototestFailureException("substr or container is null");
            }
            if (container.IndexOf(substr, StringComparison.InvariantCulture) == -1)
            {
                throw new PrototestFailureException("substr was not found in container");
            }
        }

        public void DoesNotContain<T>(T value, IEnumerable<T> collection)
        {
            foreach (var t in collection)
            {
                if (t == null)
                {
                    if (value == null)
                    {
                        throw new PrototestFailureException("A is null and B is null");
                    }
                }
                else
                {
                    if (t.Equals(value))
                    {
                        throw new PrototestFailureException("A is equal to B");
                    }
                }
            }
        }

        public void DoesNotContain<T>(IEnumerable<T> collection, Predicate<T> condition)
        {
            foreach (var t in collection)
            {
                if (condition(t))
                {
                    return;
                }
            }

            throw new PrototestFailureException("expected value not found in collection");
        }

        public void DoesNotContain(string substr, string container)
        {
            if (substr == null || container == null)
            {
                throw new PrototestFailureException("substr or container is null");
            }
            if (container.IndexOf(substr, StringComparison.InvariantCulture) != -1)
            {
                throw new PrototestFailureException("substr was found in container");
            }
        }

        public void Empty(IEnumerable collection)
        {
            if (collection.Cast<object>().Any())
            {
                throw new PrototestFailureException("The collection is not empty");
            }
        }

        public void NotEmpty(IEnumerable collection)
        {
            if (!collection.Cast<object>().Any())
            {
                throw new PrototestFailureException("The collection is empty");
            }
        }

        public void Equal<T>(T a, T b) where T : IEquatable<T>
        {
            if (a == null)
            {
                if (b != null)
                {
                    throw new PrototestFailureException("A has value while B is null");
                }
            }
            else
            {
                if (b == null)
                {
                    throw new PrototestFailureException("A is null while B has value");
                }

                if (!a.Equals(b))
                {
                    throw new PrototestFailureException("A is not equal to B");
                }
            }
        }

        public void NotEqual<T>(T a, T b) where T : IEquatable<T>
        {
            if (a == null)
            {
                if (b == null)
                {
                    throw new PrototestFailureException("A and B are both null");
                }
            }
            else
            {
                if (b == null)
                {
                    return;
                }

                if (a.Equals(b))
                {
                    throw new PrototestFailureException("A equals B");
                }
            }
        }

        public void NotEqual<T>(T[] a, T[] b) where T : IEquatable<T>
        {
            if ((a == null && b != null) || (a != null && b == null))
            {
                return;
            }
            if (a == null && b == null)
            {
                throw new PrototestFailureException("Both A and B are null");
            }
            if (a.Length != b.Length)
            {
                return;
            }
            for (var i = 0; i < a.Length; i++)
            {
                if (a[i] == null)
                {
                    if (b[i] == null)
                    {
                    }
                }
                else
                {
                    if (b[i] == null)
                    {
                        return;
                    }

                    if (!a[i].Equals(b[i]))
                    {
                        return;
                    }
                }
            }
            throw new PrototestFailureException("Both A and B are equal arrays");
        }

        public void Same(object a, object b)
        {
            if (!object.ReferenceEquals(a, b))
            {
                throw new PrototestFailureException("A and B are not the same object");
            }
        }

        public void Null(object a)
        {
            if (a != null)
            {
                throw new PrototestFailureException("A is not null");
            }
        }

        public void NotNull(object a)
        {
            if (a == null)
            {
                throw new PrototestFailureException("A is null");
            }
        }
    }
}