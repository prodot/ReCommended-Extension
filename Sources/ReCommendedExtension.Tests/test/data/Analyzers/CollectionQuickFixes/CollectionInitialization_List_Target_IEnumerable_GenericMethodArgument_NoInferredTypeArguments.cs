using System;
using System.Collections.Generic;

namespace TargetEnumerable
{
    public class NonGenericClass
    {
        void Method(int a, int b, int c, IEnumerable<int> seq)
        {
            ConsumerGeneric<int>(ne{caret}w List<int>(8));
        }

        void ConsumerGeneric<T>(IEnumerable<T> items) { }
    }
}