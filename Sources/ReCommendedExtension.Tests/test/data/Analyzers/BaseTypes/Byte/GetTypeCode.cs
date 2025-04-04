namespace Test
{
    public class Bytes
    {
        public void ExpressionResult(byte number)
        {
            var result = number.GetTypeCode();
        }

        public void NoDetection(byte number)
        {
            flag.GetTypeCode();
        }
    }
}