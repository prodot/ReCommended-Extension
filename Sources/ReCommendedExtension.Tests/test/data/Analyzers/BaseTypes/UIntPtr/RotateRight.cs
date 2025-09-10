namespace Test
{
    public class UIntPtrs
    {
        public void ExpressionResult(nuint n)
        {
            var result = nuint.RotateRight(n, 0);
        }

        public void NoDetection(nuint n, int rotateAmount)
        {
            var result1 = nuint.RotateRight(n, 1);
            var result2 = nuint.RotateRight(n, rotateAmount);

            nuint.RotateRight(n, 0);
        }
    }
}