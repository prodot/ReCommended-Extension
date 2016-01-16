using System;

namespace Test
{
    internal class Availability
    {
        enum NumberWithFourMembers
        {
            One,
            Two,
            Three,
            Four,
        }

        enum NumberWithTwoMembers
        {
            One,

            [Obsolete]
            Two,

            Three,
        }

        [Flags]
        enum Flagged
        {
            One = 0,
            Two = 1,
            Three = 2,
        }

        enum Empty { }

        void Available(NumberWithFourMembers one{on}, NumberWithTwoMembers two{on}) { }

        void NotAvailable(Flagged one{off}, Empty two{off}, int three{off}) { }
    }
}