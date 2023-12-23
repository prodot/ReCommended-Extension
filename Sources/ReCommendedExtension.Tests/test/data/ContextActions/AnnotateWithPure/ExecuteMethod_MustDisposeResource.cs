using System;
using JetBrains.Annotations;

namespace Test
{
    internal class ExecuteAnnotatedMethod
    {
        [MustDisposeResource]
        int Meth{caret}od() => throw new NotImplementedException();
    }
}