using System;

namespace Test
{
    internal class Types
    {
        void Method()
        {
            Action<int> lambda = fi{caret}rst => { };
        }
    }
}