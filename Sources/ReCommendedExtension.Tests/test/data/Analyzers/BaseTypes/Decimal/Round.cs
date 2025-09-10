using System;

namespace Test
{
    public class Decimals
    {
        public void RedundantArgument(decimal d, int decimals, MidpointRounding mode)
        {
            var result11 = decimal.Round(d, 0);
            var result12 = decimal.Round(d, 0, mode);
            var result13 = decimal.Round(d, MidpointRounding.ToEven);
            var result14 = decimal.Round(d, decimals, MidpointRounding.ToEven);
            var result15 = decimal.Round(d, 0, MidpointRounding.ToEven);

            var result21 = Math.Round(d, 0);
            var result22 = Math.Round(d, 0, mode);
            var result23 = Math.Round(d, MidpointRounding.ToEven);
            var result24 = Math.Round(d, decimals, MidpointRounding.ToEven);
            var result25 = Math.Round(d, 0, MidpointRounding.ToEven);
        }

        public void NoDetection(decimal d, int decimals, MidpointRounding mode)
        {
            var result11 = decimal.Round(d, decimals);
            var result12 = decimal.Round(d, mode);
            var result13 = decimal.Round(d, decimals, mode);

            var result21 = Math.Round(d, decimals);
            var result22 = Math.Round(d, mode);
            var result23 = Math.Round(d, decimals, mode);
        }
    }
}