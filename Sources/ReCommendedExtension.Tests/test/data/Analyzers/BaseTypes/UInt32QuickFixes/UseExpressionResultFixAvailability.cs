using System;

namespace Test
{
    public class UInt32s
    {
        public void Clamp(uint number)
        {
            const int c = 1;

            var result11 = uint.Clamp(number, 1, 0x0001);
            var result12 = uint.Clamp(number, 1, 0x0001u);
            var result13 = uint.Clamp(number, 1, c);
            var result14 = uint.Clamp(number, 97, 'a');

            uint result21 = uint.Clamp(number, 1, 0x0001);
            uint result22 = uint.Clamp(number, 1, 0x0001u);
            uint result23 = uint.Clamp(number, 1, c);
            uint result24 = uint.Clamp(number, 97, 'a');

            var result31 = uint.Clamp(number, uint.MinValue, uint.MaxValue);
            var result32 = uint.Clamp(1, uint.MinValue, uint.MaxValue);
            var result33 = uint.Clamp(1u, uint.MinValue, uint.MaxValue);
            var result34 = uint.Clamp(c, uint.MinValue, uint.MaxValue);
            var result35 = uint.Clamp('a', uint.MinValue, uint.MaxValue);

            uint result41 = uint.Clamp(number, uint.MinValue, uint.MaxValue);
            uint result42 = uint.Clamp(1, uint.MinValue, uint.MaxValue);
            uint result43 = uint.Clamp(1u, uint.MinValue, uint.MaxValue);
            uint result44 = uint.Clamp(c, uint.MinValue, uint.MaxValue);
            uint result45 = uint.Clamp('a', uint.MinValue, uint.MaxValue);

            var result51 = Math.Clamp(number, 1u, 1u);
            var result52 = Math.Clamp(number, uint.MinValue, uint.MaxValue);
        }

        public void DivRem(uint left)
        {
            var result11 = uint.DivRem(0, 10);

            (uint, uint) result21 = uint.DivRem(0, 10);

            (uint quotient, uint remainder) result32 = uint.DivRem(0, 10);

            var result71 = Math.DivRem(0u, 10u);
        }

        public void Equals(uint number)
        {
            var result = number.Equals(null);
        }

        public void GetTypeCode(uint number)
        {
            var result = number.GetTypeCode();
        }

        public void Max()
        {
            const int c = 10;

            var result11 = uint.Max(10, 0x0A);
            var result12 = uint.Max(10, 10u);
            var result13 = uint.Max(10, c);
            var result14 = uint.Max(97, 'a');

            uint result21 = uint.Max(10, 0x0A);
            uint result22 = uint.Max(10, 10u);
            uint result23 = uint.Max(10, c);
            uint result24 = uint.Max(97, 'a');

            var result31 = Math.Max(10u, 10u);
        }

        public void Min()
        {
            const int c = 10;

            var result11 = uint.Min(10, 0x0A);
            var result12 = uint.Min(10, 10u);
            var result13 = uint.Min(10, c);
            var result14 = uint.Min(97, 'a');

            uint result21 = uint.Min(10, 0x0A);
            uint result22 = uint.Min(10, 10u);
            uint result23 = uint.Min(10, c);
            uint result24 = uint.Min(97, 'a');

            var result31 = Math.Min(10u, 10u);
        }

        public void RotateLeft(uint n)
        {
            const int c = 1;

            var result11 = uint.RotateLeft(n, 0);
            var result12 = uint.RotateLeft(0x01, 0);
            var result13 = uint.RotateLeft(0x01u, 0);
            var result14 = uint.RotateLeft(1 + 1, 0);
            var result15 = uint.RotateLeft(1u + 1u, 0);
            var result16 = uint.RotateLeft(c, 0);
            var result17 = uint.RotateLeft('a', 0);

            uint result21 = uint.RotateLeft(n, 0);
            uint result22 = uint.RotateLeft(1, 0);
            uint result23 = uint.RotateLeft(1u, 0);
            uint result24 = uint.RotateLeft(1 + 1, 0);
            uint result25 = uint.RotateLeft(1u + 1u, 0);
            uint result26 = uint.RotateLeft(c, 0);
            uint result27 = uint.RotateLeft('a', 0);
        }

        public void RotateRight(uint n)
        {
            const int c = 1;

            var result11 = uint.RotateRight(n, 0);
            var result12 = uint.RotateRight(0x01, 0);
            var result13 = uint.RotateRight(0x01u, 0);
            var result14 = uint.RotateRight(1 + 1, 0);
            var result15 = uint.RotateRight(1u + 1u, 0);
            var result16 = uint.RotateRight(c, 0);
            var result17 = uint.RotateRight('a', 0);

            uint result21 = uint.RotateRight(n, 0);
            uint result22 = uint.RotateRight(1, 0);
            uint result23 = uint.RotateRight(1u, 0);
            uint result24 = uint.RotateRight(1 + 1, 0);
            uint result25 = uint.RotateRight(1u + 1u, 0);
            uint result26 = uint.RotateRight(c, 0);
            uint result27 = uint.RotateRight('a', 0);
        }
    }
}