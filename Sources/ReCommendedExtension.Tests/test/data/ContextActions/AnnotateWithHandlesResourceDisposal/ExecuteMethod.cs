using System;
using System.Threading.Tasks;

namespace Test
{
    internal class DisposableClass : IDisposable
    {
        public void Dispose() { }

        public void Clo{caret}se() { }
    }
}