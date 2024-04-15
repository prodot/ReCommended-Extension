using System;
using System.Collections.Generic;

namespace TargetEnumerable
{
    public class GenericClass<T> where T : new()
    {
        void Method(T a, T b, T c)
        {
            IEnumerable<T> var7 = new{caret}[] { a, b, c };
        }
    }
}