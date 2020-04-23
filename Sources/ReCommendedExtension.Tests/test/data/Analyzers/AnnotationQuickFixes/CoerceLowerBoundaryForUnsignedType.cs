using System;
using JetBrains.Annotations;

namespace Test
{
    internal class Types
    {
        void Method([ValueRange(-1{caret} * 10, 1)] uint a) { }
    }
}