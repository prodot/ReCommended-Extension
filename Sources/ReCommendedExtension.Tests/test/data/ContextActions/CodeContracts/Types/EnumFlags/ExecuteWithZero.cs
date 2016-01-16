using System;

namespace Test
{
    internal class ExecuteWithZero
    {
        [Flags]
        enum Numbers
        {
            None = 0,
            One = 1,
            Two = 2,
            Four = 4,
        }

        void Method(Numbers one{caret}) { }
    }
}