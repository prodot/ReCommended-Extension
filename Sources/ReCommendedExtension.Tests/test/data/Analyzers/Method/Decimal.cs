namespace Test
{
    public class Methods
    {
        public void BinaryOperator(decimal d1, decimal d2)
        {
            var result11 = decimal.Add(d1, d2);
            var result12 = decimal.Subtract(d1, d2);
            var result13 = decimal.Multiply(d1, d2);
            var result14 = decimal.Divide(d1, d2);
            var result15 = decimal.Remainder(d1, d2);

            var result21 = d1.Equals(d2);
        }

        public void NoDetection(decimal d1, decimal d2)
        {
            decimal.Add(d1, d2);
            decimal.Subtract(d1, d2);
            decimal.Multiply(d1, d2);
            decimal.Divide(d1, d2);
            decimal.Remainder(d1, d2);

            d1.Equals(d2);
        }
    }
}