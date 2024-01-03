using System;
using System.Collections.Generic;

namespace TargetEnumerable
{
    public class NonGenericClass
    {
        void Method(int a, int b, int c)
        {
            Consumer(new{caret}[] { a, b, c });
        }

        void Consumer(IEnumerable<int> items) { }
    }
}