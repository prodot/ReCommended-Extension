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
            var result11 = Int128.DivRem(0, 10);

            (Int128, Int128) result21 = Int128.DivRem(0, 10);

            (Int128 quotient, Int128 remainder) result32 = Int128.DivRem(0, 10);
        }

        public void Equals(Int128 number)
        {
            var result = number.Equals(null);
        }

        public void Max()
        {
            const int c = 10;

            var result11 = Int128.Max(10, 0x0A);
            var result12 = Int128.Max(10, 10ul);
            var result13 = Int128.Max(10, 10u);
            var result14 = Int128.Max(10, 10L);
            var result15 = Int128.Max(10, c);
            var result16 = Int128.Max(97, 'a');

            Int128 result21 = Int128.Max(10, 0x0A);
            Int128 result22 = Int128.Max(10, 10ul);
            Int128 result23 = Int128.Max(10, 10u);
            Int128 result24 = Int128.Max(10, 10L);
            Int128 result25 = Int128.Max(10, c);
            Int128 result26 = Int128.Max(97, 'a');
        }

        public void MaxMagnitude()
        {
            const int c = 10;

            var result11 = Int128.MaxMagnitude(10, 0x0A);
            var result12 = Int128.MaxMagnitude(10, 10ul);
            var result13 = Int128.MaxMagnitude(10, 10u);
            var result14 = Int128.MaxMagnitude(10, 10L);
            var result15 = Int128.MaxMagnitude(10, c);
            var result16 = Int128.MaxMagnitude(97, 'a');

            Int128 result21 = Int128.MaxMagnitude(10, 0x0A);
            Int128 result22 = Int128.MaxMagnitude(10, 10ul);
            Int128 result23 = Int128.MaxMagnitude(10, 10u);
            Int128 result24 = Int128.MaxMagnitude(10, 10L);
            Int128 result25 = Int128.MaxMagnitude(10, c);
            Int128 result26 = Int128.MaxMagnitude(97, 'a');
        }

        public void Min()
        {
            const int c = 10;

            var result11 = Int128.Min(10, 0x0A);
            var result12 = Int128.Min(10, 10ul);
            var result13 = Int128.Min(10, 10u);
            var result14 = Int128.Min(10, 10L);
            var result15 = Int128.Min(10, c);
            var result16 = Int128.Min(97, 'a');

            Int128 result21 = Int128.Min(10, 0x0A);
            Int128 result22 = Int128.Min(10, 10ul);
            Int128 result23 = Int128.Min(10, 10u);
            Int128 result24 = Int128.Min(10, 10L);
            Int128 result25 = Int128.Min(10, c);
            Int128 result26 = Int128.Min(97, 'a');
        }

        public void MinMagnitude()
        {
            const int c = 10;

            var result11 = Int128.MinMagnitude(10, 0x0A);
            var result12 = Int128.MinMagnitude(10, 10ul);
            var result13 = Int128.MinMagnitude(10, 10u);
            var result14 = Int128.MinMagnitude(10, 10L);
            var result15 = Int128.MinMagnitude(10, c);
            var result16 = Int128.MinMagnitude(97, 'a');

            Int128 result21 = Int128.MinMagnitude(10, 0x0A);
            Int128 result22 = Int128.MinMagnitude(10, 10ul);
            Int128 result23 = Int128.MinMagnitude(10, 10u);
            Int128 result24 = Int128.MinMagnitude(10, 10L);
            Int128 result25 = Int128.MinMagnitude(10, c);
            Int128 result26 = Int128.MinMagnitude(97, 'a');
        }

        public void RotateLeft(Int128 n)
        {
            const int c = 1;

            var result11 = Int128.RotateLeft(n, 0);
            var result12 = Int128.RotateLeft(0x01, 0);
            var result13 = Int128.RotateLeft(0x01u, 0);
            var result14 = Int128.RotateLeft(0x01L, 0);
            var result15 = Int128.RotateLeft(0x01ul, 0);
            var result16 = Int128.RotateLeft(1 + 1, 0);
            var result17 = Int128.RotateLeft(1u + 1u, 0);
            var result18 = Int128.RotateLeft(1L + 1L, 0);
            var result19 = Int128.RotateLeft(1ul + 1ul, 0);
            var result1A = Int128.RotateLeft(c, 0);
            var result1B = Int128.RotateLeft('a', 0);

            Int128 result21 = Int128.RotateLeft(n, 0);
            Int128 result22 = Int128.RotateLeft(0x01, 0);
            Int128 result23 = Int128.RotateLeft(0x01u, 0);
            Int128 result24 = Int128.RotateLeft(0x01L, 0);
            Int128 result25 = Int128.RotateLeft(0x01ul, 0);
            Int128 result26 = Int128.RotateLeft(1 + 1, 0);
            Int128 result27 = Int128.RotateLeft(1u + 1u, 0);
            Int128 result28 = Int128.RotateLeft(1L + 1L, 0);
            Int128 result29 = Int128.RotateLeft(1ul + 1ul, 0);
            Int128 result2A = Int128.RotateLeft(c, 0);
            Int128 result2B = Int128.RotateLeft('a', 0);
        }

        public void RotateRight(Int128 n)
        {
            const int c = 1;

            var result11 = Int128.RotateRight(n, 0);
            var result12 = Int128.RotateRight(0x01, 0);
            var result13 = Int128.RotateRight(0x01u, 0);
            var result14 = Int128.RotateRight(0x01L, 0);
            var result15 = Int128.RotateRight(0x01ul, 0);
            var result16 = Int128.RotateRight(1 + 1, 0);
            var result17 = Int128.RotateRight(1u + 1u, 0);
            var result18 = Int128.RotateRight(1L + 1L, 0);
            var result19 = Int128.RotateRight(1ul + 1ul, 0);
            var result1A = Int128.RotateRight(c, 0);
            var result1B = Int128.RotateRight('a', 0);

            Int128 result21 = Int128.RotateRight(n, 0);
            Int128 result22 = Int128.RotateRight(0x01, 0);
            Int128 result23 = Int128.RotateRight(0x01u, 0);
            Int128 result24 = Int128.RotateRight(0x01L, 0);
            Int128 result25 = Int128.RotateRight(0x01ul, 0);
            Int128 result26 = Int128.RotateRight(1 + 1, 0);
            Int128 result27 = Int128.RotateRight(1u + 1u, 0);
            Int128 result28 = Int128.RotateRight(1L + 1L, 0);
            Int128 result29 = Int128.RotateRight(1ul + 1ul, 0);
            Int128 result2A = Int128.RotateRight(c, 0);
            Int128 result2B = Int128.RotateRight('a', 0);
        }
    }
}