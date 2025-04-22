namespace Test
{
    public class UInt32s
    {
        public void ExpressionResult(uint n)
        {
            var result = uint.RotateLeft(n, 0);
        }

        public void NoDetection(uint n, int rotateAmount)
        {
            var result1 = uint.RotateLeft(n, 1);
            var result2 = uint.RotateLeft(n, rotateAmount);

            uint.RotateLeft(n, 0);
        }
    }
}