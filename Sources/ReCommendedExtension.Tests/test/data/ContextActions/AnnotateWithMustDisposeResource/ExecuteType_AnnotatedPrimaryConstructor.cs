using System;

namespace Types
{
    [method: MustDisposeResource]
    class Disposable{caret}Class() : IDisposable
    {
        public void Dispose() { }
    }
}