namespace Test
{
    public class Bytes
    {
        public void ExpressionResult(byte left)
        {
            var result1 = byte.DivRem(0, 10);
            var result2 = byte.DivRem(left, 1);
        }

        public void NoDetection(byte left, byte right)
        {
            var result1 = byte.DivRem(0, right);
            var result2 = byte.DivRem(left, 2);

            byte.DivRem(0, 10);
            byte.DivRem(left, 1);
        }
    }
}