using System;

namespace Test
{
    public class Int128s
    {
        public void Clamp(Int128 number)
        {
            const int c = 1;

            var result11 = Int128.Clamp(number, 1, 0x0001);
            var result12 = Int128.Clamp(number, 1, 0x0001uL);
            var result13 = Int128.Clamp(number, 1, 0x0001u);
            var result14 = Int128.Clamp(number, 1, 0x0001L);
            var result15 = Int128.Clamp(number, 1, c);
            var result16 = Int128.Clamp(number, 97, 'a');

            Int128 result21 = Int128.Clamp(number, 1, 0x0001);
            Int128 result22 = Int128.Clamp(number, 1, 0x0001uL);
            Int128 result23 = Int128.Clamp(number, 1, 0x0001u);
            Int128 result24 = Int128.Clamp(number, 1, 0x0001L);
            Int128 result25 = Int128.Clamp(number, 1, c);
            Int128 result26 = Int128.Clamp(number, 97, 'a');
        }

        public void DivRem(Int128 left)
        {
            const int c = 1;

            var result11 = Int128.DivRem(0, 10);

            (Int128, Int128) result21 = Int128.DivRem(0, 10);

            (Int128 quotient, Int128 remainder) result32 = Int128.DivRem(0, 10);

            var result31 = Int128.DivRem(left, 1);
            var result32 = Int128.DivRem(0x10, 1);
            var result33 = Int128.DivRem(0x10ul, 1);
            var result34 = Int128.DivRem(0x10u, 1);
            var result35 = Int128.DivRem(0x10L, 1);
            var result36 = Int128.DivRem(c, 1);
            var result37 = Int128.DivRem('a', 1);

            (Int128, Int128) result41 = Int128.DivRem(left, 1);
            (Int128, Int128) result42 = Int128.DivRem(0x10, 1);
            (Int128, Int128) result43 = Int128.DivRem(0x10ul, 1);
            (Int128, Int128) result44 = Int128.DivRem(0x10u, 1);
            (Int128, Int128) result45 = Int128.DivRem(0x10L, 1);
            (Int128, Int128) result46 = Int128.DivRem(c, 1);
            (Int128, Int128) result47 = Int128.DivRem('a', 1);

            (Int128 quotient, Int128 remainder) result51 = Int128.DivRem(left, 1);
            (Int128 quotient, Int128 remainder) result52 = Int128.DivRem(0x10, 1);
            (Int128 quotient, Int128 remainder) result53 = Int128.DivRem(0x10ul, 1);
            (Int128 quotient, Int128 remainder) result54 = Int128.DivRem(0x10u, 1);
            (Int128 quotient, Int128 remainder) result55 = Int128.DivRem(0x10L, 1);
            (Int128 quotient, Int128 remainder) result56 = Int128.DivRem(c, 1);
            (Int128 quotient, Int128 remainder) result57 = Int128.DivRem('a', 1);
        }

        public void Equals(Int128 number)
        {
            var result = number.Equals(null);
        }
    }
}