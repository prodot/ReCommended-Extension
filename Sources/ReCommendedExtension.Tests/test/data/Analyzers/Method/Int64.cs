namespace Test
{
    public class Methods
    {
        public void BinaryOperator(long number, long value)
        {
            var result11 = number.Equals(value);

            var result21 = long.IsNegative(number);
            var result22 = long.IsPositive(number);
        }

        public void NoDetection(long number, long value)
        {
            number.Equals(value);

            long.IsNegative(number);
            long.IsPositive(number);
        }
    }
}