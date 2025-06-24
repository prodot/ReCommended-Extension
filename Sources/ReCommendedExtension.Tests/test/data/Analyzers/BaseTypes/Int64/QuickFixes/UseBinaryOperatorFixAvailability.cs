namespace Test
{
    public class Int64s
    {
        public void Equals(long number, long obj)
        {
            var result = number.Equals(obj);
        }

        public void IsNegative(long number)
        {
            var result = long.IsNegative(number);
        }

        public void IsPositive(long number)
        {
            var result = long.IsPositive(number);
        }
    }
}