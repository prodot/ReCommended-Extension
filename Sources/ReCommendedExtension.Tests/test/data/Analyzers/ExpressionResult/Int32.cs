namespace Test
{
    public class ExpressionResults
    {
        public void ExpressionResult(int number)
        {
            (int quotient, int remainder) = int.DivRem(0, 10);

            var result11 = int.DivRem(0, 10);

            var result21 = int.RotateLeft(number, 0);
            var result22 = int.RotateRight(number, 0);

            var result31 = number.Equals(null);

            var result41 = int.Clamp(number, 1, 1);
            var result42 = int.Clamp(number, int.MinValue, int.MaxValue);

            var result51 = number.GetTypeCode();

            var result61 = int.Max(1, 1);
            var result62 = int.Min(1, 1);

            var result71 = int.MaxMagnitude(1, 1);
            var result72 = int.MaxMagnitude(1, 1);
        }

        public void NoDetection(int number, int left, int right, int rotateAmount, int min, int max)
        {
            var result11 = int.DivRem(left, right);
            var result12 = int.DivRem(left, 10);
            var result13 = int.DivRem(0, right);
            var result14 = int.DivRem(0, 0);

            var result21 = int.RotateLeft(number, rotateAmount);
            var result22 = int.RotateRight(number, rotateAmount);

            var result31 = int.Clamp(number, min, max);

            int.DivRem(0, 10);

            int.RotateLeft(number, 0);
            int.RotateRight(number, 0);

            number.Equals(null);

            int.Clamp(number, 1, 1);
            int.Clamp(number, int.MinValue, int.MaxValue);

            number.GetTypeCode();

            int.Max(1, 1);
            int.Min(1, 1);

            int.MaxMagnitude(1, 1);
            int.MaxMagnitude(1, 1);
        }
    }
}