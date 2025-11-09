using System;

namespace Test
{
    public class ExpressionResults
    {
        public void ExpressionResult(Int128 number)
        {
            (Int128 quotient, Int128 remainder) = Int128.DivRem(0, 10);

            var result11 = Int128.DivRem(0, 10);

            var result21 = Int128.RotateLeft(number, 0);
            var result22 = Int128.RotateRight(number, 0);

            var result31 = number.Equals(null);

            var result41 = Int128.Clamp(number, 1, 1);

            var result51 = Int128.Max(1, 1);
            var result52 = Int128.Min(1, 1);

            var result61 = Int128.MaxMagnitude(1, 1);
            var result62 = Int128.MaxMagnitude(1, 1);
        }

        public void NoDetection(Int128 number, Int128 left, Int128 right, int rotateAmount, Int128 min, Int128 max)
        {
            var result11 = Int128.DivRem(left, right);
            var result12 = Int128.DivRem(left, 10);
            var result13 = Int128.DivRem(0, right);
            var result14 = Int128.DivRem(0, 0);

            var result21 = Int128.RotateLeft(number, rotateAmount);
            var result22 = Int128.RotateRight(number, rotateAmount);

            var result31 = Int128.Clamp(number, min, max);
            var result32 = Int128.Clamp(number, Int128.MinValue, Int128.MaxValue);

            Int128.DivRem(0, 10);

            Int128.RotateLeft(number, 0);
            Int128.RotateRight(number, 0);

            number.Equals(null);

            Int128.Clamp(number, 1, 1);

            Int128.Max(1, 1);
            Int128.Min(1, 1);

            Int128.MaxMagnitude(1, 1);
            Int128.MaxMagnitude(1, 1);
        }
    }
}