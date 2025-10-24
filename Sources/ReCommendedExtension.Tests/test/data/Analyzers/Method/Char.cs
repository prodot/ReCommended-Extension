namespace Test
{
    public class Methods
    {
        public void BinaryOperator(char c, char value)
        {
            var result = c.Equals(value);
        }

        public void NoDetection(char c, char value)
        {
            c.Equals(value);
        }
    }
}