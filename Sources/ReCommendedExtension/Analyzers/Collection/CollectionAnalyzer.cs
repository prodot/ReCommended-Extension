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

    [Conditional("DEBUG")]
    static void AssertListConstructors(IPsiModule psiModule)
    {
        var listType = PredefinedType.GENERIC_LIST_FQN.TryGetTypeElement(psiModule);
        Debug.Assert(listType is { });

        var constructors =
            (from c in listType.Constructors orderby c.Parameters.Count, c.Parameters.FirstOrDefault()?.Type.IsGenericIEnumerable() select c)
            .ToList();

        Debug.Assert(
            constructors is [{ Parameters: [] }, { Parameters: [{ Type: var intParameter }] }, { Parameters: [{ Type: var enumerableParameter }] }]
            && intParameter.IsInt()
            && enumerableParameter.IsGenericIEnumerable());
    }

    [Pure]
    static IType? TryGetTargetType(IExpression expression)
    {
        var targetType = expression.GetImplicitlyConvertedTo();

        return targetType.IsUnknown ? null : targetType;
    }

    [Pure]
    static IReferenceExpression? TryGetMethodReferenceToSetInferredTypeArguments(ICreationExpression treeNode)
        => treeNode is
            {
                Parent: ICSharpArgument
                {
                    MatchingParameter: { Substitution: var substitution, Element.ContainingParametersOwner: IMethod { TypeParameters: not [] } },
                    Invocation: IInvocationExpression { InvokedExpression: IReferenceExpression { TypeArguments: [] } methodReference },
                },
            }
            && !substitution.IsEmpty()
                ? methodReference
                : null;

    [Pure]
    static IType? TryConstructType(IClrTypeName genericTypeName, IType[] typeArguments, IPsiModule psiModule)
        => genericTypeName.TryGetTypeElement(psiModule) is { } typeElement ? TypeFactory.CreateType(typeElement, typeArguments) : null;

    [Pure]
    static IType? TryGetArrayItemType(IArrayInitializer arrayInitializer)
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

    static void AnalyzeArrayInitializer(IHighlightingConsumer consumer, IArrayInitializer arrayInitializer)
    {
        switch (arrayInitializer)
        {
            case { InitializerElements: [_, ..] } when TryGetArrayItemType(arrayInitializer) is { } arrayItemType
                && arrayInitializer.InitializerElements.All(item => item is { FirstChild: { } treeNode } && treeNode.IsDefaultValueOf(arrayItemType)):
            {
                // { d, default, default(T) }  ->  new T[n] // where d is the default value for the T

                var arrayCreationExpressionCode = BuildArrayCreationExpressionCode(arrayInitializer, arrayItemType);

                consumer.AddHighlighting(
                    new ArrayWithDefaultValuesInitializationSuggestion(
                        $"Use '{arrayCreationExpressionCode}'.",
                        arrayCreationExpressionCode,
                        arrayInitializer));
                break;
            }

            case { InitializerElements: [], Parent: ITypeOwnerDeclaration declaration }
                when arrayInitializer.GetCSharpLanguageLevel() < CSharpLanguageLevel.CSharp120
                && declaration.Type.GetScalarType() is { } arrayItemType
                && ArrayEmptyMethodExists(arrayInitializer.GetPsiModule()):
            {
                // T[] variable = { };                ->  T[] variable = Array.Empty<T>();
                // T[] Property { get; } = { };       ->  T[] Property { get; } = Array.Empty<T>();
                // T[] Property { get; set; } = { };  ->  T[] Property { get; set; } = Array.Empty<T>();

                Debug.Assert(CSharpLanguage.Instance is { });

                consumer.AddHighlighting(
                    new UseEmptyForArrayInitializationWarning(
                        $"Use '{nameof(Array)}.{nameof(Array.Empty)}<{arrayItemType.GetPresentableName(CSharpLanguage.Instance)}>()'.",
                        arrayInitializer,
                        arrayItemType));
                break;
            }
        }
    }

    static void AnalyzeArrayCreationExpression(IHighlightingConsumer consumer, IArrayCreationExpression arrayCreationExpression)
    {
        Debug.Assert(arrayCreationExpression.Dimensions is [1]);

        var psiModule = arrayCreationExpression.GetPsiModule();

        if (arrayCreationExpression.GetCSharpLanguageLevel() >= CSharpLanguageLevel.CSharp120)
        {
            if (arrayCreationExpression.DimInits is []
                || arrayCreationExpression.DimInits is [{ ConstantValue: { Kind: ConstantValueKind.Int, IntValue: var count } }]
                && count == (arrayCreationExpression.ArrayInitializer?.InitializerElements.Count ?? 0))
            {
                var itemType = arrayCreationExpression.GetElementType();

                if (TryGetTargetType(arrayCreationExpression) is { } targetType)
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
                        ? TryGetMethodReferenceToSetInferredTypeArguments(arrayCreationExpression)
                        : null;

                    var typeArguments = new[] { itemType };

                    [Pure]
                    IClrTypeName? TryGetClrTypeNameIfTargetTypedToAnyOf(params IClrTypeName[] clrTypeNames)
                        => clrTypeNames.FirstOrDefault(
                            clrTypeName => TypeEqualityComparer.Default.Equals(targetType, TryConstructType(clrTypeName, typeArguments, psiModule)));

                    [Pure]
                    bool IsTargetTypedToArray() => TypeEqualityComparer.Default.Equals(targetType, TypeFactory.CreateArrayType(itemType, 1));

                    // target-typed to IEnumerable<T> or IReadOnlyCollection<T> or IReadOnlyList<T>
                    if (TryGetClrTypeNameIfTargetTypedToAnyOf(
                            PredefinedType.GENERIC_IENUMERABLE_FQN,
                            PredefinedType.GENERIC_IREADONLYCOLLECTION_FQN,
                            PredefinedType.GENERIC_IREADONLYLIST_FQN) is { })
                    {
                        consumer.AddHighlighting(
                            new UseTargetTypedCollectionExpressionSuggestion(
                                isEmptyArray
                                    ? "Use collection expression."
                                    : "Use collection expression (a compiler-synthesized read-only collection will be used).",
                                arrayCreationExpression,
                                null,
                                arrayCreationExpression.ArrayInitializer?.InitializerElements,
                                methodReferenceToSetInferredTypeArguments));
                    }

                    // target-typed to ICollection<T> or IList<T>
                    if (TryGetClrTypeNameIfTargetTypedToAnyOf(PredefinedType.GENERIC_ICOLLECTION_FQN, PredefinedType.GENERIC_ILIST_FQN) is
                        { } targetTypeClrTypeName)
                    {
                        // get target-typed item type to preserve the nullability
                        var collectionItemType = targetType.GetGenericUnderlyingType(targetTypeClrTypeName.TryGetTypeElement(psiModule));

                        Debug.Assert(collectionItemType is { });
                        Debug.Assert(CSharpLanguage.Instance is { });

                        var typeName = TryConstructType(PredefinedType.GENERIC_LIST_FQN, [collectionItemType], psiModule)
                            ?.GetPresentableName(CSharpLanguage.Instance);

                        consumer.AddHighlighting(
                            new UseTargetTypedCollectionExpressionSuggestion(
                                $"Use collection expression ('{typeName}' will be used).",
                                arrayCreationExpression,
                                null,
                                arrayCreationExpression.ArrayInitializer?.InitializerElements,
                                methodReferenceToSetInferredTypeArguments));
                    }

                    // target-typed to T[]: cases not covered by R#
                    // - empty arrays passed to a method, which requires setting inferred type arguments
                    // - empty arrays without items ('new T[0]')
                    if (isEmptyArray
                        && IsTargetTypedToArray()
                        && (arrayCreationExpression.ArrayInitializer is not { }
                            || arrayCreationExpression.Parent is ICSharpArgument && methodReferenceToSetInferredTypeArguments is { }))
                    {
                        consumer.AddHighlighting(
                            new UseTargetTypedCollectionExpressionSuggestion(
                                "Use collection expression.",
                                arrayCreationExpression,
                                null,
                                null,
                                methodReferenceToSetInferredTypeArguments));
                    }
                }
                else
                {
                    if (arrayCreationExpression is { DimInits: [], ArrayInitializer.InitializerElements: [] }
                        && arrayCreationExpression.GetContainingNode<IAttribute>() is not { }
                        && ArrayEmptyMethodExists(psiModule))
                    {
                        // new T[] { }  ->  Array.Empty<T>();

                        Debug.Assert(CSharpLanguage.Instance is { });

                        consumer.AddHighlighting(
                            new UseEmptyForArrayInitializationWarning(
                                $"Use '{nameof(Array)}.{nameof(Array.Empty)}<{itemType.GetPresentableName(CSharpLanguage.Instance)}>()'.",
                                arrayCreationExpression,
                                itemType));
                    }
                }
            }
        }
        else
        {
            if (arrayCreationExpression is { DimInits: [], ArrayInitializer.InitializerElements: [] }
                && arrayCreationExpression.GetContainingNode<IAttribute>() is not { }
                && ArrayEmptyMethodExists(psiModule))
            {
                // new T[] { }  ->  Array.Empty<T>();

                var arrayItemType = arrayCreationExpression.GetElementType();

                Debug.Assert(CSharpLanguage.Instance is { });

                consumer.AddHighlighting(
                    new UseEmptyForArrayInitializationWarning(
                        $"Use '{nameof(Array)}.{nameof(Array.Empty)}<{arrayItemType.GetPresentableName(CSharpLanguage.Instance)}>()'.",
                        arrayCreationExpression,
                        arrayItemType));
            }
        }
    }

    static void AnalyzeObjectCreationExpression(IHighlightingConsumer consumer, IObjectCreationExpression objectCreationExpression)
    {
        if (objectCreationExpression.GetCSharpLanguageLevel() >= CSharpLanguageLevel.CSharp120
            && TryGetTargetType(objectCreationExpression) is { } targetType)
        {
            var type = objectCreationExpression.Type();

            if (type.IsGenericList())
            {
                AnalyzeListCreationExpression(consumer, objectCreationExpression, type, targetType);
            }
        }
    }

    static void AnalyzeListCreationExpression(
        IHighlightingConsumer consumer,
        IObjectCreationExpression listCreationExpression,
        IType type,
        IType targetType)
    {
        var psiModule = listCreationExpression.GetPsiModule();

        AssertListConstructors(psiModule);

        var itemType = type.GetGenericUnderlyingType(PredefinedType.GENERIC_LIST_FQN.TryGetTypeElement(psiModule));
        Debug.Assert(itemType is { });

        var parameterType = listCreationExpression.Arguments.FirstOrDefault()?.MatchingParameter?.Type;

        var isEmptyList = (parameterType is not { } || !parameterType.IsGenericIEnumerable())
            && listCreationExpression.Initializer is not { InitializerElements: [_, ..] };
        var isCapacitySpecified = parameterType.IsInt()
            && listCreationExpression.Arguments[0].Expression is { } arg
            && arg.IsConstantValue()
            && arg.ConstantValue.IntValue > (listCreationExpression.Initializer?.InitializerElements.Count ?? 0);

        var methodReferenceToSetInferredTypeArguments = isEmptyList ? TryGetMethodReferenceToSetInferredTypeArguments(listCreationExpression) : null;

        var typeArguments = new[] { itemType };

        [Pure]
        bool IsTargetTypedTo(IClrTypeName clrTypeName)
            => TypeEqualityComparer.Default.Equals(targetType, TryConstructType(clrTypeName, typeArguments, psiModule));

        [Pure]
        IClrTypeName? TryGetClrTypeNameIfTargetTypedToAnyOf(params IClrTypeName[] clrTypeNames)
            => clrTypeNames.FirstOrDefault(
                clrTypeName => TypeEqualityComparer.Default.Equals(targetType, TryConstructType(clrTypeName, typeArguments, psiModule)));

        // target-typed to IEnumerable<T> or IReadOnlyCollection<T> or IReadOnlyList<T>
        if (TryGetClrTypeNameIfTargetTypedToAnyOf(
                PredefinedType.GENERIC_IENUMERABLE_FQN,
                PredefinedType.GENERIC_IREADONLYCOLLECTION_FQN,
                PredefinedType.GENERIC_IREADONLYLIST_FQN) is { } targetTypeClrTypeName)
        {
            // get target-typed item type to preserve the nullability
            var arrayItemType = targetType.GetGenericUnderlyingType(targetTypeClrTypeName.TryGetTypeElement(psiModule));

            Debug.Assert(arrayItemType is { });
            Debug.Assert(CSharpLanguage.Instance is { });

            var typeName = TypeFactory.CreateArrayType(arrayItemType, 1).GetPresentableName(CSharpLanguage.Instance);

            consumer.AddHighlighting(
                new UseTargetTypedCollectionExpressionSuggestion(
                    isEmptyList
                        ? $"Use collection expression ('{typeName}' will be used)."
                        : "Use collection expression (a compiler-synthesized read-only collection will be used).",
                    listCreationExpression,
                    parameterType.IsGenericIEnumerable() ? listCreationExpression.Arguments[0].Value : null,
                    listCreationExpression.Initializer?.InitializerElements,
                    methodReferenceToSetInferredTypeArguments));
        }

        // target-typed to ICollection<T> or IList<T>
        if (TryGetClrTypeNameIfTargetTypedToAnyOf(PredefinedType.GENERIC_ICOLLECTION_FQN, PredefinedType.GENERIC_ILIST_FQN) is { })
        {
            consumer.AddHighlighting(
                new UseTargetTypedCollectionExpressionSuggestion(
                    "Use collection expression.",
                    listCreationExpression,
                    parameterType.IsGenericIEnumerable() ? listCreationExpression.Arguments[0].Value : null,
                    listCreationExpression.Initializer?.InitializerElements,
                    methodReferenceToSetInferredTypeArguments));
        }

        // target-typed to List<T>: cases not covered by R#
        // - empty list without a specified capacity passed to a method, which requires setting inferred type arguments
        if (isEmptyList
            && !isCapacitySpecified
            && methodReferenceToSetInferredTypeArguments is { }
            && IsTargetTypedTo(PredefinedType.GENERIC_LIST_FQN))
        {
            consumer.AddHighlighting(
                new UseTargetTypedCollectionExpressionSuggestion(
                    "Use collection expression.",
                    listCreationExpression,
                    null,
                    null,
                    methodReferenceToSetInferredTypeArguments));
        }
    }

    protected override void Run(ICSharpTreeNode element, ElementProblemAnalyzerData data, IHighlightingConsumer consumer)
    {
        switch (element)
        {
            case IArrayInitializer arrayInitializer:
                AnalyzeArrayInitializer(consumer, arrayInitializer);
                break;

            case IArrayCreationExpression { Dimensions: [1] } arrayCreationExpression:
                AnalyzeArrayCreationExpression(consumer, arrayCreationExpression);
                break;

            case IObjectCreationExpression objectCreationExpression:
                AnalyzeObjectCreationExpression(consumer, objectCreationExpression);
                break;

            // todo: target-typed empty dictionary: new Dictionary<K,V>() -> []
            // todo: target-typed empty dictionary: new() -> []

            // todo: target-typed empty array: Array.Empty<T>() -> [] (when target type is: T[], IEnumerable<T>, IReadOnlyCollection<T>, IReadOnlyList<T>)
            // todo: target-typed empty array: Array.Empty<T>() -> [] (List<T> will be used) (when target type is: ICollection<T>, IList<T>)
        }
    }
}