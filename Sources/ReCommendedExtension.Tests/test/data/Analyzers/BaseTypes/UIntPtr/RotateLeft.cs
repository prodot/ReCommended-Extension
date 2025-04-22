namespace Test
{
    public class UIntPtrs
    {
        public void ExpressionResult(nuint n)
        {
            var result = nuint.RotateLeft(n, 0);
        }

        public void NoDetection(nuint n, int rotateAmount)
        {
            var result1 = nuint.RotateLeft(n, 1);
            var result2 = nuint.RotateLeft(n, rotateAmount);

            nuint.RotateLeft(n, 0);
        }
    }
}