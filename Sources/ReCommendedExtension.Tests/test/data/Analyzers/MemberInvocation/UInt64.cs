namespace Test
{
    public class Methods
    {
        public void BinaryOperator(ulong number, ulong value)
        {
            var result = number.Equals(value);
        }

        public void NoDetection(ulong number, ulong value)
        {
            number.Equals(value);
        }
    }
}