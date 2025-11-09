using System;

namespace Test
{
    public class Arguments
    {
        public void RedundantArgument(decimal n1, double n2)
        {
            var result11 = Math.Round(n1, 0);
            var result12 = Math.Round(n1, MidpointRounding.ToEven);
            var result13 = Math.Round(n1, 0, MidpointRounding.ToEven);

            var result21 = Math.Round(n2, 0);
            var result22 = Math.Round(n2, MidpointRounding.ToEven);
            var result23 = Math.Round(n2, 0, MidpointRounding.ToEven);
        }

        public void NoDetection(decimal n1, double n2, int digits, MidpointRounding rounding)
        {
            var result11 = Math.Round(n1, digits);
            var result12 = Math.Round(n1, rounding);
            var result13 = Math.Round(n1, digits, rounding);

            var result21 = Math.Round(n2, digits);
            var result22 = Math.Round(n2, rounding);
            var result23 = Math.Round(n2, digits, rounding);
        }
    }
}