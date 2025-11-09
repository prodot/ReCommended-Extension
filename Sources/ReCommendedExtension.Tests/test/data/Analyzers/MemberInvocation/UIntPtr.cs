namespace Test
{
    public class Methods
    {
        public void BinaryOperator(nuint number, nuint value)
        {
            var result = number.Equals(value);
        }

        public void NoDetection(nuint number, nuint value)
        {
            number.Equals(value);
        }
    }
}