using System;

namespace Test
{
    internal class Execute
    {
        enum Number
        {
            One,
            Two,
            Three,
        }

        void Method(Number one{caret}) { }
    }
}