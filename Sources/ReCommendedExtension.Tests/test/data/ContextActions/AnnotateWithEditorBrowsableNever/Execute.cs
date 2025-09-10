using System;

namespace Test
{
    public class SomeClass
    {
        public void Deconstruc{caret}t(out int x, out bool y)
        {
            x = 1;
            y = true;
        }
    }
}