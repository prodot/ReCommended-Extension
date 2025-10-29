using JetBrains.Metadata.Reader.API;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.CSharp;
using JetBrains.ReSharper.Psi.CSharp.Tree;
using ReCommendedExtension.Extensions;

namespace ReCommendedExtension.Analyzers.BaseTypes.NumberInfos;

public abstract record NumberInfo
{
    [Pure]
    static byte? TryGetByteConstant(ICSharpExpression? expression, out bool implicitlyConverted)
    {
        if (expression is IConstantValueOwner constantValueOwner)
        {
            switch (constantValueOwner.ConstantValue)
            {
                case { Kind: ConstantValueKind.Byte, ByteValue: var value }:
                    implicitlyConverted = false;
                    return value;

                case { Kind: ConstantValueKind.Int, IntValue: >= byte.MinValue and <= byte.MaxValue and var value }:
                    implicitlyConverted = true;
                    return unchecked((byte)value);
            }
        }

        implicitlyConverted = false;
        return null;
    }

    [Pure]
    static sbyte? TryGetSByteConstant(ICSharpExpression? expression, out bool implicitlyConverted)
    {
        if (expression is IConstantValueOwner constantValueOwner)
        {
            switch (constantValueOwner.ConstantValue)
            {
                case { Kind: ConstantValueKind.Sbyte, SbyteValue: var value }:
                    implicitlyConverted = false;
                    return value;

                case { Kind: ConstantValueKind.Int, IntValue: >= sbyte.MinValue and <= sbyte.MaxValue and var value }:
                    implicitlyConverted = true;
                    return unchecked((sbyte)value);
            }
        }

        implicitlyConverted = false;
        return null;
    }

    [Pure]
    static short? TryGetInt16Constant(ICSharpExpression? expression, out bool implicitlyConverted)
    {
        if (expression is IConstantValueOwner constantValueOwner)
        {
            switch (constantValueOwner.ConstantValue)
            {
                case { Kind: ConstantValueKind.Short, ShortValue: var value }:
                    implicitlyConverted = false;
                    return value;

                case { Kind: ConstantValueKind.Byte, ByteValue: var value }:
                    implicitlyConverted = true;
                    return value;

                case { Kind: ConstantValueKind.Sbyte, SbyteValue: var value }:
                    implicitlyConverted = true;
                    return value;

                case { Kind: ConstantValueKind.Int, IntValue: >= short.MinValue and <= short.MaxValue and var value }:
                    implicitlyConverted = true;
                    return unchecked((short)value);
            }
        }

        implicitlyConverted = false;
        return null;
    }

    [Pure]
    static ushort? TryGetUInt16Constant(ICSharpExpression? expression, out bool implicitlyConverted)
    {
        if (expression is IConstantValueOwner constantValueOwner)
        {
            switch (constantValueOwner.ConstantValue)
            {
                case { Kind: ConstantValueKind.Ushort, UshortValue: var value }:
                    implicitlyConverted = false;
                    return value;

                case { Kind: ConstantValueKind.Byte, ByteValue: var value }:
                    implicitlyConverted = true;
                    return value;

                case { Kind: ConstantValueKind.Int, IntValue: >= ushort.MinValue and <= ushort.MaxValue and var value }:
                    implicitlyConverted = true;
                    return unchecked((ushort)value);

                case { Kind: ConstantValueKind.Char, CharValue: var value }:
                    implicitlyConverted = true;
                    return value;
            }
        }

        implicitlyConverted = false;
        return null;
    }

    [Pure]
    static int? TryGetInt32Constant(ICSharpExpression? expression, out bool implicitlyConverted)
    {
        if (expression is IConstantValueOwner constantValueOwner)
        {
            switch (constantValueOwner.ConstantValue)
            {
                case { Kind: ConstantValueKind.Int, IntValue: var value }:
                    implicitlyConverted = false;
                    return value;

                case { Kind: ConstantValueKind.Byte, ByteValue: var value }:
                    implicitlyConverted = true;
                    return value;

                case { Kind: ConstantValueKind.Sbyte, SbyteValue: var value }:
                    implicitlyConverted = true;
                    return value;

                case { Kind: ConstantValueKind.Short, ShortValue: var value }:
                    implicitlyConverted = true;
                    return value;

                case { Kind: ConstantValueKind.Ushort, UshortValue: var value }:
                    implicitlyConverted = true;
                    return value;

                case { Kind: ConstantValueKind.Char, CharValue: var value }:
                    implicitlyConverted = true;
                    return value;
            }
        }

        implicitlyConverted = false;
        return null;
    }

    [Pure]
    static uint? TryGetUInt32Constant(ICSharpExpression? expression, out bool implicitlyConverted)
    {
        if (expression is IConstantValueOwner constantValueOwner)
        {
            switch (constantValueOwner.ConstantValue)
            {
                case { Kind: ConstantValueKind.Uint, UintValue: var value }:
                    implicitlyConverted = false;
                    return value;

                case { Kind: ConstantValueKind.Int, IntValue: >= 0 and var value }:
                    implicitlyConverted = true;
                    return unchecked((uint)value);

                case { Kind: ConstantValueKind.Byte, ByteValue: var value }:
                    implicitlyConverted = true;
                    return value;

                case { Kind: ConstantValueKind.Ushort, UshortValue: var value }:
                    implicitlyConverted = true;
                    return value;

                case { Kind: ConstantValueKind.Char, CharValue: var value }:
                    implicitlyConverted = true;
                    return value;
            }
        }

        implicitlyConverted = false;
        return null;
    }

