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
            const int c = 1;

            var result11 = uint.DivRem(0, 10);

            (uint, uint) result21 = uint.DivRem(0, 10);

            (uint quotient, uint remainder) result32 = uint.DivRem(0, 10);

            var result31 = uint.DivRem(left, 1);
            var result32 = uint.DivRem(0x10, 1);
            var result33 = uint.DivRem(0x10u, 1);
            var result34 = uint.DivRem(c, 1);
            var result35 = uint.DivRem('a', 1);

            (uint, uint) result41 = uint.DivRem(left, 1);
            (uint, uint) result42 = uint.DivRem(0x10, 1);
            (uint, uint) result43 = uint.DivRem(0x10u, 1);
            (uint, uint) result44 = uint.DivRem(c, 1);
            (uint, uint) result45 = uint.DivRem('a', 1);

            (uint quotient, uint remainder) result51 = uint.DivRem(left, 1);
            (uint quotient, uint remainder) result52 = uint.DivRem(0x10, 1);
            (uint quotient, uint remainder) result53 = uint.DivRem(0x10u, 1);
            (uint quotient, uint remainder) result54 = uint.DivRem(c, 1);
            (uint quotient, uint remainder) result55 = uint.DivRem('a', 1);

            var result61 = Math.DivRem(0u, 10u);
            var result62 = Math.DivRem(left, 1u);
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
    }
}