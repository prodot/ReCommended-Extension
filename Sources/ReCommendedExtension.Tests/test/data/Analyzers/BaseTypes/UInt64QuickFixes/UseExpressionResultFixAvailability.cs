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

            var result41 = ulong.DivRem(left, 1);
            var result42 = ulong.DivRem(0x10, 1);
            var result43 = ulong.DivRem(0x10u, 1);
            var result44 = ulong.DivRem(0x10L, 1);
            var result45 = ulong.DivRem(0x10ul, 1);
            var result46 = ulong.DivRem(c, 1);
            var result47 = ulong.DivRem('a', 1);

            (ulong, ulong) result51 = ulong.DivRem(left, 1);
            (ulong, ulong) result52 = ulong.DivRem(0x10, 1);
            (ulong, ulong) result53 = ulong.DivRem(0x10u, 1);
            (ulong, ulong) result54 = ulong.DivRem(0x10L, 1);
            (ulong, ulong) result55 = ulong.DivRem(0x10ul, 1);
            (ulong, ulong) result56 = ulong.DivRem(c, 1);
            (ulong, ulong) result57 = ulong.DivRem('a', 1);

            (ulong quotient, ulong remainder) result61 = ulong.DivRem(left, 1);
            (ulong quotient, ulong remainder) result62 = ulong.DivRem(0x10, 1);
            (ulong quotient, ulong remainder) result63 = ulong.DivRem(0x10u, 1);
            (ulong quotient, ulong remainder) result64 = ulong.DivRem(0x10L, 1);
            (ulong quotient, ulong remainder) result65 = ulong.DivRem(0x10ul, 1);
            (ulong quotient, ulong remainder) result66 = ulong.DivRem(c, 1);
            (ulong quotient, ulong remainder) result67 = ulong.DivRem('a', 1);

            var result71 = Math.DivRem(0ul, 10ul);
            var result72 = Math.DivRem(left, 1ul);
        }

        public void Equals(ulong number)
        {
            var result = number.Equals(null);
        }

        public void GetTypeCode(ulong number)
        {
            var result = number.GetTypeCode();
        }

        public void Max()
        {
            const int c = 10;

            var result11 = ulong.Max(10, 0x0A);
            var result12 = ulong.Max(10, 10u);
            var result13 = ulong.Max(10, 10L);
            var result14 = ulong.Max(10, 10ul);
            var result15 = ulong.Max(10, c);
            var result16 = ulong.Max(97, 'a');

            ulong result21 = ulong.Max(10, 0x0A);
            ulong result22 = ulong.Max(10, 10u);
            ulong result23 = ulong.Max(10, 10L);
            ulong result24 = ulong.Max(10, 10ul);
            ulong result25 = ulong.Max(10, c);
            ulong result26 = ulong.Max(97, 'a');

            var result31 = Math.Max(10ul, 10ul);
        }

        public void Min()
        {
            const int c = 10;

            var result11 = ulong.Min(10, 0x0A);
            var result12 = ulong.Min(10, 10u);
            var result13 = ulong.Min(10, 10L);
            var result14 = ulong.Min(10, 10ul);
            var result15 = ulong.Min(10, c);
            var result16 = ulong.Min(97, 'a');

            ulong result21 = ulong.Min(10, 0x0A);
            ulong result22 = ulong.Min(10, 10u);
            ulong result23 = ulong.Min(10, 10L);
            ulong result24 = ulong.Min(10, 10ul);
            ulong result25 = ulong.Min(10, c);
            ulong result26 = ulong.Min(97, 'a');

            var result31 = Math.Min(10ul, 10ul);
        }
    }
}