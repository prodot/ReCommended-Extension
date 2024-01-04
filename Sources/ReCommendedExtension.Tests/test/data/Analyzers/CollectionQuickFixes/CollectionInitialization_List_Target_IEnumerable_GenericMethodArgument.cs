using System;
using System.Collections.Generic;

namespace TargetEnumerable
{
    public class NonGenericClass
    {
        void Method(int a, int b, int c, IEnumerable<int> seq)
        {
            ConsumerGeneric(n{caret}ew List<int>(seq) { a, b, c });
        }

        void ConsumerGeneric<T>(IEnumerable<T> items) { }
    }
}