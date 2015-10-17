using System;
using System.Collections;
using System.Collections.Generic;

namespace Prototest.Library.Version1
{
    public class Assert : IAssert
    {
        public void True(bool expression)
        {
            if (!expression)
            {
                throw new PrototestFailureException("Expected true, got false");
            }
        }

        public void False(bool expression)
        {
            if (expression)
            {
                throw new PrototestFailureException("Expected false, got true");
            }
        }

        public void All<T>(IEnumerable<T> collection, Action<T> action)
        {
            
        }

        public void Contains<T>(T expected, IEnumerable<T> collection)
        {
            
        }

        public void Contains<T>(IEnumerable<T> collection, Predicate<T> condition)
        {
            
        }

        public void DoesNotContain<T>(T value, IEnumerable<T> collection)
        {
            
        }

        public void DoesNotContain<T>(IEnumerable<T> collection, Predicate<T> condition)
        {
            
        }

        public void Empty(IEnumerable collection)
        {
            
        }

        public void NotEmpty(IEnumerable collection)
        {
            
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