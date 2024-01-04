using System;
using System.Collections.Generic;

namespace TargetEnumerable
{
    public class GenericClass<T> where T : new()
    {
        void Method(T a, T b, T c)
        {
            Consumer(new{caret}[] { a, b, c });
        }

        void Consumer(IEnumerable<T> items) { }
    }
}