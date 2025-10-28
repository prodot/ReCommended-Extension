namespace Test
{
    public class ExpressionResults
    {
        public void ExpressionResult(double number)
        {
            var result11 = number.Equals(null);

            var result21 = number.GetTypeCode();
        }

        public void NoDetection(double number)
        {
            number.Equals(null);

            number.GetTypeCode();
        }
    }
}