using System;
using System.Collections.Generic;

namespace TargetEnumerable
{
    public class NonGenericClass
    {
        IEnumerable<int> field7 = new{caret}[] { 1, 2, 3 };
    }
}