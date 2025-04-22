namespace Test
{
    public class UInt32s
    {
        public void ExpressionResult(uint n)
        {
            var result = uint.RotateRight(n, 0);
        }

        public void NoDetection(uint n, int rotateAmount)
        {
            var result1 = uint.RotateRight(n, 1);
            var result2 = uint.RotateRight(n, rotateAmount);

            uint.RotateRight(n, 0);
        }
    }
}