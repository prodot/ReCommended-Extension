using JetBrains.ReSharper.Feature.Services.Util;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.CSharp;
using JetBrains.ReSharper.Psi.CSharp.Tree;
using JetBrains.ReSharper.Psi.CSharp.Util;
using JetBrains.ReSharper.Psi.Tree;
using JetBrains.ReSharper.Psi.Util;

namespace ReCommendedExtension.Extensions;

internal static class TypeExtensions
{
    [Pure]
    public static bool IsGenericArray(this IType type) => type is IArrayType { Rank: 1 };

    [Pure]
    public static bool IsGenericArrayOfAnyRank(this IType type) => type is IArrayType;

    [Pure]
    public static bool IsGenericArrayOfObject(this IType type) => type is IArrayType(var elementType, 1) && elementType.IsObject();

    [Pure]
    public static bool IsGenericArrayOfString(this IType type) => type is IArrayType(var elementType, 1) && elementType.IsString();

    [Pure]
    public static bool IsGenericArrayOfChar(this IType type) => type is IArrayType(var elementType, 1) && elementType.IsChar();

    [Pure]
    public static bool IsReadOnlySpanOfObject(this IType type) => type.IsReadOnlySpan(out var spanTypeArgument) && spanTypeArgument.IsObject();

    [Pure]
    public static bool IsReadOnlySpanOfString(this IType type) => type.IsReadOnlySpan(out var spanTypeArgument) && spanTypeArgument.IsString();

    [Pure]
    public static bool IsReadOnlySpanOfChar(this IType type) => type.IsReadOnlySpan(out var spanTypeArgument) && spanTypeArgument.IsChar();

    [Pure]
    public static bool IsReadOnlySpanOfByte(this IType type) => type.IsReadOnlySpan(out var spanTypeArgument) && spanTypeArgument.IsByte();

    [Pure]
    public static bool IsInt128(this IType type) => type.IsClrType(ClrTypeNames.Int128);

    [Pure]
    public static bool IsUInt128(this IType type) => type.IsClrType(ClrTypeNames.UInt128);

    [Pure]
    public static bool IsHalf(this IType type) => type.IsClrType(ClrTypeNames.Half);

    [Pure]
    public static bool IsDateOnly(this IType type) => type.IsClrType(PredefinedType.DATE_ONLY_FQN);

    [Pure]
    public static bool IsTimeOnly(this IType type) => type.IsClrType(PredefinedType.TIME_ONLY_FQN);

    [Pure]
    public static bool IsTimeSpanStyles([NotNullWhen(true)] this IType? type) => type.IsClrType(ClrTypeNames.TimeSpanStyles);

    [Pure]
    public static bool IsDateTimeStyles([NotNullWhen(true)] this IType? type) => type.IsClrType(ClrTypeNames.DateTimeStyles);

    [Pure]
    public static bool IsDateTimeKind([NotNullWhen(true)] this IType? type) => type.IsClrType(ClrTypeNames.DateTimeKind);

    [Pure]
    public static bool IsCalendar(this IType type) => type.IsClrType(ClrTypeNames.Calendar);

    [Pure]
    public static bool IsNumberStyles([NotNullWhen(true)] this IType? type) => type.IsClrType(ClrTypeNames.NumberStyles);

    [Pure]
    public static bool IsMidpointRounding([NotNullWhen(true)] this IType? type) => type.IsClrType(ClrTypeNames.MidpointRounding);

    [Pure]
    public static bool IsStringComparison([NotNullWhen(true)] this IType? type) => type.IsClrType(PredefinedType.STRING_COMPARISON_CLASS);

    [Pure]
    public static bool IsStringSplitOptions([NotNullWhen(true)] this IType? type) => type.IsClrType(ClrTypeNames.StringSplitOptions);

    [Pure]
    public static bool IsStringBuilder(this IType type) => type.IsClrType(PredefinedType.STRING_BUILDER_FQN);

    [Pure]
    public static bool IsGenericEqualityComparer([NotNullWhen(true)] this IType? type)
        => type.IsClrType(PredefinedType.GENERIC_IEQUALITY_COMPARER_FQN);

    [Pure]
    public static bool IsEnumType(this IType type, [NotNullWhen(true)] out IType? enumBaseType)
    {
        if (type is IDeclaredType declaredType)
        {
            var (typeElement, _) = declaredType;
            if (typeElement is IEnum enumType)
            {
                enumBaseType = enumType.GetUnderlyingType();
                return true;
            }
        }

        enumBaseType = null;
        return false;
    }

