﻿using JetBrains.Metadata.Reader.API;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.CSharp.Conversions;
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
}