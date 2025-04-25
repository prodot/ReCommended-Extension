namespace Test
{
    public class Int64s
    {
        public void ExpressionResult(long number)
        {
            var result = long.MinMagnitude(10, 10);
        }

        public void NoDetection(long x, long y)
        {
            var result11 = long.MinMagnitude(1, 2);
            var result12 = long.MinMagnitude(x, 2);
            var result13 = long.MinMagnitude(1, y);

            long.MinMagnitude(10, 10);
        }
    }
}