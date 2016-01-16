using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Test
{
    internal class ExecuteLazyOnDelegate
    {
        delegate Lazy<string> Call{caret}back();
    }
}