    [Pure]
    static long? TryGetInt64Constant(ICSharpExpression? expression, out bool implicitlyConverted)
    {
        if (expression is IConstantValueOwner constantValueOwner)
        {
            switch (constantValueOwner.ConstantValue)
            {
                case { Kind: ConstantValueKind.Long, LongValue: var value }:
                    implicitlyConverted = false;
                    return value;

                case { Kind: ConstantValueKind.Int, IntValue: var value }:
                    implicitlyConverted = true;
                    return value;

                case { Kind: ConstantValueKind.Uint, UintValue: var value }:
                    implicitlyConverted = true;
                    return value;

                case { Kind: ConstantValueKind.Byte, ByteValue: var value }:
                    implicitlyConverted = true;
                    return value;

                case { Kind: ConstantValueKind.Sbyte, SbyteValue: var value }:
                    implicitlyConverted = true;
                    return value;

                case { Kind: ConstantValueKind.Short, ShortValue: var value }:
                    implicitlyConverted = true;
                    return value;

                case { Kind: ConstantValueKind.Ushort, UshortValue: var value }:
                    implicitlyConverted = true;
                    return value;

                case { Kind: ConstantValueKind.Nint, IntValue: var value }:
                    implicitlyConverted = true;
                    return value;

                case { Kind: ConstantValueKind.Char, CharValue: var value }:
                    implicitlyConverted = true;
                    return value;
            }
        }

        implicitlyConverted = false;
        return null;
    }

    [Pure]
    static ulong? TryGetUInt64Constant(ICSharpExpression? expression, out bool implicitlyConverted)
    {
        if (expression is IConstantValueOwner constantValueOwner)
        {
            switch (constantValueOwner.ConstantValue)
            {
                case { Kind: ConstantValueKind.Ulong, UlongValue: var value }:
                    implicitlyConverted = false;
                    return value;

                case { Kind: ConstantValueKind.Long, LongValue: >= 0 and var value }:
                    implicitlyConverted = true;
                    return unchecked((ulong)value);

                case { Kind: ConstantValueKind.Int, IntValue: >= 0 and var value }:
                    implicitlyConverted = true;
                    return unchecked((ulong)value);

                case { Kind: ConstantValueKind.Uint, UintValue: var value }:
                    implicitlyConverted = true;
                    return value;

                case { Kind: ConstantValueKind.Byte, ByteValue: var value }:
                    implicitlyConverted = true;
                    return value;

                case { Kind: ConstantValueKind.Ushort, UshortValue: var value }:
                    implicitlyConverted = true;
                    return value;

                case { Kind: ConstantValueKind.Nuint, UintValue: var value }:
                    implicitlyConverted = true;
                    return value;

                case { Kind: ConstantValueKind.Char, CharValue: var value }:
                    implicitlyConverted = true;
                    return value;
            }
        }

        implicitlyConverted = false;
        return null;
    }

    [Pure]
    static Int128? TryGetInt128Constant(ICSharpExpression? expression, out bool implicitlyConverted)
    {
        if (expression is IConstantValueOwner constantValueOwner)
        {
            switch (constantValueOwner.ConstantValue)
            {
                case { Kind: ConstantValueKind.Long, LongValue: var value }:
                    implicitlyConverted = true;
                    return value;

                case { Kind: ConstantValueKind.Ulong, UlongValue: var value }:
                    implicitlyConverted = true;
                    return value;

                case { Kind: ConstantValueKind.Int, IntValue: var value }:
                    implicitlyConverted = true;
                    return value;

                case { Kind: ConstantValueKind.Uint, UintValue: var value }:
                    implicitlyConverted = true;
                    return value;

                case { Kind: ConstantValueKind.Byte, ByteValue: var value }:
                    implicitlyConverted = true;
                    return value;

                case { Kind: ConstantValueKind.Sbyte, SbyteValue: var value }:
                    implicitlyConverted = true;
                    return value;

                case { Kind: ConstantValueKind.Short, ShortValue: var value }:
                    implicitlyConverted = true;
                    return value;

                case { Kind: ConstantValueKind.Ushort, UshortValue: var value }:
                    implicitlyConverted = true;
                    return value;

                case { Kind: ConstantValueKind.Nint, IntValue: var value }:
                    implicitlyConverted = true;
                    return value;

                case { Kind: ConstantValueKind.Nuint, UintValue: var value }:
                    implicitlyConverted = true;
                    return value;

                case { Kind: ConstantValueKind.Char, CharValue: var value }:
                    implicitlyConverted = true;
                    return value;
            }
        }

        implicitlyConverted = false;
        return null;
    }

    [Pure]
    static UInt128? TryGetUInt128Constant(ICSharpExpression? expression, out bool implicitlyConverted)
    {
        if (expression is IConstantValueOwner constantValueOwner)
        {
            switch (constantValueOwner.ConstantValue)
            {
                case { Kind: ConstantValueKind.Long, LongValue: >= 0 and var value }:
                    implicitlyConverted = true;
                    return value;

                case { Kind: ConstantValueKind.Ulong, UlongValue: var value }:
                    implicitlyConverted = true;
                    return value;

                case { Kind: ConstantValueKind.Int, IntValue: >= 0 and var value }:
                    implicitlyConverted = true;
                    return value;

                case { Kind: ConstantValueKind.Uint, UintValue: var value }:
                    implicitlyConverted = true;
                    return value;

                case { Kind: ConstantValueKind.Byte, ByteValue: var value }:
                    implicitlyConverted = true;
                    return value;

                case { Kind: ConstantValueKind.Ushort, UshortValue: var value }:
                    implicitlyConverted = true;
                    return value;

                case { Kind: ConstantValueKind.Nuint, UintValue: var value }:
                    implicitlyConverted = true;
                    return value;

                case { Kind: ConstantValueKind.Char, CharValue: var value }:
                    implicitlyConverted = true;
                    return value;
            }
        }

        implicitlyConverted = false;
        return null;
    }

