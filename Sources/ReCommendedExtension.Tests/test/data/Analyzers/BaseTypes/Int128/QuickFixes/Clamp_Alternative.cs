using System;

namespace Test
{
    public class Int128s
    {
        public void Clamp(Int128 number)
        {
            var result = Int128.Clamp{caret}(number, 1, 0x01);
        }
    }
}