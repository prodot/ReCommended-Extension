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

        public void DivRem(int left)
        {
            var result11 = int.DivRem(0, 10);

            (int, int) result21 = int.DivRem(0, 10);

            (int quotient, int remainder) result32 = int.DivRem(0, 10);

            var result71 = Math.DivRem(0, 10);
        }

        public void Equals(int number)
        {
            var result = number.Equals(null);
        }

        public void GetTypeCode(int number)
        {
            var result = number.GetTypeCode();
        }

        public void Max()
        {
            const int c = 10;

            var result11 = int.Max(10, 0x0A);
            var result12 = int.Max(10, c);
            var result13 = int.Max(97, 'a');

            int result21 = int.Max(10, 0x0A);
            int result22 = int.Max(10, c);
            int result23 = int.Max(97, 'a');

            var result31 = Math.Max(10, 10);
        }

        public void Min()
        {
            const int c = 10;

            var result11 = int.Min(10, 0x0A);
            var result12 = int.Min(10, c);
            var result13 = int.Min(97, 'a');

            int result21 = int.Min(10, 0x0A);
            int result22 = int.Min(10, c);
            int result23 = int.Min(97, 'a');

            var result31 = Math.Min(10, 10);
        }

        public void RotateLeft(int n)
        {
            const int c = 1;

            var result11 = int.RotateLeft(n, 0);
            var result12 = int.RotateLeft(0x01, 0);
            var result13 = int.RotateLeft(1 + 1, 0);
            var result14 = int.RotateLeft(c, 0);
            var result15 = int.RotateLeft('a', 0);

            int result21 = int.RotateLeft(n, 0);
            int result22 = int.RotateLeft(1, 0);
            int result23 = int.RotateLeft(1 + 1, 0);
            int result24 = int.RotateLeft(c, 0);
            int result25 = int.RotateLeft('a', 0);
        }

        public void RotateRight(int n)
        {
            const int c = 1;

            var result11 = int.RotateRight(n, 0);
            var result12 = int.RotateRight(0x01, 0);
            var result13 = int.RotateRight(1 + 1, 0);
            var result14 = int.RotateRight(c, 0);
            var result15 = int.RotateRight('a', 0);

            int result21 = int.RotateRight(n, 0);
            int result22 = int.RotateRight(1, 0);
            int result23 = int.RotateRight(1 + 1, 0);
            int result24 = int.RotateRight(c, 0);
            int result25 = int.RotateRight('a', 0);
        }
    }
}