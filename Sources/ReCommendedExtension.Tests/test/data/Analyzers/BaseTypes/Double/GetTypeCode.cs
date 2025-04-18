namespace Test
{
    public class Doubles
    {
        public void ExpressionResult(double number)
        {
            var result = number.GetTypeCode();
        }

        public void NoDetection(double number)
        {
            number.GetTypeCode();
        }
    }
}