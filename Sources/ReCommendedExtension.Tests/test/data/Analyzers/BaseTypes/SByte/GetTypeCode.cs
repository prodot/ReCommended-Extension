namespace Test
{
    public class SBytes
    {
        public void ExpressionResult(sbyte number)
        {
            var result = number.GetTypeCode();
        }

        public void NoDetection(sbyte number)
        {
            number.GetTypeCode();
        }
    }
}