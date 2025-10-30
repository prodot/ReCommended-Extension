namespace Test
{
    public class Methods
    {
        public void BinaryOperator(byte number, byte value)
        {
            var result = number.Equals(value);
        }

        public void NoDetection(byte number, byte value)
        {
            number.Equals(value);
        }
    }
}