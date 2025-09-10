using System;

namespace Test
{
    public class UInt16s
    {
        public void Clamp(ushort number)
        {
            var result11 = ushort.Clamp(number, 1, 0x01);
            var result12 = ushort.Clamp(number, 97, 'a');

            ushort result21 = ushort.Clamp(number, 1, 0x01);
            ushort result22 = ushort.Clamp(number, 97, 'a');

            var result31 = ushort.Clamp(number, ushort.MinValue, ushort.MaxValue);
            var result32 = ushort.Clamp(1, ushort.MinValue, ushort.MaxValue);
            var result33 = ushort.Clamp('a', ushort.MinValue, ushort.MaxValue);

            ushort result41 = ushort.Clamp(number, ushort.MinValue, ushort.MaxValue);
            ushort result42 = ushort.Clamp(1, ushort.MinValue, ushort.MaxValue);
            ushort result43 = ushort.Clamp('a', ushort.MinValue, ushort.MaxValue);

            var result51 = Math.Clamp(number, (ushort)1, (ushort)1);
            var result52 = Math.Clamp(number, ushort.MinValue, ushort.MaxValue);
        }

        public void DivRem(ushort left)
        {
            var result11 = ushort.DivRem(0, 10);

            (ushort, ushort) result21 = ushort.DivRem(0, 10);

            (ushort quotient, ushort remainder) result32 = ushort.DivRem(0, 10);

            var result71 = Math.DivRem((ushort)0, (ushort)10);
        }

        public void Equals(ushort number)
        {
            var result = number.Equals(null);
        }

        public void GetTypeCode(ushort number)
        {
            var result = number.GetTypeCode();
        }

        public void Max()
        {
            const int c = 10;

            var result11 = ushort.Max(10, 0x0A);
            var result12 = ushort.Max(10, c);
            var result13 = ushort.Max(97, 'a');

            ushort result21 = ushort.Max(10, 0x0A);
            ushort result22 = ushort.Max(10, c);
            ushort result23 = ushort.Max(97, 'a');

            var result31 = Math.Max((ushort)10, (ushort)10);
        }

        public void Min()
        {
            const int c = 10;

            var result11 = ushort.Min(10, 0x0A);
            var result12 = ushort.Min(10, c);
            var result13 = ushort.Min(97, 'a');

            ushort result21 = ushort.Min(10, 0x0A);
            ushort result22 = ushort.Min(10, c);
            ushort result23 = ushort.Min(97, 'a');

            var result31 = Math.Min((ushort)10, (ushort)10);
        }

        public void RotateLeft(ushort n)
        {
            const int c = 1;

            var result11 = ushort.RotateLeft(n, 0);
            var result12 = ushort.RotateLeft(0x01, 0);
            var result13 = ushort.RotateLeft(1 + 1, 0);
            var result14 = ushort.RotateLeft(c, 0);
            var result15 = ushort.RotateLeft('a', 0);

            ushort result21 = ushort.RotateLeft(n, 0);
            ushort result22 = ushort.RotateLeft(1, 0);
            ushort result23 = ushort.RotateLeft(1 + 1, 0);
            ushort result24 = ushort.RotateLeft(c, 0);
            ushort result25 = ushort.RotateLeft('a', 0);
        }

        public void RotateRight(ushort n)
        {
            const int c = 1;

            var result11 = ushort.RotateRight(n, 0);
            var result12 = ushort.RotateRight(0x01, 0);
            var result13 = ushort.RotateRight(1 + 1, 0);
            var result14 = ushort.RotateRight(c, 0);
            var result15 = ushort.RotateRight('a', 0);

            ushort result21 = ushort.RotateRight(n, 0);
            ushort result22 = ushort.RotateRight(1, 0);
            ushort result23 = ushort.RotateRight(1 + 1, 0);
            ushort result24 = ushort.RotateRight(c, 0);
            ushort result25 = ushort.RotateRight('a', 0);
        }
    }
}