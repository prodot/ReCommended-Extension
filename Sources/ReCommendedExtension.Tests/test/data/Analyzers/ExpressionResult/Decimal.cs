namespace Test
{
    public class ExpressionResults
    {
        public void ExpressionResult(decimal number)
        {
            var result11 = number.Equals(null);

            var result21 = decimal.Clamp(number, 1, 1);
            var result22 = decimal.Clamp(number, decimal.MinValue, decimal.MaxValue);

            var result31 = number.GetTypeCode();

            var result41 = decimal.Max(1, 1);
            var result42 = decimal.Min(1, 1);
        }

        public void NoDetection(decimal number, decimal min, decimal max)
        {
            var result11 = decimal.Clamp(number, min, max);

            number.Equals(null);

            decimal.Clamp(number, 1, 1);
            decimal.Clamp(number, decimal.MinValue, decimal.MaxValue);

            number.GetTypeCode();

            decimal.Max(1, 1);
            decimal.Min(1, 1);
        }
    }
}