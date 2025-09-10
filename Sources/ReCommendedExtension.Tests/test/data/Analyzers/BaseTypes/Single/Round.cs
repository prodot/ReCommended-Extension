using System;

namespace Test
{
    public class Singles
    {
        public void RedundantArgument(float x, int digits, MidpointRounding mode)
        {
            var result11 = float.Round(x, 0);
            var result12 = float.Round(x, 0, mode);
            var result13 = float.Round(x, MidpointRounding.ToEven);
            var result14 = float.Round(x, digits, MidpointRounding.ToEven);
            var result15 = float.Round(x, 0, MidpointRounding.ToEven);

            var result21 = MathF.Round(x, 0);
            var result22 = MathF.Round(x, 0, mode);
            var result23 = MathF.Round(x, MidpointRounding.ToEven);
            var result24 = MathF.Round(x, digits, MidpointRounding.ToEven);
            var result25 = MathF.Round(x, 0, MidpointRounding.ToEven);
        }

        public void NoDetection(float x, int digits, MidpointRounding mode)
        {
            var result11 = float.Round(x, digits);
            var result12 = float.Round(x, mode);
            var result13 = float.Round(x, digits, mode);

            var result21 = MathF.Round(x, digits);
            var result22 = MathF.Round(x, mode);
            var result23 = MathF.Round(x, digits, mode);
        }
    }
}