using System;
using System.Collections.Generic;

namespace TargetEnumerable
{
    public class NonGenericClass
    {
        void Method(int a, int b, int c)
        {
            ConsumerGeneric(new{caret}[] { a, b, c });
        }

        void ConsumerGeneric<T>(IEnumerable<T> items) { }
    }
}