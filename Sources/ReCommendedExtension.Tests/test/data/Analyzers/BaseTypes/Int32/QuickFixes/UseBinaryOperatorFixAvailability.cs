namespace Test
{
    public class Int32s
    {
        public void Equals(int number, int obj)
        {
            var result = number.Equals(obj);
        }

        public void IsNegative(int number)
        {
            var result = int.IsNegative(number);
        }

        public void IsPositive(int number)
        {
            var result = int.IsPositive(number);
        }
    }
}