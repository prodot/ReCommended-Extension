namespace Test
{
    public class ExpressionResults
    {
        public void ExpressionResult(ulong number)
        {
            (ulong quotient, ulong remainder) = ulong.DivRem(0, 10);

            var result11 = ulong.DivRem(0, 10);

            var result21 = ulong.RotateLeft(number, 0);
            var result22 = ulong.RotateRight(number, 0);

            var result31 = number.Equals(null);

            var result41 = ulong.Clamp(number, 1, 1);
            var result42 = ulong.Clamp(number, 0, ulong.MaxValue);

            var result51 = number.GetTypeCode();

            var result61 = ulong.Max(1, 1);
            var result62 = ulong.Min(1, 1);
        }

        public void NoDetection(ulong number, ulong left, ulong right, int rotateAmount, ulong min, ulong max)
        {
            var result11 = ulong.DivRem(left, right);
            var result12 = ulong.DivRem(left, 10);
            var result13 = ulong.DivRem(0, right);
            var result14 = ulong.DivRem(0, 0);

            var result21 = ulong.RotateLeft(number, rotateAmount);
            var result22 = ulong.RotateRight(number, rotateAmount);

            var result31 = ulong.Clamp(number, min, max);

            ulong.DivRem(0, 10);

            ulong.RotateLeft(number, 0);
            ulong.RotateRight(number, 0);

            number.Equals(null);

            ulong.Clamp(number, 1, 1);
            ulong.Clamp(number, 0, ulong.MaxValue);

            number.GetTypeCode();

            ulong.Max(1, 1);
            ulong.Min(1, 1);
        }
    }
}