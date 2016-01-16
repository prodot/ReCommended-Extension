using System;
using System.Collections.Generic;

namespace Test
{
    internal class ExecuteGenericEnumerable
    {
        void Method(IEnumerable<int> o{caret}ne) { }
    }
}