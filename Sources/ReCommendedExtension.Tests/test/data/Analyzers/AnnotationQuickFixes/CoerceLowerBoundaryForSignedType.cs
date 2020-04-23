using System;
using JetBrains.Annotations;

namespace Test
{
    internal class Types
    {
        void Method([ValueRange(-2_147_{caret}483_648L - 1, 1)] int a) { }
    }
}