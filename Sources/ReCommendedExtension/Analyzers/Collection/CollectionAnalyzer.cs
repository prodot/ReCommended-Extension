using System.Text;
using JetBrains.Metadata.Reader.API;
using JetBrains.ReSharper.Feature.Services.Daemon;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.CSharp;
using JetBrains.ReSharper.Psi.CSharp.Tree;
using JetBrains.ReSharper.Psi.Modules;
using JetBrains.ReSharper.Psi.Resolve;
using JetBrains.ReSharper.Psi.Tree;
using JetBrains.ReSharper.Psi.Util;
using ReCommendedExtension.Extensions;
using ReCommendedExtension.Extensions.MethodFinding;
using MethodSignature = ReCommendedExtension.Extensions.MethodFinding.MethodSignature;

namespace ReCommendedExtension.Analyzers.Collection;

[ElementProblemAnalyzer(
    typeof(ICSharpTreeNode),
    HighlightingTypes =
    [
        typeof(UseEmptyForArrayInitializationWarning),
        typeof(UseTargetTypedCollectionExpressionSuggestion),
        typeof(ArrayWithDefaultValuesInitializationSuggestion),
    ])]
public sealed class CollectionAnalyzer : ElementProblemAnalyzer<ICSharpTreeNode>
{
    [Flags]
    enum ListArguments
    {
        Capacity = 1 << 0,
        Collection = 1 << 1,
    }

    [Flags]
    enum HashSetArguments
    {
        Capacity = 1 << 0,
        Collection = 1 << 1,
        Comparer = 1 << 2,
    }

    [Flags]
    enum DictionaryArguments
    {
        Capacity = 1 << 0,
        Dictionary = 1 << 1,
        Pairs = 1 << 2,
        Comparer = 1 << 3,
    }

    [Pure]
    static bool ArrayEmptyMethodExists(IPsiModule psiModule)
        => PredefinedType.ARRAY_FQN.HasMethod(
            new MethodSignature { Name = nameof(Array.Empty), ParameterTypes = [], GenericParametersCount = 1, IsStatic = true },
            psiModule);

    [Pure]
    static bool HasAccessibleAddMethod(IAccessContext accessContext, ITypeElement typeElement, bool checkBaseClasses = true)
    {
        if (typeElement.IsObjectClass() || typeElement is IInterface)
        {
            return false;
        }

        foreach (var method in typeElement.Methods)
        {
            if (method is { ShortName: "Add", TypeParameters: [] } && AccessUtil.IsSymbolAccessible(method, accessContext))

            {
                return true;
            }
        }

        if (checkBaseClasses)
        {
            return typeElement.GetAllSuperTypeElements().Any(baseClass => HasAccessibleAddMethod(accessContext, baseClass, false));
        }

        // todo: find accessible "Add" extension methods

        return false;
    }

    [Conditional("DEBUG")]
    static void AssertListConstructors(IPsiModule psiModule)
    {
        var listType = PredefinedType.GENERIC_LIST_FQN.TryGetTypeElement(psiModule);
        Debug.Assert(listType is { });

        [Pure]
        static int GetOrder(IType? parameterType)
            => parameterType switch
            {
                _ when parameterType.IsInt() => 0,
                _ when parameterType.IsGenericIEnumerable() => 1,

                _ => -1,
            };

        var constructors =
        (
            from c in listType.Constructors
            where c.AccessibilityDomain.DomainType == AccessibilityDomain.AccessibilityDomainType.PUBLIC
            orderby c.Parameters.Count, GetOrder(c.Parameters is [var parameter, ..] ? parameter.Type : null)
            select c).ToList();

        Debug.Assert(
            constructors is [{ Parameters: [] }, { Parameters: [{ Type: var intParameter }] }, { Parameters: [{ Type: var enumerableParameter }] }]
            && intParameter.IsInt()
            && enumerableParameter.IsGenericIEnumerable());
    }

    [Conditional("DEBUG")]
    static void AssertHashSetConstructors(IPsiModule psiModule)
    {
        var hashSetType = PredefinedType.HASHSET_FQN.TryGetTypeElement(psiModule);
        Debug.Assert(hashSetType is { });

        [Pure]
        static int GetOrder(IType? parameterType)
            => parameterType switch
            {
                _ when parameterType.IsInt() => 0,
                _ when parameterType.IsGenericIEnumerable() => 1,
                _ when parameterType.IsClrType(PredefinedType.GENERIC_IEQUALITY_COMPARER_FQN) => 2,

                _ => -1,
            };

        var constructors =
        (
            from c in hashSetType.Constructors
            where c.AccessibilityDomain.DomainType == AccessibilityDomain.AccessibilityDomainType.PUBLIC
            orderby c.Parameters.Count, GetOrder(c.Parameters is [var parameter, ..] ? parameter.Type : null)
            select c).ToList();

        Debug.Assert(
            constructors is
            [
                { Parameters: [] },
                { Parameters: [{ Type: var intParameter1 }] },
                { Parameters: [{ Type: var enumerableParameter1 }] },
                { Parameters: [{ Type: var comparerParameter1 }] },
                { Parameters: [{ Type: var intParameter2 }, { Type: var comparerParameter2 }] },
                { Parameters: [{ Type: var enumerableParameter2 }, { Type: var comparerParameter3 }] },
            ]
            && intParameter1.IsInt()
            && intParameter2.IsInt()
            && enumerableParameter1.IsGenericIEnumerable()
            && enumerableParameter2.IsGenericIEnumerable()
            && comparerParameter1.IsClrType(PredefinedType.GENERIC_IEQUALITY_COMPARER_FQN)
            && comparerParameter2.IsClrType(PredefinedType.GENERIC_IEQUALITY_COMPARER_FQN)
            && comparerParameter2.IsClrType(PredefinedType.GENERIC_IEQUALITY_COMPARER_FQN)
            && comparerParameter3.IsClrType(PredefinedType.GENERIC_IEQUALITY_COMPARER_FQN));
    }

