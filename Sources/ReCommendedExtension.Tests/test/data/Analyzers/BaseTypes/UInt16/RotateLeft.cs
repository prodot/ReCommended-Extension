namespace Test
{
    public class UInt16s
    {
        public void ExpressionResult(ushort n)
        {
            var result = ushort.RotateLeft(n, 0);
        }

        public void NoDetection(ushort n, int rotateAmount)
        {
            var result1 = ushort.RotateLeft(n, 1);
            var result2 = ushort.RotateLeft(n, rotateAmount);

            ushort.RotateLeft(n, 0);
        }
    }
}