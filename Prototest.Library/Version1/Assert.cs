using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
#if !PLATFORM_UNITY
using System.Threading.Tasks;
#endif
#if PLATFORM_PCL
using System.Reflection;
#endif

namespace Prototest.Library.Version1
{
    internal class Assert : IAssert
    {
        public void True(bool expression)
        {
            if (!expression)
            {
                throw new PrototestTruthFailureException();
            }
        }

        public void True(bool expression, string message)
        {
            if (!expression)
            {
                throw new PrototestTruthFailureException(message);
            }
        }

        public void True(Expression<Func<bool>> expression)
        {
            if (!expression.Compile()())
            {
                throw new PrototestTruthFailureException(expression);
            }
        }

        public void True(Expression<Func<bool>> expression, string message)
        {
            if (!expression.Compile()())
            {
                throw new PrototestTruthFailureException(expression, message);
            }
        }

        public void False(bool expression)
        {
            if (expression)
            {
                throw new PrototestFalsityFailureException();
            }
        }

        public void False(bool expression, string message)
        {
            if (expression)
            {
                throw new PrototestFalsityFailureException(message);
            }
        }

        public void False(Expression<Func<bool>> expression)
        {
            if (expression.Compile()())
            {
                throw new PrototestFalsityFailureException(expression);
            }
        }

        public void False(Expression<Func<bool>> expression, string message)
        {
            if (expression.Compile()())
            {
                throw new PrototestFalsityFailureException(expression, message);
            }
        }

        public void IsType<T>(object obj)
        {
            if (!(obj is T))
            {
                throw new PrototestIsTypeFailureException(typeof(T), obj);
            }
        }

        public void IsType(Type type, object obj)
        {
#if PLATFORM_PCL
            if (obj == null || type == null || !type.GetTypeInfo().IsAssignableFrom(obj.GetType().GetTypeInfo()))
#else
            if (type == null || !type.IsInstanceOfType(obj))
#endif
            {
                throw new PrototestIsTypeFailureException(type, obj);
            }
        }

        public void NotSame(object a, object b)
        {
            if (object.ReferenceEquals(a, b))
            {
                throw new PrototestReferenceInequalityFailureException(a, b);
            }
        }

