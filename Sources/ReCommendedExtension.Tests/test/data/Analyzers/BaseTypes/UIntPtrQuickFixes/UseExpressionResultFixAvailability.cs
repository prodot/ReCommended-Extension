using System;

namespace Test
{
    public class UIntPtrs
    {
        public void Clamp(nuint number)
        {
            const int c = 1;

            var result11 = nuint.Clamp(number, 1, 0x01);
            var result12 = nuint.Clamp(number, 1, 0x01u);
            var result13 = nuint.Clamp(number, 1, c);
            var result14 = nuint.Clamp(number, 97, 'a');

            nuint result21 = nuint.Clamp(number, 1, 0x01);
            nuint result22 = nuint.Clamp(number, 1, 0x01u);
            nuint result23 = nuint.Clamp(number, 1, c);
            nuint result24 = nuint.Clamp(number, 97, 'a');

            var result3 = Math.Clamp(number, (nuint)1, (nuint)1);
        }

        public void DivRem(nuint left)
        {
            const int c = 1;

            var result11 = nuint.DivRem(0, 10);

            (nuint, nuint) result21 = nuint.DivRem(0, 10);

            (nuint quotient, nuint remainder) result32 = nuint.DivRem(0, 10);

            var result31 = nuint.DivRem(left, 1);
            var result32 = nuint.DivRem(0x10, 1);
            var result33 = nuint.DivRem(0x10u, 1);
            var result34 = nuint.DivRem(c, 1);
            var result35 = nuint.DivRem('a', 1);

            (nuint, nuint) result41 = nuint.DivRem(left, 1);
            (nuint, nuint) result42 = nuint.DivRem(0x10, 1);
            (nuint, nuint) result43 = nuint.DivRem(0x10u, 1);
            (nuint, nuint) result44 = nuint.DivRem(c, 1);
            (nuint, nuint) result45 = nuint.DivRem('a', 1);

            (nuint quotient, nuint remainder) result51 = nuint.DivRem(left, 1);
            (nuint quotient, nuint remainder) result52 = nuint.DivRem(0x10, 1);
            (nuint quotient, nuint remainder) result53 = nuint.DivRem(0x10u, 1);
            (nuint quotient, nuint remainder) result54 = nuint.DivRem(c, 1);
            (nuint quotient, nuint remainder) result55 = nuint.DivRem('a', 1);

            var result61 = Math.DivRem((nuint)0, (nuint)10);
            var result62 = Math.DivRem(left, (nuint)1);
        }

        public void Equals(nuint number)
        {
            var result = number.Equals(null);
        }

        public void Max()
        {
            const int c = 10;

            var result11 = nuint.Max(10, 0x0A);
            var result12 = nuint.Max(10, 10u);
            var result13 = nuint.Max(10, c);
            var result14 = nuint.Max(97, 'a');

            nuint result21 = nuint.Max(10, 0x0A);
            nuint result22 = nuint.Max(10, 10u);
            nuint result23 = nuint.Max(10, c);
            nuint result24 = nuint.Max(97, 'a');

            var result31 = Math.Max((nuint)10, (nuint)10);
        }

        public void Min()
        {
            const int c = 10;

            var result11 = nuint.Min(10, 0x0A);
            var result12 = nuint.Min(10, 10u);
            var result13 = nuint.Min(10, c);
            var result14 = nuint.Min(97, 'a');

            nuint result21 = nuint.Min(10, 0x0A);
            nuint result22 = nuint.Min(10, 10u);
            nuint result23 = nuint.Min(10, c);
            nuint result24 = nuint.Min(97, 'a');

            var result31 = Math.Min((nuint)10, (nuint)10);
        }
    }
}