using System;

namespace Test
{
    public class UInt128s
    {
        public void Clamp(UInt128 number)
        {
            var result = UInt128.Clamp{caret}(number, 1, 0x01);
        }
    }
}