        public void Equal<T>(T[] a, T[] b) where T : IEquatable<T>
        {
            if (!((a == null && b == null) || (a != null && b != null)))
            {
                throw new PrototestEqualityFailureException(a, b);
            }
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
                    throw new PrototestEqualityFailureException(a, b);
                }
            }
            else
            {
                if (b == null)
                {
                    throw new PrototestEqualityFailureException(a, b);
                }

                if (!a.Equals(b))
                {
                    throw new PrototestEqualityFailureException(a, b);
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

        public void All<T>(IEnumerable<T> collection, Func<T, bool> action)
        {
            var enumeratedCollection = collection.ToList();
            if (!enumeratedCollection.All(action))
            {
                throw new PrototestAllFilterFailureException<T>(enumeratedCollection);
            }
        }

        public void All<T>(IEnumerable<T> collection, Expression<Func<T, bool>> filter)
        {
            var action = filter.Compile();
            var enumeratedCollection = collection.ToList();
            if (!enumeratedCollection.All(action))
            {
                throw new PrototestAllFilterFailureException<T>(enumeratedCollection, filter);
            }
        }

        public void Contains<T>(T expected, IEnumerable<T> collection)
        {
            var enumeratedCollection = collection.ToList();
            if (!enumeratedCollection.Contains(expected))
            {
                throw new PrototestContainsFailureException<T>(enumeratedCollection, expected);
            }
        }

        public void Contains<T>(IEnumerable<T> collection, Predicate<T> condition)
        {
            var enumeratedCollection = collection.ToList();
            foreach (var t in enumeratedCollection)
            {
                if (condition(t))
                {
                    return;
                }
            }

            throw new PrototestContainsFailureException<T>(enumeratedCollection, condition);
        }

        public void Contains(string substr, string container)
        {
            if (substr == null || container == null)
            {
                throw new PrototestStringContainsFailureException(substr, container);
            }
#if PLATFORM_PCL
            if (container.IndexOf(substr, StringComparison.Ordinal) == -1)
#else
            if (container.IndexOf(substr, StringComparison.InvariantCulture) == -1)
#endif
            {
                throw new PrototestStringContainsFailureException(substr, container);
            }
        }

        public void DoesNotContain<T>(T value, IEnumerable<T> collection)
        {
            var enumeratedCollection = collection.ToList();
            if (enumeratedCollection.Contains(value))
            {
                throw new PrototestDoesNotContainFailureException<T>(enumeratedCollection, value);
            }
        }

        public void DoesNotContain<T>(IEnumerable<T> collection, Predicate<T> condition)
        {
            var enumeratedCollection = collection.ToList();
            foreach (var t in enumeratedCollection)
            {
                if (condition(t))
                {
                    return;
                }
            }

            throw new PrototestDoesNotContainFailureException<T>(enumeratedCollection, condition);
        }

        public void DoesNotContain(string substr, string container)
        {
            if (substr == null || container == null)
            {
                throw new PrototestStringDoesNotContainFailureException(substr, container);
            }
#if PLATFORM_PCL
            if (container.IndexOf(substr, StringComparison.Ordinal) != -1)
#else
            if (container.IndexOf(substr, StringComparison.InvariantCulture) != -1)
#endif
            {
                throw new PrototestStringDoesNotContainFailureException(substr, container);
            }
        }

        public void Empty(IEnumerable collection)
        {
            var enumeratedCollection = collection.Cast<object>().ToList();
            if (enumeratedCollection.Any())
            {
                throw new PrototestEmptyFailureException(enumeratedCollection);
            }
        }

        public void NotEmpty(IEnumerable collection)
        {
            var enumeratedCollection = collection.Cast<object>().ToList();
            if (!enumeratedCollection.Any())
            {
                throw new PrototestNotEmptyFailureException(enumeratedCollection);
            }
        }

        public void Equal<T>(T a, T b) where T : IEquatable<T>
        {
            if (a == null)
            {
                if (b != null)
                {
                    throw new PrototestEqualityFailureException(a, b);
                }
            }
            else
            {
                if (b == null)
                {
                    throw new PrototestEqualityFailureException(a, b);
                }

                if (!a.Equals(b))
                {
                    throw new PrototestEqualityFailureException(a, b);
                }
            }
        }

        public void NotEqual<T>(T a, T b) where T : IEquatable<T>
        {
            if (a == null)
            {
                if (b == null)
                {
                    throw new PrototestInequalityFailureException(a, b);
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
                    throw new PrototestInequalityFailureException(a, b);
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
                throw new PrototestInequalityFailureException(a, b);
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

            throw new PrototestInequalityFailureException(a, b);
        }

        public void Same(object a, object b)
        {
            if (!object.ReferenceEquals(a, b))
            {
                throw new PrototestReferenceEqualityFailureException(a, b);
            }
        }

        public void Null(object a)
        {
            if (a != null)
            {
                throw new PrototestNullFailureException(a);
            }
        }

        public void Null(object a, string message)
        {
            if (a != null)
            {
                throw new PrototestNullFailureException(a, message);
            }
        }

        public void NotNull(object a)
        {
            if (a == null)
            {
                throw new PrototestNotNullFailureException();
            }
        }

        public void NotNull(object a, string message)
        {
            if (a == null)
            {
                throw new PrototestNotNullFailureException(message);
            }
        }

        public Exception Throws(Action code)
        {
            try
            {
                code();
                throw new PrototestThrowsFailureException();
            }
            catch (Exception ex)
            {
                // Expected
                return ex;
            }
        }

        public Exception Throws(Action code, string message)
        {
            try
            {
                code();
                throw new PrototestThrowsFailureException(message);
            }
            catch (Exception ex)
            {
                // Expected
                return ex;
            }
        }

        public T Throws<T>(Action code, string message) where T : Exception
        {
            try
            {
                code();
                throw new PrototestThrowsFailureException(typeof(T), message);
            }
            catch (T ex)
            {
                // Expected
                return ex;
            }
            catch (Exception ex)
            {
                throw new PrototestThrowsFailureException(typeof(T), ex, message);
            }
        }

        public T Throws<T>(Action code) where T : Exception
        {
            try
            {
                code();
                throw new PrototestThrowsFailureException(typeof(T));
            }
            catch (T ex)
            {
                // Expected
                return ex;
            }
            catch (Exception ex)
            {
                throw new PrototestThrowsFailureException(typeof(T), ex);
            }
        }

        public void DoesNotThrow(Action code)
        {
            try
            {
                code();
            }
            catch (Exception ex)
            {
                throw new PrototestDoesNotThrowFailureException(ex);
            }
        }

        public void DoesNotThrow(Action code, string message)
        {
            try
            {
                code();
            }
            catch (Exception ex)
            {
                throw new PrototestDoesNotThrowFailureException(ex, message);
            }
        }

        public void DoesNotThrow<T>(Action code) where T : Exception
        {
            try
            {
                code();
            }
            catch (T ex)
            {
                throw new PrototestDoesNotThrowFailureException(typeof(T), ex);
            }
            catch
            {
                // Ignored
            }
        }

        public void DoesNotThrow<T>(Action code, string message) where T : Exception
        {
            try
            {
                code();
            }
            catch (T ex)
            {
                throw new PrototestDoesNotThrowFailureException(typeof(T), ex, message);
            }
            catch
            {
                // Ignored
            }
        }

#if !PLATFORM_UNITY
        public Exception Throws(Func<Task> code)
        {
            var task = Task.Run(code);
            try
            {
                task.Wait();
            }
            catch (Exception ex)
            {
                var aggregateException = ex as AggregateException;
                if (aggregateException != null)
                {
                    if (aggregateException.InnerExceptions.Count > 0)
                    {
                        // Expected
                        if (aggregateException.InnerExceptions.Count == 1)
                        {
                            return aggregateException.InnerExceptions.First();
                        }

                        return aggregateException;
                    }

                    throw new PrototestThrowsFailureException();
                }
            }

            throw new PrototestThrowsFailureException();
        }

        public Exception Throws(Func<Task> code, string message)
        {
            var task = Task.Run(code);
            try
            {
                task.Wait();
            }
            catch (Exception ex)
            {
                var aggregateException = ex as AggregateException;
                if (aggregateException != null)
                {
                    if (aggregateException.InnerExceptions.Count > 0)
                    {
                        // Expected
                        if (aggregateException.InnerExceptions.Count == 1)
                        {
                            return aggregateException.InnerExceptions.First();
                        }

                        return aggregateException;
                    }

                    throw new PrototestThrowsFailureException(message);
                }
            }

            throw new PrototestThrowsFailureException(message);
        }

        public T Throws<T>(Func<Task> code) where T : Exception
        {
            var task = Task.Run(code);
            try
            {
                task.Wait();
            }
            catch (Exception ex)
            {
                var aggregateException = ex as AggregateException;
                if (aggregateException != null)
                {
                    if (aggregateException.InnerExceptions.OfType<T>().Any())
                    {
                        // Expected
                        return aggregateException.InnerExceptions.OfType<T>().First();
                    }

                    throw new PrototestThrowsFailureException(typeof(T), aggregateException.InnerExceptions.First());
                }

                var throws = ex as T;
                if (throws != null)
                {
                    // Expected
                    return throws;
                }

                throw new PrototestThrowsFailureException(typeof(T), ex);
            }
            
            throw new PrototestThrowsFailureException(typeof(T));
        }

        public T Throws<T>(Func<Task> code, string message) where T : Exception
        {
            var task = Task.Run(code);
            try
            {
                task.Wait();
            }
            catch (Exception ex)
            {
                var aggregateException = ex as AggregateException;
                if (aggregateException != null)
                {
                    if (aggregateException.InnerExceptions.OfType<T>().Any())
                    {
                        // Expected
                        return aggregateException.InnerExceptions.OfType<T>().First();
                    }

                    throw new PrototestThrowsFailureException(typeof(T), aggregateException.InnerExceptions.First(), message);
                }

                var throws = ex as T;
                if (throws != null)
                {
                    // Expected
                    return throws;
                }

                throw new PrototestThrowsFailureException(typeof(T), ex, message);
            }

            throw new PrototestThrowsFailureException(typeof(T), message);
        }

        public void DoesNotThrow(Func<Task> code)
        {
            var task = Task.Run(code);
            try
            {
                task.Wait();

                if (!task.IsFaulted || task.Exception == null)
                {
                    // Expected
                    return;
                }

                if (task.Exception.InnerExceptions.Count == 1)
                {
                    throw new PrototestDoesNotThrowFailureException(task.Exception.InnerExceptions.First());
                }

                throw new PrototestDoesNotThrowFailureException(task.Exception);
            }
            catch (AggregateException ex)
            {
                if (ex.InnerExceptions.Count == 1)
                {
                    throw new PrototestDoesNotThrowFailureException(ex.InnerExceptions.First());
                }

                throw new PrototestDoesNotThrowFailureException(ex);
            }
            catch (Exception ex)
            {
                throw new PrototestDoesNotThrowFailureException(ex);
            }
        }

        public void DoesNotThrow(Func<Task> code, string message)
        {
            var task = Task.Run(code);
            try
            {
                task.Wait();

                if (!task.IsFaulted || task.Exception == null)
                {
                    // Expected
                    return;
                }

                if (task.Exception.InnerExceptions.Count == 1)
                {
                    throw new PrototestDoesNotThrowFailureException(task.Exception.InnerExceptions.First(), message);
                }

                throw new PrototestDoesNotThrowFailureException(task.Exception, message);
            }
            catch (AggregateException ex)
            {
                if (ex.InnerExceptions.Count == 1)
                {
                    throw new PrototestDoesNotThrowFailureException(ex.InnerExceptions.First(), message);
                }

                throw new PrototestDoesNotThrowFailureException(ex, message);
            }
            catch (Exception ex)
            {
                throw new PrototestDoesNotThrowFailureException(ex, message);
            }
        }

        public void DoesNotThrow<T>(Func<Task> code) where T : Exception
        {
            var task = Task.Run(code);
            try
            {
                task.Wait();

                if (!task.IsFaulted || task.Exception == null)
                {
                    // Expected
                    return;
                }

                if (task.Exception.InnerExceptions.OfType<T>().Any())
                {
                    throw new PrototestDoesNotThrowFailureException(typeof(T),
                        task.Exception.InnerExceptions.OfType<T>().First());
                }

                // Expected
            }
            catch (AggregateException ex)
            {
                if (ex.InnerExceptions.OfType<T>().Any())
                {
                    throw new PrototestDoesNotThrowFailureException(typeof(T), ex.InnerExceptions.OfType<T>().First());
                }

                // Expected
            }
            catch (T ex)
            {
                throw new PrototestDoesNotThrowFailureException(typeof(T), ex);
            }
            catch
            {
                // Expected
            }

            // Expected
        }

        public void DoesNotThrow<T>(Func<Task> code, string message) where T : Exception
        {
            var task = Task.Run(code);
            try
            {
                task.Wait();

                if (!task.IsFaulted || task.Exception == null)
                {
                    // Expected
                    return;
                }

                if (task.Exception.InnerExceptions.OfType<T>().Any())
                {
                    throw new PrototestDoesNotThrowFailureException(typeof(T),
                        task.Exception.InnerExceptions.OfType<T>().First(), message);
                }

                // Expected
            }
            catch (AggregateException ex)
            {
                if (ex.InnerExceptions.OfType<T>().Any())
                {
                    throw new PrototestDoesNotThrowFailureException(typeof(T), ex.InnerExceptions.OfType<T>().First(), message);
                }

                // Expected
            }
            catch (T ex)
            {
                throw new PrototestDoesNotThrowFailureException(typeof(T), ex, message);
            }
            catch
            {
                // Expected
            }

            // Expected
        }
#endif
    }
}