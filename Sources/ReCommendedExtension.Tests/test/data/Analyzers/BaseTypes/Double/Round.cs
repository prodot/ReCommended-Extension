using System;

namespace Test
{
    public class Doubles
    {
        public void RedundantArgument(double x, int digits, MidpointRounding mode)
        {
            var result11 = double.Round(x, 0);
            var result12 = double.Round(x, 0, mode);
            var result13 = double.Round(x, MidpointRounding.ToEven);
            var result14 = double.Round(x, digits, MidpointRounding.ToEven);
            var result15 = double.Round(x, 0, MidpointRounding.ToEven);

            var result21 = Math.Round(x, 0);
            var result22 = Math.Round(x, 0, mode);
            var result23 = Math.Round(x, MidpointRounding.ToEven);
            var result24 = Math.Round(x, digits, MidpointRounding.ToEven);
            var result25 = Math.Round(x, 0, MidpointRounding.ToEven);
        }

        public void NoDetection(double x, int digits, MidpointRounding mode)
        {
            var result11 = double.Round(x, digits);
            var result12 = double.Round(x, mode);
            var result13 = double.Round(x, digits, mode);

            var result21 = Math.Round(x, digits);
            var result22 = Math.Round(x, mode);
            var result23 = Math.Round(x, digits, mode);
        }
    }
}