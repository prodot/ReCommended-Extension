using System;

namespace Test
{
    public class Bytes
    {
        public void Clamp(byte number)
        {
            var result11 = byte.Clamp(number, 1, 0x01);

            byte result21 = byte.Clamp(number, 1, 0x01);

            var result31 = byte.Clamp(number, 0, 255);
            var result32 = byte.Clamp(1, 0, 255);

            byte result41 = byte.Clamp(number, 0, 255);
            byte result42 = byte.Clamp(1, 0, 255);

            var result51 = Math.Clamp(number, (byte)1, (byte)1);
            var result52 = Math.Clamp(number, byte.MinValue, byte.MaxValue);
        }

        public void DivRem(byte left)
        {
            const int c = 1;

            var result11 = byte.DivRem(0, 10);

            (byte, byte) result21 = byte.DivRem(0, 10);

            (byte quotient, byte remainder) result32 = byte.DivRem(0, 10);

            var result41 = byte.DivRem(left, 1);
            var result42 = byte.DivRem(0x10, 1);
            var result43 = byte.DivRem(c, 1);

            (byte, byte) result51 = byte.DivRem(left, 1);
            (byte, byte) result52 = byte.DivRem(0x10, 1);
            (byte, byte) result53 = byte.DivRem(c, 1);

            (byte quotient, byte remainder) result61 = byte.DivRem(left, 1);
            (byte quotient, byte remainder) result62 = byte.DivRem(0x10, 1);
            (byte quotient, byte remainder) result63 = byte.DivRem(c, 1);

            var result71 = Math.DivRem((byte)0, (byte)10);
            var result72 = Math.DivRem(left, (byte)1);
        }

        public void Equals(byte number)
        {
            var result = number.Equals(null);
        }

        public void GetTypeCode(byte number)
        {
            var result = number.GetTypeCode();
        }

        public void Max()
        {
            var result1 = byte.Max(10, 0x0A);

            byte result2 = byte.Max(10, 0x0A);

            var result3 = Math.Max((byte)10, (byte)10);
        }

        public void Min()
        {
            var result1 = byte.Min(10, 0x0A);

            byte result2 = byte.Min(10, 0x0A);

            var result3 = Math.Min((byte)10, (byte)10);
        }

        public void RotateLeft(byte n)
        {
            var result = byte.RotateLeft(n, 0);
        }

        public void RotateRight(byte n)
        {
            var result = byte.RotateRight(n, 0);
        }
    }
}