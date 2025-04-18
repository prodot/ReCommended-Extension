using System;

namespace Test
{
    public class Int16s
    {
        public void ExpressionResult(short number)
        {
            var result1 = short.Max(10, 10);

            var result2 = Math.Max((short)10, (short)10);
        }

        public void NoDetection(short x, short y)
        {
            var result11 = short.Max(1, 2);
            var result12 = short.Max(x, 2);
            var result13 = short.Max(1, y);

            short.Max(10, 10);

            Math.Max((short)10, (short)10);
        }
    }
}