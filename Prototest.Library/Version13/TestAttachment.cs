using System.Collections.Generic;

namespace Prototest.Library.Version13
{
    public class TestAttachment : ITestAttachment
    {
        private readonly Dictionary<string, object> _attachments = new Dictionary<string, object>();

        public void Attach(string name, object obj)
        {
            _attachments[name] = obj;
        }

        public Dictionary<string, object> GetAttachments()
        {
            return _attachments;
        }
    }
}
