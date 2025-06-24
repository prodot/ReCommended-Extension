using System;

namespace Test
{
    public class UInt128s
    {
        public void Max()
        {
            var result = UInt128.Max{caret}(10, 10);
        }
    }
}