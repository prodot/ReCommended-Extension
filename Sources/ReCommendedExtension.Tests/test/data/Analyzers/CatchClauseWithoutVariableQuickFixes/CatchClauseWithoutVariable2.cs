using System;

namespace Test
{
    internal class CatchClauseWithoutVariable
    {
        void Method()
        {
            try { }
            catch ({caret}Exception) when(true) { }}
        }
    }
}