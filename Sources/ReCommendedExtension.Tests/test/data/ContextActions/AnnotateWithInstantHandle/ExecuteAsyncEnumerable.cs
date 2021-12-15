using System;
using System.Collections.Generic;

namespace Test
{
    internal class ExecuteGenericEnumerable
    {
        void Method(IAsyncEnumerable<int> o{caret}ne) { }
    }
}