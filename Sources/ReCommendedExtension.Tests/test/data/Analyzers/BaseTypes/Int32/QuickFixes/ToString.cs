using System;

namespace Test
{
    public class Int32s
    {
        public void ToString(int number)
        {
            var result = number.ToString("E{caret}6");
        }
    }
}