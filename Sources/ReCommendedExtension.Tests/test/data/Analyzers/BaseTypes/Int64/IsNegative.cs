namespace Test
{
    public class Int64s
    {
        public void Operator(long number)
        {
            var result = long.IsNegative(number);
        }

        public void NoDetection(long number)
        {
            long.IsNegative(number);
        }
    }
}