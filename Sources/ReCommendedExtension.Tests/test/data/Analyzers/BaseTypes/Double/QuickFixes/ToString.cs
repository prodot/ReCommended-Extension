using System;

namespace Test
{
    public class Doubles
    {
        public void ToString(double number)
        {
            var result = number.ToString(format: "R{caret}");
        }
    }
}