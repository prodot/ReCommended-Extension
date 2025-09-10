using System;

namespace Test
{
    public class UInt32s
    {
        public void ToString(uint number)
        {
            var result = number.ToString("E{caret}6");
        }
    }
}