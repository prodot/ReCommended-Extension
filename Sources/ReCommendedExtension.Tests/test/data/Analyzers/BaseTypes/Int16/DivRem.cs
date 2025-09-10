using System;

namespace Test
{
    public class Int16s
    {
        public void ExpressionResult(short left)
        {
            var result11 = short.DivRem(0, 10);

            var result21 = Math.DivRem((short)0, (short)10);
        }

        public void NoDetection(short left, short right)
        {
            var result11 = short.DivRem(0, right);

            var result21 = Math.DivRem((short)0, right);

            short.DivRem(0, 10);

            Math.DivRem((short)0, (short)10);
        }
    }
}