using JetBrains.Annotations;

namespace Test
{
    internal class ExecutePureMethod
    {
        [Pure]
        string Meth{caret}od() => null;
    }
}