using System;

namespace Prototest.Library.Version13
{
    public interface ICategorize
    {
        void Method<T>(string category, Action<T> method);
    }
}