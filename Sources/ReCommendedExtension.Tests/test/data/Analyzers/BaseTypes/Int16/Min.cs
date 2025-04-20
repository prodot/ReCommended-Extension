using System;

namespace Test
{
    public class Int16s
    {
        public void ExpressionResult(short number)
        {
            var result1 = short.Min(10, 10);

            var result2 = Math.Min((short)10, (short)10);
        }

        public void NoDetection(short x, short y)
        {
            var result11 = short.Min(1, 2);
            var result12 = short.Min(x, 2);
            var result13 = short.Min(1, y);

            short.Min(10, 10);

            Math.Min((short)10, (short)10);
        }
    }
}