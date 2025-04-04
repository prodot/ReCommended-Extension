namespace Test
{
    public class Bytes
    {
        public void ExpressionResult(byte number)
        {
            var result1 = byte.Clamp(number, 1, 1);
            var result2 = byte.Clamp(number, 0, 255);
        }

        public void NoDetection(byte number, byte min, byte max)
        {
            var result1 = byte.Clamp(number, 1, max);
            var result2 = byte.Clamp(number, min, 10);

            byte.Clamp(number, 1, 1);
            byte.Clamp(number, 0, 255);
        }
    }
}