    [Pure]
    static nint? TryGetIntPtrConstant(ICSharpExpression? expression, out bool implicitlyConverted)
    {
        if (expression is IConstantValueOwner constantValueOwner)
        {
            switch (constantValueOwner.ConstantValue)
            {
                case { Kind: ConstantValueKind.Nint, IntValue: var value }:
                    implicitlyConverted = false;
                    return value;

                case { Kind: ConstantValueKind.Int, IntValue: var value }:
                    implicitlyConverted = true;
                    return value;

                case { Kind: ConstantValueKind.Byte, ByteValue: var value }:
                    implicitlyConverted = true;
                    return value;

                case { Kind: ConstantValueKind.Sbyte, SbyteValue: var value }:
                    implicitlyConverted = true;
                    return value;

                case { Kind: ConstantValueKind.Short, ShortValue: var value }:
                    implicitlyConverted = true;
                    return value;

                case { Kind: ConstantValueKind.Ushort, UshortValue: var value }:
                    implicitlyConverted = true;
                    return value;

                case { Kind: ConstantValueKind.Char, CharValue: var value }:
                    implicitlyConverted = true;
                    return value;
            }
        }

        implicitlyConverted = false;
        return null;
    }

    [Pure]
    static nuint? TryGetUIntPtrConstant(ICSharpExpression? expression, out bool implicitlyConverted)
    {
        if (expression is IConstantValueOwner constantValueOwner)
        {
            switch (constantValueOwner.ConstantValue)
            {
                case { Kind: ConstantValueKind.Nuint, UintValue: var value }:
                    implicitlyConverted = false;
                    return value;

                case { Kind: ConstantValueKind.Int, IntValue: >= 0 and var value }:
                    implicitlyConverted = true;
                    return (nuint)value;

                case { Kind: ConstantValueKind.Uint, UintValue: var value }:
                    implicitlyConverted = true;
                    return value;

                case { Kind: ConstantValueKind.Byte, ByteValue: var value }:
                    implicitlyConverted = true;
                    return value;

                case { Kind: ConstantValueKind.Ushort, UshortValue: var value }:
                    implicitlyConverted = true;
                    return value;

                case { Kind: ConstantValueKind.Char, CharValue: var value }:
                    implicitlyConverted = true;
                    return value;
            }
        }

        implicitlyConverted = false;
        return null;
    }

    [Pure]
    static decimal? TryGetDecimalConstant(ICSharpExpression? expression, out bool implicitlyConverted)
    {
        if (expression is IConstantValueOwner constantValueOwner)
        {
            switch (constantValueOwner.ConstantValue)
            {
                case { Kind: ConstantValueKind.Decimal, DecimalValue: var value }:
                    implicitlyConverted = false;
                    return value;

                case { Kind: ConstantValueKind.Ulong, UlongValue: var value }:
                    implicitlyConverted = true;
                    return value;

                case { Kind: ConstantValueKind.Long, LongValue: var value }:
                    implicitlyConverted = true;
                    return value;

                case { Kind: ConstantValueKind.Int, IntValue: var value }:
                    implicitlyConverted = true;
                    return value;

                case { Kind: ConstantValueKind.Uint, UintValue: var value }:
                    implicitlyConverted = true;
                    return value;

                case { Kind: ConstantValueKind.Byte, ByteValue: var value }:
                    implicitlyConverted = true;
                    return value;

                case { Kind: ConstantValueKind.Sbyte, SbyteValue: var value }:
                    implicitlyConverted = true;
                    return value;

                case { Kind: ConstantValueKind.Short, ShortValue: var value }:
                    implicitlyConverted = true;
                    return value;

                case { Kind: ConstantValueKind.Ushort, UshortValue: var value }:
                    implicitlyConverted = true;
                    return value;

                case { Kind: ConstantValueKind.Nint, IntValue: var value }:
                    implicitlyConverted = true;
                    return value;

                case { Kind: ConstantValueKind.Nuint, UintValue: var value }:
                    implicitlyConverted = true;
                    return value;

                case { Kind: ConstantValueKind.Char, CharValue: var value }:
                    implicitlyConverted = true;
                    return value;
            }
        }

        implicitlyConverted = false;
        return null;
    }

    [Pure]
    static double? TryGetDoubleConstant(ICSharpExpression? expression, out bool implicitlyConverted)
    {
        if (expression is IConstantValueOwner constantValueOwner)
        {
            switch (constantValueOwner.ConstantValue)
            {
                case { Kind: ConstantValueKind.Double, DoubleValue: var value }:
                    implicitlyConverted = false;
                    return value;

                case { Kind: ConstantValueKind.Float, FloatValue: var value }:
                    implicitlyConverted = true;
                    return value;

                case { Kind: ConstantValueKind.Ulong, UlongValue: var value }:
                    implicitlyConverted = true;
                    return value;

                case { Kind: ConstantValueKind.Long, LongValue: var value }:
                    implicitlyConverted = true;
                    return value;

                case { Kind: ConstantValueKind.Int, IntValue: var value }:
                    implicitlyConverted = true;
                    return value;

                case { Kind: ConstantValueKind.Uint, UintValue: var value }:
                    implicitlyConverted = true;
                    return value;

                case { Kind: ConstantValueKind.Byte, ByteValue: var value }:
                    implicitlyConverted = true;
                    return value;

                case { Kind: ConstantValueKind.Sbyte, SbyteValue: var value }:
                    implicitlyConverted = true;
                    return value;

                case { Kind: ConstantValueKind.Short, ShortValue: var value }:
                    implicitlyConverted = true;
                    return value;

                case { Kind: ConstantValueKind.Ushort, UshortValue: var value }:
                    implicitlyConverted = true;
                    return value;

                case { Kind: ConstantValueKind.Nint, IntValue: var value }:
                    implicitlyConverted = true;
                    return value;

                case { Kind: ConstantValueKind.Nuint, UintValue: var value }:
                    implicitlyConverted = true;
                    return value;

                case { Kind: ConstantValueKind.Char, CharValue: var value }:
                    implicitlyConverted = true;
                    return value;
            }
        }

        implicitlyConverted = false;
        return null;
    }

