using System;

namespace Test
{
    public class Int32s
    {
        public void ExpressionResult(int number)
        {
            var result11 = int.Clamp(number, 1, 1);
            var result12 = int.Clamp(number, int.MinValue, int.MaxValue);

            var result21 = Math.Clamp(number, 1, 1);
            var result22 = Math.Clamp(number, int.MinValue, int.MaxValue);
        }

        public void NoDetection(int number, int min, int max)
        {
            var result11 = int.Clamp(number, 1, max);
            var result12 = int.Clamp(number, min, 10);

            var result21 = Math.Clamp(number, 1, max);
            var result22 = Math.Clamp(number, min, int.MaxValue);

            int.Clamp(number, 1, 1);
            int.Clamp(number, int.MinValue, int.MaxValue);

            Math.Clamp(number, 1, 1);
            Math.Clamp(number, int.MinValue, int.MaxValue);
        }
    }
}