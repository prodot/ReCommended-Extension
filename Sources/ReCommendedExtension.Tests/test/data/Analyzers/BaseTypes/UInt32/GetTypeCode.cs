namespace Test
{
    public class UInt32s
    {
        public void ExpressionResult(uint number)
        {
            var result = number.GetTypeCode();
        }

        public void NoDetection(uint number)
        {
            number.GetTypeCode();
        }
    }
}