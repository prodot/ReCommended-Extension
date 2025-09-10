using System;
using System.Collections.Generic;

namespace TargetEnumerable
{
    public class NonGenericClass
    {
        IEnumerable<int> field02 = ne{caret}w List<int>() { 1, 2, 3 };
    }
}