    [Pure]
    static float? TryGetSingleConstant(ICSharpExpression? expression, out bool implicitlyConverted)
    {
        if (expression is IConstantValueOwner constantValueOwner)
        {
            switch (constantValueOwner.ConstantValue)
            {
                case { Kind: ConstantValueKind.Float, FloatValue: var value }:
                    implicitlyConverted = false;
                    return value;

                case { Kind: ConstantValueKind.Ulong, UlongValue: var value }:
                    implicitlyConverted = true;
                    return value;

                case { Kind: ConstantValueKind.Long, LongValue: var value }:
                    implicitlyConverted = true;
                    return value;

                case { Kind: ConstantValueKind.Int, IntValue: var value }:
                    implicitlyConverted = true;
                    return value;

                case { Kind: ConstantValueKind.Uint, UintValue: var value }:
                    implicitlyConverted = true;
                    return value;

                case { Kind: ConstantValueKind.Byte, ByteValue: var value }:
                    implicitlyConverted = true;
                    return value;

                case { Kind: ConstantValueKind.Sbyte, SbyteValue: var value }:
                    implicitlyConverted = true;
                    return value;

                case { Kind: ConstantValueKind.Short, ShortValue: var value }:
                    implicitlyConverted = true;
                    return value;

                case { Kind: ConstantValueKind.Ushort, UshortValue: var value }:
                    implicitlyConverted = true;
                    return value;

                case { Kind: ConstantValueKind.Nint, IntValue: var value }:
                    implicitlyConverted = true;
                    return value;

                case { Kind: ConstantValueKind.Nuint, UintValue: var value }:
                    implicitlyConverted = true;
                    return value;

                case { Kind: ConstantValueKind.Char, CharValue: var value }:
                    implicitlyConverted = true;
                    return value;
            }
        }

        implicitlyConverted = false;
        return null;
    }

    [Pure]
    static Half? TryGetHalfConstant(ICSharpExpression? expression, out bool implicitlyConverted)
    {
        if (expression is IConstantValueOwner constantValueOwner)
        {
            switch (constantValueOwner.ConstantValue)
            {
                case { Kind: ConstantValueKind.Byte, ByteValue: var value }:
                    implicitlyConverted = true;
                    return value;

                case { Kind: ConstantValueKind.Sbyte, SbyteValue: var value }:
                    implicitlyConverted = true;
                    return value;
            }
        }

        implicitlyConverted = false;
        return null;
    }

    [Pure]
    static string CastConstantToByte(ICSharpExpression constant, bool implicitlyConverted)
    {
        if (implicitlyConverted)
        {
            return constant.Cast("byte").GetText();
        }

        return constant.GetText();
    }

    [Pure]
    static string CastConstantToSByte(ICSharpExpression constant, bool implicitlyConverted)
    {
        if (implicitlyConverted)
        {
            return constant.Cast("sbyte").GetText();
        }

        return constant.GetText();
    }

    [Pure]
    static string CastConstantToInt16(ICSharpExpression constant, bool implicitlyConverted)
    {
        if (implicitlyConverted)
        {
            return constant.Cast("short").GetText();
        }

        return constant.GetText();
    }

    [Pure]
    static string CastConstantToUInt16(ICSharpExpression constant, bool implicitlyConverted)
    {
        if (implicitlyConverted)
        {
            return constant.Cast("ushort").GetText();
        }

        return constant.GetText();
    }

    [Pure]
    static string CastConstantToInt32(ICSharpExpression constant, bool implicitlyConverted)
    {
        if (implicitlyConverted)
        {
            if (constant is ICSharpLiteralExpression)
            {
                if (constant.Type().IsChar())
                {
                    return constant.Cast("int").GetText();
                }
            }
            else
            {
                return constant.Cast("int").GetText();
            }
        }

        return constant.GetText();
    }

    [Pure]
    static string CastConstantToUInt32(ICSharpExpression constant, bool implicitlyConverted)
    {
        if (implicitlyConverted)
        {
            if (constant is ICSharpLiteralExpression)
            {
                if (constant.Type().IsChar())
                {
                    return constant.Cast("uint").GetText();
                }

                return $"{constant.GetText()}u";
            }

            return constant.Cast("uint").GetText();
        }

        return constant.GetText();
    }

    [Pure]
    static string CastConstantToInt64(ICSharpExpression constant, bool implicitlyConverted)
    {
        if (implicitlyConverted)
        {
            if (constant is ICSharpLiteralExpression)
            {
                if (constant.Type().IsChar())
                {
                    return constant.Cast("long").GetText();
                }

                var result = constant.GetText();

                if (result is [.. var rest, 'u' or 'U'])
                {
                    return $"{rest}L";
                }

                return $"{result}L";
            }

            return constant.Cast("long").GetText();
        }

        return constant.GetText();
    }

