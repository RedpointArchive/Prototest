using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq.Expressions;
#if !PLATFORM_UNITY
using System.Threading.Tasks;
#endif

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
        void Equal<T>(T? a, T? b) where T : struct, IEquatable<T>;
        void Equal<T>(T?[] a, T?[] b) where T : struct, IEquatable<T>;
        void True(bool expression);
        void True(bool expression, string message);
        void True(Expression<Func<bool>> expression);
        void True(Expression<Func<bool>> expression, string message);
        void False(bool expression);
        void False(bool expression, string message);
        void False(Expression<Func<bool>> expression);
        void False(Expression<Func<bool>> expression, string message);
        void NotEmpty(IEnumerable collection);
        void NotEqual<T>(T a, T b) where T : IEquatable<T>;
        void NotEqual<T>(T[] a, T[] b) where T : IEquatable<T>;
        void NotNull(object a);
        void NotNull(object a, string message);
        void Null(object a);
        void Null(object a, string message);
        void Same(object a, object b);
        void IsType<T>(object obj);
        void IsType(Type type, object obj);
        void NotSame(object a, object b);
        Exception Throws(Action code);
        T Throws<T>(Action code) where T : Exception;
        Exception Throws(Action code, string message);
        T Throws<T>(Action code, string message) where T : Exception;
        void DoesNotThrow(Action code);
        void DoesNotThrow<T>(Action code) where T : Exception;
        void DoesNotThrow(Action code, string message);
        void DoesNotThrow<T>(Action code, string message) where T : Exception;
#if !PLATFORM_UNITY
        [Obsolete("This method can introduce deadlocks because it is not awaitable; use ThrowsAsync instead.")]
        Exception Throws(Func<Task> code);
        [Obsolete("This method can introduce deadlocks because it is not awaitable; use ThrowsAsync instead.")]
        T Throws<T>(Func<Task> code) where T : Exception;
        [Obsolete("This method can introduce deadlocks because it is not awaitable; use ThrowsAsync instead.")]
        Exception Throws(Func<Task> code, string message);
        [Obsolete("This method can introduce deadlocks because it is not awaitable; use ThrowsAsync instead.")]
        T Throws<T>(Func<Task> code, string message) where T : Exception;
        [Obsolete("This method can introduce deadlocks because it is not awaitable; use DoesNotThrowAsync instead.")]
        void DoesNotThrow(Func<Task> code);
        [Obsolete("This method can introduce deadlocks because it is not awaitable; use DoesNotThrowAsync instead.")]
        void DoesNotThrow<T>(Func<Task> code) where T : Exception;
        [Obsolete("This method can introduce deadlocks because it is not awaitable; use DoesNotThrowAsync instead.")]
        void DoesNotThrow(Func<Task> code, string message);
        [Obsolete("This method can introduce deadlocks because it is not awaitable; use DoesNotThrowAsync instead.")]
        void DoesNotThrow<T>(Func<Task> code, string message) where T : Exception;
        Task<Exception> ThrowsAsync(Func<Task> code);
        Task<T> ThrowsAsync<T>(Func<Task> code) where T : Exception;
        Task<Exception> ThrowsAsync(Func<Task> code, string message);
        Task<T> ThrowsAsync<T>(Func<Task> code, string message) where T : Exception;
        Task DoesNotThrowAsync(Func<Task> code);
        Task DoesNotThrowAsync<T>(Func<Task> code) where T : Exception;
        Task DoesNotThrowAsync(Func<Task> code, string message);
        Task DoesNotThrowAsync<T>(Func<Task> code, string message) where T : Exception;
#endif
    }
}