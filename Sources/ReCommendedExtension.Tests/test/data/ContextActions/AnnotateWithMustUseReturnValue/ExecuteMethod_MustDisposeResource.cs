using System;
using JetBrains.Annotations;

namespace Test
{
    internal class ExecutePureMethod
    {
        [MustDisposeResource]
        int Meth{caret}od() => throw new NotImplementedException();
    }
}