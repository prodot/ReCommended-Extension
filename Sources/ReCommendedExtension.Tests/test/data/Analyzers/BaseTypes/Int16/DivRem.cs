using System;

namespace Test
{
    public class Int16s
    {
        public void ExpressionResult(short left)
        {
            var result11 = short.DivRem(0, 10);
            var result12 = short.DivRem(left, 1);

            var result21 = Math.DivRem((short)0, (short)10);
            var result22 = Math.DivRem(left, (short)1);
        }

        public void NoDetection(short left, short right)
        {
            var result11 = short.DivRem(0, right);
            var result12 = short.DivRem(left, 2);

            var result21 = Math.DivRem((short)0, right);
            var result22 = Math.DivRem(left, (short)2);

            short.DivRem(0, 10);
            short.DivRem(left, 1);

            Math.DivRem((short)0, (short)10);
            Math.DivRem(left, (short)1);
        }
    }
}