using JetBrains.Annotations;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.CodeAnnotations;
using JetBrains.ReSharper.Psi.CSharp.DeclaredElements;
using JetBrains.ReSharper.Psi.CSharp.Util;
using JetBrains.ReSharper.Psi.Tree;
using JetBrains.ReSharper.Psi.Util;

namespace ReCommendedExtension.Providers
{
    internal static class NullableAnnotationContextExtensions
    {
        [Pure]
        static bool IsNullableReferenceType([NotNull] this IType type)
        {
            switch (type.Classify)
            {
                case TypeClassification.REFERENCE_TYPE:
                    switch (type.NullableAnnotation)
                    {
                        case NullableAnnotation.NotAnnotated:
                            if (type.IsOpenType)
                            {
                                var typeParameterType = type.GetTypeParameterType();
                                switch (typeParameterType?.Nullability)
                                {
                                    case TypeParameterNullability.NotNullableValueType: return false;

                                    case TypeParameterNullability.NotNullableValueOrReferenceType:
                                    case TypeParameterNullability.NotNullableReferenceType:
                                    case TypeParameterNullability.NotNullableSuperType:
                                        return false;

                                    case TypeParameterNullability.NullableValueOrReferenceType:
                                    case TypeParameterNullability.NullableReferenceType:
                                    case TypeParameterNullability.NullableSuperType:
                                        return true;

                                    default: return false;
                                }
                            }

                            return false;

                        case NullableAnnotation.NotNullable: return false;

                        case NullableAnnotation.Annotated:
                        case NullableAnnotation.Nullable:
                            return true;

                        default: return false;
                    }

                case TypeClassification.VALUE_TYPE: return false;

                default: goto case TypeClassification.REFERENCE_TYPE;
            }
        }

        [Pure]
        static bool IsNonNullableReferenceType([NotNull] this IType type)
        {
            switch (type.Classify)
            {
                case TypeClassification.REFERENCE_TYPE:
                    switch (type.NullableAnnotation)
                    {
                        case NullableAnnotation.NotAnnotated:
                            if (type.IsOpenType)
                            {
                                var typeParameterType = type.GetTypeParameterType();
                                switch (typeParameterType?.Nullability)
                                {
                                    case TypeParameterNullability.NotNullableValueType: return false;

                                    case TypeParameterNullability.NotNullableValueOrReferenceType:
                                    case TypeParameterNullability.NotNullableReferenceType:
                                    case TypeParameterNullability.NotNullableSuperType:
                                        return true;

                                    case TypeParameterNullability.NullableValueOrReferenceType:
                                    case TypeParameterNullability.NullableReferenceType:
                                    case TypeParameterNullability.NullableSuperType:
                                        return false;

                                    default: return false;
                                }
                            }

                            return true;

                        case NullableAnnotation.NotNullable: return true;

                        case NullableAnnotation.Annotated:
                        case NullableAnnotation.Nullable:
                            return false;

                        default: return false;
                    }

                case TypeClassification.VALUE_TYPE: return false;

                default: goto case TypeClassification.REFERENCE_TYPE;
            }
        }

        [Pure]
        static CodeAnnotationNullableValue? TryGetAnnotationNullableValue(
            [NotNull] this IType type,
            bool canStillBeNull = false,
            bool shouldNeverBeNull = false)
        {
            switch (type.Classify)
            {
                case TypeClassification.REFERENCE_TYPE:
                    switch (type.NullableAnnotation)
                    {
                        case NullableAnnotation.NotAnnotated:
                            if (type.IsOpenType)
                            {
                                var typeParameterType = type.GetTypeParameterType();
                                switch (typeParameterType?.Nullability)
                                {
                                    case TypeParameterNullability.NotNullableValueType: return null;

                                    case TypeParameterNullability.NotNullableValueOrReferenceType:
                                    case TypeParameterNullability.NotNullableReferenceType:
                                    case TypeParameterNullability.NotNullableSuperType:
                                        if (canStillBeNull)
                                        {
                                            return CodeAnnotationNullableValue.CAN_BE_NULL;
                                        }
                                        return CodeAnnotationNullableValue.NOT_NULL;

                                    case TypeParameterNullability.NullableValueOrReferenceType:
                                    case TypeParameterNullability.NullableReferenceType:
                                    case TypeParameterNullability.NullableSuperType:
                                        if (shouldNeverBeNull)
                                        {
                                            return CodeAnnotationNullableValue.NOT_NULL;
                                        }
                                        return CodeAnnotationNullableValue.CAN_BE_NULL;
                                }
                            }

                            if (canStillBeNull)
                            {
                                return CodeAnnotationNullableValue.CAN_BE_NULL;
                            }

                            return CodeAnnotationNullableValue.NOT_NULL;

                        case NullableAnnotation.NotNullable:
                            if (canStillBeNull)
                            {
                                return CodeAnnotationNullableValue.CAN_BE_NULL;
                            }
                            return CodeAnnotationNullableValue.NOT_NULL;

                        case NullableAnnotation.Annotated:
                        case NullableAnnotation.Nullable:
                            if (shouldNeverBeNull)
                            {
                                return CodeAnnotationNullableValue.NOT_NULL;
                            }
                            return CodeAnnotationNullableValue.CAN_BE_NULL;

                        default: return null;
                    }

                case TypeClassification.VALUE_TYPE: return null;

                default: goto case TypeClassification.REFERENCE_TYPE;
            }
        }

