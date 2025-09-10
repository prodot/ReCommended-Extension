namespace Test
{
    public class SBytes
    {
        public void ExpressionResult(sbyte number)
        {
            var result = sbyte.MinMagnitude(10, 10);
        }

        public void NoDetection(sbyte x, sbyte y)
        {
            var result11 = sbyte.MinMagnitude(1, 2);
            var result12 = sbyte.MinMagnitude(x, 2);
            var result13 = sbyte.MinMagnitude(1, y);

            sbyte.MinMagnitude(10, 10);
        }
    }
}