    [Pure]
    static string CastConstantToUInt64(ICSharpExpression constant, bool implicitlyConverted)
    {
        if (implicitlyConverted)
        {
            if (constant is ICSharpLiteralExpression)
            {
                if (constant.Type().IsChar())
                {
                    return constant.Cast("ulong").GetText();
                }

                var result = constant.GetText();

                return result switch
                {
                    [.. var rest, 'l' or 'L'] => $"{rest}ul",
                    [.. var rest, 'u' or 'U'] => $"{rest}ul",

                    _ => $"{result}ul",
                };
            }

            return constant.Cast("ulong").GetText();
        }

        return constant.GetText();
    }

    [Pure]
    static string CastConstantToInt128(ICSharpExpression constant, bool implicitlyConverted) => constant.Cast("Int128").GetText();

    [Pure]
    static string CastConstantToUInt128(ICSharpExpression constant, bool implicitlyConverted) => constant.Cast("UInt128").GetText();

    [Pure]
    static string CastConstantToIntPtr(ICSharpExpression constant, bool implicitlyConverted)
    {
        if (implicitlyConverted)
        {
            return constant.Cast(constant.GetCSharpLanguageLevel() >= CSharpLanguageLevel.CSharp90 ? "nint" : "IntPtr").GetText();
        }

        return constant.GetText();
    }

    [Pure]
    static string CastConstantToUIntPtr(ICSharpExpression constant, bool implicitlyConverted)
    {
        if (implicitlyConverted)
        {
            return constant.Cast(constant.GetCSharpLanguageLevel() >= CSharpLanguageLevel.CSharp90 ? "nuint" : "UIntPtr").GetText();
        }

        return constant.GetText();
    }

    [Pure]
    static string CastConstantToDecimal(ICSharpExpression constant, bool implicitlyConverted)
    {
        if (implicitlyConverted)
        {
            if (constant is ICSharpLiteralExpression)
            {
                if (constant.Type().IsChar())
                {
                    return constant.Cast("decimal").GetText();
                }

                var result = constant.GetText();

                var magnitude = result is ['-' or '+', .. var m] ? m : result;
                if (magnitude is ['0', 'x' or 'X' or 'b' or 'B', ..])
                {
                    return constant.Cast("decimal").GetText();
                }

                return result switch
                {
                    [.. var rest, 'u' or 'U', 'l' or 'L'] => $"{rest}m",
                    [.. var rest, 'l' or 'L', 'u' or 'U'] => $"{rest}m",
                    [.. var rest, 'u' or 'U'] => $"{rest}m",
                    [.. var rest, 'l' or 'L'] => $"{rest}m",

                    _ => $"{result}m",
                };
            }

            return constant.Cast("decimal").GetText();
        }

        return constant.GetText();
    }

    static NumberInfo<byte> Byte { get; } = new()
    {
        ClrTypeName = PredefinedType.BYTE_FQN,
        TypeCode = System.TypeCode.Byte,
        FormatSpecifiers =
            FormatSpecifiers.GeneralCaseInsensitiveWithoutPrecision
            | FormatSpecifiers.GeneralZeroPrecisionRedundant
            | FormatSpecifiers.Binary
            | FormatSpecifiers.Hexadecimal
            | FormatSpecifiers.Decimal,
        MaxValueStringLength = byte.MaxValue.ToString().Length,
        TryGetConstant = TryGetByteConstant,
        CastConstant = CastConstantToByte,
        Cast = expression => expression.Cast("byte").GetText(),
        CastZero = _ => "(byte)0",
        IsZeroConstant = expression => TryGetByteConstant(expression, out _) == 0,
        IsNonZeroConstant = expression => TryGetByteConstant(expression, out _) is { } and not 0,
        AreEqualConstants = (a, b) => TryGetByteConstant(a, out _) is { } x && TryGetByteConstant(b, out _) is { } y && x == y,
        AreMinMaxConstants = (a, b) => TryGetByteConstant(a, out _) == byte.MinValue && TryGetByteConstant(b, out _) == byte.MaxValue,
    };

    static NumberInfo<sbyte> SByte { get; } = new()
    {
        ClrTypeName = PredefinedType.SBYTE_FQN,
        TypeCode = System.TypeCode.SByte,
        FormatSpecifiers =
            FormatSpecifiers.GeneralCaseInsensitiveWithoutPrecision
            | FormatSpecifiers.GeneralZeroPrecisionRedundant
            | FormatSpecifiers.Binary
            | FormatSpecifiers.Hexadecimal
            | FormatSpecifiers.Decimal,
        MaxValueStringLength = sbyte.MaxValue.ToString().Length,
        TryGetConstant = TryGetSByteConstant,
        CastConstant = CastConstantToSByte,
        Cast = expression => expression.Cast("sbyte").GetText(),
        CastZero = _ => "(sbyte)0",
        IsZeroConstant = expression => TryGetSByteConstant(expression, out _) == 0,
        IsNonZeroConstant = expression => TryGetSByteConstant(expression, out _) is { } and not 0,
        AreEqualConstants = (a, b) => TryGetSByteConstant(a, out _) is { } x && TryGetSByteConstant(b, out _) is { } y && x == y,
        AreMinMaxConstants = (a, b) => TryGetSByteConstant(a, out _) == sbyte.MinValue && TryGetSByteConstant(b, out _) == sbyte.MaxValue,
    };

