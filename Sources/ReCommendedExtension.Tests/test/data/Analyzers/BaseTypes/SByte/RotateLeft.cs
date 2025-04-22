namespace Test
{
    public class SBytes
    {
        public void ExpressionResult(sbyte n)
        {
            var result = sbyte.RotateLeft(n, 0);
        }

        public void NoDetection(sbyte n, int rotateAmount)
        {
            var result1 = sbyte.RotateLeft(n, 1);
            var result2 = sbyte.RotateLeft(n, rotateAmount);

            sbyte.RotateLeft(n, 0);
        }
    }
}