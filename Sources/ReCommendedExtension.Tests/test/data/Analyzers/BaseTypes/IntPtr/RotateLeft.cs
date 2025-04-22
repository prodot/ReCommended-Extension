namespace Test
{
    public class IntPtrs
    {
        public void ExpressionResult(nint n)
        {
            var result = nint.RotateLeft(n, 0);
        }

        public void NoDetection(nint n, int rotateAmount)
        {
            var result1 = nint.RotateLeft(n, 1);
            var result2 = nint.RotateLeft(n, rotateAmount);

            nint.RotateLeft(n, 0);
        }
    }
}