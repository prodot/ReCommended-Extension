using System;
using System.Collections.Generic;

namespace TargetEnumerable
{
    public class NonGenericClass
    {
        void Method(int a, int b, int c)
        {
            ConsumerGeneric<int>(new int{caret}[0] { });
        }

        void ConsumerGeneric<T>(IEnumerable<T> items) { }
    }
}