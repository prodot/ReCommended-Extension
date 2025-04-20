using System;

namespace Test
{
    public class UInt128s
    {
        public void Clamp(UInt128 number)
        {
            const int c = 1;

            var result11 = UInt128.Clamp(number, 1, 0x0001);
            var result12 = UInt128.Clamp(number, 1, 0x0001L);
            var result13 = UInt128.Clamp(number, 1, 0x0001u);
            var result14 = UInt128.Clamp(number, 1, 0x0001ul);
            var result15 = UInt128.Clamp(number, 1, c);
            var result16 = UInt128.Clamp(number, 97, 'a');

            UInt128 result21 = UInt128.Clamp(number, 1, 0x0001);
            UInt128 result22 = UInt128.Clamp(number, 1, 0x0001L);
            UInt128 result23 = UInt128.Clamp(number, 1, 0x0001u);
            UInt128 result24 = UInt128.Clamp(number, 1, 0x0001ul);
            UInt128 result25 = UInt128.Clamp(number, 1, c);
            UInt128 result26 = UInt128.Clamp(number, 97, 'a');
        }

        public void DivRem(UInt128 left)
        {
            const int c = 1;

            var result11 = UInt128.DivRem(0, 10);

            (UInt128, UInt128) result21 = UInt128.DivRem(0, 10);

            (UInt128 quotient, UInt128 remainder) result32 = UInt128.DivRem(0, 10);

            var result31 = UInt128.DivRem(left, 1);
            var result32 = UInt128.DivRem(0x10, 1);
            var result33 = UInt128.DivRem(0x10L, 1);
            var result34 = UInt128.DivRem(0x10u, 1);
            var result35 = UInt128.DivRem(0x10ul, 1);
            var result36 = UInt128.DivRem(c, 1);
            var result37 = UInt128.DivRem('a', 1);

            (UInt128, UInt128) result41 = UInt128.DivRem(left, 1);
            (UInt128, UInt128) result42 = UInt128.DivRem(0x10, 1);
            (UInt128, UInt128) result43 = UInt128.DivRem(0x10L, 1);
            (UInt128, UInt128) result44 = UInt128.DivRem(0x10u, 1);
            (UInt128, UInt128) result45 = UInt128.DivRem(0x10ul, 1);
            (UInt128, UInt128) result46 = UInt128.DivRem(c, 1);
            (UInt128, UInt128) result47 = UInt128.DivRem('a', 1);

            (UInt128 quotient, UInt128 remainder) result51 = UInt128.DivRem(left, 1);
            (UInt128 quotient, UInt128 remainder) result52 = UInt128.DivRem(0x10, 1);
            (UInt128 quotient, UInt128 remainder) result53 = UInt128.DivRem(0x10L, 1);
            (UInt128 quotient, UInt128 remainder) result54 = UInt128.DivRem(0x10u, 1);
            (UInt128 quotient, UInt128 remainder) result55 = UInt128.DivRem(0x10ul, 1);
            (UInt128 quotient, UInt128 remainder) result56 = UInt128.DivRem(c, 1);
            (UInt128 quotient, UInt128 remainder) result57 = UInt128.DivRem('a', 1);
        }

        public void Equals(UInt128 number)
        {
            var result = number.Equals(null);
        }

        public void Max()
        {
            const int c = 10;

            var result11 = UInt128.Max(10, 0x0A);
            var result12 = UInt128.Max(10, 10ul);
            var result13 = UInt128.Max(10, 10u);
            var result14 = UInt128.Max(10, 10L);
            var result15 = UInt128.Max(10, c);
            var result16 = UInt128.Max(97, 'a');

            UInt128 result11 = UInt128.Max(10, 0x0A);
            UInt128 result12 = UInt128.Max(10, 10ul);
            UInt128 result13 = UInt128.Max(10, 10u);
            UInt128 result14 = UInt128.Max(10, 10L);
            UInt128 result15 = UInt128.Max(10, c);
            UInt128 result16 = UInt128.Max(97, 'a');
        }

        public void Min()
        {
            const int c = 10;

            var result11 = UInt128.Min(10, 0x0A);
            var result12 = UInt128.Min(10, 10ul);
            var result13 = UInt128.Min(10, 10u);
            var result14 = UInt128.Min(10, 10L);
            var result15 = UInt128.Min(10, c);
            var result16 = UInt128.Min(97, 'a');

            UInt128 result11 = UInt128.Min(10, 0x0A);
            UInt128 result12 = UInt128.Min(10, 10ul);
            UInt128 result13 = UInt128.Min(10, 10u);
            UInt128 result14 = UInt128.Min(10, 10L);
            UInt128 result15 = UInt128.Min(10, c);
            UInt128 result16 = UInt128.Min(97, 'a');
        }
    }
}