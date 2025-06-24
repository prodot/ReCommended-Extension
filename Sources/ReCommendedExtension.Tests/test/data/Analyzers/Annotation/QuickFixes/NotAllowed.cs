using JetBrains.Annotations;

namespace Test
{
    internal interface IBase
    {
        string NotAllowed();
    }

    internal class BaseInterfaceImplementation : IBase
    {
        [Not{caret}Null]
        public string NotAllowed() => "";
    }
}