using System;

namespace Test
{
    public class Singles
    {
        public void ToString(float number)
        {
            var result = number.ToString("R{caret}");
        }
    }
}