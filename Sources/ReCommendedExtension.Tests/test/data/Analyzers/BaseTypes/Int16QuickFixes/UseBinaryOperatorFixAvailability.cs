namespace Test
{
    public class Int16s
    {
        public void Equals(short number, short obj)
        {
            var result = number.Equals(obj);
        }

        public void IsNegative(short number)
        {
            var result = short.IsNegative(number);
        }

        public void IsPositive(short number)
        {
            var result = short.IsPositive(number);
        }
    }
}