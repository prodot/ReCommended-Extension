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
            var result11 = nuint.DivRem(0, 10);

            (nuint, nuint) result21 = nuint.DivRem(0, 10);

            (nuint quotient, nuint remainder) result32 = nuint.DivRem(0, 10);

            var result71 = Math.DivRem((nuint)0, (nuint)10);
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

        public void RotateLeft(nuint n)
        {
            const int c = 1;

            var result11 = nuint.RotateLeft(n, 0);
            var result12 = nuint.RotateLeft(0x01, 0);
            var result13 = nuint.RotateLeft(1 + 1, 0);
            var result14 = nuint.RotateLeft(c, 0);
            var result15 = nuint.RotateLeft('a', 0);

            nuint result21 = nuint.RotateLeft(n, 0);
            nuint result22 = nuint.RotateLeft(0x01, 0);
            nuint result23 = nuint.RotateLeft(1 + 1, 0);
            nuint result24 = nuint.RotateLeft(c, 0);
            nuint result25 = nuint.RotateLeft('a', 0);
        }

        public void RotateRight(nuint n)
        {
            const int c = 1;

            var result11 = nuint.RotateRight(n, 0);
            var result12 = nuint.RotateRight(0x01, 0);
            var result13 = nuint.RotateRight(1 + 1, 0);
            var result14 = nuint.RotateRight(c, 0);
            var result15 = nuint.RotateRight('a', 0);

            nuint result21 = nuint.RotateRight(n, 0);
            nuint result22 = nuint.RotateRight(0x01, 0);
            nuint result23 = nuint.RotateRight(1 + 1, 0);
            nuint result24 = nuint.RotateRight(c, 0);
            nuint result25 = nuint.RotateRight('a', 0);
        }
    }
}