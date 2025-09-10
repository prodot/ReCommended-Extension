namespace Test
{
    public class UInt16s
    {
        public void ExpressionResult(ushort number)
        {
            var result = number.GetTypeCode();
        }

        public void NoDetection(ushort number)
        {
            number.GetTypeCode();
        }
    }
}