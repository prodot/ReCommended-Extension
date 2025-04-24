namespace Test
{
    public class Decimals
    {
        public void Operator(decimal number)
        {
            var result = decimal.IsPositive(number);
        }

        public void NoDetection(decimal number)
        {
            decimal.IsPositive(number);
        }
    }
}