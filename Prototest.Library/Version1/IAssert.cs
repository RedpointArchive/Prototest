using System;
using System.Collections;
using System.Collections.Generic;

namespace Prototest.Library.Version1
{
    public interface IAssert
    {
        void All<T>(IEnumerable<T> collection, Func<T, bool> action);
        void Contains<T>(T expected, IEnumerable<T> collection);
        void Contains<T>(IEnumerable<T> collection, Predicate<T> condition);
        void Contains(string substr, string container);
        void DoesNotContain<T>(T value, IEnumerable<T> collection);
        void DoesNotContain<T>(IEnumerable<T> collection, Predicate<T> condition);
        void DoesNotContain(string substr, string container);
        void Empty(IEnumerable collection);
        void Equal<T>(T a, T b) where T : IEquatable<T>;
        void Equal<T>(T[] a, T[] b) where T : IEquatable<T>;
        void False(bool expression);
        void False(bool expression, string message);
        void NotEmpty(IEnumerable collection);
        void NotEqual<T>(T a, T b) where T : IEquatable<T>;
        void NotEqual<T>(T[] a, T[] b) where T : IEquatable<T>;
        void NotNull(object a);
        void Null(object a);
        void Same(object a, object b);
        void True(bool expression);
        void True(bool expression, string message);
    }
}