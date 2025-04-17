using JetBrains.ReSharper.Feature.Services.Daemon;
using JetBrains.ReSharper.Feature.Services.Util;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.CSharp;
using JetBrains.ReSharper.Psi.CSharp.Impl.ControlFlow.NullableAnalysis.Runner;
using JetBrains.ReSharper.Psi.CSharp.Tree;
using JetBrains.ReSharper.Psi.Resolve;
using JetBrains.ReSharper.Psi.Tree;
using JetBrains.ReSharper.Psi.Util;
using ReCommendedExtension.Extensions;

namespace ReCommendedExtension.Analyzers.Linq;

/// <remarks>
/// C# language version checks are only done when a quick fix would require it.
/// </remarks>
[ElementProblemAnalyzer(
    typeof(IInvocationExpression),
    HighlightingTypes =
    [
        typeof(UseIndexerSuggestion),
        typeof(UseLinqListPatternSuggestion),
        typeof(UseSwitchExpressionSuggestion),
        typeof(UseCollectionPropertySuggestion),
        typeof(SuspiciousElementAccessWarning),
    ])]
public sealed class LinqAnalyzer(NullableReferenceTypesDataFlowAnalysisRunSynchronizer nullableReferenceTypesDataFlowAnalysisRunSynchronizer)
    : ElementProblemAnalyzer<IInvocationExpression>
{
    [Pure]
    static IEnumerable<IProperty> GetAllProperties(ITypeElement typeElement)
        => typeElement.Properties.Concat(
            from baseTypeElement in typeElement.GetAllSuperTypeElements() from property in baseTypeElement.Properties select property);

    [Pure]
    static IEnumerable<IProperty> GetIndexers(IEnumerable<IProperty> properties)
        => from property in properties where property.Parameters is [{ Type: var parameterType }] && parameterType.IsInt() select property;

    [Pure]
    static bool IsCollection(IType type, ITreeNode context)
    {
        var psiModule = context.GetPsiModule();

        return type.IsGenericICollection()
            || type.IsGenericIReadOnlyCollection()
            || type.GetTypeElement() is { } typeElement
            && (typeElement.IsDescendantOf(PredefinedType.GENERIC_ICOLLECTION_FQN.TryGetTypeElement(psiModule))
                || typeElement.IsDescendantOf(PredefinedType.GENERIC_IREADONLYCOLLECTION_FQN.TryGetTypeElement(psiModule)));
    }

    [Pure]
    static bool IsIndexableCollectionOrString(IType type, ITreeNode context)
    {
        if (type.IsGenericIList() || type.IsGenericIReadOnlyList() || type.IsGenericList() || type.IsGenericArray(context) || type.IsString())
        {
            return true;
        }

        if (type.GetTypeElement() is { } typeElement)
        {
            var psiModule = context.GetPsiModule();

            return typeElement.IsDescendantOf(PredefinedType.GENERIC_ILIST_FQN.TryGetTypeElement(psiModule))
                || typeElement.IsDescendantOf(ClrTypeNames.IReadOnlyList.TryGetTypeElement(psiModule));
        }

        return false;
    }

    [Pure]
    static bool IsIndexableCollectionOrString(IType type, ICSharpExpression expression, out bool hasAccessibleIndexer)
    {
        if (type.IsGenericIList() || type.IsGenericIReadOnlyList() || type.IsGenericList() || type.IsGenericArray(expression) || type.IsString())
        {
            hasAccessibleIndexer = true;
            return true;
        }

        if (type.GetTypeElement() is { } typeElement)
        {
            var psiModule = expression.GetPsiModule();

            var implementedListTypeElement = PredefinedType.GENERIC_ILIST_FQN.TryGetTypeElement(psiModule) is { } t1 && typeElement.IsDescendantOf(t1)
                ? t1
                : null;

            var implementedReadOnlyListTypeElement =
                ClrTypeNames.IReadOnlyList.TryGetTypeElement(psiModule) is { } t2 && typeElement.IsDescendantOf(t2) ? t2 : null;

            if (implementedListTypeElement is { } || implementedReadOnlyListTypeElement is { })
            {
                if (typeElement is ITypeParameter typeParameter)
                {
                    var hasAccessibleIndexerIfIndexableCollection = false;
                    if (typeParameter.TypeConstraints.Any(
                        t => IsIndexableCollectionOrString(t, expression, out hasAccessibleIndexerIfIndexableCollection)))
                    {
                        hasAccessibleIndexer = hasAccessibleIndexerIfIndexableCollection;
                        return true;
                    }
                }

                var listIndexer = implementedListTypeElement is { } ? GetIndexers(implementedListTypeElement.Properties).FirstOrDefault() : null;
                var readOnlyListIndexer = implementedReadOnlyListTypeElement is { }
                    ? GetIndexers(implementedReadOnlyListTypeElement.Properties).FirstOrDefault()
                    : null;

                hasAccessibleIndexer = GetIndexers(GetAllProperties(typeElement))
                    .Any(
                        indexer => (listIndexer is { } && indexer.OverridesOrImplements(listIndexer)
                                || readOnlyListIndexer is { } && indexer.OverridesOrImplements(readOnlyListIndexer))
                            && AccessUtil.IsSymbolAccessible(indexer, new ElementAccessContext(expression)));
                return true;
            }
        }

        hasAccessibleIndexer = false;
        return false;
    }

    [Pure]
    static bool IsDistinctCollection(IType type, ITreeNode context)
    {
        if (type.IsClrType(PredefinedType.ISET_FQN)
            || type.IsClrType(ClrTypeNames.IReadOnlySet)
            || type.IsClrType(ClrTypeNames.DictionaryKeyCollection))
        {
            return true;
        }

        if (type.GetTypeElement() is { } typeElement)
        {
            var psiModule = context.GetPsiModule();

            return typeElement.IsDescendantOf(PredefinedType.ISET_FQN.TryGetTypeElement(psiModule))
                || typeElement.IsDescendantOf(ClrTypeNames.IReadOnlySet.TryGetTypeElement(psiModule));
        }

        return false;
    }

    [Pure]
    static string? TryGetItemDefaultValue(IType collectionType, ITreeNode context)
    {
        if (collectionType is IDeclaredType declaredType && declaredType.TryGetGenericParameterTypes() is [{ } itemType])
        {
            Debug.Assert(CSharpLanguage.Instance is { });

            var defaultValue = DefaultValueUtil.GetClrDefaultValue(itemType, CSharpLanguage.Instance, context);

            if (itemType.IsEnumType() && defaultValue is { } and not ICastExpression)
            {
                return $"{itemType.GetPresentableName(CSharpLanguage.Instance)}.{defaultValue.GetText()}";
            }

            return defaultValue?.GetText();
        }

        return null;
    }

    /// <remarks>
    /// <c>list.ElementAt(int)</c> → <c>list[int]</c><para/>
    /// <c>list.ElementAt(Index)</c> → <c>list[Index]</c><para/>
    /// <c>set.ElementAt(int)</c> → ⚠️
    /// </remarks>
    static void AnalyzeElementAt(
        IHighlightingConsumer consumer,
        IInvocationExpression invocationExpression,
        IReferenceExpression invokedExpression,
        ICSharpArgument indexArgument)
    {
        Debug.Assert(invokedExpression.QualifierExpression is { });
        Debug.Assert(indexArgument.Value is { });

        var type = invokedExpression.QualifierExpression.Type();

        if (IsIndexableCollectionOrString(type, invokedExpression.QualifierExpression, out var hasAccessibleIndexer))
        {
            if (hasAccessibleIndexer)
            {
                consumer.AddHighlighting(
                    new UseIndexerSuggestion(
                        "Use the indexer.",
                        invocationExpression,
                        invokedExpression,
                        indexArgument.Value.GetText(),
                        type.IsGenericArray(invocationExpression) || type.IsString()));
            }

            return;
        }

        if (IsDistinctCollection(type, invokedExpression))
        {
            consumer.AddHighlighting(
                new SuspiciousElementAccessWarning(
                    "The collection doesn't guarantee ordering, so retrieving the item by its index could result in unpredictable behavior.",
                    invocationExpression,
                    invokedExpression));
        }
    }

    /// <remarks>
    /// <c>set.ElementAtOrDefault(int)</c> → ⚠️<para/>
    /// <c>set.ElementAtOrDefault(Index)</c> → ⚠️
    /// </remarks>
    static void AnalyzeElementAtOrDefault(
        IHighlightingConsumer consumer,
        IInvocationExpression invocationExpression,
        IReferenceExpression invokedExpression)
    {
        Debug.Assert(invokedExpression.QualifierExpression is { });

        var type = invokedExpression.QualifierExpression.Type();

        if (IsIndexableCollectionOrString(type, invokedExpression))
        {
            return;
        }

        if (IsDistinctCollection(type, invokedExpression))
        {
            consumer.AddHighlighting(
                new SuspiciousElementAccessWarning(
                    "The collection doesn't guarantee ordering, so retrieving an item by its index could result in unpredictable behavior.",
                    invocationExpression,
                    invokedExpression));
        }
    }

    /// <remarks>
    /// <c>list.First()</c> → <c>list[0]</c><para/>
    /// <c>set.First()</c> → ⚠️
    /// </remarks>
    static void AnalyzeFirst(IHighlightingConsumer consumer, IInvocationExpression invocationExpression, IReferenceExpression invokedExpression)
    {
        Debug.Assert(invokedExpression.QualifierExpression is { });

        var type = invokedExpression.QualifierExpression.Type();

        if (IsIndexableCollectionOrString(type, invokedExpression, out var hasAccessibleIndexer))
        {
            if (hasAccessibleIndexer)
            {
                consumer.AddHighlighting(new UseIndexerSuggestion("Use the indexer.", invocationExpression, invokedExpression, "0", true));
            }

            return;
        }

        if (IsDistinctCollection(type, invokedExpression))
        {
            consumer.AddHighlighting(
                new SuspiciousElementAccessWarning(
                    """The collection doesn't guarantee ordering, so retrieving the "first" item could result in unpredictable behavior.""",
                    invocationExpression,
                    invokedExpression));
        }
    }

    /// <remarks>
    /// <c>list.FirstOrDefault()</c> → <c>list is [var first, ..] ? first : default</c> (C# 11)<para/>
    /// <c>list.FirstOrDefault(defaultValue)</c> → <c>list is [var first, ..] ? first : defaultValue</c> (C# 11)<para/>
    /// <c>set.FirstOrDefault()</c> → ⚠️<para/>
    /// <c>set.FirstOrDefault(defaultValue)</c> → ⚠️
    /// </remarks>
    void AnalyzeFirstOrDefault(
        IHighlightingConsumer consumer,
        IInvocationExpression invocationExpression,
        IReferenceExpression invokedExpression,
        ICSharpArgument? defaultValueArgument = null)
    {
        Debug.Assert(invokedExpression.QualifierExpression is { });
        Debug.Assert(defaultValueArgument is null or { Value: { } });

        var type = invokedExpression.QualifierExpression.Type();

        if (IsIndexableCollectionOrString(type, invokedExpression, out var hasAccessibleIndexer))
        {
            if (hasAccessibleIndexer
                && invocationExpression.GetCSharpLanguageLevel() >= CSharpLanguageLevel.CSharp110
                && invokedExpression.QualifierExpression.IsNotNullHere(nullableReferenceTypesDataFlowAnalysisRunSynchronizer))
            {
                consumer.AddHighlighting(
                    new UseLinqListPatternSuggestion(
                        "Use list pattern.",
                        invocationExpression,
                        invokedExpression,
                        ListPatternSuggestionKind.FirstOrDefault,
                        defaultValueArgument?.Value.GetText()
                        ?? TryGetItemDefaultValue(invokedExpression.QualifierExpression.Type(), invokedExpression)));
            }

            return;
        }

        if (IsDistinctCollection(type, invokedExpression))
        {
            consumer.AddHighlighting(
                new SuspiciousElementAccessWarning(
                    """The collection doesn't guarantee ordering, so retrieving a "first" item could result in unpredictable behavior.""",
                    invocationExpression,
                    invokedExpression));
        }
    }

    /// <remarks>
    /// <c>list.Last()</c> → <c>list[^1]</c> (C# 8)<para/>
    /// <c>set.Last()</c> → ⚠️
    /// </remarks>
    static void AnalyzeLast(IHighlightingConsumer consumer, IInvocationExpression invocationExpression, IReferenceExpression invokedExpression)
    {
        Debug.Assert(invokedExpression.QualifierExpression is { });

        var type = invokedExpression.QualifierExpression.Type();

        if (IsIndexableCollectionOrString(type, invokedExpression, out var hasAccessibleIndexer))
        {
            if (hasAccessibleIndexer && invocationExpression.GetCSharpLanguageLevel() >= CSharpLanguageLevel.CSharp80)
            {
                consumer.AddHighlighting(new UseIndexerSuggestion("Use the indexer.", invocationExpression, invokedExpression, "^1", true));
            }

            return;
        }

        if (IsDistinctCollection(type, invokedExpression))
        {
            consumer.AddHighlighting(
                new SuspiciousElementAccessWarning(
                    """The collection doesn't guarantee ordering, so retrieving the "last" item could result in unpredictable behavior.""",
                    invocationExpression,
                    invokedExpression));
        }
    }

    /// <remarks>
    /// <c>list.LastOrDefault()</c> → <c>list is [.., var last] ? last : default</c> (C# 11)<para/>
    /// <c>list.LastOrDefault(defaultValue)</c> → <c>list is [.., var last] ? last : defaultValue</c> (C# 11)<para/>
    /// <c>set.LastOrDefault()</c> → ⚠️<para/>
    /// <c>set.LastOrDefault(defaultValue)</c> → ⚠️
    /// </remarks>
    void AnalyzeLastOrDefault(
        IHighlightingConsumer consumer,
        IInvocationExpression invocationExpression,
        IReferenceExpression invokedExpression,
        ICSharpArgument? defaultValueArgument = null)
    {
        Debug.Assert(invokedExpression.QualifierExpression is { });
        Debug.Assert(defaultValueArgument is null or { Value: { } });

        var type = invokedExpression.QualifierExpression.Type();

        if (IsIndexableCollectionOrString(type, invokedExpression, out var hasAccessibleIndexer))
        {
            if (hasAccessibleIndexer
                && invocationExpression.GetCSharpLanguageLevel() >= CSharpLanguageLevel.CSharp110
                && invokedExpression.QualifierExpression.IsNotNullHere(nullableReferenceTypesDataFlowAnalysisRunSynchronizer))
            {
                consumer.AddHighlighting(
                    new UseLinqListPatternSuggestion(
                        "Use list pattern.",
                        invocationExpression,
                        invokedExpression,
                        ListPatternSuggestionKind.LastOrDefault,
                        defaultValueArgument?.Value.GetText()
                        ?? TryGetItemDefaultValue(invokedExpression.QualifierExpression.Type(), invokedExpression)));
            }

            return;
        }

        if (IsDistinctCollection(type, invokedExpression))
        {
            consumer.AddHighlighting(
                new SuspiciousElementAccessWarning(
                    """The collection doesn't guarantee ordering, so retrieving a "last" item could result in unpredictable behavior.""",
                    invocationExpression,
                    invokedExpression));
        }
    }

    /// <remarks>
    /// <c>array.LongCount()</c> → <c>array.Length</c><para/>
    /// <c>string.LongCount()</c> → <c>string.Length</c><para/>
    /// <c>collection.LongCount()</c> → <c>collection.Count</c>
    /// </remarks>
    static void AnalyzeLongCount(IHighlightingConsumer consumer, IInvocationExpression invocationExpression, IReferenceExpression invokedExpression)
    {
        Debug.Assert(invokedExpression.QualifierExpression is { });

        var type = invokedExpression.QualifierExpression.Type();

        if (type.IsString() || type.IsGenericArray(invokedExpression))
        {
            consumer.AddHighlighting(
                new UseCollectionPropertySuggestion(
                    $"Use the '{nameof(string.Length)}' property.",
                    invocationExpression,
                    invokedExpression,
                    nameof(string.Length)));

            return;
        }

        if (IsCollection(type, invokedExpression))
        {
            consumer.AddHighlighting(
                new UseCollectionPropertySuggestion(
                    $"Use the '{nameof(ICollection<int>.Count)}' property.",
                    invocationExpression,
                    invokedExpression,
                    nameof(ICollection<int>.Count)));
        }
    }

    /// <remarks>
    /// <c>list.Single()</c> → <c>list is [var item] ? item : throw new InvalidOperationException(...)</c> (C# 11)
    /// </remarks>
    void AnalyzeSingle(IHighlightingConsumer consumer, IInvocationExpression invocationExpression, IReferenceExpression invokedExpression)
    {
        Debug.Assert(invokedExpression.QualifierExpression is { });

        if (invocationExpression.GetCSharpLanguageLevel() >= CSharpLanguageLevel.CSharp110)
        {
            var type = invokedExpression.QualifierExpression.Type();

            if (IsIndexableCollectionOrString(type, invokedExpression, out var hasAccessibleIndexer)
                && hasAccessibleIndexer
                && invokedExpression.QualifierExpression.IsNotNullHere(nullableReferenceTypesDataFlowAnalysisRunSynchronizer))
            {
                consumer.AddHighlighting(
                    new UseLinqListPatternSuggestion("Use list pattern.", invocationExpression, invokedExpression, ListPatternSuggestionKind.Single));
            }
        }
    }

    /// <remarks>
    /// <c>list.SingleOrDefault()</c> → <c>list switch { [] => default, [var item] => item, _ => throw new InvalidOperationException(...) }</c> (C# 11)<para/>
    /// <c>list.SingleOrDefault(defaultValue)</c> → <c>list switch { [] => defaultValue, [var item] => item, _ => throw new InvalidOperationException(...) }</c> (C# 11)
    /// </remarks>
    void AnalyzeSingleOrDefault(
        IHighlightingConsumer consumer,
        IInvocationExpression invocationExpression,
        IReferenceExpression invokedExpression,
        ICSharpArgument? defaultValueArgument = null)
    {
        Debug.Assert(invokedExpression.QualifierExpression is { });
        Debug.Assert(defaultValueArgument is null or { Value: { } });

        if (invocationExpression.GetCSharpLanguageLevel() >= CSharpLanguageLevel.CSharp110) // introduced list patterns
        {
            var type = invokedExpression.QualifierExpression.Type();

            if (IsIndexableCollectionOrString(type, invokedExpression, out var hasAccessibleIndexer)
                && hasAccessibleIndexer
                && invokedExpression.QualifierExpression.IsNotNullHere(nullableReferenceTypesDataFlowAnalysisRunSynchronizer))
            {
                consumer.AddHighlighting(
                    new UseSwitchExpressionSuggestion(
                        "Use switch expression.",
                        invocationExpression,
                        invokedExpression,
                        defaultValueArgument?.Value.GetText()
                        ?? TryGetItemDefaultValue(invokedExpression.QualifierExpression.Type(), invokedExpression)));
            }
        }
    }

    protected override void Run(IInvocationExpression element, ElementProblemAnalyzerData data, IHighlightingConsumer consumer)
    {
        if (!element.IsUsedAsStatement()
            && element is { InvokedExpression: IReferenceExpression { QualifierExpression: { }, Reference: var reference } invokedExpression }
            && reference.Resolve().DeclaredElement is IMethod
            {
                TypeParameters: [var typeParameter],
                IsExtensionMethod: true,
                AccessibilityDomain.DomainType: AccessibilityDomain.AccessibilityDomainType.PUBLIC,
            } method
            && method.ContainingType.IsClrType(PredefinedType.ENUMERABLE_CLASS))
        {
            switch (method.ShortName)
            {
                case nameof(Enumerable.ElementAt):
                    switch (method.Parameters, element.Arguments)
                    {
                        case ([_, { Type: IDeclaredType indexType }], [{ Value: { } } indexArgument])
                            when indexType.IsInt() || indexType.IsSystemIndex():
                            AnalyzeElementAt(consumer, element, invokedExpression, indexArgument);
                            break;
                    }
                    break;

                case nameof(Enumerable.ElementAtOrDefault):
                    switch (method.Parameters, element.Arguments)
                    {
                        case ([_, { Type: IDeclaredType indexType }], _) when indexType.IsInt() || indexType.IsSystemIndex():
                            AnalyzeElementAtOrDefault(consumer, element, invokedExpression);
                            break;
                    }
                    break;

                case nameof(Enumerable.First):
                    switch (method.Parameters, element.Arguments)
                    {
                        case ([_], []): AnalyzeFirst(consumer, element, invokedExpression); break;
                    }
                    break;

                case nameof(Enumerable.FirstOrDefault):
                    switch (method.Parameters, element.Arguments)
                    {
                        case ([_], []): AnalyzeFirstOrDefault(consumer, element, invokedExpression); break;

                        case ([_, { Type: IDeclaredType defaultValueType }], [{ Value: { } } defaultValueArgument])
                            when typeParameter.Equals(defaultValueType.GetTypeElement()):
                            AnalyzeFirstOrDefault(consumer, element, invokedExpression, defaultValueArgument);
                            break;
                    }
                    break;

                case nameof(Enumerable.Last):
                    switch (method.Parameters, element.Arguments)
                    {
                        case ([_], []): AnalyzeLast(consumer, element, invokedExpression); break;
                    }
                    break;

                case nameof(Enumerable.LastOrDefault):
                    switch (method.Parameters, element.Arguments)
                    {
                        case ([_], []): AnalyzeLastOrDefault(consumer, element, invokedExpression); break;

                        case ([_, { Type: IDeclaredType defaultValueType }], [{ Value: { } } defaultValueArgument])
                            when typeParameter.Equals(defaultValueType.GetTypeElement()):

                            AnalyzeLastOrDefault(consumer, element, invokedExpression, defaultValueArgument);
                            break;
                    }
                    break;

                case nameof(Enumerable.LongCount):
                    switch (method.Parameters, element.Arguments)
                    {
                        case ([_], []): AnalyzeLongCount(consumer, element, invokedExpression); break;
                    }
                    break;

                case nameof(Enumerable.Single):
                    switch (method.Parameters, element.Arguments)
                    {
                        case ([_], []): AnalyzeSingle(consumer, element, invokedExpression); break;
                    }
                    break;

                case nameof(Enumerable.SingleOrDefault):
                    switch (method.Parameters, element.Arguments)
                    {
                        case ([_], []): AnalyzeSingleOrDefault(consumer, element, invokedExpression); break;

                        case ([_, { Type: IDeclaredType defaultValueType }], [{ Value: { } } defaultValueArgument])
                            when typeParameter.Equals(defaultValueType.GetTypeElement()):

                            AnalyzeSingleOrDefault(consumer, element, invokedExpression, defaultValueArgument);
                            break;
                    }
                    break;
            }
        }
    }
}