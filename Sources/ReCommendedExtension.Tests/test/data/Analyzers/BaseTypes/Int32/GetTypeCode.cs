namespace Test
{
    public class Int32s
    {
        public void ExpressionResult(int number)
        {
            var result = number.GetTypeCode();
        }

        public void NoDetection(int number)
        {
            number.GetTypeCode();
        }
    }
}