namespace Test
{
    public class ExpressionResults
    {
        public void ExpressionResult(nint number)
        {
            (nint quotient, nint remainder) = nint.DivRem(0, 10);

            var result11 = nint.DivRem(0, 10);

            var result21 = nint.RotateLeft(number, 0);
            var result22 = nint.RotateRight(number, 0);

            var result31 = number.Equals(null);

            var result41 = nint.Clamp(number, 1, 1);

            var result51 = nint.Max(1, 1);
            var result52 = nint.Min(1, 1);

            var result61 = nint.MaxMagnitude(1, 1);
            var result62 = nint.MaxMagnitude(1, 1);
        }

        public void NoDetection(nint number, nint left, nint right, int rotateAmount, nint min, nint max)
        {
            var result11 = nint.DivRem(left, right);
            var result12 = nint.DivRem(left, 10);
            var result13 = nint.DivRem(0, right);
            var result14 = nint.DivRem(0, 0);

            var result21 = nint.RotateLeft(number, rotateAmount);
            var result22 = nint.RotateRight(number, rotateAmount);

            var result31 = nint.Clamp(number, min, max);

            nint.DivRem(0, 10);

            nint.RotateLeft(number, 0);
            nint.RotateRight(number, 0);

            number.Equals(null);

            nint.Clamp(number, 1, 1);

            nint.Max(1, 1);
            nint.Min(1, 1);

            nint.MaxMagnitude(1, 1);
            nint.MaxMagnitude(1, 1);
        }
    }
}