    [Conditional("DEBUG")]
    static void AssertDictionaryConstructors(IPsiModule psiModule)
    {
        var dictionaryType = PredefinedType.GENERIC_DICTIONARY_FQN.TryGetTypeElement(psiModule);
        Debug.Assert(dictionaryType is { });

        [Pure]
        static int GetOrder(IType? parameterType)
            => parameterType switch
            {
                _ when parameterType.IsInt() => 0,
                _ when parameterType.IsIDictionary() => 1,
                _ when parameterType.IsGenericIEnumerable() => 2,
                _ when parameterType.IsClrType(PredefinedType.GENERIC_IEQUALITY_COMPARER_FQN) => 3,

                _ => -1,
            };

        var constructors =
        (
            from c in dictionaryType.Constructors
            where c.AccessibilityDomain.DomainType == AccessibilityDomain.AccessibilityDomainType.PUBLIC
            orderby c.Parameters.Count, GetOrder(c.Parameters is [var parameter, ..] ? parameter.Type : null)
            select c).ToList();

        Debug.Assert(
            constructors is
            [
                { Parameters: [] },
                { Parameters: [{ Type: var intParameter1 }] },
                { Parameters: [{ Type: var dictionaryParameter1 }] },
                { Parameters: [{ Type: var enumerableParameter1 }] },
                { Parameters: [{ Type: var comparerParameter }] },
                { Parameters: [{ Type: var intParameter2 }, { Type: var comparerParameter2 }] },
                { Parameters: [{ Type: var dictionaryParameter2 }, { Type: var comparerParameter3 }] },
                { Parameters: [{ Type: var enumerableParameter2 }, { Type: var comparerParameter4 }] },
            ]
            && intParameter1.IsInt()
            && intParameter2.IsInt()
            && dictionaryParameter1.IsIDictionary()
            && dictionaryParameter2.IsIDictionary()
            && enumerableParameter1.IsGenericIEnumerable()
            && enumerableParameter2.IsGenericIEnumerable()
            && comparerParameter.IsClrType(PredefinedType.GENERIC_IEQUALITY_COMPARER_FQN)
            && comparerParameter2.IsClrType(PredefinedType.GENERIC_IEQUALITY_COMPARER_FQN)
            && comparerParameter2.IsClrType(PredefinedType.GENERIC_IEQUALITY_COMPARER_FQN)
            && comparerParameter3.IsClrType(PredefinedType.GENERIC_IEQUALITY_COMPARER_FQN)
            && comparerParameter4.IsClrType(PredefinedType.GENERIC_IEQUALITY_COMPARER_FQN));
    }

