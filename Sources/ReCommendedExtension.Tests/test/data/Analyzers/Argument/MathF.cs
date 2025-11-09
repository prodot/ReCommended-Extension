using System;

namespace Test
{
    public class Arguments
    {
        public void RedundantArgument(float n)
        {
            var result11 = MathF.Round(n, 0);
            var result12 = MathF.Round(n, MidpointRounding.ToEven);
            var result13 = MathF.Round(n, 0, MidpointRounding.ToEven);
        }

        public void NoDetection(float n, int digits, MidpointRounding rounding)
        {
            var result11 = MathF.Round(n, digits);
            var result12 = MathF.Round(n, rounding);
            var result13 = MathF.Round(n, digits, rounding);
        }
    }
}