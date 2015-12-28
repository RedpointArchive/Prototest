using System;
using System.Collections.Generic;
using System.Linq;
using Prototest.Library.Version1;

namespace Prototest.Library.Version11
{
    public class Categorize : ICategorize
    {
        private readonly List<string> _categories;

        private List<Action> _registeredActions; 

        public Categorize(List<string> categories)
        {
            _categories = categories;
            _registeredActions = new List<Action>();
        }

        public List<Action> GetAndClearRegisteredMethods()
        {
            var copy = _registeredActions.ToList();
            _registeredActions.Clear();
            return copy;
        }

        public void Method(string category, Action method)
        {
            if (_categories.Contains(category))
            {
                _registeredActions.Add(method);
            }
        }
    }
}