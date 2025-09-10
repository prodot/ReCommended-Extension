namespace Test
{
    public class Booleans
    {
        public void ExpressionResult(bool flag)
        {
            var result = flag.GetTypeCode();
        }

        public void NoDetection(bool flag)
        {
            flag.GetTypeCode();
        }
    }
}