namespace Test
{
    public class Int16s
    {
        public void ExpressionResult(short n)
        {
            var result = short.RotateLeft(n, 0);
        }

        public void NoDetection(short n, int rotateAmount)
        {
            var result1 = short.RotateLeft(n, 1);
            var result2 = short.RotateLeft(n, rotateAmount);

            short.RotateLeft(n, 0);
        }
    }
}