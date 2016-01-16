using System;

namespace Test
{
    internal class ExecuteWithZeroSingleMember
    {
        [Flags]
        enum NumbersWithZeroSingleMember
        {
            Zero = 0,
        }

        void Method(NumbersWithZeroSingleMember three{caret}) { }
    }
}