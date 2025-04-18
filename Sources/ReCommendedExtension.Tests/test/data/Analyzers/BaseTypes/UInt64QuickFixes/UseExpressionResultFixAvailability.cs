using System;

namespace Test
{
    public class UInt64s
    {
        public void Clamp(ulong number)
        {
            const int c = 1;

            var result11 = ulong.Clamp(number, 1, 1);
            var result12 = ulong.Clamp(number, 1, 1u);
            var result13 = ulong.Clamp(number, 1, 1L);
            var result14 = ulong.Clamp(number, 1, 1ul);
            var result15 = ulong.Clamp(number, 1, c);
            var result16 = ulong.Clamp(number, 97, 'a');

            ulong result21 = ulong.Clamp(number, 1, 1);
            ulong result22 = ulong.Clamp(number, 1, 1u);
            ulong result23 = ulong.Clamp(number, 1, 1L);
            ulong result24 = ulong.Clamp(number, 1, 1ul);
            ulong result25 = ulong.Clamp(number, 1, c);
            ulong result26 = ulong.Clamp(number, 97, 'a');

            var result31 = ulong.Clamp(number, ulong.MinValue, ulong.MaxValue);
            var result32 = ulong.Clamp(1, ulong.MinValue, ulong.MaxValue);
            var result33 = ulong.Clamp(1u, ulong.MinValue, ulong.MaxValue);
            var result34 = ulong.Clamp(1L, ulong.MinValue, ulong.MaxValue);
            var result35 = ulong.Clamp(1ul, ulong.MinValue, ulong.MaxValue);
            var result36 = ulong.Clamp(c, ulong.MinValue, ulong.MaxValue);
            var result37 = ulong.Clamp('a', ulong.MinValue, ulong.MaxValue);

            ulong result41 = ulong.Clamp(number, ulong.MinValue, ulong.MaxValue);
            ulong result42 = ulong.Clamp(1, ulong.MinValue, ulong.MaxValue);
            ulong result43 = ulong.Clamp(1u, ulong.MinValue, ulong.MaxValue);
            ulong result44 = ulong.Clamp(1L, ulong.MinValue, ulong.MaxValue);
            ulong result45 = ulong.Clamp(1ul, ulong.MinValue, ulong.MaxValue);
            ulong result46 = ulong.Clamp(c, ulong.MinValue, ulong.MaxValue);
            ulong result47 = ulong.Clamp('a', ulong.MinValue, ulong.MaxValue);

            var result51 = Math.Clamp(number, 1ul, 1ul);
            var result52 = Math.Clamp(number, ulong.MinValue, ulong.MaxValue);
        }

        public void DivRem(ulong left)
        {
            const int c = 1;

            var result11 = ulong.DivRem(0, 10);

            (ulong, ulong) result21 = ulong.DivRem(0, 10);

            (ulong quotient, ulong remainder) result32 = ulong.DivRem(0, 10);

            var result31 = ulong.DivRem(left, 1);
            var result32 = ulong.DivRem(0x10, 1);
            var result33 = ulong.DivRem(0x10u, 1);
            var result34 = ulong.DivRem(0x10L, 1);
            var result35 = ulong.DivRem(0x10ul, 1);
            var result36 = ulong.DivRem(c, 1);
            var result37 = ulong.DivRem('a', 1);

            (ulong, ulong) result41 = ulong.DivRem(left, 1);
            (ulong, ulong) result42 = ulong.DivRem(0x10, 1);
            (ulong, ulong) result43 = ulong.DivRem(0x10u, 1);
            (ulong, ulong) result44 = ulong.DivRem(0x10L, 1);
            (ulong, ulong) result45 = ulong.DivRem(0x10ul, 1);
            (ulong, ulong) result46 = ulong.DivRem(c, 1);
            (ulong, ulong) result47 = ulong.DivRem('a', 1);

            (ulong quotient, ulong remainder) result51 = ulong.DivRem(left, 1);
            (ulong quotient, ulong remainder) result52 = ulong.DivRem(0x10, 1);
            (ulong quotient, ulong remainder) result53 = ulong.DivRem(0x10u, 1);
            (ulong quotient, ulong remainder) result54 = ulong.DivRem(0x10L, 1);
            (ulong quotient, ulong remainder) result55 = ulong.DivRem(0x10ul, 1);
            (ulong quotient, ulong remainder) result56 = ulong.DivRem(c, 1);
            (ulong quotient, ulong remainder) result57 = ulong.DivRem('a', 1);

            var result61 = Math.DivRem(0ul, 10ul);
            var result62 = Math.DivRem(left, 1ul);
        }

        public void Equals(ulong number)
        {
            var result = number.Equals(null);
        }

        public void GetTypeCode(ulong number)
        {
            var result = number.GetTypeCode();
        }
    }
}