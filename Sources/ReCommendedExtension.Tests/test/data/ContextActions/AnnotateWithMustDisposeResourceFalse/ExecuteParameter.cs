using System;

namespace Types
{
    class Test
    {
        public void MethodOutput(out IDisposable p{caret}1) => throw new NotImplementedException();
    }
}