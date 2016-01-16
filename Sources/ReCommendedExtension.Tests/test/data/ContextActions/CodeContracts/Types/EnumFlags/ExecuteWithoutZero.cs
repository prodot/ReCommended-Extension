using System;

namespace Test
{
    internal class ExecuteWithoutZero
    {
        [Flags]
        enum NumbersWithoutZero
        {
            One = 1,
            Two = 2,
            Four = 4,
            Eight = 8,
        }

        void Available(NumbersWithoutZero two{caret}) { }
    }
}