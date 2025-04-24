namespace Test
{
    public class SBytes
    {
        public void Operator(sbyte number)
        {
            var result = sbyte.IsPositive(number);
        }

        public void NoDetection(sbyte number)
        {
            sbyte.IsPositive(number);
        }
    }
}