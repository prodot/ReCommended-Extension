namespace Test
{
    public class Int32s
    {
        public void ExpressionResult(int number)
        {
            var result = int.MaxMagnitude(10, 10);
        }

        public void NoDetection(int x, int y)
        {
            var result11 = int.MaxMagnitude(1, 2);
            var result12 = int.MaxMagnitude(x, 2);
            var result13 = int.MaxMagnitude(1, y);

            int.MaxMagnitude(10, 10);
        }
    }
}