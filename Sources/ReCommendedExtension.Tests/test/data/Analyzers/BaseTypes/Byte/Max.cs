namespace Test
{
    public class Bytes
    {
        public void ExpressionResult()
        {
            var result = byte.Max(10, 10);
        }

        public void NoDetection(byte x, byte y)
        {
            var result1 = byte.Max(1, 2);
            var result2 = byte.Max(x, 2);
            var result3 = byte.Max(1, y);

            byte.Max(10, 10);
        }
    }
}