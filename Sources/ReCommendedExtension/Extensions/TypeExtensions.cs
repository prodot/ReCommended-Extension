using JetBrains.Metadata.Reader.API;
using JetBrains.ReSharper.Feature.Services.Util;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.CSharp;
using JetBrains.ReSharper.Psi.CSharp.Conversions;
using JetBrains.ReSharper.Psi.CSharp.Tree;
using JetBrains.ReSharper.Psi.CSharp.Util;
using JetBrains.ReSharper.Psi.Tree;
using JetBrains.ReSharper.Psi.Util;

namespace ReCommendedExtension.Extensions;

internal static class TypeExtensions
{
    [Pure]
    public static bool IsGenericArray(this IType type, ITreeNode context)
        => CollectionTypeUtil.ElementTypeByCollectionType(type, context, false) is { } elementType
            && type.IsImplicitlyConvertibleTo(
                TypeFactory.CreateArrayType(elementType, 1, NullableAnnotation.Unknown),
                context.GetTypeConversionRule());

    [Pure]
    public static bool IsGenericArrayOfAnyRank(this IType type, ITreeNode context)
    {
        if (CollectionTypeUtil.ElementTypeByCollectionType(type, context, false) is { } elementType)
        {
            for (var i = 1; i <= 16; i++)
            {
                if (type.IsImplicitlyConvertibleTo(
                    TypeFactory.CreateArrayType(elementType, i, NullableAnnotation.Unknown),
                    context.GetTypeConversionRule()))
                {
                    return true;
                }
            }
        }

        return false;
    }

    [Pure]
    public static bool IsGenericArrayOf(this IType type, IClrTypeName elementTypeName, ITreeNode context)
        => CollectionTypeUtil.ElementTypeByCollectionType(type, context, false) is { } elementType
            && elementType.IsClrType(elementTypeName)
            && type.IsImplicitlyConvertibleTo(
                TypeFactory.CreateArrayType(elementType, 1, NullableAnnotation.Unknown),
                context.GetTypeConversionRule());

    [Pure]
    public static bool IsReadOnlySpanOfObject(this IType type) => type.IsReadOnlySpan(out var spanTypeArgument) && spanTypeArgument.IsObject();

    [Pure]
    public static bool IsReadOnlySpanOfString(this IType type) => type.IsReadOnlySpan(out var spanTypeArgument) && spanTypeArgument.IsString();

    [Pure]
    public static bool IsGenericEnumerableOrDescendant(this IType type)
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
    public static bool IsDisposable(this IType type, ITreeNode context)
    {
        if (type.GetTypeElement() is { } typeElement)
        {
            var psiModule = context.GetPsiModule();

            return typeElement.IsDisposable(psiModule) && !type.IsTask() && !type.IsGenericTask()
                || typeElement.IsNullableOfT()
                && TypesUtil.GetTypeArgumentValue(type, 0).GetTypeElement() is { } structType
                && structType.IsDisposable(psiModule)
                || typeElement is IStruct { IsByRefLike: true } s && s.HasDisposeMethods();
        }

        return false;
    }

    [Pure]
    public static bool IsTasklikeOfDisposable(this IType type, ITreeNode context)
    {
        if (type.IsTasklike(context)
            && type.GetTasklikeUnderlyingType(context) is { } awaitedType
            && awaitedType.GetTypeElement() is { } awaitedTypeElement)
        {
            var psiModule = context.GetPsiModule();

            return awaitedTypeElement.IsDisposable(psiModule) && !awaitedType.IsTask() && !awaitedType.IsGenericTask()
                || awaitedTypeElement.IsNullableOfT()
                && TypesUtil.GetTypeArgumentValue(awaitedType, 0).GetTypeElement() is { } structType
                && structType.IsDisposable(psiModule);
        }

        return false;
    }

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
            if (type.IsClrType(ClrTypeNames.Int128) || type.IsClrType(ClrTypeNames.UInt128))
            {
                return "0";
            }

            if (type.IsClrType(ClrTypeNames.Half))
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