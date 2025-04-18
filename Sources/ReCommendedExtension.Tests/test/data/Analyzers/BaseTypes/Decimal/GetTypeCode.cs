namespace Test
{
    public class Decimals
    {
        public void ExpressionResult(decimal number)
        {
            var result = number.GetTypeCode();
        }

        public void NoDetection(decimal number)
        {
            number.GetTypeCode();
        }
    }
}