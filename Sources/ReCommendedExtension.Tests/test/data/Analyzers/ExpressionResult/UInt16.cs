namespace Test
{
    public class ExpressionResults
    {
        public void ExpressionResult(ushort number)
        {
            (ushort quotient, ushort remainder) = ushort.DivRem(0, 10);

            var result11 = ushort.DivRem(0, 10);

            var result21 = ushort.RotateLeft(number, 0);
            var result22 = ushort.RotateRight(number, 0);

            var result31 = number.Equals(null);

            var result41 = ushort.Clamp(number, 1, 1);
            var result42 = ushort.Clamp(number, 0, ushort.MaxValue);

            var result51 = number.GetTypeCode();

            var result61 = ushort.Max(1, 1);
            var result62 = ushort.Min(1, 1);
        }

        public void NoDetection(ushort number, ushort left, ushort right, int rotateAmount, ushort min, ushort max)
        {
            var result11 = ushort.DivRem(left, right);
            var result12 = ushort.DivRem(left, 10);
            var result13 = ushort.DivRem(0, right);
            var result14 = ushort.DivRem(0, 0);

            var result21 = ushort.RotateLeft(number, rotateAmount);
            var result22 = ushort.RotateRight(number, rotateAmount);

            var result31 = ushort.Clamp(number, min, max);

            ushort.DivRem(0, 10);

            ushort.RotateLeft(number, 0);
            ushort.RotateRight(number, 0);

            number.Equals(null);

            ushort.Clamp(number, 1, 1);
            ushort.Clamp(number, 0, ushort.MaxValue);

            number.GetTypeCode();

            ushort.Max(1, 1);
            ushort.Min(1, 1);
        }
    }
}