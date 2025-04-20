namespace ReCommendedExtension.Tests.Missing;

internal static class MissingMathMethods
{
    [Pure]
    public static byte Clamp(byte value, byte min, byte max) => MissingByteMethods.Clamp(value, min, max);

    [Pure]
    public static sbyte Clamp(sbyte value, sbyte min, sbyte max) => MissingSByteMethods.Clamp(value, min, max);

    [Pure]
    public static short Clamp(short value, short min, short max) => MissingInt16Methods.Clamp(value, min, max);

    [Pure]
    public static ushort Clamp(ushort value, ushort min, ushort max) => MissingUInt16Methods.Clamp(value, min, max);

    [Pure]
    public static int Clamp(int value, int min, int max) => MissingInt32Methods.Clamp(value, min, max);

    [Pure]
    public static uint Clamp(uint value, uint min, uint max) => MissingUInt32Methods.Clamp(value, min, max);

    [Pure]
    public static long Clamp(long value, long min, long max) => MissingInt64Methods.Clamp(value, min, max);

    [Pure]
    public static ulong Clamp(ulong value, ulong min, ulong max) => MissingUInt64Methods.Clamp(value, min, max);

    [Pure]
    public static decimal Clamp(decimal value, decimal min, decimal max) => MissingDecimalMethods.Clamp(value, min, max);

    [Pure]
    public static nint Clamp(nint value, nint min, nint max) => MissingIntPtrMethods.Clamp(value, min, max);

    [Pure]
    public static nuint Clamp(nuint value, nuint min, nuint max) => MissingUIntPtrMethods.Clamp(value, min, max);

    [Pure]
    public static (byte Quotient, byte Remainder) DivRem(byte left, [ValueRange(1, byte.MaxValue)] byte right)
        => MissingByteMethods.DivRem(left, right);

    [Pure]
    public static (sbyte Quotient, sbyte Remainder) DivRem(sbyte left, [ValueRange(sbyte.MinValue, -1)][ValueRange(1, sbyte.MaxValue)] sbyte right)
        => MissingSByteMethods.DivRem(left, right);

    [Pure]
    public static (short Quotient, short Remainder) DivRem(short left, [ValueRange(short.MinValue, -1)][ValueRange(1, short.MaxValue)] short right)
        => MissingInt16Methods.DivRem(left, right);

    [Pure]
    public static (ushort Quotient, ushort Remainder) DivRem(ushort left, [ValueRange(1, ushort.MaxValue)] ushort right)
        => MissingUInt16Methods.DivRem(left, right);

    [Pure]
    public static (int Quotient, int Remainder) DivRem(int left, [ValueRange(int.MinValue, -1)][ValueRange(1, int.MaxValue)] int right)
        => MissingInt32Methods.DivRem(left, right);

    [Pure]
    public static (uint Quotient, uint Remainder) DivRem(uint left, [ValueRange(1, uint.MaxValue)] uint right)
        => MissingUInt32Methods.DivRem(left, right);

    [Pure]
    public static (long Quotient, long Remainder) DivRem(long left, [ValueRange(long.MinValue, -1)][ValueRange(1, long.MaxValue)] long right)
        => MissingInt64Methods.DivRem(left, right);

    [Pure]
    public static (ulong Quotient, ulong Remainder) DivRem(ulong left, [ValueRange(1, ulong.MaxValue)] ulong right)
        => MissingUInt64Methods.DivRem(left, right);

    [Pure]
    public static (nint Quotient, nint Remainder) DivRem(nint left, nint right) => MissingIntPtrMethods.DivRem(left, right);

    [Pure]
    public static (nuint Quotient, nuint Remainder) DivRem(nuint left, nuint right) => MissingUIntPtrMethods.DivRem(left, right);

    [Pure]
    public static nint Max(nint x, nint y) => MissingIntPtrMethods.Max(x, y);

    [Pure]
    public static nuint Max(nuint x, nuint y) => MissingUIntPtrMethods.Max(x, y);

    [Pure]
    public static nint Min(nint x, nint y) => MissingIntPtrMethods.Min(x, y);

    [Pure]
    public static nuint Min(nuint x, nuint y) => MissingUIntPtrMethods.Min(x, y);
}