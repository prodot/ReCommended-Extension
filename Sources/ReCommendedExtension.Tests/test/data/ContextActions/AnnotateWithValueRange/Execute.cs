using JetBrains.Annotations;

namespace Test
{
    internal class Types
    {
        void Method([ValueRange(1,5)] int {caret}arg) { }
    }
}