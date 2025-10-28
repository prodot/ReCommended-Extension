using System;

namespace Test
{
    public class ExpressionResults
    {
        public void ExpressionResult(byte byteValue)
        {
            var result11 = byte.DivRem(0, 10);
            var result12 = sbyte.DivRem(0, 10);
            var result13 = short.DivRem(0, 10);
            var result14 = ushort.DivRem(0, 10);
            var result15 = int.DivRem(0, 10);
            var result16 = uint.DivRem(0, 10);
            var result17 = long.DivRem(0, 10);
            var result18 = ulong.DivRem(0, 10);
            var result19 = Int128.DivRem(0, 10);
            var result1A = UInt128.DivRem(0, 10);
            var result1B = nint.DivRem(0, 10);
            var result1C = nuint.DivRem(0, 10);

            (byte, byte) result21 = byte.DivRem(0, 10);
            (sbyte, sbyte) result22 = sbyte.DivRem(0, 10);
            (short, short) result23 = short.DivRem(0, 10);
            (ushort, ushort) result24 = ushort.DivRem(0, 10);
            (int, int) result25 = int.DivRem(0, 10);
            (uint, uint) result26 = uint.DivRem(0, 10);
            (long, long) result27 = long.DivRem(0, 10);
            (ulong, ulong) result28 = ulong.DivRem(0, 10);
            (Int128, Int128) result29 = Int128.DivRem(0, 10);
            (UInt128, UInt128) result2A = UInt128.DivRem(0, 10);
            (nint, nint) result2B = nint.DivRem(0, 10);
            (nuint, nuint) result2C = nuint.DivRem(0, 10);

            var result31 = byte.RotateLeft(byteValue, 0);
            byte result32 = byte.RotateLeft(1, 0);
            var result33 = byte.RotateLeft(1, 0);
            var result34 = uint.RotateLeft(byteValue, 0);

            var result41 = int.Clamp(byteValue, 1, 0x01);
        }
    }
}