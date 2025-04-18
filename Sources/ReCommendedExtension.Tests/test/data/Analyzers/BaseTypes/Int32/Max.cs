using System;

namespace Test
{
    public class Int32s
    {
        public void ExpressionResult(int number)
        {
            var result1 = int.Max(10, 10);

            var result2 = Math.Max(10, 10);
        }

        public void NoDetection(int x, int y)
        {
            var result11 = int.Max(1, 2);
            var result12 = int.Max(x, 2);
            var result13 = int.Max(1, y);

            int.Max(10, 10);

            Math.Max(10, 10);
        }
    }
}