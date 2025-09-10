namespace Test
{
    public class Int16s
    {
        public void Operator(short number)
        {
            var result = short.IsPositive(number);
        }

        public void NoDetection(short number)
        {
            short.IsPositive(number);
        }
    }
}