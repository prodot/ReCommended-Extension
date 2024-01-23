using JetBrains.ReSharper.Psi;

namespace ReCommendedExtension.ContextActions.CodeContracts.Internal;

internal abstract class CSharpNumericTypeInfo(bool isSigned, string? epsilonLiteral, string literalSuffix)
{
    [Pure]
    public static CSharpNumericTypeInfo? TryCreate(IType type)
    {
        if (type.IsInt())
        {
            return new CSharpNumericTypeInfo<int>(
                true,
                1,
                "",
                null,
                (x, y) => x <= y,
                value => value == 0,
                value => value + 1,
                value => value * 2,
                value => value,
                c => c.IntValue);
        }

        if (type.IsUint())
        {
            return new CSharpNumericTypeInfo<uint>(
                false,
                1u,
                "u",
                null,
                (x, y) => x <= y,
                value => value == 0u,
                value => value + 1u,
                value => value * 2u,
                value => value,
                c => c.UintValue);
        }

        if (type.IsLong())
        {
            return new CSharpNumericTypeInfo<long>(
                true,
                1L,
                "L",
                null,
                (x, y) => x <= y,
                value => value == 0L,
                value => value + 1L,
                value => value * 2L,
                value => value,
                c => c.LongValue);
        }

        if (type.IsUlong())
        {
            return new CSharpNumericTypeInfo<ulong>(
                false,
                1ul,
                "ul",
                null,
                (x, y) => x <= y,
                value => value == 0ul,
                value => value + 1ul,
                value => value * 2ul,
                value => value,
                c => c.UlongValue);
        }

        if (type.IsByte())
        {
            return new CSharpNumericTypeInfo<byte>(
                false,
                1,
                "",
                null,
                (x, y) => x <= y,
                value => value == 0,
                value => (byte)(value + 1),
                value => (byte)(value * 2),
                value => value,
                c => c.ByteValue);
        }

        if (type.IsSbyte())
        {
            return new CSharpNumericTypeInfo<sbyte>(
                true,
                1,
                "",
                null,
                (x, y) => x <= y,
                value => value == 0,
                value => (sbyte)(value + 1),
                value => (sbyte)(value * 2),
                value => value,
                c => c.SbyteValue);
        }

        if (type.IsShort())
        {
            return new CSharpNumericTypeInfo<short>(
                true,
                1,
                "",
                null,
                (x, y) => x <= y,
                value => value == 0,
                value => (short)(value + 1),
                value => (short)(value * 2),
                value => value,
                c => c.ShortValue);
        }

        if (type.IsUshort())
        {
            return new CSharpNumericTypeInfo<ushort>(
                false,
                1,
                "",
                null,
                (x, y) => x <= y,
                value => value == 0,
                value => (ushort)(value + 1),
                value => (ushort)(value * 2),
                value => value,
                c => c.UshortValue);
        }

        if (type.IsDecimal())
        {
            return new CSharpNumericTypeInfo<decimal>(
                true,
                1m,
                "m",
                null,
                (x, y) => x <= y,
                value => value == 0m,
                null,
                value => value * 2m,
                value => (double)value,
                c => c.DecimalValue);
        }

        if (type.IsDouble())
        {
            return new CSharpNumericTypeInfo<double>(
                true,
                1d,
                "d",
                $"double.{nameof(double.Epsilon)}",
                (x, y) => x <= y,
                value => Math.Abs(value - 0d) < double.Epsilon,
                null,
                value => value * 2d,
                value => value,
                c => c.DoubleValue);
        }

        if (type.IsFloat())
        {
            return new CSharpNumericTypeInfo<float>(
                true,
                1f,
                "f",
                $"float.{nameof(float.Epsilon)}",
                (x, y) => x <= y,
                value => Math.Abs(value - 0f) < float.Epsilon,
                null,
                value => value * 2f,
                value => value,
                c => c.FloatValue);
        }

        return null;
    }

    public bool IsSigned { get; } = isSigned;

    public string? EpsilonLiteral { get; } = epsilonLiteral;

    public string LiteralSuffix { get; } = literalSuffix;

    [Pure]
    public abstract EnumBetweenFirstAndLast.EnumContractInfo? TryCreateEnumContractInfoForEnumBetweenFirstAndLast(IField[] members);

    [Pure]
    public abstract EnumFlags.EnumContractInfo? TryCreateEnumFlags(IField[] members);
}