    [Pure]
    static IReferenceExpression? TryGetMethodReferenceToSetInferredTypeArguments(ICSharpExpression treeNode)
        => treeNode is
            {
                Parent: ICSharpArgument
                {
                    MatchingParameter: { Substitution: var substitution, Element.ContainingParametersOwner: IMethod { TypeParameters: [_, ..] } },
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
    static (IType? collectionItemType, bool isCovariant)? TryGetIfTargetTypedToGenericType(
        ICSharpExpression expression,
        IType itemType,
        IType targetType,
        IClrTypeName desiredClrTypeName)
    {
        var psiModule = expression.GetPsiModule();

        var checkDesiredTypeCovariance = expression is IObjectCreationExpression;

        if (TryConstructType(desiredClrTypeName, [itemType], psiModule) is { } desiredClrType
            && targetType.IsGenericType()
            && TypesUtil.GetTypeArgumentValue(targetType, 0) is { } targetItemType)
        {
            if (itemType.Classify == TypeClassification.REFERENCE_TYPE
                && targetItemType.Classify == TypeClassification.REFERENCE_TYPE
                && (!checkDesiredTypeCovariance || TypesUtil.GetTypeParameters(desiredClrType) is [{ Variance: TypeParameterVariance.OUT }])
                && itemType.IsImplicitlyConvertibleTo(targetItemType, ClrPredefinedTypeConversionRule.INSTANCE))
            {
                if (TypeEqualityComparer.Default.Equals(targetType, TryConstructType(desiredClrTypeName, [targetItemType], psiModule)))
                {
                    return expression is ICreationExpression creationExpression
                        ? WouldItemTypesChange(creationExpression, targetItemType) ? null : (targetItemType, true)
                        : (targetItemType, true);
                }

                return null;
            }

            return TypeEqualityComparer.Default.Equals(targetType, desiredClrType) ? (targetItemType, false) : null;
        }

        return null;
    }

    [Pure]
    static (IType? arrayItemType, bool isCovariant)? TryGetIfTargetTypedToGenericArray(ICSharpExpression expression, IType itemType, IType targetType)
    {
        if (targetType.IsGenericArray(expression) && TypesUtil.GetEnumerableOrArrayElementType(targetType) is { } targetItemType)
        {
            if (itemType.Classify == TypeClassification.REFERENCE_TYPE
                && targetItemType.Classify == TypeClassification.REFERENCE_TYPE
                && itemType.IsImplicitlyConvertibleTo(targetItemType, ClrPredefinedTypeConversionRule.INSTANCE))
            {
                if (TypeEqualityComparer.Default.Equals(targetType, TypeFactory.CreateArrayType(targetItemType, 1)))
                {
                    return expression is ICreationExpression creationExpression
                        ? WouldItemTypesChange(creationExpression, targetItemType) ? null : (targetItemType, true)
                        : (targetItemType, true);
                }

                return null;
            }

            return TypeEqualityComparer.Default.Equals(targetType, TypeFactory.CreateArrayType(itemType, 1)) ? (targetItemType, false) : null;
        }

        return null;
    }

    [Pure]
    static bool WouldItemTypesChange(ICreationExpression creationExpression, IType targetItemType)
    {
        if (creationExpression.Initializer is { InitializerElements: [_, ..] })
        {
            var factory = CSharpElementFactory.GetInstance(creationExpression);

            var arrayCreationExpression = (IArrayCreationExpression)factory.CreateExpression(
                $"new $0[] {{ {string.Join(", ", from item in creationExpression.Initializer.InitializerElements select item.GetText())} }}",
                targetItemType);
            arrayCreationExpression.SetResolveContextForSandBox(creationExpression);

            Debug.Assert(
                creationExpression.Initializer.InitializerElements.Count == arrayCreationExpression.ArrayInitializer.ElementInitializers.Count);

            for (var i = 0; i < creationExpression.Initializer.InitializerElements.Count; i++)
            {
                if (arrayCreationExpression.ArrayInitializer.ElementInitializers[i] is IExpressionInitializer { Value: var newExpression })
                {
                    var oldExpression = creationExpression.Initializer.InitializerElements[i] switch
                    {
                        IExpressionInitializer expressionInitializer => expressionInitializer.Value,
                        ICollectionElementInitializer { Arguments: [{ } argument] } => argument.Value,

                        _ => null,
                    };

                    if (TypeEqualityComparer.Default.Equals(newExpression.Type(), oldExpression?.Type()))
                    {
                        continue;
                    }
                }

                return true; // item types are different or an element initializer if not an IExpressionInitializer
            }
        }

        return false;
    }

    [Pure]
    static string BuildArrayCreationExpressionCode(
        IType arrayElementType,
        [ValueRange(1, int.MaxValue)] int itemCount,
        bool isNullableAnnotationsContextEnabled)
    {
        var builder = new StringBuilder();
        builder.Append("new ");

        Debug.Assert(CSharpLanguage.Instance is { });

        builder.Append(arrayElementType.GetPresentableName(CSharpLanguage.Instance));

        if (builder is [.., not '?'])
        {
            var isNullableReferenceType = isNullableAnnotationsContextEnabled
                && arrayElementType is { Classify: TypeClassification.REFERENCE_TYPE, NullableAnnotation: NullableAnnotation.NotAnnotated };

            if (isNullableReferenceType)
            {
                builder.Append('?');
            }
        }
        else
        {
            // workaround for R# 2020.2

            if (isNullableAnnotationsContextEnabled)
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
        builder.Append(itemCount);
        builder.Append(']');

        return builder.ToString();
    }

    static void AnalyzeArrayInitializer(IHighlightingConsumer consumer, IArrayInitializer arrayInitializer)
    {
        switch (arrayInitializer)
        {
            case { InitializerElements: [_, ..] } when TryGetArrayItemType(arrayInitializer) is { } arrayItemType
                && arrayInitializer.InitializerElements.All(
                    item => item is { FirstChild: ICSharpTreeNode treeNode } && treeNode.IsDefaultValueOf(arrayItemType)):
            {
                // { d, default, default(T) }  ->  new T[n] // where d is the default value for the T

                var arrayCreationExpressionCode = BuildArrayCreationExpressionCode(
                    arrayItemType,
                    arrayInitializer.InitializerElements.Count,
                    arrayInitializer.IsNullableAnnotationsContextEnabled());

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

                if (arrayCreationExpression.TryGetTargetType(true) is { } targetType)
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

                    [Pure]
                    (IType? collectionItemType, bool isCovariant)? TryGetIfTargetTypedTo(IClrTypeName clrTypeName)
                        => TryGetIfTargetTypedToGenericType(arrayCreationExpression, itemType, targetType, clrTypeName);

                    [Pure]
                    (IType? arrayItemType, bool isCovariant)? TryGetIfTargetTypedToArray()
                        => TryGetIfTargetTypedToGenericArray(arrayCreationExpression, itemType, targetType);

                    // target-typed to IEnumerable<T> or IReadOnlyCollection<T> or IReadOnlyList<T>: cases not covered by R#
                    // - arrays passed to a method, which requires setting inferred type arguments
                    // - empty arrays without items ('new T[0]')
                    // - arrays of covariant types when type is specified
                    {
                        if ((TryGetIfTargetTypedTo(PredefinedType.GENERIC_IENUMERABLE_FQN)
                                ?? TryGetIfTargetTypedTo(PredefinedType.GENERIC_IREADONLYCOLLECTION_FQN)
                                ?? TryGetIfTargetTypedTo(PredefinedType.GENERIC_IREADONLYLIST_FQN)) is var (collectionItemType,
                            isCollectionItemTypeCovariant)
                            && (isCollectionItemTypeCovariant && arrayCreationExpression.TypeName is { }
                                || isEmptyArray
                                && (arrayCreationExpression.ArrayInitializer == null
                                    || arrayCreationExpression.Parent is ICSharpArgument && methodReferenceToSetInferredTypeArguments is { }
                                    || isCollectionItemTypeCovariant && arrayCreationExpression.TypeName is null)))
                        {
                            string? covariantTypeName;
                            if (isCollectionItemTypeCovariant)
                            {
                                Debug.Assert(collectionItemType is { });
                                Debug.Assert(CSharpLanguage.Instance is { });

                                covariantTypeName = TypeFactory.CreateArrayType(collectionItemType, 1).GetPresentableName(CSharpLanguage.Instance);
                            }
                            else
                            {
                                covariantTypeName = null;
                            }

                            consumer.AddHighlighting(
                                new UseTargetTypedCollectionExpressionSuggestion(
                                    isEmptyArray
                                        ? covariantTypeName is { }
                                            ? $"Use collection expression ('{covariantTypeName}' will be used)."
                                            : "Use collection expression."
                                        : "Use collection expression (a compiler-synthesized read-only collection will be used).",
                                    isEmptyArray
                                        ? covariantTypeName is { } ? $"'{covariantTypeName}' will be used" : null
                                        : "a compiler-synthesized read-only collection will be used",
                                    arrayCreationExpression,
                                    null,
                                    arrayCreationExpression.ArrayInitializer?.InitializerElements,
                                    methodReferenceToSetInferredTypeArguments));
                        }
                    }

                    // target-typed to ICollection<T> or IList<T>: cases not covered by R#
                    // - arrays passed to a method, which requires setting inferred type arguments
                    // - empty arrays without items ('new T[0]')
                    // - arrays of covariant types when type is specified
                    {
                        if ((TryGetIfTargetTypedTo(PredefinedType.GENERIC_ICOLLECTION_FQN) ?? TryGetIfTargetTypedTo(PredefinedType.GENERIC_ILIST_FQN))
                            is var (collectionItemType, isCollectionItemTypeCovariant)
                            && (isCollectionItemTypeCovariant && arrayCreationExpression.TypeName is { }
                                || isEmptyArray
                                && (arrayCreationExpression.ArrayInitializer == null
                                    || arrayCreationExpression.Parent is ICSharpArgument && methodReferenceToSetInferredTypeArguments is { })))
                        {
                            Debug.Assert(CSharpLanguage.Instance is { });

                            if (collectionItemType is { }
                                && TryConstructType(PredefinedType.GENERIC_LIST_FQN, [collectionItemType], psiModule)
                                    ?.GetPresentableName(CSharpLanguage.Instance) is { } typeName)
                            {
                                consumer.AddHighlighting(
                                    new UseTargetTypedCollectionExpressionSuggestion(
                                        $"Use collection expression ('{typeName}' will be used).",
                                        $"'{typeName}' will be used",
                                        arrayCreationExpression,
                                        null,
                                        arrayCreationExpression.ArrayInitializer?.InitializerElements,
                                        methodReferenceToSetInferredTypeArguments));
                            }
                        }
                    }

                    // target-typed to T[]: cases not covered by R#
                    // - empty arrays passed to a method, which requires setting inferred type arguments
                    // - empty covariant arrays without items ('new T[0]')
                    // - arrays of covariant types when type is specified
                    if (TryGetIfTargetTypedToArray() is var (arrayItemType, isArrayItemTypeCovariant)
                        && (isArrayItemTypeCovariant && arrayCreationExpression.TypeName is { }
                            || isEmptyArray
                            && (arrayCreationExpression.ArrayInitializer == null && isArrayItemTypeCovariant
                                || arrayCreationExpression.Parent is ICSharpArgument && methodReferenceToSetInferredTypeArguments is { })))
                    {
                        string? covariantTypeName;
                        if (isArrayItemTypeCovariant)
                        {
                            Debug.Assert(arrayItemType is { });
                            Debug.Assert(CSharpLanguage.Instance is { });

                            covariantTypeName = TypeFactory.CreateArrayType(arrayItemType, 1).GetPresentableName(CSharpLanguage.Instance);
                        }
                        else
                        {
                            covariantTypeName = null;
                        }

                        consumer.AddHighlighting(
                            new UseTargetTypedCollectionExpressionSuggestion(
                                covariantTypeName is { }
                                    ? $"Use collection expression ('{covariantTypeName}' will be used)."
                                    : "Use collection expression.",
                                covariantTypeName is { } ? $"'{covariantTypeName}' will be used" : null,
                                arrayCreationExpression,
                                null,
                                arrayCreationExpression.ArrayInitializer?.InitializerElements,
                                methodReferenceToSetInferredTypeArguments));
                    }
                }
                else
                {
                    if (arrayCreationExpression is { DimInits: [], ArrayInitializer.InitializerElements: [] }
                        && arrayCreationExpression.GetContainingNode<IAttribute>() == null
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
                && arrayCreationExpression.GetContainingNode<IAttribute>() == null
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
            && objectCreationExpression.TryGetTargetType(true) is { } targetType)
        {
            switch (objectCreationExpression.Type())
            {
                case var type when type.IsGenericList():
                    AnalyzeListCreationExpression(consumer, objectCreationExpression, type, targetType);
                    break;

                case var type when type.IsClrType(PredefinedType.HASHSET_FQN):
                    AnalyzeHashSetCreationExpression(consumer, objectCreationExpression, type, targetType);
                    break;

                case var type when type.IsClrType(PredefinedType.GENERIC_DICTIONARY_FQN):
                    AnalyzeDictionaryCreationExpression(consumer, objectCreationExpression, type, targetType);
                    break;

                case var type when type.GetTypeElement<ITypeElement>() is { } typeElement
                    && typeElement.IsDescendantOf(typeElement.Module.GetPredefinedType().GenericIEnumerable.GetTypeElement())
                    && typeElement.CanInstantiateWithPublicDefaultConstructor():
                    AnalyzeOtherCollectionCreationExpression(
                        consumer,
                        objectCreationExpression,
                        type,
                        HasAccessibleAddMethod(new ElementAccessContext(objectCreationExpression), typeElement),
                        targetType);
                    break;
            }
        }
    }

    static void AnalyzeListCreationExpression(
        IHighlightingConsumer consumer,
        IObjectCreationExpression listCreationExpression,
        IType type,
        IType targetType)
    {
        AssertListConstructors(listCreationExpression.GetPsiModule());

        if (TypesUtil.GetTypeArgumentValue(type, 0) is { } itemType)
        {
            var parameterType = listCreationExpression.Arguments is [{ MatchingParameter.Type: var t }] ? t : null;
            var arguments =
                (parameterType.IsInt()
                    && listCreationExpression.Arguments[0].Expression is { } arg
                    && arg.IsConstantValue()
                    && arg.ConstantValue.IntValue > (listCreationExpression.Initializer?.InitializerElements.Count ?? 0)
                        ? ListArguments.Capacity
                        : 0)
                | (parameterType.IsGenericIEnumerable() ? ListArguments.Collection : 0);

            var isEmptyList = (arguments & ListArguments.Collection) == 0
                && listCreationExpression.Initializer is null or { InitializerElements: [] };

            var methodReferenceToSetInferredTypeArguments =
                isEmptyList ? TryGetMethodReferenceToSetInferredTypeArguments(listCreationExpression) : null;

            [Pure]
            (IType? collectionItemType, bool isCovariant)? TryGetIfTargetTypedTo(IClrTypeName clrTypeName)
                => TryGetIfTargetTypedToGenericType(listCreationExpression, itemType, targetType, clrTypeName);

            // target-typed to IEnumerable<T> or IReadOnlyCollection<T> or IReadOnlyList<T>
            if ((TryGetIfTargetTypedTo(PredefinedType.GENERIC_IENUMERABLE_FQN)
                    ?? TryGetIfTargetTypedTo(PredefinedType.GENERIC_IREADONLYCOLLECTION_FQN)
                    ?? TryGetIfTargetTypedTo(PredefinedType.GENERIC_IREADONLYLIST_FQN)) is var (collectionItemType, _))
            {
                Debug.Assert(collectionItemType is { });
                Debug.Assert(CSharpLanguage.Instance is { });

                var typeName = TypeFactory.CreateArrayType(collectionItemType, 1).GetPresentableName(CSharpLanguage.Instance);

                consumer.AddHighlighting(
                    new UseTargetTypedCollectionExpressionSuggestion(
                        isEmptyList
                            ? $"Use collection expression ('{typeName}' will be used)."
                            : "Use collection expression (a compiler-synthesized read-only collection will be used).",
                        isEmptyList ? $"'{typeName}' will be used" : "a compiler-synthesized read-only collection will be used",
                        listCreationExpression,
                        parameterType.IsGenericIEnumerable() ? listCreationExpression.Arguments[0].Value : null,
                        listCreationExpression.Initializer?.InitializerElements,
                        methodReferenceToSetInferredTypeArguments));
            }

            // target-typed to ICollection<T> or IList<T>
            if ((arguments & ListArguments.Capacity) == 0
                && (TryGetIfTargetTypedTo(PredefinedType.GENERIC_ICOLLECTION_FQN)
                    ?? TryGetIfTargetTypedTo(PredefinedType.GENERIC_ILIST_FQN)) is var (_, _))
            {
                consumer.AddHighlighting(
                    new UseTargetTypedCollectionExpressionSuggestion(
                        "Use collection expression.",
                        null,
                        listCreationExpression,
                        parameterType.IsGenericIEnumerable() ? listCreationExpression.Arguments[0].Value : null,
                        listCreationExpression.Initializer?.InitializerElements,
                        methodReferenceToSetInferredTypeArguments));
            }

            // target-typed to List<T>: cases not covered by R#
            // - empty list without a specified capacity passed to a method, which requires setting inferred type arguments
            if (isEmptyList
                && (arguments & ListArguments.Capacity) == 0
                && methodReferenceToSetInferredTypeArguments is { }
                && TryGetIfTargetTypedTo(PredefinedType.GENERIC_LIST_FQN) is var (_, _))
            {
                consumer.AddHighlighting(
                    new UseTargetTypedCollectionExpressionSuggestion(
                        "Use collection expression.",
                        null,
                        listCreationExpression,
                        null,
                        null,
                        methodReferenceToSetInferredTypeArguments));
            }
        }
    }

    static void AnalyzeHashSetCreationExpression(
        IHighlightingConsumer consumer,
        IObjectCreationExpression hashSetCreationExpression,
        IType type,
        IType targetType)
    {
        AssertHashSetConstructors(hashSetCreationExpression.GetPsiModule());

        if (TypesUtil.GetTypeArgumentValue(type, 0) is { } itemType)
        {
            var parameterTypes = hashSetCreationExpression.Arguments switch
            {
                [{ MatchingParameter.Type: var t }] => [t, null],
                [{ MatchingParameter.Type: var t0 }, { MatchingParameter.Type: var t1 }] => [t0, t1],
                _ => new IType?[2],
            };
            var arguments =
                (parameterTypes[0].IsInt()
                    && hashSetCreationExpression.Arguments[0].Expression is { } arg
                    && arg.IsConstantValue()
                    && arg.ConstantValue.IntValue > (hashSetCreationExpression.Initializer?.InitializerElements.Count ?? 0)
                        ? HashSetArguments.Capacity
                        : 0)
                | (parameterTypes[0].IsGenericIEnumerable() ? HashSetArguments.Collection : 0)
                | (parameterTypes[0].IsClrType(PredefinedType.GENERIC_IEQUALITY_COMPARER_FQN)
                    && hashSetCreationExpression.Arguments[0].Expression is { } a0
                    && !(a0.IsConstantValue() && a0.ConstantValue.IsNull())
                    || parameterTypes[1].IsClrType(PredefinedType.GENERIC_IEQUALITY_COMPARER_FQN)
                    && hashSetCreationExpression.Arguments[1].Expression is { } a1
                    && !(a1.IsConstantValue() && a1.ConstantValue.IsNull())
                        ? HashSetArguments.Comparer
                        : 0);

            var isEmptyHashSet = (arguments & HashSetArguments.Collection) == 0
                && hashSetCreationExpression.Initializer is null or { InitializerElements: [] };

            var methodReferenceToSetInferredTypeArguments =
                isEmptyHashSet ? TryGetMethodReferenceToSetInferredTypeArguments(hashSetCreationExpression) : null;

            [Pure]
            (IType? collectionItemType, bool isCovariant)? TryGetIfTargetTypedTo(IClrTypeName clrTypeName)
                => TryGetIfTargetTypedToGenericType(hashSetCreationExpression, itemType, targetType, clrTypeName);

            // target-typed to IEnumerable<T> or IReadOnlyCollection<T>
            if (isEmptyHashSet
                && (TryGetIfTargetTypedTo(PredefinedType.GENERIC_IENUMERABLE_FQN)
                    ?? TryGetIfTargetTypedTo(PredefinedType.GENERIC_IREADONLYCOLLECTION_FQN)) is var (collectionItemType, _))
            {
                Debug.Assert(collectionItemType is { });
                Debug.Assert(CSharpLanguage.Instance is { });

                var typeName = TypeFactory.CreateArrayType(collectionItemType, 1).GetPresentableName(CSharpLanguage.Instance);

                consumer.AddHighlighting(
                    new UseTargetTypedCollectionExpressionSuggestion(
                        $"Use collection expression ('{typeName}' will be used).",
                        $"'{typeName}' will be used",
                        hashSetCreationExpression,
                        null,
                        null,
                        methodReferenceToSetInferredTypeArguments));
            }

            // target-typed to HashSet<T>: cases not covered by R#
            // - empty hash set without a specified capacity or comparer, or passed to a method, which requires setting inferred type arguments
            if (isEmptyHashSet
                && (arguments & (HashSetArguments.Capacity | HashSetArguments.Comparer)) == 0
                && methodReferenceToSetInferredTypeArguments is { }
                && TryGetIfTargetTypedTo(PredefinedType.HASHSET_FQN) is var (_, _))
            {
                consumer.AddHighlighting(
                    new UseTargetTypedCollectionExpressionSuggestion(
                        "Use collection expression.",
                        null,
                        hashSetCreationExpression,
                        null,
                        null,
                        methodReferenceToSetInferredTypeArguments));
            }
        }
    }

    static void AnalyzeDictionaryCreationExpression(
        IHighlightingConsumer consumer,
        IObjectCreationExpression dictionaryCreationExpression,
        IType type,
        IType targetType)
    {
        var psiModule = dictionaryCreationExpression.GetPsiModule();

        AssertDictionaryConstructors(psiModule);

        if (TypesUtil.GetTypeArgumentValue(type, 0) is { } keyType && TypesUtil.GetTypeArgumentValue(type, 1) is { } valueType)
        {
            var parameterTypes = dictionaryCreationExpression.Arguments switch
            {
                [{ MatchingParameter.Type: var t }] => [t, null],
                [{ MatchingParameter.Type: var t0 }, { MatchingParameter.Type: var t1 }] => [t0, t1],
                _ => new IType?[2],
            };
            var arguments =
                (parameterTypes[0].IsInt()
                    && dictionaryCreationExpression.Arguments[0].Expression is { } arg
                    && arg.IsConstantValue()
                    && arg.ConstantValue.IntValue > (dictionaryCreationExpression.Initializer?.InitializerElements.Count ?? 0)
                        ? DictionaryArguments.Capacity
                        : 0)
                | (parameterTypes[0].IsIDictionary() ? DictionaryArguments.Dictionary : 0)
                | (parameterTypes[0].IsGenericIEnumerable() ? DictionaryArguments.Pairs : 0)
                | (parameterTypes[0].IsClrType(PredefinedType.GENERIC_IEQUALITY_COMPARER_FQN)
                    && dictionaryCreationExpression.Arguments[0].Expression is { } a0
                    && !(a0.IsConstantValue() && a0.ConstantValue.IsNull())
                    || parameterTypes[1].IsClrType(PredefinedType.GENERIC_IEQUALITY_COMPARER_FQN)
                    && dictionaryCreationExpression.Arguments[1].Expression is { } a1
                    && !(a1.IsConstantValue() && a1.ConstantValue.IsNull())
                        ? DictionaryArguments.Comparer
                        : 0);

            var isEmptyDictionary = (arguments & (DictionaryArguments.Dictionary | DictionaryArguments.Pairs)) == 0
                && dictionaryCreationExpression.Initializer is null or { InitializerElements: [] };

            var methodReferenceToSetInferredTypeArguments =
                isEmptyDictionary ? TryGetMethodReferenceToSetInferredTypeArguments(dictionaryCreationExpression) : null;

            var typeArguments = new[] { keyType, valueType };

            [Pure]
            bool IsTargetTypedTo(IClrTypeName clrTypeName)
                => TypeEqualityComparer.Default.Equals(targetType, TryConstructType(clrTypeName, typeArguments, psiModule));

            // target-typed to Dictionary<T>: cases not covered by R#
            // - empty dictionary without a specified capacity or comparer
            if (isEmptyDictionary
                && (arguments & (DictionaryArguments.Capacity | DictionaryArguments.Comparer)) == 0
                && IsTargetTypedTo(PredefinedType.GENERIC_DICTIONARY_FQN))
            {
                consumer.AddHighlighting(
                    new UseTargetTypedCollectionExpressionSuggestion(
                        "Use collection expression.",
                        null,
                        dictionaryCreationExpression,
                        null,
                        null,
                        methodReferenceToSetInferredTypeArguments));
            }
        }
    }

    static void AnalyzeOtherCollectionCreationExpression(
        IHighlightingConsumer consumer,
        IObjectCreationExpression collectionCreationExpression,
        IType type,
        bool hasAccessibleAddMethod,
        IType targetType)
    {
        var isPublicDefaultCtorUsed = collectionCreationExpression is { Arguments: [], Initializer: null or { InitializerElements: [] } };

        var methodReferenceToSetInferredTypeArguments =
            isPublicDefaultCtorUsed ? TryGetMethodReferenceToSetInferredTypeArguments(collectionCreationExpression) : null;

        [Pure]
        bool IsTargetTypedToItsOwnType() => TypeEqualityComparer.Default.Equals(targetType, type);

        // target-typed to its own type: cases not covered by R#
        // - public default constructor used or passed to a method, which requires setting inferred type arguments
        if (isPublicDefaultCtorUsed && IsTargetTypedToItsOwnType() && (!hasAccessibleAddMethod || methodReferenceToSetInferredTypeArguments is { }))
        {
            consumer.AddHighlighting(
                new UseTargetTypedCollectionExpressionSuggestion(
                    "Use collection expression.",
                    null,
                    collectionCreationExpression,
                    null,
                    null,
                    methodReferenceToSetInferredTypeArguments));
        }
    }

    static void AnalyzeCollectionExpression(IHighlightingConsumer consumer, ICollectionExpression collectionExpression)
    {
        if (collectionExpression.CollectionElements is [_, ..])
        {
            var targetTypeInfo = collectionExpression.GetTargetTypeInfo();

            if (targetTypeInfo.ElementType is { } collectionItemType)
            {
                var psiModule = collectionExpression.GetPsiModule();

                var typeArguments = new[] { collectionItemType };

                [Pure]
                bool IsTargetTypedToArray()
                    => TypeEqualityComparer.Default.Equals(targetTypeInfo.TargetType, TypeFactory.CreateArrayType(collectionItemType, 1));

                [Pure]
                bool IsTargetTypedTo(IClrTypeName clrTypeName)
                    => TypeEqualityComparer.Default.Equals(targetTypeInfo.TargetType, TryConstructType(clrTypeName, typeArguments, psiModule));

                if (collectionExpression.CollectionElements.All(
                        item => item is IExpressionElement expressionElement && expressionElement.Expression.IsDefaultValueOf(collectionItemType))
                    && (IsTargetTypedToArray()
                        || IsTargetTypedTo(PredefinedType.GENERIC_IENUMERABLE_FQN)
                        || IsTargetTypedTo(PredefinedType.GENERIC_IREADONLYCOLLECTION_FQN)
                        || IsTargetTypedTo(PredefinedType.GENERIC_IREADONLYLIST_FQN)))
                {
                    var arrayCreationExpressionCode = BuildArrayCreationExpressionCode(
                        collectionItemType,
                        collectionExpression.CollectionElements.Count,
                        collectionExpression.IsNullableAnnotationsContextEnabled());

                    consumer.AddHighlighting(
                        new ArrayWithDefaultValuesInitializationSuggestion(
                            $"Use '{arrayCreationExpressionCode}'.",
                            arrayCreationExpressionCode,
                            collectionExpression));
                }
            }
        }
    }

    static void AnalyzeArrayEmptyInvocation(IHighlightingConsumer consumer, IInvocationExpression arrayEmptyInvocationExpression)
    {
        Debug.Assert(arrayEmptyInvocationExpression.TypeArguments is [_]);

        if (arrayEmptyInvocationExpression.GetCSharpLanguageLevel() >= CSharpLanguageLevel.CSharp120
            && arrayEmptyInvocationExpression.TryGetTargetType(true) is { } targetType)
        {
            var psiModule = arrayEmptyInvocationExpression.GetPsiModule();

            var itemType = arrayEmptyInvocationExpression.TypeArguments[0];

            var methodReferenceToSetInferredTypeArguments = TryGetMethodReferenceToSetInferredTypeArguments(arrayEmptyInvocationExpression);

            [Pure]
            (IType? collectionItemType, bool isCovariant)? TryGetIfTargetTypedTo(IClrTypeName clrTypeName)
                => TryGetIfTargetTypedToGenericType(arrayEmptyInvocationExpression, itemType, targetType, clrTypeName);

            [Pure]
            (IType? arrayItemType, bool isCovariant)? TryGetIfTargetTypedToArray()
                => TryGetIfTargetTypedToGenericArray(arrayEmptyInvocationExpression, itemType, targetType);

            // target-typed to IEnumerable<T> or IReadOnlyCollection<T> or IReadOnlyList<T> - inferred
            if ((TryGetIfTargetTypedTo(PredefinedType.GENERIC_IENUMERABLE_FQN)
                    ?? TryGetIfTargetTypedTo(PredefinedType.GENERIC_IREADONLYCOLLECTION_FQN)
                    ?? TryGetIfTargetTypedTo(PredefinedType.GENERIC_IREADONLYLIST_FQN)) is var (covariantCollectionItemType,
                isCollectionItemTypeCovariant)
                && arrayEmptyInvocationExpression.Parent is ICSharpArgument
                && methodReferenceToSetInferredTypeArguments is { })
            {
                string? covariantTypeName;
                if (isCollectionItemTypeCovariant)
                {
                    Debug.Assert(covariantCollectionItemType is { });
                    Debug.Assert(CSharpLanguage.Instance is { });

                    covariantTypeName =
                        TypeFactory.CreateArrayType(covariantCollectionItemType, 1).GetPresentableName(CSharpLanguage.Instance);
                }
                else
                {
                    covariantTypeName = null;
                }

                consumer.AddHighlighting(
                    new UseTargetTypedCollectionExpressionSuggestion(
                        covariantTypeName is { } ? $"Use collection expression ('{covariantTypeName}' will be used)." : "Use collection expression.",
                        covariantTypeName is { } ? $"'{covariantTypeName}' will be used" : null,
                        arrayEmptyInvocationExpression,
                        null,
                        null,
                        methodReferenceToSetInferredTypeArguments));
            }

            // target-typed to ICollection<T> or IList<T> - inferred, but not covariant
            if ((TryGetIfTargetTypedTo(PredefinedType.GENERIC_ICOLLECTION_FQN) ?? TryGetIfTargetTypedTo(PredefinedType.GENERIC_ILIST_FQN)) is var (
                collectionItemType, _)
                && arrayEmptyInvocationExpression.Parent is ICSharpArgument
                && methodReferenceToSetInferredTypeArguments is { })
            {
                Debug.Assert(CSharpLanguage.Instance is { });

                if (collectionItemType is { }
                    && TryConstructType(PredefinedType.GENERIC_LIST_FQN, [collectionItemType], psiModule)
                        ?.GetPresentableName(CSharpLanguage.Instance) is { } typeName)
                {
                    consumer.AddHighlighting(
                        new UseTargetTypedCollectionExpressionSuggestion(
                            $"Use collection expression ('{typeName}' will be used).",
                            $"'{typeName}' will be used",
                            arrayEmptyInvocationExpression,
                            null,
                            null,
                            methodReferenceToSetInferredTypeArguments));
                }
            }

            // target-typed to T[] - inferred, but not covariant
            if (TryGetIfTargetTypedToArray() is (_, false)
                && arrayEmptyInvocationExpression.Parent is ICSharpArgument
                && methodReferenceToSetInferredTypeArguments is { })
            {
                consumer.AddHighlighting(
                    new UseTargetTypedCollectionExpressionSuggestion(
                        "Use collection expression.",
                        null,
                        arrayEmptyInvocationExpression,
                        null,
                        null,
                        methodReferenceToSetInferredTypeArguments));
            }
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

            case ICollectionExpression collectionExpression:
                AnalyzeCollectionExpression(consumer, collectionExpression);
                break;

            case IInvocationExpression { InvokedExpression: IReferenceExpression { Reference: var reference } } invocationExpression
                when reference.Resolve().DeclaredElement is IMethod
                {
                    ShortName: nameof(Array.Empty),
                    IsStatic: true,
                    AccessibilityDomain.DomainType: AccessibilityDomain.AccessibilityDomainType.PUBLIC,
                    TypeParameters: [_],
                    Parameters: [],
                } method
                && method.ContainingType.IsSystemArray():

                AnalyzeArrayEmptyInvocation(consumer, invocationExpression);
                break;
        }
    }
}