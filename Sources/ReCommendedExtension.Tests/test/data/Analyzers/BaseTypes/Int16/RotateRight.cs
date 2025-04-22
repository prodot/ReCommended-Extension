namespace Test
{
    public class Int16s
    {
        public void ExpressionResult(short n)
        {
            var result = short.RotateRight(n, 0);
        }

        public void NoDetection(short n, int rotateAmount)
        {
            var result1 = short.RotateRight(n, 1);
            var result2 = short.RotateRight(n, rotateAmount);

            short.RotateRight(n, 0);
        }
    }
}