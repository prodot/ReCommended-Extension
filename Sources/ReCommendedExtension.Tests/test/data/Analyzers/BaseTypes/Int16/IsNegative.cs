namespace Test
{
    public class Int16s
    {
        public void Operator(short number)
        {
            var result = short.IsNegative(number);
        }

        public void NoDetection(short number)
        {
            short.IsNegative(number);
        }
    }
}