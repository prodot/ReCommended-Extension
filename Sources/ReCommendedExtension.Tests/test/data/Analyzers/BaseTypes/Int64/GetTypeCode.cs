namespace Test
{
    public class Int64s
    {
        public void ExpressionResult(long number)
        {
            var result = number.GetTypeCode();
        }

        public void NoDetection(long number)
        {
            number.GetTypeCode();
        }
    }
}