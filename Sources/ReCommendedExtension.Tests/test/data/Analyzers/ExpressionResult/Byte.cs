namespace Test
{
    public class ExpressionResults
    {
        public void ExpressionResult(byte number)
        {
            (byte quotient, byte remainder) = byte.DivRem(0, 10);

            var result11 = byte.DivRem(0, 10);

            var result21 = byte.RotateLeft(number, 0);
            var result22 = byte.RotateRight(number, 0);

            var result31 = number.Equals(null);

            var result41 = byte.Clamp(number, 1, 1);
            var result42 = byte.Clamp(number, 0, byte.MaxValue);

            var result51 = number.GetTypeCode();

            var result61 = byte.Max(1, 1);
            var result62 = byte.Min(1, 1);
        }

        public void NoDetection(byte number, byte left, byte right, int rotateAmount, byte min, byte max)
        {
            var result11 = byte.DivRem(left, right);
            var result12 = byte.DivRem(left, 10);
            var result13 = byte.DivRem(0, right);
            var result14 = byte.DivRem(0, 0);

            var result21 = byte.RotateLeft(number, rotateAmount);
            var result22 = byte.RotateRight(number, rotateAmount);

            var result31 = byte.Clamp(number, min, max);

            byte.DivRem(0, 10);

            byte.RotateLeft(number, 0);
            byte.RotateRight(number, 0);

            number.Equals(null);

            byte.Clamp(number, 1, 1);
            byte.Clamp(number, 0, byte.MaxValue);

            number.GetTypeCode();

            byte.Max(1, 1);
            byte.Min(1, 1);
        }
    }
}