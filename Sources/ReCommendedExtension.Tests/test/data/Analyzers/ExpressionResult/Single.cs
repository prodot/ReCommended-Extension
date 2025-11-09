namespace Test
{
    public class ExpressionResults
    {
        public void ExpressionResult(float number)
        {
            var result11 = number.Equals(null);

            var result21 = number.GetTypeCode();
        }

        public void NoDetection(float number)
        {
            number.Equals(null);

            number.GetTypeCode();
        }
    }
}