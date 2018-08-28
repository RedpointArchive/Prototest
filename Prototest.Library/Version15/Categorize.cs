#if !PLATFORM_UNITY && !PLATFORM_PCL

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace Prototest.Library.Version15
{
    public class Categorize : Version1.ICategorize, Version13.ICategorize, Version14.ICategorize, Version15.ICategorizeClearable
    {
        private List<Action<object>> _registeredActions;
        
        private List<Func<object, Task>> _registeredAsyncActions;

        public Categorize()
        {
            _registeredActions = new List<Action<object>>();
            _registeredAsyncActions = new List<Func<object, Task>>();
        }

        public List<Action<object>> GetAndClearRegisteredMethods()
        {
            var copy = _registeredActions.ToList();
            _registeredAsyncActions.Clear();
            _registeredActions.Clear();
            return copy;
        }
        
        public List<Func<object, Task>> GetAndClearRegisteredAsyncMethods()
        {
            var copy = _registeredAsyncActions.ToList();
            _registeredAsyncActions.Clear();
            _registeredActions.Clear();
            return copy;
        }

        public void Method(string category, Action method)
        {
            _registeredActions.Add(o => method());
        }

        public void Method<T>(string category, Action<T> method)
        {
            _registeredActions.Add(t => method((T) t));
        }
        
        public void MethodAsync<T>(string category, Func<T, Task> method)
        {
            _registeredAsyncActions.Add(t => method((T)t));
        }
    }
}

#endif