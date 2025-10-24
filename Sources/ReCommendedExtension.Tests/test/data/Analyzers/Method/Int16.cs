namespace Test
{
    public class Methods
    {
        public void BinaryOperator(short number, short value)
        {
            var result11 = number.Equals(value);

            var result21 = short.IsNegative(number);
            var result22 = short.IsPositive(number);
        }

        public void NoDetection(short number, short value)
        {
            number.Equals(value);

            short.IsNegative(number);
            short.IsPositive(number);
        }
    }
}