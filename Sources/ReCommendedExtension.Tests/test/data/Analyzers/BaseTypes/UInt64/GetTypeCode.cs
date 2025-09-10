namespace Test
{
    public class UInt64s
    {
        public void ExpressionResult(ulong number)
        {
            var result = number.GetTypeCode();
        }

        public void NoDetection(ulong number)
        {
            number.GetTypeCode();
        }
    }
}