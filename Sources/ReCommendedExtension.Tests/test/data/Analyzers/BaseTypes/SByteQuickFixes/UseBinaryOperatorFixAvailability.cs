namespace Test
{
    public class SBytes
    {
        public void Equals(sbyte number, sbyte obj)
        {
            var result = number.Equals(obj);
        }

        public void IsNegative(sbyte number)
        {
            var result = sbyte.IsNegative(number);
        }

        public void IsPositive(sbyte number)
        {
            var result = sbyte.IsPositive(number);
        }
    }
}