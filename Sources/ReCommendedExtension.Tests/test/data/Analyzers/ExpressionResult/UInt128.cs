using System;

namespace Test
{
    public class ExpressionResults
    {
        public void ExpressionResult(UInt128 number)
        {
            (UInt128 quotient, UInt128 remainder) = UInt128.DivRem(0, 10);

            var result11 = UInt128.DivRem(0, 10);

            var result21 = UInt128.RotateLeft(number, 0);
            var result22 = UInt128.RotateRight(number, 0);

            var result31 = number.Equals(null);

            var result41 = UInt128.Clamp(number, 1, 1);

            var result51 = UInt128.Max(1, 1);
            var result52 = UInt128.Min(1, 1);
        }

        public void NoDetection(UInt128 number, UInt128 left, UInt128 right, int rotateAmount, UInt128 min, UInt128 max)
        {
            var result11 = UInt128.DivRem(left, right);
            var result12 = UInt128.DivRem(left, 10);
            var result13 = UInt128.DivRem(0, right);
            var result14 = UInt128.DivRem(0, 0);

            var result21 = UInt128.RotateLeft(number, rotateAmount);
            var result22 = UInt128.RotateRight(number, rotateAmount);

            var result31 = UInt128.Clamp(number, min, max);
            var result32 = UInt128.Clamp(number, UInt128.MinValue, UInt128.MaxValue);

            UInt128.DivRem(0, 10);

            UInt128.RotateLeft(number, 0);
            UInt128.RotateRight(number, 0);

            number.Equals(null);

            UInt128.Clamp(number, 1, 1);

            UInt128.Max(1, 1);
            UInt128.Min(1, 1);
        }
    }
}