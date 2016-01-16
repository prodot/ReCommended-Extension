using System;

namespace Test
{
    internal class ExecuteTwoMembers
    {
        enum NumberWithTwoMembers
        {
            One,

            [Obsolete]
            Two,

            Three,
        }

        void Method(NumberWithTwoMembers two{caret}) { }
    }
}