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
            var result11 = nint.DivRem(0, 10);

            (nint, nint) result21 = nint.DivRem(0, 10);

            (nint quotient, nint remainder) result32 = nint.DivRem(0, 10);

            var result71 = Math.DivRem((nint)0, (nint)10);
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

        public void RotateLeft(nint n)
        {
            const int c = 1;

            var result11 = nint.RotateLeft(n, 0);
            var result12 = nint.RotateLeft(0x01, 0);
            var result13 = nint.RotateLeft(1 + 1, 0);
            var result14 = nint.RotateLeft(c, 0);
            var result15 = nint.RotateLeft('a', 0);

            nint result21 = nint.RotateLeft(n, 0);
            nint result22 = nint.RotateLeft(0x01, 0);
            nint result23 = nint.RotateLeft(1 + 1, 0);
            nint result24 = nint.RotateLeft(c, 0);
            nint result25 = nint.RotateLeft('a', 0);
        }

        public void RotateRight(nint n)
        {
            const int c = 1;

            var result11 = nint.RotateRight(n, 0);
            var result12 = nint.RotateRight(0x01, 0);
            var result13 = nint.RotateRight(1 + 1, 0);
            var result14 = nint.RotateRight(c, 0);
            var result15 = nint.RotateRight('a', 0);

            nint result21 = nint.RotateRight(n, 0);
            nint result22 = nint.RotateRight(0x01, 0);
            nint result23 = nint.RotateRight(1 + 1, 0);
            nint result24 = nint.RotateRight(c, 0);
            nint result25 = nint.RotateRight('a', 0);
        }
    }
}