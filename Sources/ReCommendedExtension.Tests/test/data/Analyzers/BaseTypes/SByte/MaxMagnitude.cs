namespace Test
{
    public class SBytes
    {
        public void ExpressionResult(sbyte number)
        {
            var result = sbyte.MaxMagnitude(10, 10);
        }

        public void NoDetection(sbyte x, sbyte y)
        {
            var result11 = sbyte.MaxMagnitude(1, 2);
            var result12 = sbyte.MaxMagnitude(x, 2);
            var result13 = sbyte.MaxMagnitude(1, y);

            sbyte.MaxMagnitude(10, 10);
        }
    }
}