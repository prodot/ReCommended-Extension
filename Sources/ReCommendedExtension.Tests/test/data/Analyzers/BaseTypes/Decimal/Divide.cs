namespace Test
{
    public class Decimals
    {
        public void Operator(decimal d1, decimal d2)
        {
            var result = decimal.Divide(d1, d2);
        }

        public void NoDetection(decimal d1, decimal d2)
        {
            decimal.Divide(d1, d2);
        }
    }
}