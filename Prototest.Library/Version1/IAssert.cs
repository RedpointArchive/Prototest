using System;
using System.Collections;
using System.Collections.Generic;

namespace Prototest.Library.Version1
{
    public interface IAssert
    {
        void All<T>(IEnumerable<T> collection, Action<T> action);
        void Contains<T>(T expected, IEnumerable<T> collection);
        void Contains<T>(IEnumerable<T> collection, Predicate<T> condition);
        void DoesNotContain<T>(T value, IEnumerable<T> collection);
        void DoesNotContain<T>(IEnumerable<T> collection, Predicate<T> condition);
        void Empty(IEnumerable collection);
        void Equal<T>(T a, T b) where T : IEquatable<T>;
        void False(bool expression);
        void NotEmpty(IEnumerable collection);
        void NotEqual<T>(T a, T b) where T : IEquatable<T>;
        void NotNull(object a);
        void Null(object a);
        void Same(object a, object b);
        void True(bool expression);
    }
}