namespace Test
{
    public class ExpressionResults
    {
        public void ExpressionResult(nuint number)
        {
            (nuint quotient, nuint remainder) = nuint.DivRem(0, 10);

            var result11 = nuint.DivRem(0, 10);

            var result21 = nuint.RotateLeft(number, 0);
            var result22 = nuint.RotateRight(number, 0);

            var result31 = number.Equals(null);

            var result41 = nuint.Clamp(number, 1, 1);

            var result51 = nuint.Max(1, 1);
            var result52 = nuint.Min(1, 1);
        }

        public void NoDetection(nuint number, nuint left, nuint right, int rotateAmount, nuint min, nuint max)
        {
            var result11 = nuint.DivRem(left, right);
            var result12 = nuint.DivRem(left, 10);
            var result13 = nuint.DivRem(0, right);
            var result14 = nuint.DivRem(0, 0);

            var result21 = nuint.RotateLeft(number, rotateAmount);
            var result22 = nuint.RotateRight(number, rotateAmount);

            var result31 = nuint.Clamp(number, min, max);

            nuint.DivRem(0, 10);

            nuint.RotateLeft(number, 0);
            nuint.RotateRight(number, 0);

            number.Equals(null);

            nuint.Clamp(number, 1, 1);

            nuint.Max(1, 1);
            nuint.Min(1, 1);
        }
    }
}