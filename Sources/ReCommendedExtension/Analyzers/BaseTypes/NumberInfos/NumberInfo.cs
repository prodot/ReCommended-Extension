using System.Globalization;
using JetBrains.Metadata.Reader.API;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.CSharp;
using JetBrains.ReSharper.Psi.CSharp.Tree;
using JetBrains.ReSharper.Psi.Util;
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

    internal static NumberInfo<byte> Byte { get; } = new()
    {
        ClrTypeName = PredefinedType.BYTE_FQN,
        TypeCode = System.TypeCode.Byte,
        DefaultNumberStyles = NumberStyles.Integer,
        CanUseEqualityOperator = true,
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
        IsZero = value => value == 0,
        AreEqual = (x, y) => x == y,
        AreMinMaxValues = (min, max) => (min, max) == (byte.MinValue, byte.MaxValue),
    };

    internal static NumberInfo<sbyte> SByte { get; } = new()
    {
        ClrTypeName = PredefinedType.SBYTE_FQN,
        TypeCode = System.TypeCode.SByte,
        DefaultNumberStyles = NumberStyles.Integer,
        CanUseEqualityOperator = true,
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
        IsZero = value => value == 0,
        AreEqual = (x, y) => x == y,
        AreMinMaxValues = (min, max) => (min, max) == (sbyte.MinValue, sbyte.MaxValue),
    };

    internal static NumberInfo<short> Int16 { get; } = new()
    {
        ClrTypeName = PredefinedType.SHORT_FQN,
        TypeCode = System.TypeCode.Int16,
        DefaultNumberStyles = NumberStyles.Integer,
        CanUseEqualityOperator = true,
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
        IsZero = value => value == 0,
        AreEqual = (x, y) => x == y,
        AreMinMaxValues = (min, max) => (min, max) == (short.MinValue, short.MaxValue),
    };

    internal static NumberInfo<ushort> UInt16 { get; } = new()
    {
        ClrTypeName = PredefinedType.USHORT_FQN,
        TypeCode = System.TypeCode.UInt16,
        DefaultNumberStyles = NumberStyles.Integer,
        CanUseEqualityOperator = true,
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
        IsZero = value => value == 0,
        AreEqual = (x, y) => x == y,
        AreMinMaxValues = (min, max) => (min, max) == (ushort.MinValue, ushort.MaxValue),
    };

    internal static NumberInfo<int> Int32 { get; } = new()
    {
        ClrTypeName = PredefinedType.INT_FQN,
        TypeCode = System.TypeCode.Int32,
        DefaultNumberStyles = NumberStyles.Integer,
        CanUseEqualityOperator = true,
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
        IsZero = value => value == 0,
        AreEqual = (x, y) => x == y,
        AreMinMaxValues = (min, max) => (min, max) == (int.MinValue, int.MaxValue),
    };

    internal static NumberInfo<uint> UInt32 { get; } = new()
    {
        ClrTypeName = PredefinedType.UINT_FQN,
        TypeCode = System.TypeCode.UInt32,
        DefaultNumberStyles = NumberStyles.Integer,
        CanUseEqualityOperator = true,
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
        IsZero = value => value == 0,
        AreEqual = (x, y) => x == y,
        AreMinMaxValues = (min, max) => (min, max) == (uint.MinValue, uint.MaxValue),
    };

    internal static NumberInfo<long> Int64 { get; } = new()
    {
        ClrTypeName = PredefinedType.LONG_FQN,
        TypeCode = System.TypeCode.Int64,
        DefaultNumberStyles = NumberStyles.Integer,
        CanUseEqualityOperator = true,
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
        IsZero = value => value == 0,
        AreEqual = (x, y) => x == y,
        AreMinMaxValues = (min, max) => (min, max) == (long.MinValue, long.MaxValue),
    };

    internal static NumberInfo<ulong> UInt64 { get; } = new()
    {
        ClrTypeName = PredefinedType.ULONG_FQN,
        TypeCode = System.TypeCode.UInt64,
        DefaultNumberStyles = NumberStyles.Integer,
        CanUseEqualityOperator = true,
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
        IsZero = value => value == 0,
        AreEqual = (x, y) => x == y,
        AreMinMaxValues = (min, max) => (min, max) == (ulong.MinValue, ulong.MaxValue),
    };

    internal static NumberInfo<Int128> Int128 { get; } = new()
    {
        ClrTypeName = ClrTypeNames.Int128,
        DefaultNumberStyles = NumberStyles.Integer,
        CanUseEqualityOperator = true,
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
        IsZero = value => value == 0,
        AreEqual = (x, y) => x == y,
        AreMinMaxValues = (min, max) => (min, max) == (BaseTypes.NumberInfos.Int128.MinValue, BaseTypes.NumberInfos.Int128.MaxValue),
    };

    internal static NumberInfo<UInt128> UInt128 { get; } = new()
    {
        ClrTypeName = ClrTypeNames.UInt128,
        DefaultNumberStyles = NumberStyles.Integer,
        CanUseEqualityOperator = true,
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
        IsZero = value => value == 0,
        AreEqual = (x, y) => x == y,
        AreMinMaxValues = (min, max) => (min, max) == (NumberInfos.UInt128.MinValue, NumberInfos.UInt128.MaxValue),
    };

    internal static NumberInfo<nint> IntPtr { get; } = new()
    {
        ClrTypeName = PredefinedType.INTPTR_FQN,
        DefaultNumberStyles = NumberStyles.Integer,
        CanUseEqualityOperator = true,
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
        IsZero = value => value == 0,
        AreEqual = (x, y) => x == y,
        AreMinMaxValues = (_, _) => false, // nint.MinValue and nint.MaxValue are platform-dependent
    };

    internal static NumberInfo<nuint> UIntPtr { get; } = new()
    {
        ClrTypeName = PredefinedType.UINTPTR_FQN,
        DefaultNumberStyles = NumberStyles.Integer,
        CanUseEqualityOperator = true,
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
        IsZero = value => value == 0,
        AreEqual = (x, y) => x == y,
        AreMinMaxValues = (_, _) => false, // nuint.MinValue and nuint.MaxValue are platform-dependent
    };

    internal static NumberInfo<decimal> Decimal { get; } = new()
    {
        ClrTypeName = PredefinedType.DECIMAL_FQN,
        TypeCode = System.TypeCode.Decimal,
        DefaultNumberStyles = NumberStyles.Number,
        CanUseEqualityOperator = true,
        FormatSpecifiers = FormatSpecifiers.GeneralCaseInsensitiveWithoutPrecision,
        TryGetConstant = TryGetDecimalConstant,
        CastConstant = CastConstantToDecimal,
        Cast = expression => expression.Cast("decimal").GetText(),
        AreEqual = (x, y) => x == y,
        AreMinMaxValues = (min, max) => (min, max) == (decimal.MinValue, decimal.MaxValue),
    };

    internal static NumberInfo<double> Double { get; } = new()
    {
        ClrTypeName = PredefinedType.DOUBLE_FQN,
        TypeCode = System.TypeCode.Double,
        DefaultNumberStyles = NumberStyles.Float | NumberStyles.AllowThousands,
        CanUseEqualityOperator = false, // can only be checked by comparing literals
        FormatSpecifiers = FormatSpecifiers.GeneralZeroPrecisionRedundant | FormatSpecifiers.RoundtripToBeReplaced,
        RoundTripFormatSpecifierReplacement = "G17",
        TryGetConstant = TryGetDoubleConstant,
        Cast = expression => expression.Cast("double").GetText(),
        AreEqual = (_, _) => false, // can only be checked by comparing literals
        AreMinMaxValues = (_, _) => false, // can only be checked by comparing literals
        NanConstant = $"double.{nameof(double.NaN)}",
    };

    internal static NumberInfo<float> Single { get; } = new()
    {
        ClrTypeName = PredefinedType.FLOAT_FQN,
        TypeCode = System.TypeCode.Single,
        DefaultNumberStyles = NumberStyles.Float | NumberStyles.AllowThousands,
        CanUseEqualityOperator = false, // can only be checked by comparing literals
        FormatSpecifiers = FormatSpecifiers.GeneralZeroPrecisionRedundant | FormatSpecifiers.RoundtripToBeReplaced,
        RoundTripFormatSpecifierReplacement = "G9",
        TryGetConstant = TryGetSingleConstant,
        Cast = expression => expression.Cast("float").GetText(),
        AreEqual = (_, _) => false, // can only be checked by comparing literals
        AreMinMaxValues = (_, _) => false, // can only be checked by comparing literals
        NanConstant = $"float.{nameof(float.NaN)}",
    };

    internal static NumberInfo<Half> Half { get; } = new()
    {
        ClrTypeName = ClrTypeNames.Half,
        DefaultNumberStyles = NumberStyles.Float | NumberStyles.AllowThousands,
        CanUseEqualityOperator = false, // can only be checked by comparing literals
        FormatSpecifiers = FormatSpecifiers.GeneralZeroPrecisionRedundant | FormatSpecifiers.RoundtripPrecisionRedundant,
        TryGetConstant = TryGetHalfConstant,
        Cast = expression => expression.Cast("Half").GetText(),
        AreEqual = (_, _) => false, // can only be checked by comparing literals
        AreMinMaxValues = (_, _) => false, // can only be checked by comparing literals
    };

    [Pure]
    internal static NumberInfo? TryGet(IType type)
        => type switch
        {
            var t when t.IsByte() || t.IsNullable() && t.Unlift().IsByte() => Byte,
            var t when t.IsSbyte() || t.IsNullable() && t.Unlift().IsSbyte() => SByte,
            var t when t.IsShort() || t.IsNullable() && t.Unlift().IsShort() => Int16,
            var t when t.IsUshort() || t.IsNullable() && t.Unlift().IsUshort() => UInt16,
            var t when t.IsInt() || t.IsNullable() && t.Unlift().IsInt() => Int32,
            var t when t.IsUint() || t.IsNullable() && t.Unlift().IsUint() => UInt32,
            var t when t.IsLong() || t.IsNullable() && t.Unlift().IsLong() => Int64,
            var t when t.IsUlong() || t.IsNullable() && t.Unlift().IsUlong() => UInt64,
            var t when t.IsClrType(ClrTypeNames.Int128) || t.IsNullable() && t.Unlift().IsClrType(ClrTypeNames.Int128) => Int128,
            var t when t.IsClrType(ClrTypeNames.UInt128) || t.IsNullable() && t.Unlift().IsClrType(ClrTypeNames.UInt128) => UInt128,
            var t when t.IsIntPtr() || t.IsNullable() && t.Unlift().IsIntPtr() => IntPtr,
            var t when t.IsUIntPtr() || t.IsNullable() && t.Unlift().IsUIntPtr() => UIntPtr,
            var t when t.IsDecimal() || t.IsNullable() && t.Unlift().IsDecimal() => Decimal,
            var t when t.IsDouble() || t.IsNullable() && t.Unlift().IsDouble() => Double,
            var t when t.IsFloat() || t.IsNullable() && t.Unlift().IsFloat() => Single,
            var t when t.IsClrType(ClrTypeNames.Half) || t.IsNullable() && t.Unlift().IsClrType(ClrTypeNames.Half) => Half,
            _ => null,
        };

    public required IClrTypeName ClrTypeName { get; init; }

    internal TypeCode? TypeCode { get; private init; }

    public required NumberStyles DefaultNumberStyles { get; init; }

    public required bool CanUseEqualityOperator { get; init; }

    public required FormatSpecifiers FormatSpecifiers { get; init; }

    internal string? RoundTripFormatSpecifierReplacement { get; private init; }

    internal int? MaxValueStringLength { get; private init; }

    internal string? NanConstant { get; private init; }

    internal Func<ICSharpExpression, bool, string>? CastConstant { get; private init; }

    internal Func<ICSharpExpression, string>? Cast { get; private init; }

    internal Func<CSharpLanguageLevel, string>? CastZero { get; private init; }
}