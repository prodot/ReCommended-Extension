using System;

namespace Test
{
    public class Int16s
    {
        public void Clamp(short number)
        {
            var result11 = short.Clamp(number, 1, 0x01);

            short result21 = short.Clamp(number, 1, 0x01);

            var result31 = short.Clamp(number, short.MinValue, short.MaxValue);
            var result32 = short.Clamp(1, short.MinValue, short.MaxValue);

            short result41 = short.Clamp(number, short.MinValue, short.MaxValue);
            short result42 = short.Clamp(1, short.MinValue, short.MaxValue);

            var result51 = Math.Clamp(number, (short)1, (short)1);
            var result52 = Math.Clamp(number, short.MinValue, short.MaxValue);
        }

        public void DivRem(short left)
        {
            var result11 = short.DivRem(0, 10);

            (short, short) result21 = short.DivRem(0, 10);

            (short quotient, short remainder) result32 = short.DivRem(0, 10);

            var result71 = Math.DivRem((short)0, (short)10);
        }

        public void Equals(short number)
        {
            var result = number.Equals(null);
        }

        public void GetTypeCode(short number)
        {
            var result = number.GetTypeCode();
        }

        public void Max()
        {
            var result1 = short.Max(10, 0x0A);

            short result2 = short.Max(10, 0x0A);

            var result3 = Math.Max((short)10, (short)10);
        }

        public void MaxMagnitude()
        {
            var result1 = short.MaxMagnitude(10, 0x0A);

            short result2 = short.MaxMagnitude(10, 0x0A);
        }

        public void Min()
        {
            var result1 = short.Min(10, 0x0A);

            short result2 = short.Min(10, 0x0A);

            var result3 = Math.Min((short)10, (short)10);
        }

        public void MinMagnitude()
        {
            var result1 = short.MinMagnitude(10, 0x0A);

            short result2 = short.MinMagnitude(10, 0x0A);
        }

        public void RotateLeft(short n)
        {
            const int c = 1;

            var result11 = short.RotateLeft(n, 0);
            var result12 = short.RotateLeft(0x01, 0);
            var result13 = short.RotateLeft(1 + 1, 0);
            var result14 = short.RotateLeft(c, 0);

            short result21 = short.RotateLeft(n, 0);
            short result22 = short.RotateLeft(1, 0);
            short result23 = short.RotateLeft(1 + 1, 0);
            short result24 = short.RotateLeft(c, 0);
        }

        public void RotateRight(short n)
        {
            const int c = 1;

            var result11 = short.RotateRight(n, 0);
            var result12 = short.RotateRight(0x01, 0);
            var result13 = short.RotateRight(1 + 1, 0);
            var result14 = short.RotateRight(c, 0);

            short result21 = short.RotateRight(n, 0);
            short result22 = short.RotateRight(1, 0);
            short result23 = short.RotateRight(1 + 1, 0);
            short result24 = short.RotateRight(c, 0);
        }
    }
}