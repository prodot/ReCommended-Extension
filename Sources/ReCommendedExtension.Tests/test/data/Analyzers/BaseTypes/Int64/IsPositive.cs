namespace Test
{
    public class Int64s
    {
        public void Operator(long number)
        {
            var result = long.IsPositive(number);
        }

        public void NoDetection(long number)
        {
            long.IsPositive(number);
        }
    }
}