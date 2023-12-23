using System;
using JetBrains.Annotations;

namespace Types
{
    class Test
    {
        [Pure]
        [MustUseReturnValue]
        [MustDisposeResource(false)]
        public IDisposable IDisposable{caret}Method() => throw new NotImplementedException();
    }
}