namespace Test
{
    public class ExpressionResults
    {
        public void ExpressionResult(short number)
        {
            (short quotient, short remainder) = short.DivRem(0, 10);

            var result11 = short.DivRem(0, 10);

            var result21 = short.RotateLeft(number, 0);
            var result22 = short.RotateRight(number, 0);

            var result31 = number.Equals(null);

            var result41 = short.Clamp(number, 1, 1);
            var result42 = short.Clamp(number, short.MinValue, short.MaxValue);

            var result51 = number.GetTypeCode();

            var result61 = short.Max(1, 1);
            var result62 = short.Min(1, 1);

            var result71 = short.MaxMagnitude(1, 1);
            var result72 = short.MaxMagnitude(1, 1);
        }

        public void NoDetection(short number, short left, short right, int rotateAmount, short min, short max)
        {
            var result11 = short.DivRem(left, right);
            var result12 = short.DivRem(left, 10);
            var result13 = short.DivRem(0, right);
            var result14 = short.DivRem(0, 0);

            var result21 = short.RotateLeft(number, rotateAmount);
            var result22 = short.RotateRight(number, rotateAmount);

            var result31 = short.Clamp(number, min, max);

            short.DivRem(0, 10);

            short.RotateLeft(number, 0);
            short.RotateRight(number, 0);

            number.Equals(null);

            short.Clamp(number, 1, 1);
            short.Clamp(number, short.MinValue, short.MaxValue);

            number.GetTypeCode();

            short.Max(1, 1);
            short.Min(1, 1);

            short.MaxMagnitude(1, 1);
            short.MaxMagnitude(1, 1);
        }
    }
}