    static NumberInfo<short> Int16 { get; } = new()
    {
        ClrTypeName = PredefinedType.SHORT_FQN,
        TypeCode = System.TypeCode.Int16,
        FormatSpecifiers =
            FormatSpecifiers.GeneralCaseInsensitiveWithoutPrecision
            | FormatSpecifiers.GeneralZeroPrecisionRedundant
            | FormatSpecifiers.Binary
            | FormatSpecifiers.Hexadecimal
            | FormatSpecifiers.Decimal,
        MaxValueStringLength = short.MaxValue.ToString().Length,
        TryGetConstant = TryGetInt16Constant,
        CastConstant = CastConstantToInt16,
        Cast = expression => expression.Cast("short").GetText(),
        CastZero = _ => "(short)0",
        IsZeroConstant = expression => TryGetInt16Constant(expression, out _) == 0,
        IsNonZeroConstant = expression => TryGetInt16Constant(expression, out _) is { } and not 0,
        AreEqualConstants = (a, b) => TryGetInt16Constant(a, out _) is { } x && TryGetInt16Constant(b, out _) is { } y && x == y,
        AreMinMaxConstants = (a, b) => TryGetInt16Constant(a, out _) == short.MinValue && TryGetInt16Constant(b, out _) == short.MaxValue,
    };

    static NumberInfo<ushort> UInt16 { get; } = new()
    {
        ClrTypeName = PredefinedType.USHORT_FQN,
        TypeCode = System.TypeCode.UInt16,
        FormatSpecifiers =
            FormatSpecifiers.GeneralCaseInsensitiveWithoutPrecision
            | FormatSpecifiers.GeneralZeroPrecisionRedundant
            | FormatSpecifiers.Binary
            | FormatSpecifiers.Hexadecimal
            | FormatSpecifiers.Decimal,
        MaxValueStringLength = ushort.MaxValue.ToString().Length,
        TryGetConstant = TryGetUInt16Constant,
        CastConstant = CastConstantToUInt16,
        Cast = expression => expression.Cast("ushort").GetText(),
        CastZero = _ => "(ushort)0",
        IsZeroConstant = expression => TryGetUInt16Constant(expression, out _) == 0,
        IsNonZeroConstant = expression => TryGetUInt16Constant(expression, out _) is { } and not 0,
        AreEqualConstants = (a, b) => TryGetUInt16Constant(a, out _) is { } x && TryGetUInt16Constant(b, out _) is { } y && x == y,
        AreMinMaxConstants = (a, b) => TryGetUInt16Constant(a, out _) == ushort.MinValue && TryGetUInt16Constant(b, out _) == ushort.MaxValue,
    };

    internal static NumberInfo<int> Int32 { get; } = new()
    {
        ClrTypeName = PredefinedType.INT_FQN,
        TypeCode = System.TypeCode.Int32,
        FormatSpecifiers =
            FormatSpecifiers.GeneralCaseInsensitiveWithoutPrecision
            | FormatSpecifiers.GeneralZeroPrecisionRedundant
            | FormatSpecifiers.Binary
            | FormatSpecifiers.Hexadecimal
            | FormatSpecifiers.Decimal,
        MaxValueStringLength = int.MaxValue.ToString().Length,
        TryGetConstant = TryGetInt32Constant,
        CastConstant = CastConstantToInt32,
        Cast = expression => expression.Cast("int").GetText(),
        CastZero = _ => "0",
        IsZeroConstant = expression => TryGetInt32Constant(expression, out _) == 0,
        IsNonZeroConstant = expression => TryGetInt32Constant(expression, out _) is { } and not 0,
        AreEqualConstants = (a, b) => TryGetInt32Constant(a, out _) is { } x && TryGetInt32Constant(b, out _) is { } y && x == y,
        AreMinMaxConstants = (a, b) => TryGetInt32Constant(a, out _) == int.MinValue && TryGetInt32Constant(b, out _) == int.MaxValue,
    };

    static NumberInfo<uint> UInt32 { get; } = new()
    {
        ClrTypeName = PredefinedType.UINT_FQN,
        TypeCode = System.TypeCode.UInt32,
        FormatSpecifiers =
            FormatSpecifiers.GeneralCaseInsensitiveWithoutPrecision
            | FormatSpecifiers.GeneralZeroPrecisionRedundant
            | FormatSpecifiers.Binary
            | FormatSpecifiers.Hexadecimal
            | FormatSpecifiers.Decimal,
        MaxValueStringLength = uint.MaxValue.ToString().Length,
        TryGetConstant = TryGetUInt32Constant,
        CastConstant = CastConstantToUInt32,
        Cast = expression => expression.Cast("uint").GetText(),
        CastZero = _ => "0u",
        IsZeroConstant = expression => TryGetUInt32Constant(expression, out _) == 0,
        IsNonZeroConstant = expression => TryGetUInt32Constant(expression, out _) is { } and not 0,
        AreEqualConstants = (a, b) => TryGetUInt32Constant(a, out _) is { } x && TryGetUInt32Constant(b, out _) is { } y && x == y,
        AreMinMaxConstants = (a, b) => TryGetUInt32Constant(a, out _) == uint.MinValue && TryGetUInt32Constant(b, out _) == uint.MaxValue,
    };

    internal static NumberInfo<long> Int64 { get; } = new()
    {
        ClrTypeName = PredefinedType.LONG_FQN,
        TypeCode = System.TypeCode.Int64,
        FormatSpecifiers =
            FormatSpecifiers.GeneralCaseInsensitiveWithoutPrecision
            | FormatSpecifiers.GeneralZeroPrecisionRedundant
            | FormatSpecifiers.Binary
            | FormatSpecifiers.Hexadecimal
            | FormatSpecifiers.Decimal,
        MaxValueStringLength = long.MaxValue.ToString().Length,
        TryGetConstant = TryGetInt64Constant,
        CastConstant = CastConstantToInt64,
        Cast = expression => expression.Cast("long").GetText(),
        CastZero = _ => "0L",
        IsZeroConstant = expression => TryGetInt64Constant(expression, out _) == 0,
        IsNonZeroConstant = expression => TryGetInt64Constant(expression, out _) is { } and not 0,
        AreEqualConstants = (a, b) => TryGetInt64Constant(a, out _) is { } x && TryGetInt64Constant(b, out _) is { } y && x == y,
        AreMinMaxConstants = (a, b) => TryGetInt64Constant(a, out _) == long.MinValue && TryGetInt64Constant(b, out _) == long.MaxValue,
    };

