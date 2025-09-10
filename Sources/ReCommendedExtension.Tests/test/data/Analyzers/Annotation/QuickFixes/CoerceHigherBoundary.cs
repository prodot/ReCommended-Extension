using System;
using JetBrains.Annotations;

namespace Test
{
    internal class Types
    {
        void Method([ValueRange(10, 0x{caret}FFFF)] byte a) { }
    }
}