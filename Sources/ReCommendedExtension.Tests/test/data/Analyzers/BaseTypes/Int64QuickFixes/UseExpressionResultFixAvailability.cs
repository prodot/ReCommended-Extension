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
            var result11 = long.DivRem(0, 10);

            (long, long) result21 = long.DivRem(0, 10);

            (long quotient, long remainder) result32 = long.DivRem(0, 10);

            var result71 = Math.DivRem(0L, 10L);
        }

        public void Equals(long number)
        {
            var result = number.Equals(null);
        }

        public void GetTypeCode(long number)
        {
            var result = number.GetTypeCode();
        }

        public void Max()
        {
            const int c = 10;

            var result11 = long.Max(10, 0x0A);
            var result12 = long.Max(10, 10L);
            var result13 = long.Max(10, 10u);
            var result14 = long.Max(10, c);
            var result15 = long.Max(97, 'a');

            long result21 = long.Max(10, 0x0A);
            long result22 = long.Max(10, 10L);
            long result23 = long.Max(10, 10u);
            long result24 = long.Max(10, c);
            long result25 = long.Max(97, 'a');

            var result31 = Math.Max(10L, 10L);
        }

        public void Min()
        {
            const int c = 10;

            var result11 = long.Min(10, 0x0A);
            var result12 = long.Min(10, 10L);
            var result13 = long.Min(10, 10u);
            var result14 = long.Min(10, c);
            var result15 = long.Min(97, 'a');

            long result21 = long.Min(10, 0x0A);
            long result22 = long.Min(10, 10L);
            long result23 = long.Min(10, 10u);
            long result24 = long.Min(10, c);
            long result25 = long.Min(97, 'a');

            var result31 = Math.Min(10L, 10L);
        }

        public void RotateLeft(long n)
        {
            const int c = 1;

            var result11 = long.RotateLeft(n, 0);
            var result12 = long.RotateLeft(0x01, 0);
            var result13 = long.RotateLeft(0x01u, 0);
            var result14 = long.RotateLeft(0x01L, 0);
            var result15 = long.RotateLeft(1 + 1, 0);
            var result16 = long.RotateLeft(1u + 1u, 0);
            var result17 = long.RotateLeft(1L + 1L, 0);
            var result18 = long.RotateLeft(c, 0);
            var result19 = long.RotateLeft('a', 0);

            long result21 = long.RotateLeft(n, 0);
            long result22 = long.RotateLeft(0x01, 0);
            long result23 = long.RotateLeft(0x01u, 0);
            long result24 = long.RotateLeft(0x01L, 0);
            long result25 = long.RotateLeft(1 + 1, 0);
            long result26 = long.RotateLeft(1u + 1u, 0);
            long result27 = long.RotateLeft(1L + 1L, 0);
            long result28 = long.RotateLeft(c, 0);
            long result29 = long.RotateLeft('a', 0);
        }

        public void RotateRight(long n)
        {
            const int c = 1;

            var result11 = long.RotateRight(n, 0);
            var result12 = long.RotateRight(0x01, 0);
            var result13 = long.RotateRight(0x01u, 0);
            var result14 = long.RotateRight(0x01L, 0);
            var result15 = long.RotateRight(1 + 1, 0);
            var result16 = long.RotateRight(1u + 1u, 0);
            var result17 = long.RotateRight(1L + 1L, 0);
            var result18 = long.RotateRight(c, 0);
            var result19 = long.RotateRight('a', 0);

            long result21 = long.RotateRight(n, 0);
            long result22 = long.RotateRight(0x01, 0);
            long result23 = long.RotateRight(0x01u, 0);
            long result24 = long.RotateRight(0x01L, 0);
            long result25 = long.RotateRight(1 + 1, 0);
            long result26 = long.RotateRight(1u + 1u, 0);
            long result27 = long.RotateRight(1L + 1L, 0);
            long result28 = long.RotateRight(c, 0);
            long result29 = long.RotateRight('a', 0);
        }
    }
}