    static NumberInfo<ulong> UInt64 { get; } = new()
    {
        ClrTypeName = PredefinedType.ULONG_FQN,
        TypeCode = System.TypeCode.UInt64,
        FormatSpecifiers =
            FormatSpecifiers.GeneralCaseInsensitiveWithoutPrecision
            | FormatSpecifiers.GeneralZeroPrecisionRedundant
            | FormatSpecifiers.Binary
            | FormatSpecifiers.Hexadecimal
            | FormatSpecifiers.Decimal,
        MaxValueStringLength = ulong.MaxValue.ToString().Length,
        TryGetConstant = TryGetUInt64Constant,
        CastConstant = CastConstantToUInt64,
        Cast = expression => expression.Cast("ulong").GetText(),
        CastZero = _ => "0ul",
        IsZeroConstant = expression => TryGetUInt64Constant(expression, out _) == 0,
        IsNonZeroConstant = expression => TryGetUInt64Constant(expression, out _) is { } and not 0,
        AreEqualConstants = (a, b) => TryGetUInt64Constant(a, out _) is { } x && TryGetUInt64Constant(b, out _) is { } y && x == y,
        AreMinMaxConstants = (a, b) => TryGetUInt64Constant(a, out _) == ulong.MinValue && TryGetUInt64Constant(b, out _) == ulong.MaxValue,
    };

    static NumberInfo<Int128> Int128 { get; } = new()
    {
        ClrTypeName = ClrTypeNames.Int128,
        FormatSpecifiers =
            FormatSpecifiers.GeneralCaseInsensitiveWithoutPrecision
            | FormatSpecifiers.GeneralZeroPrecisionRedundant
            | FormatSpecifiers.Binary
            | FormatSpecifiers.Hexadecimal
            | FormatSpecifiers.Decimal,
        MaxValueStringLength = BaseTypes.NumberInfos.Int128.MaxValue.ToString().Length,
        TryGetConstant = TryGetInt128Constant,
        CastConstant = CastConstantToInt128,
        Cast = expression => expression.Cast("Int128").GetText(),
        CastZero = _ => "(Int128)0",
        IsZeroConstant = expression => TryGetInt128Constant(expression, out _) == 0,
        IsNonZeroConstant = expression => TryGetInt128Constant(expression, out _) is { } value && value != 0,
        AreEqualConstants = (a, b) => TryGetInt128Constant(a, out _) is { } x && TryGetInt128Constant(b, out _) is { } y && x == y,
        AreMinMaxConstants =
            (a, b) => TryGetInt128Constant(a, out _) == BaseTypes.NumberInfos.Int128.MinValue
                && TryGetInt128Constant(b, out _) == BaseTypes.NumberInfos.Int128.MaxValue,
    };

    static NumberInfo<UInt128> UInt128 { get; } = new()
    {
        ClrTypeName = ClrTypeNames.UInt128,
        FormatSpecifiers =
            FormatSpecifiers.GeneralCaseInsensitiveWithoutPrecision
            | FormatSpecifiers.GeneralZeroPrecisionRedundant
            | FormatSpecifiers.Binary
            | FormatSpecifiers.Hexadecimal
            | FormatSpecifiers.Decimal,
        MaxValueStringLength = NumberInfos.UInt128.MaxValue.ToString().Length,
        TryGetConstant = TryGetUInt128Constant,
        CastConstant = CastConstantToUInt128,
        Cast = expression => expression.Cast("UInt128").GetText(),
        CastZero = _ => "(UInt128)0",
        IsZeroConstant = expression => TryGetUInt128Constant(expression, out _) == 0,
        IsNonZeroConstant = expression => TryGetUInt128Constant(expression, out _) is { } value && value != 0,
        AreEqualConstants = (a, b) => TryGetUInt128Constant(a, out _) is { } x && TryGetUInt128Constant(b, out _) is { } y && x == y,
        AreMinMaxConstants =
            (a, b) => TryGetUInt128Constant(a, out _) == BaseTypes.NumberInfos.UInt128.MinValue
                && TryGetUInt128Constant(b, out _) == BaseTypes.NumberInfos.UInt128.MaxValue,
    };

    static NumberInfo<nint> IntPtr { get; } = new()
    {
        ClrTypeName = PredefinedType.INTPTR_FQN,
        FormatSpecifiers =
            FormatSpecifiers.GeneralCaseInsensitiveWithoutPrecision
            | FormatSpecifiers.GeneralZeroPrecisionRedundant
            | FormatSpecifiers.Binary
            | FormatSpecifiers.Hexadecimal
            | FormatSpecifiers.Decimal,
        TryGetConstant = TryGetIntPtrConstant,
        CastConstant = CastConstantToIntPtr,
        Cast = expression => expression.Cast(expression.GetCSharpLanguageLevel() >= CSharpLanguageLevel.CSharp90 ? "nint" : "IntPtr").GetText(),
        CastZero = languageLevel => languageLevel >= CSharpLanguageLevel.CSharp90 ? "(nint)0" : "(IntPtr)0",
        IsZeroConstant = expression => TryGetIntPtrConstant(expression, out _) == 0,
        IsNonZeroConstant = expression => TryGetIntPtrConstant(expression, out _) is { } and not 0,
        AreEqualConstants = (a, b) => TryGetIntPtrConstant(a, out _) is { } x && TryGetIntPtrConstant(b, out _) is { } y && x == y,
    };

