using System;

namespace Test
{
    public class Int32s
    {
        public void Clamp(int number)
        {
            var result11 = int.Clamp(number, 1, 0x0001);
            var result12 = int.Clamp(number, 97, 'a');

            int result21 = int.Clamp(number, 1, 0x0001);
            int result22 = int.Clamp(number, 97, 'a');

            var result31 = int.Clamp(number, int.MinValue, int.MaxValue);
            var result32 = int.Clamp(1, int.MinValue, int.MaxValue);
            var result33 = int.Clamp('a', int.MinValue, int.MaxValue);

            int result41 = int.Clamp(number, int.MinValue, int.MaxValue);
            int result42 = int.Clamp(1, int.MinValue, int.MaxValue);
            int result43 = int.Clamp('a', int.MinValue, int.MaxValue);

            var result51 = Math.Clamp(number, 1, 1);
            var result52 = Math.Clamp(number, int.MinValue, int.MaxValue);
        }

        public void DivRem(short left)
        {
            const int c = 1;

            var result11 = int.DivRem(0, 10);

            (int, int) result21 = int.DivRem(0, 10);

            (int quotient, int remainder) result32 = int.DivRem(0, 10);

            var result31 = int.DivRem(left, 1);
            var result32 = int.DivRem(0x10, 1);
            var result32 = int.DivRem(c, 1);
            var result33 = int.DivRem('a', 1);

            (int, int) result41 = int.DivRem(left, 1);
            (int, int) result42 = int.DivRem(0x10, 1);
            (int, int) result42 = int.DivRem(c, 1);
            (int, int) result43 = int.DivRem('a', 1);

            (int quotient, int remainder) result51 = int.DivRem(left, 1);
            (int quotient, int remainder) result52 = int.DivRem(0x10, 1);
            (int quotient, int remainder) result52 = int.DivRem(c, 1);
            (int quotient, int remainder) result53 = int.DivRem('a', 1);

            var result61 = Math.DivRem(0, 10);
            var result62 = Math.DivRem(left, 1);
        }

        public void Equals(int number)
        {
            var result = number.Equals(null);
        }

        public void GetTypeCode(int number)
        {
            var result = number.GetTypeCode();
        }
    }
}