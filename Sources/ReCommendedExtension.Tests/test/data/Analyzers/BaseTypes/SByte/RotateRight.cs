namespace Test
{
    public class SBytes
    {
        public void ExpressionResult(sbyte n)
        {
            var result = sbyte.RotateRight(n, 0);
        }

        public void NoDetection(sbyte n, int rotateAmount)
        {
            var result1 = sbyte.RotateRight(n, 1);
            var result2 = sbyte.RotateRight(n, rotateAmount);

            sbyte.RotateRight(n, 0);
        }
    }
}