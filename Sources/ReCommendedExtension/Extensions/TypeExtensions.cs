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
    extension([NotNullWhen(true)] IType? type)
    {
        [Pure]
        public bool IsGenericArray() => type is IArrayType { Rank: 1 };

        [Pure]
        public bool IsGenericArrayOfAnyRank() => type is IArrayType;

        [Pure]
        public bool IsGenericArrayOfObject() => type is IArrayType(var elementType, 1) && elementType.IsObject();

        [Pure]
        public bool IsGenericArrayOfString() => type is IArrayType(var elementType, 1) && elementType.IsString();

        [Pure]
        public bool IsGenericArrayOfChar() => type is IArrayType(var elementType, 1) && elementType.IsChar();

        [Pure]
        public bool IsIEnumerableOfString()
        {
            if (type is IDeclaredType declaredType)
            {
                var (typeElement, substitution) = declaredType;

                if (typeElement is { }
                    && typeElement.IsClrType(PredefinedType.GENERIC_IENUMERABLE_FQN)
                    && typeElement.TypeParameters is [{ } typeParameter])
                {
                    return substitution[typeParameter].IsString();
                }
            }

            return false;
        }

        [Pure]
        public bool IsReadOnlySpanOfObject() => type.IsReadOnlySpan(out var spanTypeArgument) && spanTypeArgument.IsObject();

        [Pure]
        public bool IsReadOnlySpanOfString() => type.IsReadOnlySpan(out var spanTypeArgument) && spanTypeArgument.IsString();

        [Pure]
        public bool IsReadOnlySpanOfChar() => type.IsReadOnlySpan(out var spanTypeArgument) && spanTypeArgument.IsChar();

        [Pure]
        public bool IsReadOnlySpanOfByte() => type.IsReadOnlySpan(out var spanTypeArgument) && spanTypeArgument.IsByte();

        [Pure]
        public bool IsInt128() => type.IsClrType(ClrTypeNames.Int128);

        [Pure]
        public bool IsUInt128() => type.IsClrType(ClrTypeNames.UInt128);

        [Pure]
        public bool IsHalf() => type.IsClrType(ClrTypeNames.Half);

        [Pure]
        public bool IsDateOnly() => type.IsClrType(PredefinedType.DATE_ONLY_FQN);

        [Pure]
        public bool IsTimeOnly() => type.IsClrType(PredefinedType.TIME_ONLY_FQN);

        [Pure]
        public bool IsTimeSpanStyles() => type.IsClrType(ClrTypeNames.TimeSpanStyles);

        [Pure]
        public bool IsDateTimeStyles() => type.IsClrType(ClrTypeNames.DateTimeStyles);

        [Pure]
        public bool IsDateTimeKind() => type.IsClrType(ClrTypeNames.DateTimeKind);

        [Pure]
        public bool IsCalendar() => type.IsClrType(ClrTypeNames.Calendar);

        [Pure]
        public bool IsNumberStyles() => type.IsClrType(ClrTypeNames.NumberStyles);

        [Pure]
        public bool IsMidpointRounding() => type.IsClrType(ClrTypeNames.MidpointRounding);

        [Pure]
        public bool IsStringComparison() => type.IsClrType(PredefinedType.STRING_COMPARISON_CLASS);

        [Pure]
        public bool IsStringSplitOptions() => type.IsClrType(ClrTypeNames.StringSplitOptions);

        [Pure]
        public bool IsStringBuilder() => type.IsClrType(PredefinedType.STRING_BUILDER_FQN);

        [Pure]
        public bool IsGenericEqualityComparer() => type.IsClrType(PredefinedType.GENERIC_IEQUALITY_COMPARER_FQN);

        [Pure]
        public bool IsFuncOfTResult() => type.IsClrType(PredefinedType.FUNC2_FQN);

        [Pure]
        public bool IsEnumType([NotNullWhen(true)] out IType? enumBaseType)
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
        public bool IsGenericIEnumerableOrDescendant()
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
        public bool IsIAsyncEnumerableOrDescendant()
        {
            if (type.IsIAsyncEnumerable())
            {
                return true;
            }

            if (type.GetTypeElement<ITypeElement>() is { } typeElement
                && typeElement.IsDescendantOf(typeElement.Module.GetPredefinedType().IAsyncEnumerable.GetTypeElement()))
            {
                return true;
            }

            return false;
        }

        [Pure]
        public bool IsGenericListOrDescendant()
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
        public bool IsGenericType() => (type as IDeclaredType)?.GetTypeElement() is { TypeParameters: [_, ..] };

        [Pure]
        public bool IsValueTuple([NotNullWhen(true)] out IType? t1TypeArgument, [NotNullWhen(true)] out IType? t2TypeArgument)
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
        public bool IsValueTuple([NonNegativeValue] out int tupleLength)
        {
            if (type is IDeclaredType declaredType && declaredType.AsTupleType() is { } tupleType)
            {
                tupleLength = tupleType.GetComponents().Count;
                return true;
            }

            tupleLength = 0;
            return false;
        }
    }

    extension(IType type)
    {
        [Pure]
        public bool IsDisposable()
            => type.GetTypeElement() is { } typeElement
                && (typeElement.IsDisposable && !type.IsTask() && !type.IsGenericTask()
                    || typeElement.IsNullableOfT() && TypesUtil.GetTypeArgumentValue(type, 0).GetTypeElement() is { IsDisposable: true }
                    || typeElement is IStruct { IsByRefLike: true, HasDisposeMethods: true });

        [Pure]
        public bool IsTasklikeOfDisposable(ITreeNode context)
            => type.IsTasklike(context)
                && type.GetTasklikeUnderlyingType(context) is { } awaitedType
                && awaitedType.GetTypeElement() is { } awaitedTypeElement
                && (awaitedTypeElement.IsDisposable && !awaitedType.IsTask() && !awaitedType.IsGenericTask()
                    || awaitedTypeElement.IsNullableOfT() && TypesUtil.GetTypeArgumentValue(awaitedType, 0).GetTypeElement() is { IsDisposable: true });

        [Pure]
        public string? TryGetDefaultValueLiteral(ITreeNode context)
        {
            if (type is IDeclaredType declaredType && declaredType.AsTupleType() is { } tupleType)
            {
                var tupleTypes = string.Join(", ", from c in tupleType.GetComponents() select c.Type.TryGetDefaultValueLiteral(context) ?? "default");

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
}