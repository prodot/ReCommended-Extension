using System;

namespace Test
{
    public class UInt16s
    {
        public void ExpressionResult(ushort number)
        {
            var result1 = ushort.Max(10, 10);

            var result2 = Math.Max((ushort)10, (ushort)10);
        }

        public void NoDetection(ushort x, ushort y)
        {
            var result11 = ushort.Max(1, 2);
            var result12 = ushort.Max(x, 2);
            var result13 = ushort.Max(1, y);

            ushort.Max(10, 10);

            Math.Max((ushort)10, (ushort)10);
        }
    }
}