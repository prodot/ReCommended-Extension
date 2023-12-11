using System;
using JetBrains.Annotations;

namespace Test
{
    internal class ExecuteAnnotatedMethod
    {
        [MustDisposeResource]
        IDisposable Meth{caret}od() => throw new NotImplementedException();
    }
}