using System.Text;
using JetBrains.Metadata.Reader.API;
using JetBrains.ReSharper.Feature.Services.Daemon;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.CSharp;
using JetBrains.ReSharper.Psi.CSharp.Tree;
using JetBrains.ReSharper.Psi.Modules;
using JetBrains.ReSharper.Psi.Tree;
using JetBrains.ReSharper.Psi.Util;

namespace ReCommendedExtension.Analyzers.Collection;

[ElementProblemAnalyzer(
    typeof(ICSharpTreeNode),
    HighlightingTypes =
    [
        typeof(UseEmptyForArrayInitializationWarning), typeof(UseTargetTypedCollectionExpressionSuggestion),
        typeof(ArrayWithDefaultValuesInitializationSuggestion),
    ])]
public sealed class CollectionAnalyzer : ElementProblemAnalyzer<ICSharpTreeNode>
{
    [Pure]
    static bool ArrayEmptyMethodExists(IPsiModule psiModule)
        => PredefinedType.ARRAY_FQN.TryGetTypeElement(psiModule) is { } arrayType
            && arrayType.Methods.Any(
                method => method is
                {
                    ShortName: nameof(Array.Empty),
                    IsStatic: true,
                    AccessibilityDomain.DomainType: AccessibilityDomain.AccessibilityDomainType.PUBLIC,
                    TypeParameters: [_],
                    Parameters: [],
                });

    [Pure]
    static IType? TryGetTargetType(IArrayCreationExpression arrayCreationExpression)
    {
        var targetType = arrayCreationExpression.GetImplicitlyConvertedTo();

        return targetType.IsUnknown ? null : targetType;
    }

    [Pure]
    static bool IsTargetTyped(IArrayCreationExpression arrayCreationExpression, [NotNullWhen(true)] IType? targetType)
        => TypeEqualityComparer.Default.Equals(TryGetTargetType(arrayCreationExpression), targetType);

    [Pure]
    static IType? TryConstructType(IClrTypeName genericTypeName, IType[] typeArguments, IPsiModule psiModule)
        => genericTypeName.TryGetTypeElement(psiModule) is { } tryGetTypeElement ? TypeFactory.CreateType(tryGetTypeElement, typeArguments) : null;

    [Pure]
    static IType? TryGetArrayElementType(IArrayInitializer arrayInitializer)
        => arrayInitializer.Parent switch
        {
            ITypeOwnerDeclaration declaration when declaration.Type.GetScalarType() is { } type => type,
            IArrayCreationExpression arrayCreationExpression => arrayCreationExpression.GetElementType(),

            _ => null,
        };

    [Pure]
    static string BuildArrayCreationExpressionCode(IArrayInitializer arrayInitializer, IType arrayElementType)
    {
        Debug.Assert(arrayInitializer.InitializerElements is [_, ..]);

        var builder = new StringBuilder();
        builder.Append("new ");

        Debug.Assert(CSharpLanguage.Instance is { });

        builder.Append(arrayElementType.GetPresentableName(CSharpLanguage.Instance));

        if (builder is [.., not '?'])
        {
            var isNullableReferenceType = arrayInitializer.IsNullableAnnotationsContextEnabled()
                && arrayElementType is { Classify: TypeClassification.REFERENCE_TYPE, NullableAnnotation: NullableAnnotation.NotAnnotated };

            if (isNullableReferenceType)
            {
                builder.Append('?');
            }
        }
        else
        {
            // workaround for R# 2020.2

            if (arrayInitializer.IsNullableAnnotationsContextEnabled())
            {
                switch (arrayElementType.Classify)
                {
                    case TypeClassification.UNKNOWN:
                    case TypeClassification.VALUE_TYPE when !arrayElementType.IsNullable():
                        builder.Remove(builder.Length - 1, 1);
                        break;
                }
            }
        }

        builder.Append('[');
        builder.Append(arrayInitializer.InitializerElements.Count);
        builder.Append(']');

        return builder.ToString();
    }

