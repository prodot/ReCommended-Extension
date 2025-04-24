namespace Test
{
    public class SBytes
    {
        public void Operator(sbyte number)
        {
            var result = sbyte.IsNegative(number);
        }

        public void NoDetection(sbyte number)
        {
            sbyte.IsNegative(number);
        }
    }
}