        [Pure]
        static CodeAnnotationNullableValue? TryGetAnnotationItemNullableValue([NotNull] this IType type, [NotNull] ITreeNode context)
        {
            if (type.IsGenericEnumerableOrDescendant() || type.IsGenericArray(context))
            {
                var elementType = CollectionTypeUtil.ElementTypeByCollectionType(type, context, false);
                return elementType?.TryGetAnnotationNullableValue();
            }

            var resultType = type.GetTasklikeUnderlyingType(context);
            if (resultType != null)
            {
                return resultType.TryGetAnnotationNullableValue();
            }

            if (type.IsLazy())
            {
                var typeElement = TypeElementUtil.GetTypeElementByClrName(PredefinedType.LAZY_FQN, context.GetPsiModule());
                var valueType = type.GetGenericUnderlyingType(typeElement);
                return valueType?.TryGetAnnotationNullableValue();
            }

            return null;
        }

        /// <summary>
        /// Returns ReSharper's <c>[NotNull]</c> or <c>[CanBeNull]</c> from the C# annotations.
        /// </summary>
        [Pure]
        [ContractAnnotation("null => null", true)]
        public static CodeAnnotationNullableValue? TryGetReSharperNullableAnnotation(this IDeclaredElement element)
        {
            switch (element)
            {
                case IParameter parameter:
                {
                    var isInputParameter = parameter.Kind == ParameterKind.VALUE || parameter.Kind == ParameterKind.INPUT;
                    var isOutputParameter = parameter.Kind == ParameterKind.OUTPUT;

                    var canStillBeNull =
                        (isInputParameter && parameter.HasAttributeInstance(ClrTypeNames.AllowNullAttribute, false) ||
                            isOutputParameter && parameter.HasAttributeInstance(ClrTypeNames.MaybeNullAttribute, false)) &&
                        parameter.Type.IsNonNullableReferenceType();
                    var shouldNeverBeNull =
                        (isInputParameter && parameter.HasAttributeInstance(ClrTypeNames.DisallowNullAttribute, false) ||
                            isOutputParameter && parameter.HasAttributeInstance(ClrTypeNames.NotNullAttribute, false)) &&
                        parameter.Type.IsNullableReferenceType();

                    return parameter.Type.TryGetAnnotationNullableValue(canStillBeNull, shouldNeverBeNull);
                }

                case IProperty property:
                {
                    var isReadOnlyNonAuto = property.IsReadable && !property.IsWritable && !property.IsAuto;
                    var isWriteOnly = !property.IsReadable && property.IsWritable;

                    var canStillBeNull =
                        (isWriteOnly && property.HasAttributeInstance(ClrTypeNames.AllowNullAttribute, false) ||
                            isReadOnlyNonAuto && property.HasAttributeInstance(ClrTypeNames.MaybeNullAttribute, false)) &&
                        property.Type.IsNonNullableReferenceType();
                    var shouldNeverBeNull =
                        (isWriteOnly && property.HasAttributeInstance(ClrTypeNames.DisallowNullAttribute, false) ||
                            isReadOnlyNonAuto && property.HasAttributeInstance(ClrTypeNames.NotNullAttribute, false)) &&
                        property.Type.IsNullableReferenceType();

                    return property.Type.TryGetAnnotationNullableValue(canStillBeNull, shouldNeverBeNull);
                }

                case ITypeOwner typeOwner: return typeOwner.Type.TryGetAnnotationNullableValue();

                case IFunction function:
                {
                    var canStillBeNull = function.ReturnTypeAttributes.HasAttributeInstance(ClrTypeNames.MaybeNullAttribute, false) &&
                        function.ReturnType.IsNonNullableReferenceType();
                    var shouldNeverBeNull = function.ReturnTypeAttributes.HasAttributeInstance(ClrTypeNames.NotNullAttribute, false) &&
                        function.ReturnType.IsNullableReferenceType();

                    return function.ReturnType.TryGetAnnotationNullableValue(canStillBeNull, shouldNeverBeNull);
                }

                case ILocalFunction localFunction: return localFunction.ReturnType.TryGetAnnotationNullableValue();

                case IDelegate @delegate: return @delegate.InvokeMethod.TryGetReSharperNullableAnnotation();

                default: return null;
            }
        }

        /// <summary>
        /// Returns ReSharper's <c>[NotNull]</c> or <c>[CanBeNull]</c> from the C# annotations.
        /// </summary>
        [Pure]
        public static CodeAnnotationNullableValue? TryGetReSharperNullableAnnotation([NotNull] this IDeclaration declaration)
            => declaration.DeclaredElement.TryGetReSharperNullableAnnotation();

        /// <summary>
        /// Returns ReSharper's <c>[ItemNotNull]</c> or <c>[ItemCanBeNull]</c> from the C# annotations.
        /// </summary>
        [Pure]
        public static CodeAnnotationNullableValue? TryGetReSharperItemNullableAnnotation([NotNull] this IDeclaration declaration)
        {
            switch (declaration.DeclaredElement)
            {
                case ITypeOwner typeOwner: return typeOwner.Type.TryGetAnnotationItemNullableValue(declaration);

                case IFunction function: return function.ReturnType.TryGetAnnotationItemNullableValue(declaration);

                case ILocalFunction localFunction: return localFunction.ReturnType.TryGetAnnotationItemNullableValue(declaration);

                case IDelegate @delegate: return @delegate.InvokeMethod.ReturnType.TryGetAnnotationItemNullableValue(declaration);

                default: return null;
            }
        }
    }
}