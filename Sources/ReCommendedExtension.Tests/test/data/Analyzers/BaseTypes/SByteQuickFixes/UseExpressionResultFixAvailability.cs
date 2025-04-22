using System;

namespace Test
{
    public class SBytes
    {
        public void Clamp(sbyte number)
        {
            var result11 = sbyte.Clamp(number, 1, 0x01);

            sbyte result21 = sbyte.Clamp(number, 1, 0x01);

            var result31 = sbyte.Clamp(number, sbyte.MinValue, sbyte.MaxValue);
            var result32 = sbyte.Clamp(1, sbyte.MinValue, sbyte.MaxValue);

            sbyte result41 = sbyte.Clamp(number, sbyte.MinValue, sbyte.MaxValue);
            sbyte result42 = sbyte.Clamp(1, sbyte.MinValue, sbyte.MaxValue);

            var result51 = Math.Clamp(number, (sbyte)1, (sbyte)1);
            var result52 = Math.Clamp(number, sbyte.MinValue, sbyte.MaxValue);
        }

        public void DivRem(sbyte left)
        {
            var result11 = sbyte.DivRem(0, 10);

            (sbyte, sbyte) result21 = sbyte.DivRem(0, 10);

            (sbyte quotient, sbyte remainder) result32 = sbyte.DivRem(0, 10);

            var result71 = Math.DivRem((sbyte)0, (sbyte)10);
        }

        public void Equals(sbyte number)
        {
            var result = number.Equals(null);
        }

        public void GetTypeCode(sbyte number)
        {
            var result = number.GetTypeCode();
        }

        public void Max()
        {
            var result11 = sbyte.Max(10, 0x0A);

            sbyte result2 = sbyte.Max(10, 0x0A);

            var result3 = Math.Max((sbyte)10, (sbyte)10);
        }

        public void Min()
        {
            var result11 = sbyte.Min(10, 0x0A);

            sbyte result2 = sbyte.Min(10, 0x0A);

            var result3 = Math.Min((sbyte)10, (sbyte)10);
        }

        public void RotateLeft(sbyte n)
        {
            const int c = 1;

            var result11 = sbyte.RotateLeft(n, 0);
            var result12 = sbyte.RotateLeft(1, 0);
            var result13 = sbyte.RotateLeft(1 + 1, 0);
            var result14 = sbyte.RotateLeft(c, 0);

            sbyte result21 = sbyte.RotateLeft(n, 0);
            sbyte result22 = sbyte.RotateLeft(1, 0);
            sbyte result23 = sbyte.RotateLeft(1 + 1, 0);
            sbyte result24 = sbyte.RotateLeft(c, 0);
        }

        public void RotateRight(sbyte n)
        {
            const int c = 1;

            var result11 = sbyte.RotateRight(n, 0);
            var result12 = sbyte.RotateRight(1, 0);
            var result13 = sbyte.RotateRight(1 + 1, 0);
            var result14 = sbyte.RotateRight(c, 0);

            sbyte result21 = sbyte.RotateRight(n, 0);
            sbyte result22 = sbyte.RotateRight(1, 0);
            sbyte result23 = sbyte.RotateRight(1 + 1, 0);
            sbyte result24 = sbyte.RotateRight(c, 0);
        }
    }
}