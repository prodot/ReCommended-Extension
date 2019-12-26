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
        static CodeAnnotationNullableValue? TryGetAnnotationNullableValue([NotNull] this IType type)
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
                                        return CodeAnnotationNullableValue.NOT_NULL;

                                    case TypeParameterNullability.NullableValueOrReferenceType:
                                    case TypeParameterNullability.NullableReferenceType:
                                    case TypeParameterNullability.NullableSuperType:
                                        return CodeAnnotationNullableValue.CAN_BE_NULL;

                                    default: return null;
                                }
                            }

                            return CodeAnnotationNullableValue.NOT_NULL;

                        case NullableAnnotation.NotNullable: return CodeAnnotationNullableValue.NOT_NULL;

                        case NullableAnnotation.Annotated:
                        case NullableAnnotation.Nullable:
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
                case ITypeOwner typeOwner: return typeOwner.Type.TryGetAnnotationNullableValue();

                case IFunction function: return function.ReturnType.TryGetAnnotationNullableValue();

                case ILocalFunction localFunction: return localFunction.ReturnType.TryGetAnnotationNullableValue();

                case IDelegate @delegate: return @delegate.InvokeMethod.ReturnType.TryGetAnnotationNullableValue();

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