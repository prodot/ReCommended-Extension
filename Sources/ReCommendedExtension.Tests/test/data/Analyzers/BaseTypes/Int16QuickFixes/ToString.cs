using System;

namespace Test
{
    public class Int16s
    {
        public void ToString(short number)
        {
            var result = number.ToString("E{caret}6");
        }
    }
}