using System;

namespace Test
{
    public class SBytes
    {
        public void ToString(sbyte number)
        {
            var result = number.ToString("E{caret}6");
        }
    }
}