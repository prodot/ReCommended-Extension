namespace Test
{
    public class IntPtrs
    {
        public void ExpressionResult(nint number)
        {
            var result = nint.MinMagnitude(10, 10);
        }

        public void NoDetection(nint x, nint y)
        {
            var result11 = nint.MinMagnitude(1, 2);
            var result12 = nint.MinMagnitude(x, 2);
            var result13 = nint.MinMagnitude(1, y);

            nint.MinMagnitude(10, 10);
        }
    }
}