    static NumberInfo<nuint> UIntPtr { get; } = new()
    {
        ClrTypeName = PredefinedType.UINTPTR_FQN,
        FormatSpecifiers =
            FormatSpecifiers.GeneralCaseInsensitiveWithoutPrecision
            | FormatSpecifiers.GeneralZeroPrecisionRedundant
            | FormatSpecifiers.Binary
            | FormatSpecifiers.Hexadecimal
            | FormatSpecifiers.Decimal,
        TryGetConstant = TryGetUIntPtrConstant,
        CastConstant = CastConstantToUIntPtr,
        Cast = expression => expression.Cast(expression.GetCSharpLanguageLevel() >= CSharpLanguageLevel.CSharp90 ? "nuint" : "UIntPtr").GetText(),
        CastZero = languageLevel => languageLevel >= CSharpLanguageLevel.CSharp90 ? "(nuint)0" : "(UIntPtr)0",
        IsZeroConstant = expression => TryGetUIntPtrConstant(expression, out _) == 0,
        IsNonZeroConstant = expression => TryGetUIntPtrConstant(expression, out _) is { } and not 0,
        AreEqualConstants = (a, b) => TryGetUIntPtrConstant(a, out _) is { } x && TryGetUIntPtrConstant(b, out _) is { } y && x == y,
    };

    internal static NumberInfo<decimal> Decimal { get; } = new()
    {
        ClrTypeName = PredefinedType.DECIMAL_FQN,
        TypeCode = System.TypeCode.Decimal,
        FormatSpecifiers = FormatSpecifiers.GeneralCaseInsensitiveWithoutPrecision,
        TryGetConstant = TryGetDecimalConstant,
        CastConstant = CastConstantToDecimal,
        Cast = expression => expression.Cast("decimal").GetText(),
        AreEqualConstants = (a, b) => TryGetDecimalConstant(a, out _) is { } x && TryGetDecimalConstant(b, out _) is { } y && x == y,
        AreMinMaxConstants = (a, b) => TryGetDecimalConstant(a, out _) == decimal.MinValue && TryGetDecimalConstant(b, out _) == decimal.MaxValue,
    };

    internal static NumberInfo<double> Double { get; } = new()
    {
        ClrTypeName = PredefinedType.DOUBLE_FQN,
        TypeCode = System.TypeCode.Double,
        FormatSpecifiers = FormatSpecifiers.GeneralZeroPrecisionRedundant | FormatSpecifiers.RoundtripToBeReplaced,
        RoundTripFormatSpecifierReplacement = "G17",
        TryGetConstant = TryGetDoubleConstant,
        Cast = expression => expression.Cast("double").GetText(),
        NanConstant = $"double.{nameof(double.NaN)}",
    };

    internal static NumberInfo<float> Single { get; } = new()
    {
        ClrTypeName = PredefinedType.FLOAT_FQN,
        TypeCode = System.TypeCode.Single,
        FormatSpecifiers = FormatSpecifiers.GeneralZeroPrecisionRedundant | FormatSpecifiers.RoundtripToBeReplaced,
        RoundTripFormatSpecifierReplacement = "G9",
        TryGetConstant = TryGetSingleConstant,
        Cast = expression => expression.Cast("float").GetText(),
        NanConstant = $"float.{nameof(float.NaN)}",
    };

    static NumberInfo<Half> Half { get; } = new()
    {
        ClrTypeName = ClrTypeNames.Half,
        FormatSpecifiers = FormatSpecifiers.GeneralZeroPrecisionRedundant | FormatSpecifiers.RoundtripPrecisionRedundant,
        TryGetConstant = TryGetHalfConstant,
        Cast = expression => expression.Cast("Half").GetText(),
    };

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
            var t when t.IsInt128() => Int128,
            var t when t.IsUInt128() => UInt128,
            var t when t.IsIntPtr() => IntPtr,
            var t when t.IsUIntPtr() => UIntPtr,
            var t when t.IsDecimal() => Decimal,
            var t when t.IsDouble() => Double,
            var t when t.IsFloat() => Single,
            var t when t.IsHalf() => Half,
            _ => null,
        };

    public required IClrTypeName ClrTypeName { get; init; }

    internal TypeCode? TypeCode { get; private init; }

    public required FormatSpecifiers FormatSpecifiers { get; init; }

    internal string? RoundTripFormatSpecifierReplacement { get; private init; }

    internal int? MaxValueStringLength { get; private init; }

    internal string? NanConstant { get; private init; }

    internal Func<ICSharpExpression, bool, string>? CastConstant { get; private init; }

    internal Func<ICSharpExpression, string>? Cast { get; private init; }

    internal Func<CSharpLanguageLevel, string>? CastZero { get; private init; }

    internal Func<ICSharpExpression?, bool>? IsZeroConstant { get; private init; }

    internal Func<ICSharpExpression?, bool>? IsNonZeroConstant { get; private init; }

    internal Func<ICSharpExpression?, ICSharpExpression?, bool>? AreEqualConstants { get; private init; }

    internal Func<ICSharpExpression?, ICSharpExpression?, bool>? AreMinMaxConstants { get; private init; }

    [Pure]
    internal abstract string GetReplacementFromArgument(IType? targetType, ICSharpExpression argumentValue);
}