using System;

namespace Test
{
    public class ExpressionResults
    {
        public void ExpressionResult(byte byteValue, sbyte sbyteValue, short int16Value, ushort uint16Value, int int32Value, uint uint32Value, long int64Value, ulong uint64Value, nint intPtrValue, nuint uintPtrValue, decimal decimalValue)
        {
            (byte quotient1, byte remainder1) = Math.DivRem((byte)0, (byte)10);
            (sbyte quotient2, sbyte remainder2) = Math.DivRem((sbyte)0, (sbyte)10);
            (short quotient3, short remainder3) = Math.DivRem((short)0, (short)10);
            (ushort quotient4, ushort remainder4) = Math.DivRem((ushort)0, (ushort)10);
            (int quotient5, int remainder5) = Math.DivRem(0, 10);
            (uint quotient6, uint remainder6) = Math.DivRem(0u, 10u);
            (long quotient7, long remainder7) = Math.DivRem(0L, 10L);
            (ulong quotient8, ulong remainder8) = Math.DivRem(0ul, 10ul);
            (nint quotient9, nint remainder9) = Math.DivRem((nint)0, 10);
            (nuint quotientA, nuint remainderA) = Math.DivRem((nuint)0, 10u);

            var result11 = Math.DivRem((byte)0, (byte)10);
            var result12 = Math.DivRem((sbyte)0, (sbyte)10);
            var result13 = Math.DivRem((short)0, (short)10);
            var result14 = Math.DivRem((ushort)0, (ushort)10);
            var result15 = Math.DivRem(0, 10);
            var result16 = Math.DivRem(0u, 10u);
            var result17 = Math.DivRem(0L, 10L);
            var result18 = Math.DivRem(0ul, 10ul);
            var result19 = Math.DivRem((nint)0, 10);
            var result1A = Math.DivRem((nuint)0, 10);

            var result21 = Math.Clamp(byteValue, (byte)1, (byte)1);
            var result22 = Math.Clamp(sbyteValue, (sbyte)1, (sbyte)1);
            var result23 = Math.Clamp(int16Value, (short)1, (short)1);
            var result24 = Math.Clamp(uint16Value, (ushort)1, (ushort)1);
            var result25 = Math.Clamp(int32Value, 1, 1);
            var result26 = Math.Clamp(uint32Value, 1, 1);
            var result27 = Math.Clamp(int64Value, 1, 1);
            var result28 = Math.Clamp(uint64Value, 1, 1);
            var result29 = Math.Clamp(intPtrValue, 1, 1);
            var result2A = Math.Clamp(uintPtrValue, 1, 1);
            var result2B = Math.Clamp(decimalValue, 1, 1);

            var result31 = Math.Clamp(byteValue, byte.MinValue, byte.MaxValue);
            var result32 = Math.Clamp(sbyteValue, sbyte.MinValue, sbyte.MaxValue);
            var result33 = Math.Clamp(int16Value, short.MinValue, short.MaxValue);
            var result34 = Math.Clamp(uint16Value, ushort.MinValue, ushort.MaxValue);
            var result35 = Math.Clamp(int32Value, int.MinValue, int.MaxValue);
            var result36 = Math.Clamp(uint32Value, uint.MinValue, uint.MaxValue);
            var result37 = Math.Clamp(int64Value, long.MinValue, long.MaxValue);
            var result38 = Math.Clamp(uint64Value, ulong.MinValue, ulong.MaxValue);
            var result39 = Math.Clamp(decimalValue, decimal.MinValue, decimal.MaxValue);

            var result41 = Math.Max((byte)1, (byte)1);
            var result42 = Math.Max((sbyte)1, (sbyte)1);
            var result43 = Math.Max((short)1, (short)1);
            var result44 = Math.Max((ushort)1, (ushort)1);
            var result45 = Math.Max(1, 1);
            var result46 = Math.Max(1u, 1u);
            var result47 = Math.Max(1L, 1L);
            var result48 = Math.Max(1ul, 1ul);
            var result49 = Math.Max((nint)1, (nint)1);
            var result4A = Math.Max((nuint)1, (nuint)1);
            var result4B = Math.Max(1m, 1m);

            var result51 = Math.Min((byte)1, (byte)1);
            var result52 = Math.Min((sbyte)1, (sbyte)1);
            var result53 = Math.Min((short)1, (short)1);
            var result54 = Math.Min((ushort)1, (ushort)1);
            var result55 = Math.Min(1, 1);
            var result56 = Math.Min(1u, 1u);
            var result57 = Math.Min(1L, 1L);
            var result58 = Math.Min(1ul, 1ul);
            var result59 = Math.Min((nint)1, (nint)1);
            var result5A = Math.Min((nuint)1, (nuint)1);
            var result5B = Math.Min(1m, 1m);
        }

