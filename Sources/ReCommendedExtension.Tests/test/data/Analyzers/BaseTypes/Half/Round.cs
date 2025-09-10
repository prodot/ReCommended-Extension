using System;

namespace Test
{
    public class Halves
    {
        public void RedundantArgument(Half x, int digits, MidpointRounding mode)
        {
            var result1 = Half.Round(x, 0);
            var result2 = Half.Round(x, 0, mode);
            var result3 = Half.Round(x, MidpointRounding.ToEven);
            var result4 = Half.Round(x, digits, MidpointRounding.ToEven);
            var result5 = Half.Round(x, 0, MidpointRounding.ToEven);
        }

        public void NoDetection(Half x, int digits, MidpointRounding mode)
        {
            var result1 = Half.Round(x, digits);
            var result2 = Half.Round(x, mode);
            var result3 = Half.Round(x, digits, mode);
        }
    }
}