using System;
using System.Collections.Generic;

namespace TargetEnumerable
{
    public class NonGenericClass
    {
        IEnumerable<int> field01 = ne{caret}w List<int>();
    }
}