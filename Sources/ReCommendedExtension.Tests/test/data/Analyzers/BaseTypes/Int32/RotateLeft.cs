namespace Test
{
    public class Int32s
    {
        public void ExpressionResult(int n)
        {
            var result = int.RotateLeft(n, 0);
        }

        public void NoDetection(int n, int rotateAmount)
        {
            var result1 = int.RotateLeft(n, 1);
            var result2 = int.RotateLeft(n, rotateAmount);

            int.RotateLeft(n, 0);
        }
    }
}