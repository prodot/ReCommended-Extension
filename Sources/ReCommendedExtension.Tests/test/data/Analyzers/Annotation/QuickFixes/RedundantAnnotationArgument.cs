using System;
using JetBrains.Annotations;

namespace Test
{
    [MustDisposeResource(tru{caret}e)]
    internal class Class : IDisposable
    {
        public void Dispose() { }
    }
}