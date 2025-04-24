namespace Test
{
    public class Decimals
    {
        public void Operator(decimal number)
        {
            var result = decimal.IsNegative(number);
        }

        public void NoDetection(decimal number)
        {
            decimal.IsNegative(number);
        }
    }
}