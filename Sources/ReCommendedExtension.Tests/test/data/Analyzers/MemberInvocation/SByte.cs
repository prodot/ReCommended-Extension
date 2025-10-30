namespace Test
{
    public class Methods
    {
        public void BinaryOperator(sbyte number, sbyte value)
        {
            var result11 = number.Equals(value);

            var result21 = sbyte.IsNegative(number);
            var result22 = sbyte.IsPositive(number);
        }

        public void NoDetection(sbyte number, sbyte value)
        {
            number.Equals(value);

            sbyte.IsNegative(number);
            sbyte.IsPositive(number);
        }
    }
}