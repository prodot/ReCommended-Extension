using System;

namespace Test
{
    public class Int64s
    {
        public void ToString(long number)
        {
            var result = number.ToString("E{caret}6");
        }
    }
}