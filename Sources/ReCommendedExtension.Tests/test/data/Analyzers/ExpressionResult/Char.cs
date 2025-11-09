namespace Test
{
    public class ExpressionResults
    {
        public void ExpressionResult(char c)
        {
            var result11 = c.Equals(null);

            var result21 = c.GetTypeCode();
        }

        public void NoDetection(char c, object obj)
        {
            var result = c.Equals(obj);

            c.Equals(null);

            c.GetTypeCode();
        }
    }
}