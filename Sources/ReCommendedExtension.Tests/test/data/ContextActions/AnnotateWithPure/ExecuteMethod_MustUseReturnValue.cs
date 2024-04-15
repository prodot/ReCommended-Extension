using JetBrains.Annotations;

namespace Test
{
    internal class ExecuteAnnotatedMethod
    {
        [MustUseReturnValue]
        string Meth{caret}od() => null;
    }
}