namespace Test
{
    public class ExpressionResults
    {
        public void ExpressionResult(long number)
        {
            (long quotient, long remainder) = long.DivRem(0, 10);

            var result11 = long.DivRem(0, 10);

            var result21 = long.RotateLeft(number, 0);
            var result22 = long.RotateRight(number, 0);

            var result31 = number.Equals(null);

            var result41 = long.Clamp(number, 1, 1);
            var result42 = long.Clamp(number, long.MinValue, long.MaxValue);

            var result51 = number.GetTypeCode();

            var result61 = long.Max(1, 1);
            var result62 = long.Min(1, 1);

            var result71 = long.MaxMagnitude(1, 1);
            var result72 = long.MaxMagnitude(1, 1);
        }

        public void NoDetection(long number, long left, long right, int rotateAmount, long min, long max)
        {
            var result11 = long.DivRem(left, right);
            var result12 = long.DivRem(left, 10);
            var result13 = long.DivRem(0, right);
            var result14 = long.DivRem(0, 0);

            var result21 = long.RotateLeft(number, rotateAmount);
            var result22 = long.RotateRight(number, rotateAmount);

            var result31 = long.Clamp(number, min, max);

            long.DivRem(0, 10);

            long.RotateLeft(number, 0);
            long.RotateRight(number, 0);

            number.Equals(null);

            long.Clamp(number, 1, 1);
            long.Clamp(number, long.MinValue, long.MaxValue);

            number.GetTypeCode();

            long.Max(1, 1);
            long.Min(1, 1);

            long.MaxMagnitude(1, 1);
            long.MaxMagnitude(1, 1);
        }
    }
}