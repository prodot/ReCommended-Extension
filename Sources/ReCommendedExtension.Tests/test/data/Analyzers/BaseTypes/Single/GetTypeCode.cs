namespace Test
{
    public class Singles
    {
        public void ExpressionResult(float number)
        {
            var result = number.GetTypeCode();
        }

        public void NoDetection(float number)
        {
            number.GetTypeCode();
        }
    }
}