        public void NoDetection(byte byteValue, sbyte sbyteValue, short int16Value, ushort uint16Value, int int32Value, uint uint32Value, long int64Value, ulong uint64Value, nint intPtrValue, nuint uintPtrValue, decimal decimalValue)
        {
            Math.DivRem((byte)0, (byte)10);
            Math.DivRem((sbyte)0, (sbyte)10);
            Math.DivRem((short)0, (short)10);
            Math.DivRem((ushort)0, (ushort)10);
            Math.DivRem(0, 10);
            Math.DivRem(0u, 10u);
            Math.DivRem(0L, 10L);
            Math.DivRem(0ul, 10ul);
            Math.DivRem((nint)0, 10);
            Math.DivRem((nuint)0, 10u);

            Math.Clamp(byteValue, (byte)1, (byte)1);
            Math.Clamp(sbyteValue, (sbyte)1, (sbyte)1);
            Math.Clamp(int16Value, (short)1, (short)1);
            Math.Clamp(uint16Value, (ushort)1, (ushort)1);
            Math.Clamp(int32Value, 1, 1);
            Math.Clamp(uint32Value, 1, 1);
            Math.Clamp(int64Value, 1, 1);
            Math.Clamp(uint64Value, 1, 1);
            Math.Clamp(intPtrValue, 1, 1);
            Math.Clamp(uintPtrValue, 1, 1);
            Math.Clamp(decimalValue, 1, 1);

            Math.Clamp(byteValue, byte.MinValue, byte.MaxValue);
            Math.Clamp(sbyteValue, sbyte.MinValue, sbyte.MaxValue);
            Math.Clamp(int16Value, short.MinValue, short.MaxValue);
            Math.Clamp(uint16Value, ushort.MinValue, ushort.MaxValue);
            Math.Clamp(int32Value, int.MinValue, int.MaxValue);
            Math.Clamp(uint32Value, uint.MinValue, uint.MaxValue);
            Math.Clamp(int64Value, long.MinValue, long.MaxValue);
            Math.Clamp(uint64Value, ulong.MinValue, ulong.MaxValue);
            Math.Clamp(decimalValue, decimal.MinValue, decimal.MaxValue);

            Math.Max((byte)1, (byte)1);
            Math.Max((sbyte)1, (sbyte)1);
            Math.Max((short)1, (short)1);
            Math.Max((ushort)1, (ushort)1);
            Math.Max(1, 1);
            Math.Max(1u, 1u);
            Math.Max(1L, 1L);
            Math.Max(1ul, 1ul);
            Math.Max((nint)1, (nint)1);
            Math.Max((nuint)1, (nuint)1);
            Math.Max(1m, 1m);

            Math.Min((byte)1, (byte)1);
            Math.Min((sbyte)1, (sbyte)1);
            Math.Min((short)1, (short)1);
            Math.Min((ushort)1, (ushort)1);
            Math.Min(1, 1);
            Math.Min(1u, 1u);
            Math.Min(1L, 1L);
            Math.Min(1ul, 1ul);
            Math.Min((nint)1, (nint)1);
            Math.Min((nuint)1, (nuint)1);
            Math.Min(1m, 1m);
        }
    }
}