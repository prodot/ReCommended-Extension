namespace Test
{
    public class UInt16s
    {
        public void ExpressionResult(ushort n)
        {
            var result = ushort.RotateRight(n, 0);
        }

        public void NoDetection(ushort n, int rotateAmount)
        {
            var result1 = ushort.RotateRight(n, 1);
            var result2 = ushort.RotateRight(n, rotateAmount);

            ushort.RotateRight(n, 0);
        }
    }
}