    [Pure]
    public static bool IsGenericIEnumerableOrDescendant(this IType type)
    {
        if (type.IsGenericIEnumerable())
        {
            return true;
        }

        if (type.GetTypeElement<ITypeElement>() is { } typeElement
            && typeElement.IsDescendantOf(typeElement.Module.GetPredefinedType().GenericIEnumerable.GetTypeElement()))
        {
            return true;
        }

        return false;
    }

    [Pure]
    public static bool IsGenericListOrDescendant(this IType type)
    {
        if (type.IsGenericList())
        {
            return true;
        }

        if (type.GetTypeElement<ITypeElement>() is { } typeElement
            && typeElement.IsDescendantOf(typeElement.Module.GetPredefinedType().GenericList.GetTypeElement()))
        {
            return true;
        }

        return false;
    }

    [Pure]
    public static bool IsDisposable(this IType type)
        => type.GetTypeElement() is { } typeElement
            && (typeElement.IsDisposable() && !type.IsTask() && !type.IsGenericTask()
                || typeElement.IsNullableOfT()
                && TypesUtil.GetTypeArgumentValue(type, 0).GetTypeElement() is { } structType
                && structType.IsDisposable()
                || typeElement is IStruct { IsByRefLike: true } s && s.HasDisposeMethods());

    [Pure]
    public static bool IsTasklikeOfDisposable(this IType type, ITreeNode context)
        => type.IsTasklike(context)
            && type.GetTasklikeUnderlyingType(context) is { } awaitedType
            && awaitedType.GetTypeElement() is { } awaitedTypeElement
            && (awaitedTypeElement.IsDisposable() && !awaitedType.IsTask() && !awaitedType.IsGenericTask()
                || awaitedTypeElement.IsNullableOfT()
                && TypesUtil.GetTypeArgumentValue(awaitedType, 0).GetTypeElement() is { } structType
                && structType.IsDisposable());

    [Pure]
    public static bool IsGenericType(this IType type) => (type as IDeclaredType)?.GetTypeElement() is { TypeParameters: [_, ..] };

    [Pure]
    public static bool IsValueTuple(
        [NotNullWhen(true)] this IType? type,
        [NotNullWhen(true)] out IType? t1TypeArgument,
        [NotNullWhen(true)] out IType? t2TypeArgument)
    {
        if (type is IDeclaredType declaredType)
        {
            var (typeElement, substitution) = declaredType;

            if (typeElement.IsClrType(PredefinedType.VALUE_TUPLE_2_FQN) && typeElement.TypeParameters is [var t1TypeParameter, var t2TypeParameter])
            {
                t1TypeArgument = t1TypeParameter is { } ? substitution[t2TypeParameter] : TypeFactory.CreateUnknownType(type.Module);
                t2TypeArgument = t2TypeParameter is { } ? substitution[t2TypeParameter] : TypeFactory.CreateUnknownType(type.Module);
                return true;
            }
        }

        t1TypeArgument = null;
        t2TypeArgument = null;
        return false;
    }

    [Pure]
    public static bool IsValueTuple([NotNullWhen(true)] this IType? type, [NonNegativeValue] out int tupleLength)
    {
        if (type is IDeclaredType declaredType && declaredType.AsTupleType() is { } tupleType)
        {
            tupleLength = tupleType.GetComponents().Count;
            return true;
        }

        tupleLength = 0;
        return false;
    }

    [Pure]
    public static string? TryGetDefaultValue(this IType type, ITreeNode context)
    {
        if (type is IDeclaredType declaredType && declaredType.AsTupleType() is { } tupleType)
        {
            var tupleTypes = string.Join(", ", from c in tupleType.GetComponents() select c.Type.TryGetDefaultValue(context) ?? "default");

            return $"({tupleTypes})";
        }

        Debug.Assert(CSharpLanguage.Instance is { });

        var defaultValue = DefaultValueUtil.GetClrDefaultValue(type, CSharpLanguage.Instance, context);

        if (type.IsEnumType() && defaultValue is { } and not ICastExpression)
        {
            return $"{type.GetPresentableName(CSharpLanguage.Instance)}.{defaultValue.GetText()}";
        }

        if (defaultValue is IObjectCreationExpression)
        {
            if (type.IsInt128() || type.IsUInt128())
            {
                return "0";
            }

            if (type.IsHalf())
            {
                return "(Half)0"; // todo: use "nameof(Half)"
            }

            if (type.IsClrType(PredefinedType.CANCELLATION_TOKEN_FQN))
            {
                return $"{nameof(CancellationToken)}.{nameof(CancellationToken.None)}";
            }
        }

        return defaultValue?.GetText();
    }
}