using System;

namespace Test
{
    public class Int64s
    {
        public void Clamp(long number)
        {
            const int c = 1;

            var result11 = long.Clamp(number, 1, 0x0001);
            var result12 = long.Clamp(number, 1, 0x0001L);
            var result13 = long.Clamp(number, 1, 0x0001u);
            var result14 = long.Clamp(number, 1, c);
            var result15 = long.Clamp(number, 97, 'a');

            long result21 = long.Clamp(number, 1, 0x0001);
            long result22 = long.Clamp(number, 1, 0x0001L);
            long result23 = long.Clamp(number, 1, 0x0001u);
            long result24 = long.Clamp(number, 1, c);
            long result25 = long.Clamp(number, 97, 'a');

            var result31 = long.Clamp(number, long.MinValue, long.MaxValue);
            var result32 = long.Clamp(1, long.MinValue, long.MaxValue);
            var result33 = long.Clamp(1L, long.MinValue, long.MaxValue);
            var result34 = long.Clamp(1u, long.MinValue, long.MaxValue);
            var result35 = long.Clamp(c, long.MinValue, long.MaxValue);
            var result36 = long.Clamp('a', long.MinValue, long.MaxValue);

            long result41 = long.Clamp(number, long.MinValue, long.MaxValue);
            long result42 = long.Clamp(1, long.MinValue, long.MaxValue);
            long result43 = long.Clamp(1L, long.MinValue, long.MaxValue);
            long result44 = long.Clamp(1u, long.MinValue, long.MaxValue);
            long result45 = long.Clamp(c, long.MinValue, long.MaxValue);
            long result46 = long.Clamp('a', long.MinValue, long.MaxValue);

            var result51 = Math.Clamp(number, 1L, 1L);
            var result52 = Math.Clamp(number, long.MinValue, long.MaxValue);
        }

        public void DivRem(long left)
        {
            const int c = 1;

            var result11 = long.DivRem(0, 10);

            (long, long) result21 = long.DivRem(0, 10);

            (long quotient, long remainder) result32 = long.DivRem(0, 10);

            var result31 = long.DivRem(left, 1);
            var result32 = long.DivRem(0x10, 1);
            var result33 = long.DivRem(0x10L, 1);
            var result34 = long.DivRem(0x10u, 1);
            var result35 = long.DivRem(c, 1);
            var result36 = long.DivRem('a', 1);

            (long, long) result41 = long.DivRem(left, 1);
            (long, long) result42 = long.DivRem(0x10, 1);
            (long, long) result43 = long.DivRem(0x10L, 1);
            (long, long) result44 = long.DivRem(0x10u, 1);
            (long, long) result45 = long.DivRem(c, 1);
            (long, long) result46 = long.DivRem('a', 1);

            (long quotient, long remainder) result51 = long.DivRem(left, 1);
            (long quotient, long remainder) result52 = long.DivRem(0x10, 1);
            (long quotient, long remainder) result53 = long.DivRem(0x10L, 1);
            (long quotient, long remainder) result54 = long.DivRem(0x10u, 1);
            (long quotient, long remainder) result55 = long.DivRem(c, 1);
            (long quotient, long remainder) result56 = long.DivRem('a', 1);

            var result61 = Math.DivRem(0L, 10L);
            var result62 = Math.DivRem(left, 1L);
        }

        public void Equals(long number)
        {
            var result = number.Equals(null);
        }

        public void GetTypeCode(long number)
        {
            var result = number.GetTypeCode();
        }
    }
}