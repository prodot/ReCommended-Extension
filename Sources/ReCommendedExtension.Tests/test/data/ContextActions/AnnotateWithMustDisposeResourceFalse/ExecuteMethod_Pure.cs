using System;
using JetBrains.Annotations;

namespace Types
{
    class Test
    {
        [Pure]
        public IDisposable IDisposable{caret}Method() => throw new NotImplementedException();
    }
}