using JetBrains.Annotations;

namespace Test
{
    internal class BaseInterfaceImplementation : IBase
    {
        [Pure]
        [MustUse{caret}ReturnValue]
        public int Conflicting() => 0;
    }
}