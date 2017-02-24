namespace Prototest.Library.Version13
{
    public class ThreadControl : IThreadControl
    {
        private bool _runOnMainThread = false;

        public void RequireTestsToRunOnMainThread()
        {
            _runOnMainThread = true;
        }

        public bool GetAndClearThreadControlMarked()
        {
            var v = _runOnMainThread;
            _runOnMainThread = false;
            return v;
        }
    }
}
