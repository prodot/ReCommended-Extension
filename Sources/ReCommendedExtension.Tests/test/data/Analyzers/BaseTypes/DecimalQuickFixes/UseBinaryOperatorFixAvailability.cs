namespace Test
{
    public class Decimals
    {
        public void Equals(decimal number, decimal obj)
        {
            var result = number.Equals(obj);
        }

        public void Add(decimal d1, decimal d2)
        {
            var result = decimal.Add(d1, d2);
        }

        public void Divide(decimal d1, decimal d2)
        {
            var result = decimal.Divide(d1, d2);
        }

        public void Multiply(decimal d1, decimal d2)
        {
            var result = decimal.Multiply(d1, d2);
        }

        public void Remainder(decimal d1, decimal d2)
        {
            var result = decimal.Remainder(d1, d2);
        }

        public void Subtract(decimal d1, decimal d2)
        {
            var result = decimal.Subtract(d1, d2);
        }
    }
}