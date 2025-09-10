namespace Test
{
    public class Decimals
    {
        public void Operator(decimal d1, decimal d2)
        {
            var result = decimal.Add(d1, d2);
        }

        public void NoDetection(decimal d1, decimal d2)
        {
            decimal.Add(d1, d2);
        }
    }
}