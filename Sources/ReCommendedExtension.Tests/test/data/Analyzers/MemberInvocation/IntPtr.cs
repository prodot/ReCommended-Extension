namespace Test
{
    public class Methods
    {
        public void BinaryOperator(nint number, nint value)
        {
            var result11 = number.Equals(value);

            var result21 = nint.IsNegative(number);
            var result22 = nint.IsPositive(number);
        }

        public void NoDetection(nint number, nint value)
        {
            number.Equals(value);

            nint.IsNegative(number);
            nint.IsPositive(number);
        }
    }
}