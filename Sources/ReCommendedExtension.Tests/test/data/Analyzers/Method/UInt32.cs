namespace Test
{
    public class Methods
    {
        public void BinaryOperator(uint number, uint value)
        {
            var result = number.Equals(value);
        }

        public void NoDetection(uint number, uint value)
        {
            number.Equals(value);
        }
    }
}