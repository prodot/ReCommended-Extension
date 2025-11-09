namespace Test
{
    public class Methods
    {
        public void BinaryOperator(int number, int value)
        {
            var result11 = number.Equals(value);

            var result21 = int.IsNegative(number);
            var result22 = int.IsPositive(number);
        }

        public void NoDetection(int number, int value)
        {
            number.Equals(value);

            int.IsNegative(number);
            int.IsPositive(number);
        }
    }
}