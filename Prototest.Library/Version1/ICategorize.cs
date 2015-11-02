using System;
using System.Linq.Expressions;

namespace Prototest.Library.Version1
{
    public interface ICategorize
    {
        void Method(string category, Expression<Action> method);
    }
}