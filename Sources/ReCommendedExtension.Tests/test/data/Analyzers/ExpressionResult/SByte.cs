namespace Test
{
    public class ExpressionResults
    {
        public void ExpressionResult(sbyte number)
        {
            (sbyte quotient, sbyte remainder) = sbyte.DivRem(0, 10);

            var result11 = sbyte.DivRem(0, 10);

            var result21 = sbyte.RotateLeft(number, 0);
            var result22 = sbyte.RotateRight(number, 0);

            var result31 = number.Equals(null);

            var result41 = sbyte.Clamp(number, 1, 1);
            var result42 = sbyte.Clamp(number, sbyte.MinValue, sbyte.MaxValue);

            var result51 = number.GetTypeCode();

            var result61 = sbyte.Max(1, 1);
            var result62 = sbyte.Min(1, 1);

            var result71 = sbyte.MaxMagnitude(1, 1);
            var result72 = sbyte.MaxMagnitude(1, 1);
        }

        public void NoDetection(sbyte number, sbyte left, sbyte right, int rotateAmount, sbyte min, sbyte max)
        {
            var result11 = sbyte.DivRem(left, right);
            var result12 = sbyte.DivRem(left, 10);
            var result13 = sbyte.DivRem(0, right);
            var result14 = sbyte.DivRem(0, 0);

            var result21 = sbyte.RotateLeft(number, rotateAmount);
            var result22 = sbyte.RotateRight(number, rotateAmount);

            var result31 = sbyte.Clamp(number, min, max);

            sbyte.DivRem(0, 10);

            sbyte.RotateLeft(number, 0);
            sbyte.RotateRight(number, 0);

            number.Equals(null);

            sbyte.Clamp(number, 1, 1);
            sbyte.Clamp(number, sbyte.MinValue, sbyte.MaxValue);

            number.GetTypeCode();

            sbyte.Max(1, 1);
            sbyte.Min(1, 1);

            sbyte.MaxMagnitude(1, 1);
            sbyte.MaxMagnitude(1, 1);
        }
    }
}