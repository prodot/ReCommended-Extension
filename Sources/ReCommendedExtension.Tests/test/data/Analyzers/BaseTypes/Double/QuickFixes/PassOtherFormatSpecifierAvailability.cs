using System;
using System.Globalization;

namespace Test
{
    public class Doubles
    {
        public void ToString(double number, IFormatProvider provider)
        {
            var result11 = number.ToString("R");
            var result12 = number.ToString("R3");
            var result13 = number.ToString("r");
            var result14 = number.ToString("r3");

            var result21 = number.ToString("R", provider);
            var result22 = number.ToString("R3", provider);
            var result23 = number.ToString("r", provider);
            var result24 = number.ToString("r3", provider);
        }
    }
}