using System.Collections.Generic;

namespace Prototest.Library.Version13
{
    public interface ITestAttachment
    {
        void Attach(string name, object obj);

        Dictionary<string, object> GetAttachments();
    }
}