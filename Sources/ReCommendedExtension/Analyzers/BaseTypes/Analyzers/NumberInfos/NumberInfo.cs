using JetBrains.ReSharper.Psi;

namespace ReCommendedExtension.Analyzers.BaseTypes.Analyzers.NumberInfos;

public abstract class NumberInfo
{
    internal static ByteInfo Byte { get; } = new();

    internal static SByteInfo SByte { get; } = new();

    internal static Int16Info Int16 { get; } = new();

    internal static UInt16Info UInt16 { get; } = new();

    internal static Int32Info Int32 { get; } = new();

    internal static UInt32Info UInt32 { get; } = new();

    internal static Int64Info Int64 { get; } = new();

    internal static UInt64Info UInt64 { get; } = new();

    internal static Int128Info Int128 { get; } = new();

    internal static UInt128Info UInt128 { get; } = new();

    internal static IntPtrInfo IntPtr { get; } = new();

    internal static UIntPtrInfo UIntPtr { get; } = new();

    internal static DecimalInfo Decimal { get; } = new();

    internal static DoubleInfo Double { get; } = new();

    internal static SingleInfo Single { get; } = new();

    internal static HalfInfo Half { get; } = new();

    [Pure]
    internal static NumberInfo? TryGet(IType type)
        => type switch
        {
            var t when t.IsByte() => Byte,
            var t when t.IsSbyte() => SByte,
            var t when t.IsShort() => Int16,
            var t when t.IsUshort() => UInt16,
            var t when t.IsInt() => Int32,
            var t when t.IsUint() => UInt32,
            var t when t.IsLong() => Int64,
            var t when t.IsUlong() => UInt64,
            var t when t.IsClrType(ClrTypeNames.Int128) => Int128,
            var t when t.IsClrType(ClrTypeNames.UInt128) => UInt128,
            var t when t.IsIntPtr() => IntPtr,
            var t when t.IsUIntPtr() => UIntPtr,
            var t when t.IsDecimal() => Decimal,
            var t when t.IsDouble() => Double,
            var t when t.IsFloat() => Single,
            var t when t.IsClrType(ClrTypeNames.Half) => Half,
            _ => null,
        };

    internal abstract int? MaxValueStringLength { get; }
}