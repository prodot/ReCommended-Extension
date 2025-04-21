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

            var result41 = short.DivRem(left, 1);
            var result42 = short.DivRem(0x10, 1);
            var result43 = short.DivRem(c, 1);

            (short, short) result51 = short.DivRem(left, 1);
            (short, short) result52 = short.DivRem(0x10, 1);
            (short, short) result53 = short.DivRem(c, 1);

            (short quotient, short remainder) result61 = short.DivRem(left, 1);
            (short quotient, short remainder) result62 = short.DivRem(0x10, 1);
            (short quotient, short remainder) result63 = short.DivRem(c, 1);

            var result71 = Math.DivRem((short)0, (short)10);
            var result72 = Math.DivRem(left, (short)1);
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

            var result3 = Math.Max((short)10, (short)10);
        }

        public void Min()
        {
            var result1 = short.Min(10, 0x0A);

            short result2 = short.Min(10, 0x0A);

            var result3 = Math.Min((short)10, (short)10);
        }
    }
}