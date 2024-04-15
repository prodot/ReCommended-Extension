using System;
using JetBrains.Annotations;

namespace Types
{
    class Test
    {
        [MustUseReturnValue]
        public IDisposable IDisposable{caret}Method() => throw new NotImplementedException();
    }
}