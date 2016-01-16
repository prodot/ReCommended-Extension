using System;

namespace Test
{
    internal class ExecuteFourMembers
    {
        enum NumberWithFourMembers
        {
            One,
            Two,
            Three,
            Four,
        }

        void Method(NumberWithFourMembers one{caret}) { }
    }
}