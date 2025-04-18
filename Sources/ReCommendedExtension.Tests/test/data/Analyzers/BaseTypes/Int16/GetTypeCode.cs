namespace Test
{
    public class Int16s
    {
        public void ExpressionResult(short number)
        {
            var result = number.GetTypeCode();
        }

        public void NoDetection(short number)
        {
            number.GetTypeCode();
        }
    }
}