namespace Test
{
    public class IntPtrs
    {
        public void ExpressionResult(nint number)
        {
            var result = nint.MaxMagnitude(10, 10);
        }

        public void NoDetection(nint x, nint y)
        {
            var result11 = nint.MaxMagnitude(1, 2);
            var result12 = nint.MaxMagnitude(x, 2);
            var result13 = nint.MaxMagnitude(1, y);

            nint.MaxMagnitude(10, 10);
        }
    }
}