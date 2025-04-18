using System;

namespace Test
{
    public class Int16s
    {
        public void Clamp(short number)
        {
            var result11 = short.Clamp(number, 1, 0x01);

            short result21 = short.Clamp(number, 1, 0x01);

            var result31 = short.Clamp(number, short.MinValue, short.MaxValue);
            var result32 = short.Clamp(1, short.MinValue, short.MaxValue);

            short result41 = short.Clamp(number, short.MinValue, short.MaxValue);
            short result42 = short.Clamp(1, short.MinValue, short.MaxValue);

            var result51 = Math.Clamp(number, (short)1, (short)1);
            var result52 = Math.Clamp(number, short.MinValue, short.MaxValue);
        }

        public void DivRem(short left)
        {
            const int c = 1;

            var result11 = short.DivRem(0, 10);

            (short, short) result21 = short.DivRem(0, 10);

            (short quotient, short remainder) result32 = short.DivRem(0, 10);

            var result31 = short.DivRem(left, 1);
            var result32 = short.DivRem(0x10, 1);
            var result32 = short.DivRem(c, 1);

            (short, short) result41 = short.DivRem(left, 1);
            (short, short) result42 = short.DivRem(0x10, 1);
            (short, short) result42 = short.DivRem(c, 1);

            (short quotient, short remainder) result51 = short.DivRem(left, 1);
            (short quotient, short remainder) result52 = short.DivRem(0x10, 1);
            (short quotient, short remainder) result52 = short.DivRem(c, 1);

            var result61 = Math.DivRem((short)0, (short)10);
            var result62 = Math.DivRem(left, (short)1);
        }

        public void Equals(short number)
        {
            var result = number.Equals(null);
        }

        public void GetTypeCode(short number)
        {
            var result = number.GetTypeCode();
        }

        public void Max()
        {
            var result1 = short.Max(10, 0x0A);

            short result2 = short.Max(10, 0x0A);

            var result2 = Math.Max((short)10, (short)10);
        }
    }
}