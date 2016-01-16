using System;
using System.Collections;
using System.Collections.Generic;

namespace Test
{
    internal class ExecuteGenericCollection
    {
        void Method(ICollection<string> one{caret}) { }
    }
}