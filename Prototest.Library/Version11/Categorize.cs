using System;
using System.Collections.Generic;
using System.Linq;

namespace Prototest.Library.Version11
{
    public class Categorize : Version1.ICategorize, Version13.ICategorize
    {
        private readonly List<string> _categories;

        private List<Action<object>> _registeredActions; 

        public Categorize(List<string> categories)
        {
            _categories = categories;
            _registeredActions = new List<Action<object>>();
        }

        public List<Action<object>> GetAndClearRegisteredMethods()
        {
            var copy = _registeredActions.ToList();
            _registeredActions.Clear();
            return copy;
        }

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
    }
}