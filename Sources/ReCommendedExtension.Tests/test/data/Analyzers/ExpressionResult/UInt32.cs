namespace Test
{
    public class ExpressionResults
    {
        public void ExpressionResult(uint number)
        {
            (uint quotient, uint remainder) = uint.DivRem(0, 10);

            var result11 = uint.DivRem(0, 10);

            var result21 = uint.RotateLeft(number, 0);
            var result22 = uint.RotateRight(number, 0);

            var result31 = number.Equals(null);

            var result41 = uint.Clamp(number, 1, 1);
            var result42 = uint.Clamp(number, 0, uint.MaxValue);

            var result51 = number.GetTypeCode();

            var result61 = uint.Max(1, 1);
            var result62 = uint.Min(1, 1);
        }

        public void NoDetection(uint number, uint left, uint right, int rotateAmount, uint min, uint max)
        {
            var result11 = uint.DivRem(left, right);
            var result12 = uint.DivRem(left, 10);
            var result13 = uint.DivRem(0, right);
            var result14 = uint.DivRem(0, 0);

            var result21 = uint.RotateLeft(number, rotateAmount);
            var result22 = uint.RotateRight(number, rotateAmount);

            var result31 = uint.Clamp(number, min, max);

            uint.DivRem(0, 10);

            uint.RotateLeft(number, 0);
            uint.RotateRight(number, 0);

            number.Equals(null);

            uint.Clamp(number, 1, 1);
            uint.Clamp(number, 0, uint.MaxValue);

            number.GetTypeCode();

            uint.Max(1, 1);
            uint.Min(1, 1);
        }
    }
}