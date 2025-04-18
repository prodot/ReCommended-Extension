using System;

namespace Test
{
    public class SBytes
    {
        public void Clamp(sbyte number)
        {
            var result11 = sbyte.Clamp(number, 1, 0x01);

            sbyte result21 = sbyte.Clamp(number, 1, 0x01);

            var result31 = sbyte.Clamp(number, sbyte.MinValue, sbyte.MaxValue);
            var result32 = sbyte.Clamp(1, sbyte.MinValue, sbyte.MaxValue);

            sbyte result41 = sbyte.Clamp(number, sbyte.MinValue, sbyte.MaxValue);
            sbyte result42 = sbyte.Clamp(1, sbyte.MinValue, sbyte.MaxValue);

            var result51 = Math.Clamp(number, (sbyte)1, (sbyte)1);
            var result52 = Math.Clamp(number, sbyte.MinValue, sbyte.MaxValue);
        }

        public void DivRem(sbyte left)
        {
            const int c = 1;

            var result11 = sbyte.DivRem(0, 10);

            (sbyte, sbyte) result21 = sbyte.DivRem(0, 10);

            (sbyte quotient, sbyte remainder) result32 = sbyte.DivRem(0, 10);

            var result31 = sbyte.DivRem(left, 1);
            var result32 = sbyte.DivRem(0x10, 1);
            var result32 = sbyte.DivRem(c, 1);

            (sbyte, sbyte) result41 = sbyte.DivRem(left, 1);
            (sbyte, sbyte) result42 = sbyte.DivRem(0x10, 1);
            (sbyte, sbyte) result42 = sbyte.DivRem(c, 1);

            (sbyte quotient, sbyte remainder) result51 = sbyte.DivRem(left, 1);
            (sbyte quotient, sbyte remainder) result52 = sbyte.DivRem(0x10, 1);
            (sbyte quotient, sbyte remainder) result52 = sbyte.DivRem(c, 1);

            var result61 = Math.DivRem((sbyte)0, (sbyte)10);
            var result62 = Math.DivRem(left, (sbyte)1);
        }

        public void Equals(sbyte number)
        {
            var result = number.Equals(null);
        }

        public void GetTypeCode(sbyte number)
        {
            var result = number.GetTypeCode();
        }

        public void Max()
        {
            var result11 = sbyte.Max(10, 0x0A);

            sbyte result2 = sbyte.Max(10, 0x0A);

            var result3 = Math.Max((sbyte)10, (sbyte)10);
        }
    }
}