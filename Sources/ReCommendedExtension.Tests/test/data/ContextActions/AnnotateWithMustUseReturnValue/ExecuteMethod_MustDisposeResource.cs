using System;
using JetBrains.Annotations;

namespace Test
{
    internal class ExecutePureMethod
    {
        [MustDisposeResource]
        IDisposable Meth{caret}od() => throw new NotImplementedException();
    }
}