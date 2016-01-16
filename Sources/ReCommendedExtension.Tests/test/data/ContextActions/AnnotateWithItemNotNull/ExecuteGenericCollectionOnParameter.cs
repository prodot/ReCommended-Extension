using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Test
{
    internal class ExecuteGenericCollectionOnParameter
    {
        void Method(ICollection<string> t{caret}wo) { }
    }
}