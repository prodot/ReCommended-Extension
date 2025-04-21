using System;

namespace Test
{
    public class IntPtrs
    {
        public void Clamp(nint number)
        {
            const int c = 1;

            var result11 = nint.Clamp(number, 1, 0x01);
            var result12 = nint.Clamp(number, 1, c);
            var result13 = nint.Clamp(number, 97, 'a');

            nint result21 = nint.Clamp(number, 1, 0x01);
            nint result22 = nint.Clamp(number, 1, c);
            nint result23 = nint.Clamp(number, 97, 'a');

            var result3 = Math.Clamp(number, (nint)1, (nint)1);
        }

        public void DivRem(nint left)
        {
            const int c = 1;

            var result11 = nint.DivRem(0, 10);

            (nint, nint) result21 = nint.DivRem(0, 10);

            (nint quotient, nint remainder) result32 = nint.DivRem(0, 10);

            var result41 = nint.DivRem(left, 1);
            var result42 = nint.DivRem(0x10, 1);
            var result43 = nint.DivRem(c, 1);
            var result44 = nint.DivRem('a', 1);

            (nint, nint) result51 = nint.DivRem(left, 1);
            (nint, nint) result52 = nint.DivRem(0x10, 1);
            (nint, nint) result53 = nint.DivRem(c, 1);
            (nint, nint) result54 = nint.DivRem('a', 1);

            (nint quotient, nint remainder) result61 = nint.DivRem(left, 1);
            (nint quotient, nint remainder) result62 = nint.DivRem(0x10, 1);
            (nint quotient, nint remainder) result63 = nint.DivRem(c, 1);
            (nint quotient, nint remainder) result64 = nint.DivRem('a', 1);

            var result71 = Math.DivRem((nint)0, (nint)10);
            var result72 = Math.DivRem(left, (nint)1);
        }

        public void Equals(nint number)
        {
            var result = number.Equals(null);
        }

        public void Max()
        {
            const int c = 10;

            var result11 = nint.Max(10, 0x0A);
            var result12 = nint.Max(10, c);
            var result13 = nint.Max(97, 'a');

            nint result21 = nint.Max(10, 0x0A);
            nint result22 = nint.Max(10, c);
            nint result23 = nint.Max(97, 'a');

            var result31 = Math.Max((nint)10, (nint)10);
        }

        public void Min()
        {
            const int c = 10;

            var result11 = nint.Min(10, 0x0A);
            var result12 = nint.Min(10, c);
            var result13 = nint.Min(97, 'a');

            nint result21 = nint.Min(10, 0x0A);
            nint result22 = nint.Min(10, c);
            nint result23 = nint.Min(97, 'a');

            var result31 = Math.Min((nint)10, (nint)10);
        }
    }
}