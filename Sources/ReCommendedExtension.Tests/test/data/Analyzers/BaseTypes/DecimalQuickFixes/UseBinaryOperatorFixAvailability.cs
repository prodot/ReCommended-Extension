namespace Test
{
    public class Decimals
    {
        public void Equals(decimal number, decimal obj)
        {
            var result = number.Equals(obj);
        }

        public void IsNegative(decimal number)
        {
            var result = decimal.IsNegative(number);
        }

        public void IsPositive(decimal number)
        {
            var result = decimal.IsPositive(number);
        }
    }
}