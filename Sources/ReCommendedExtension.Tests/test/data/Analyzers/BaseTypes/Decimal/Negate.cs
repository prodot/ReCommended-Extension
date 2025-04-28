namespace Test
{
    public class Decimals
    {
        public void Operator(decimal d)
        {
            var result = decimal.Negate(d);
        }

        public void NoDetection(decimal d)
        {
            decimal.Negate(d);
        }
    }
}