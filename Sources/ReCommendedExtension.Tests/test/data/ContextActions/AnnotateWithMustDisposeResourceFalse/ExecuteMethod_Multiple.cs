using System;
using JetBrains.Annotations;

namespace Types
{
    class Test
    {
        [Pure]
        [MustUseReturnValue]
        [MustDisposeResource]
        public IDisposable IDisposable{caret}Method() => throw new NotImplementedException();
    }
}