    protected override void Run(ICSharpTreeNode element, ElementProblemAnalyzerData data, IHighlightingConsumer consumer)
    {
        if (element.GetCSharpLanguageLevel() >= CSharpLanguageLevel.CSharp120)
        {
            // covered by R#: (target-typed)                      T[] variable = { };                ->  T[] variable = [];
            // covered by R#: (target-typed)                      T[] Property { get; } = { };       ->  T[] Property { get; } = [];
            // covered by R#: (target-typed)                      T[] Property { get; set; } = { };  ->  T[] Property { get; set; } = [];
            // covered by R# (target-typed and non-target-typed): new T[0] { }                       ->  []
            // covered by R# (non-target-typed):                  new T[0]                           ->  []
            // NOT covered by R# (target-typed):                  new T[0]                           ->  []

            switch (element)
            {
                case IArrayInitializer { InitializerElements: [_, ..] } arrayInitializer
                    when TryGetArrayElementType(arrayInitializer) is { } arrayElementType
                    && arrayInitializer.InitializerElements.All(
                        item => item is { FirstChild: { } firstChild } && firstChild.IsDefaultValueOf(arrayElementType)):
                {
                    // { d, default, default(T) }  ->  new T[n] // where d is the default value for the T

                    var arrayCreationExpressionCode = BuildArrayCreationExpressionCode(arrayInitializer, arrayElementType);

                    consumer.AddHighlighting(
                        new ArrayWithDefaultValuesInitializationSuggestion(
                            $"Use '{arrayCreationExpressionCode}'.",
                            arrayCreationExpressionCode,
                            arrayInitializer));
                    break;
                }

                case IArrayCreationExpression { Dimensions: [1], DimInits: [], ArrayInitializer.InitializerElements: [] } creationExpression
                    when creationExpression.GetContainingNode<IAttribute>() is not { }
                    && TryGetTargetType(creationExpression) is not { }
                    && ArrayEmptyMethodExists(element.GetPsiModule()):
                {
                    // new T[] { }  ->  Array.Empty<T>();

                    var arrayElementType = creationExpression.GetElementType();

                    Debug.Assert(CSharpLanguage.Instance is { });

                    consumer.AddHighlighting(
                        new UseEmptyForArrayInitializationWarning(
                            $"Use '{nameof(Array)}.{nameof(Array.Empty)}<{arrayElementType.GetPresentableName(CSharpLanguage.Instance)}>()'.",
                            creationExpression,
                            arrayElementType));
                    break;
                }

                case IArrayCreationExpression { Dimensions: [1] } arrayCreationExpression when arrayCreationExpression.DimInits is []
                    || arrayCreationExpression.DimInits is [{ ConstantValue: { Kind: ConstantValueKind.Int, IntValue: var count } }]
                    && count == (arrayCreationExpression.ArrayInitializer?.InitializerElements.Count ?? 0):
                {
                    // new T[] { }      ->  []
                    // new T[] { ... }  ->  [...]
                    // new T[0] { }     ->  []
                    // new T[n] { ... } ->  [...]
                    // new T[0]         ->  []
                    // new[] { ... }    ->  [...]

                    var isEmptyArray = arrayCreationExpression is { DimInits: [{ ConstantValue: { Kind: ConstantValueKind.Int, IntValue: 0 } }] }
                        or { DimInits: [], ArrayInitializer.InitializerElements: [] };

                    var methodReferenceToSetInferredTypeArguments = isEmptyArray
                        && arrayCreationExpression is
                        {
                            Parent: ICSharpArgument
                            {
                                MatchingParameter:
                                {
                                    Substitution: var substitution, Element.ContainingParametersOwner: IMethod { TypeParameters: not [] },
                                },
                                Invocation: IInvocationExpression
                                {
                                    InvokedExpression: IReferenceExpression { TypeArguments: [] } methodReference,
                                },
                            },
                        }
                        && !substitution.IsEmpty()
                            ? methodReference
                            : null;

                    var psiModule = arrayCreationExpression.GetPsiModule();
                    var typeArguments = new[] { arrayCreationExpression.GetElementType() };

                    // target-typed to IEnumerable<T> or IReadOnlyCollection<T> or IReadOnlyList<T>
                    if (IsTargetTyped(arrayCreationExpression, TryConstructType(PredefinedType.GENERIC_IENUMERABLE_FQN, typeArguments, psiModule))
                        || IsTargetTyped(
                            arrayCreationExpression,
                            TryConstructType(PredefinedType.GENERIC_IREADONLYCOLLECTION_FQN, typeArguments, psiModule))
                        || IsTargetTyped(
                            arrayCreationExpression,
                            TryConstructType(PredefinedType.GENERIC_IREADONLYLIST_FQN, typeArguments, psiModule)))
                    {
                        consumer.AddHighlighting(
                            new UseTargetTypedCollectionExpressionSuggestion(
                                isEmptyArray
                                    ? "Use collection expression."
                                    : "Use collection expression (a compiler-synthesized read-only collection will be used).",
                                arrayCreationExpression,
                                methodReferenceToSetInferredTypeArguments));
                    }

                    // target-typed to ICollection<T>
                    if (IsTargetTyped(arrayCreationExpression, TryConstructType(PredefinedType.GENERIC_ICOLLECTION_FQN, typeArguments, psiModule)))
                    {
                        // get target-typed item type to preserve the nullability
                        var listItemType = TryGetTargetType(arrayCreationExpression)
                            .GetGenericUnderlyingType(PredefinedType.GENERIC_ICOLLECTION_FQN.TryGetTypeElement(psiModule));

                        Debug.Assert(listItemType is { });
                        Debug.Assert(CSharpLanguage.Instance is { });

                        var type = TryConstructType(PredefinedType.GENERIC_LIST_FQN, [listItemType], psiModule)
                            ?.GetPresentableName(CSharpLanguage.Instance);

                        consumer.AddHighlighting(
                            new UseTargetTypedCollectionExpressionSuggestion(
                                $"Use collection expression ({type} will be used).",
                                arrayCreationExpression,
                                methodReferenceToSetInferredTypeArguments));
                    }

                    // target-typed to IList<T>
                    if (IsTargetTyped(arrayCreationExpression, TryConstructType(PredefinedType.GENERIC_ILIST_FQN, typeArguments, psiModule)))
                    {
                        // get target-typed item type to preserve the nullability
                        var listItemType = TryGetTargetType(arrayCreationExpression)
                            .GetGenericUnderlyingType(PredefinedType.GENERIC_ILIST_FQN.TryGetTypeElement(psiModule));

                        Debug.Assert(listItemType is { });
                        Debug.Assert(CSharpLanguage.Instance is { });

                        var type = TryConstructType(PredefinedType.GENERIC_LIST_FQN, [listItemType], psiModule)
                            ?.GetPresentableName(CSharpLanguage.Instance);

                        consumer.AddHighlighting(
                            new UseTargetTypedCollectionExpressionSuggestion(
                                $"Use collection expression ({type} will be used).",
                                arrayCreationExpression,
                                methodReferenceToSetInferredTypeArguments));
                    }

                    // target-typed to T[]: cases not covered by R#
                    // - empty arrays passed to a method, which requires setting inferred type arguments
                    // - empty arrays without items ('new T[0]')
                    if (isEmptyArray
                        && IsTargetTyped(arrayCreationExpression, TypeFactory.CreateArrayType(arrayCreationExpression.GetElementType(), 1))
                        && (arrayCreationExpression.ArrayInitializer is not { }
                            || arrayCreationExpression.Parent is ICSharpArgument && methodReferenceToSetInferredTypeArguments is { }))
                    {
                        consumer.AddHighlighting(
                            new UseTargetTypedCollectionExpressionSuggestion(
                                "Use collection expression.",
                                arrayCreationExpression,
                                methodReferenceToSetInferredTypeArguments));
                    }

                    break;
                }

                // todo: target-typed empty dictionary: new Dictionary<K,V>() -> []
                // todo: target-typed empty dictionary: new() -> []

                // todo: target-typed empty array: Array.Empty<T>() -> [] (when target type is: T[], IEnumerable<T>, IReadOnlyCollection<T>, IReadOnlyList<T>)
                // todo: target-typed empty array: Array.Empty<T>() -> [] (List<T> will be used) (when target type is: ICollection<T>, IList<T>)
            }
        }
        else
        {
            // covered by R#:     new T[0] { }                       ->  Array.Empty<T>()
            // NOT covered by R#: T[] variable = { };                ->  T[] variable = Array.Empty<T>();
            // NOT covered by R#: T[] Property { get; } = { };       ->  T[] Property { get; } = Array.Empty<T>();
            // NOT covered by R#: T[] Property { get; set; } = { };  ->  T[] Property { get; set; } = Array.Empty<T>();

            switch (element)
            {
                case IArrayInitializer { InitializerElements: [], Parent: ITypeOwnerDeclaration declaration } arrayInitializer
                    when declaration.Type.GetScalarType() is { } arrayElementType && ArrayEmptyMethodExists(element.GetPsiModule()):
                {
                    // T[] variable = { };                ->  T[] variable = Array.Empty<T>();
                    // T[] Property { get; } = { };       ->  T[] Property { get; } = Array.Empty<T>();
                    // T[] Property { get; set; } = { };  ->  T[] Property { get; set; } = Array.Empty<T>();

                    Debug.Assert(CSharpLanguage.Instance is { });

                    consumer.AddHighlighting(
                        new UseEmptyForArrayInitializationWarning(
                            $"Use '{nameof(Array)}.{nameof(Array.Empty)}<{arrayElementType.GetPresentableName(CSharpLanguage.Instance)}>()'.",
                            arrayInitializer,
                            arrayElementType));
                    break;
                }

                case IArrayInitializer { InitializerElements: [_, ..] } arrayInitializer
                    when TryGetArrayElementType(arrayInitializer) is { } arrayElementType
                    && arrayInitializer.InitializerElements.All(
                        item => item is { FirstChild: { } firstChild } && firstChild.IsDefaultValueOf(arrayElementType)):
                {
                    // { d, default, default(T) }  ->  new T[n] // where d is the default value for the T

                    var arrayCreationExpressionCode = BuildArrayCreationExpressionCode(arrayInitializer, arrayElementType);

                    consumer.AddHighlighting(
                        new ArrayWithDefaultValuesInitializationSuggestion(
                            $"Use '{arrayCreationExpressionCode}'.",
                            arrayCreationExpressionCode,
                            arrayInitializer));
                    break;
                }

                case IArrayCreationExpression { Dimensions: [1], DimInits: [], ArrayInitializer.InitializerElements: [], } creationExpression
                    when creationExpression.GetContainingNode<IAttribute>() is not { } && ArrayEmptyMethodExists(element.GetPsiModule()):
                {
                    // new T[] { }  ->  Array.Empty<T>();

                    var arrayElementType = creationExpression.GetElementType();

                    Debug.Assert(CSharpLanguage.Instance is { });

                    consumer.AddHighlighting(
                        new UseEmptyForArrayInitializationWarning(
                            $"Use '{nameof(Array)}.{nameof(Array.Empty)}<{arrayElementType.GetPresentableName(CSharpLanguage.Instance)}>()'.",
                            creationExpression,
                            arrayElementType));
                    break;
                }
            }
        }
    }
}