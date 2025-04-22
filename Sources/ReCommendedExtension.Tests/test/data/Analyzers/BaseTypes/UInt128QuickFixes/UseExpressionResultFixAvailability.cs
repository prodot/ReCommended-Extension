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
            var result11 = UInt128.DivRem(0, 10);

            (UInt128, UInt128) result21 = UInt128.DivRem(0, 10);

            (UInt128 quotient, UInt128 remainder) result32 = UInt128.DivRem(0, 10);
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

            UInt128 result21 = UInt128.Max(10, 0x0A);
            UInt128 result22 = UInt128.Max(10, 10ul);
            UInt128 result23 = UInt128.Max(10, 10u);
            UInt128 result24 = UInt128.Max(10, 10L);
            UInt128 result25 = UInt128.Max(10, c);
            UInt128 result26 = UInt128.Max(97, 'a');
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

            UInt128 result21 = UInt128.Min(10, 0x0A);
            UInt128 result22 = UInt128.Min(10, 10ul);
            UInt128 result23 = UInt128.Min(10, 10u);
            UInt128 result24 = UInt128.Min(10, 10L);
            UInt128 result25 = UInt128.Min(10, c);
            UInt128 result26 = UInt128.Min(97, 'a');
        }

        public void RotateLeft(UInt128 n)
        {
            const int c = 1;

            var result11 = UInt128.RotateLeft(n, 0);
            var result12 = UInt128.RotateLeft(0x01, 0);
            var result13 = UInt128.RotateLeft(0x01u, 0);
            var result14 = UInt128.RotateLeft(0x01L, 0);
            var result15 = UInt128.RotateLeft(0x01ul, 0);
            var result16 = UInt128.RotateLeft(1 + 1, 0);
            var result17 = UInt128.RotateLeft(1u + 1u, 0);
            var result18 = UInt128.RotateLeft(1L + 1L, 0);
            var result19 = UInt128.RotateLeft(1ul + 1ul, 0);
            var result1A = UInt128.RotateLeft(c, 0);
            var result1B = UInt128.RotateLeft('a', 0);

            UInt128 result21 = UInt128.RotateLeft(n, 0);
            UInt128 result22 = UInt128.RotateLeft(0x01, 0);
            UInt128 result23 = UInt128.RotateLeft(0x01u, 0);
            UInt128 result24 = UInt128.RotateLeft(0x01L, 0);
            UInt128 result25 = UInt128.RotateLeft(0x01ul, 0);
            UInt128 result26 = UInt128.RotateLeft(1 + 1, 0);
            UInt128 result27 = UInt128.RotateLeft(1u + 1u, 0);
            UInt128 result28 = UInt128.RotateLeft(1L + 1L, 0);
            UInt128 result29 = UInt128.RotateLeft(1ul + 1ul, 0);
            UInt128 result2A = UInt128.RotateLeft(c, 0);
            UInt128 result2B = UInt128.RotateLeft('a', 0);
        }

        public void RotateRight(UInt128 n)
        {
            const int c = 1;

            var result11 = UInt128.RotateRight(n, 0);
            var result12 = UInt128.RotateRight(0x01, 0);
            var result13 = UInt128.RotateRight(0x01u, 0);
            var result14 = UInt128.RotateRight(0x01L, 0);
            var result15 = UInt128.RotateRight(0x01ul, 0);
            var result16 = UInt128.RotateRight(1 + 1, 0);
            var result17 = UInt128.RotateRight(1u + 1u, 0);
            var result18 = UInt128.RotateRight(1L + 1L, 0);
            var result19 = UInt128.RotateRight(1ul + 1ul, 0);
            var result1A = UInt128.RotateRight(c, 0);
            var result1B = UInt128.RotateRight('a', 0);

            UInt128 result21 = UInt128.RotateRight(n, 0);
            UInt128 result22 = UInt128.RotateRight(0x01, 0);
            UInt128 result23 = UInt128.RotateRight(0x01u, 0);
            UInt128 result24 = UInt128.RotateRight(0x01L, 0);
            UInt128 result25 = UInt128.RotateRight(0x01ul, 0);
            UInt128 result26 = UInt128.RotateRight(1 + 1, 0);
            UInt128 result27 = UInt128.RotateRight(1u + 1u, 0);
            UInt128 result28 = UInt128.RotateRight(1L + 1L, 0);
            UInt128 result29 = UInt128.RotateRight(1ul + 1ul, 0);
            UInt128 result2A = UInt128.RotateRight(c, 0);
            UInt128 result2B = UInt128.RotateRight('a', 0);
        }
    }
}