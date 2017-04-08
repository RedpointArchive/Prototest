using System;
using System.Collections.Generic;
using System.Linq;
#if !PLATFORM_UNITY
using System.Reflection;
using System.Threading.Tasks;
#endif

namespace Prototest.Library.Version11
{
    public class Categorize : Version1.ICategorize, Version13.ICategorize
#if !PLATFORM_UNITY
        , Version14.ICategorize
#endif
    {
        private readonly List<string> _categories;

        private List<Action<object>> _registeredActions;

#if !PLATFORM_UNITY
        private List<Func<object, Task>> _registeredAsyncActions;
#endif

        public Categorize(List<string> categories)
        {
            _categories = categories;
            _registeredActions = new List<Action<object>>();
#if !PLATFORM_UNITY
            _registeredAsyncActions = new List<Func<object, Task>>();
#endif
        }

        public List<Action<object>> GetAndClearRegisteredMethods()
        {
            var copy = _registeredActions.ToList();
#if !PLATFORM_UNITY
            _registeredAsyncActions.Clear();
#endif
            _registeredActions.Clear();
            return copy;
        }

#if !PLATFORM_UNITY
        public List<Func<object, Task>> GetAndClearRegisteredAsyncMethods()
        {
            var copy = _registeredAsyncActions.ToList();
            _registeredAsyncActions.Clear();
            _registeredActions.Clear();
            return copy;
        }
#endif

        public void Method(string category, Action method)
        {
            if (_categories.Contains(category))
            {
                _registeredActions.Add(o => method());
            }
        }

        public void Method<T>(string category, Action<T> method)
        {
            if (_categories.Contains(category))
            {
                _registeredActions.Add(t => method((T) t));
            }
        }

#if !PLATFORM_UNITY
        public void MethodAsync<T>(string category, Func<T, Task> method)
        {
            if (_categories.Contains(category))
            {
                _registeredAsyncActions.Add(t => method((T)t));
            }
        }
#endif
    }
}