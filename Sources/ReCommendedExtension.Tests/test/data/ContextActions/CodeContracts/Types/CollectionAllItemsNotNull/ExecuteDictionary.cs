using System;
using System.Collections;
using System.Collections.Generic;

namespace Test
{
    internal class ExecuteDictionary
    {
        void Method(IDictionary<int, string> three{caret}) { }
    }
}