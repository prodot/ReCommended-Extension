using System;

namespace Test
{
    public class UInt64s
    {
        public void Clamp(ulong number)
        {
            const int c = 1;

            var result11 = ulong.Clamp(number, 1, 1);
            var result12 = ulong.Clamp(number, 1, 1u);
            var result13 = ulong.Clamp(number, 1, 1L);
            var result14 = ulong.Clamp(number, 1, 1ul);
            var result15 = ulong.Clamp(number, 1, c);
            var result16 = ulong.Clamp(number, 97, 'a');

            ulong result21 = ulong.Clamp(number, 1, 1);
            ulong result22 = ulong.Clamp(number, 1, 1u);
            ulong result23 = ulong.Clamp(number, 1, 1L);
            ulong result24 = ulong.Clamp(number, 1, 1ul);
            ulong result25 = ulong.Clamp(number, 1, c);
            ulong result26 = ulong.Clamp(number, 97, 'a');

            var result31 = ulong.Clamp(number, ulong.MinValue, ulong.MaxValue);
            var result32 = ulong.Clamp(1, ulong.MinValue, ulong.MaxValue);
            var result33 = ulong.Clamp(1u, ulong.MinValue, ulong.MaxValue);
            var result34 = ulong.Clamp(1L, ulong.MinValue, ulong.MaxValue);
            var result35 = ulong.Clamp(1ul, ulong.MinValue, ulong.MaxValue);
            var result36 = ulong.Clamp(c, ulong.MinValue, ulong.MaxValue);
            var result37 = ulong.Clamp('a', ulong.MinValue, ulong.MaxValue);

            ulong result41 = ulong.Clamp(number, ulong.MinValue, ulong.MaxValue);
            ulong result42 = ulong.Clamp(1, ulong.MinValue, ulong.MaxValue);
            ulong result43 = ulong.Clamp(1u, ulong.MinValue, ulong.MaxValue);
            ulong result44 = ulong.Clamp(1L, ulong.MinValue, ulong.MaxValue);
            ulong result45 = ulong.Clamp(1ul, ulong.MinValue, ulong.MaxValue);
            ulong result46 = ulong.Clamp(c, ulong.MinValue, ulong.MaxValue);
            ulong result47 = ulong.Clamp('a', ulong.MinValue, ulong.MaxValue);

            var result51 = Math.Clamp(number, 1ul, 1ul);
            var result52 = Math.Clamp(number, ulong.MinValue, ulong.MaxValue);
        }

        public void DivRem(ulong left)
        {
            var result11 = ulong.DivRem(0, 10);

            (ulong, ulong) result21 = ulong.DivRem(0, 10);

            (ulong quotient, ulong remainder) result32 = ulong.DivRem(0, 10);

            var result71 = Math.DivRem(0ul, 10ul);
        }

        public void Equals(ulong number)
        {
            var result = number.Equals(null);
        }

        public void GetTypeCode(ulong number)
        {
            var result = number.GetTypeCode();
        }

        public void Max()
        {
            const int c = 10;

            var result11 = ulong.Max(10, 0x0A);
            var result12 = ulong.Max(10, 10u);
            var result13 = ulong.Max(10, 10L);
            var result14 = ulong.Max(10, 10ul);
            var result15 = ulong.Max(10, c);
            var result16 = ulong.Max(97, 'a');

            ulong result21 = ulong.Max(10, 0x0A);
            ulong result22 = ulong.Max(10, 10u);
            ulong result23 = ulong.Max(10, 10L);
            ulong result24 = ulong.Max(10, 10ul);
            ulong result25 = ulong.Max(10, c);
            ulong result26 = ulong.Max(97, 'a');

            var result31 = Math.Max(10ul, 10ul);
        }

        public void Min()
        {
            const int c = 10;

            var result11 = ulong.Min(10, 0x0A);
            var result12 = ulong.Min(10, 10u);
            var result13 = ulong.Min(10, 10L);
            var result14 = ulong.Min(10, 10ul);
            var result15 = ulong.Min(10, c);
            var result16 = ulong.Min(97, 'a');

            ulong result21 = ulong.Min(10, 0x0A);
            ulong result22 = ulong.Min(10, 10u);
            ulong result23 = ulong.Min(10, 10L);
            ulong result24 = ulong.Min(10, 10ul);
            ulong result25 = ulong.Min(10, c);
            ulong result26 = ulong.Min(97, 'a');

            var result31 = Math.Min(10ul, 10ul);
        }

        public void RotateLeft(ulong n)
        {
            const int c = 1;

            var result11 = ulong.RotateLeft(n, 0);
            var result12 = ulong.RotateLeft(0x01, 0);
            var result13 = ulong.RotateLeft(0x01u, 0);
            var result14 = ulong.RotateLeft(0x01L, 0);
            var result15 = ulong.RotateLeft(0x01ul, 0);
            var result16 = ulong.RotateLeft(1 + 1, 0);
            var result17 = ulong.RotateLeft(1u + 1u, 0);
            var result18 = ulong.RotateLeft(1L + 1L, 0);
            var result19 = ulong.RotateLeft(1ul + 1ul, 0);
            var result1A = ulong.RotateLeft(c, 0);
            var result1B = ulong.RotateLeft('a', 0);

            ulong result21 = ulong.RotateLeft(n, 0);
            ulong result22 = ulong.RotateLeft(0x01, 0);
            ulong result23 = ulong.RotateLeft(0x01u, 0);
            ulong result24 = ulong.RotateLeft(0x01L, 0);
            ulong result25 = ulong.RotateLeft(0x01ul, 0);
            ulong result26 = ulong.RotateLeft(1 + 1, 0);
            ulong result27 = ulong.RotateLeft(1u + 1u, 0);
            ulong result28 = ulong.RotateLeft(1L + 1L, 0);
            ulong result29 = ulong.RotateLeft(1ul + 1ul, 0);
            ulong result2A = ulong.RotateLeft(c, 0);
            ulong result2B = ulong.RotateLeft('a', 0);
        }

        public void RotateRight(ulong n)
        {
            const int c = 1;

            var result11 = ulong.RotateRight(n, 0);
            var result12 = ulong.RotateRight(0x01, 0);
            var result13 = ulong.RotateRight(0x01u, 0);
            var result14 = ulong.RotateRight(0x01L, 0);
            var result15 = ulong.RotateRight(0x01ul, 0);
            var result16 = ulong.RotateRight(1 + 1, 0);
            var result17 = ulong.RotateRight(1u + 1u, 0);
            var result18 = ulong.RotateRight(1L + 1L, 0);
            var result19 = ulong.RotateRight(1ul + 1ul, 0);
            var result1A = ulong.RotateRight(c, 0);
            var result1B = ulong.RotateRight('a', 0);

            ulong result21 = ulong.RotateRight(n, 0);
            ulong result22 = ulong.RotateRight(0x01, 0);
            ulong result23 = ulong.RotateRight(0x01u, 0);
            ulong result24 = ulong.RotateRight(0x01L, 0);
            ulong result25 = ulong.RotateRight(0x01ul, 0);
            ulong result26 = ulong.RotateRight(1 + 1, 0);
            ulong result27 = ulong.RotateRight(1u + 1u, 0);
            ulong result28 = ulong.RotateRight(1L + 1L, 0);
            ulong result29 = ulong.RotateRight(1ul + 1ul, 0);
            ulong result2A = ulong.RotateRight(c, 0);
            ulong result2B = ulong.RotateRight('a', 0);
        }
    }
}