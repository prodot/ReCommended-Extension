using System;
using System.Collections.Generic;

namespace TargetEnumerable
{
    public class NonGenericClass
    {
        void Method(int a, int b, int c, IEnumerable<int> seq)
        {
            IEnumerable<int> var06 = n{caret}ew List<int>(seq) { a, b, c };
        }
    }
}