using System;

namespace Test
{
    public class Decimals
    {
        public void Clamp(decimal number)
        {
            const int c = 1;

            var result11 = decimal.Clamp(number, 1, 1uL);
            var result12 = decimal.Clamp(number, 1, 1u);
            var result13 = decimal.Clamp(number, 1, 1L);
            var result14 = decimal.Clamp(number, 1, c);
            var result15 = decimal.Clamp(number, 97, 'a');

            decimal result21 = decimal.Clamp(number, 1, 1uL);
            decimal result22 = decimal.Clamp(number, 1, 1u);
            decimal result23 = decimal.Clamp(number, 1, 1L);
            decimal result24 = decimal.Clamp(number, 1, c);
            decimal result25 = decimal.Clamp(number, 97, 'a');

            var result31 = decimal.Clamp(number, decimal.MinValue, decimal.MaxValue);
            var result32 = decimal.Clamp(1, decimal.MinValue, decimal.MaxValue);
            var result33 = decimal.Clamp(1uL, decimal.MinValue, decimal.MaxValue);
            var result34 = decimal.Clamp(1L, decimal.MinValue, decimal.MaxValue);
            var result35 = decimal.Clamp(1u, decimal.MinValue, decimal.MaxValue);
            var result36 = decimal.Clamp(c, decimal.MinValue, decimal.MaxValue);
            var result37 = decimal.Clamp('a', decimal.MinValue, decimal.MaxValue);

            decimal result41 = decimal.Clamp(number, decimal.MinValue, decimal.MaxValue);
            decimal result42 = decimal.Clamp(1, decimal.MinValue, decimal.MaxValue);
            decimal result43 = decimal.Clamp(1uL, decimal.MinValue, decimal.MaxValue);
            decimal result44 = decimal.Clamp(1L, decimal.MinValue, decimal.MaxValue);
            decimal result45 = decimal.Clamp(1u, decimal.MinValue, decimal.MaxValue);
            decimal result46 = decimal.Clamp(c, decimal.MinValue, decimal.MaxValue);
            decimal result47 = decimal.Clamp('a', decimal.MinValue, decimal.MaxValue);

            var result51 = Math.Clamp(number, 1m, 1m);
            var result52 = Math.Clamp(number, decimal.MinValue, decimal.MaxValue);
        }

        public void Equals(decimal number)
        {
            var result = number.Equals(null);
        }

        public void GetTypeCode(decimal number)
        {
            var result = number.GetTypeCode();
        }
    }
}