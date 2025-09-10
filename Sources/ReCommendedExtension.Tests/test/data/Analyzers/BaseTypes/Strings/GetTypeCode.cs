namespace Test
{
    public class Strings
    {
        public void ExpressionResult(string text)
        {
            var result = text.GetTypeCode();
        }

        public void NoDetection(string text)
        {
            text.GetTypeCode();
        }

        public void NoDetection_Nullable(string? text)
        {
            var result = text?.GetTypeCode();
        }
    }
}