namespace Test
{
    public class Bytes
    {
        public void ExpressionResult()
        {
            var result = byte.Min(10, 10);
        }

        public void NoDetection(byte x, byte y)
        {
            var result1 = byte.Min(1, 2);
            var result2 = byte.Min(x, 2);
            var result3 = byte.Min(1, y);

            byte.Min(10, 10);
        }
    }
}