using JetBrains.ReSharper.Feature.Services.Daemon;
using JetBrains.ReSharper.Feature.Services.Util;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.CSharp;
using JetBrains.ReSharper.Psi.CSharp.Impl.ControlFlow.NullableAnalysis.Runner;
using JetBrains.ReSharper.Psi.CSharp.Tree;
using JetBrains.ReSharper.Psi.Tree;
using JetBrains.ReSharper.Psi.Util;

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
    static bool IsIndexableCollection(IType type, ITreeNode context)
    {
        var psiModule = context.GetPsiModule();

        return type.IsGenericIList()
            || type.IsGenericIReadOnlyList()
            || type.IsGenericArray(context)
            || type.GetTypeElement() is { } typeElement
            && (typeElement.IsDescendantOf(PredefinedType.GENERIC_ILIST_FQN.TryGetTypeElement(psiModule))
                || typeElement.IsDescendantOf(ClrTypeNames.IReadOnlyList.TryGetTypeElement(psiModule)));
    }

    [Pure]
    static bool IsDistinctCollection(IType type, ITreeNode context)
    {
        var psiModule = context.GetPsiModule();

        return type.IsClrType(PredefinedType.ISET_FQN)
            || type.IsClrType(ClrTypeNames.IReadOnlySet)
            || type.IsClrType(ClrTypeNames.DictionaryKeyCollection)
            || type.GetTypeElement() is { } typeElement
            && (typeElement.IsDescendantOf(PredefinedType.ISET_FQN.TryGetTypeElement(psiModule))
                || typeElement.IsDescendantOf(ClrTypeNames.IReadOnlySet.TryGetTypeElement(psiModule)));
    }

    [Pure]
    static IExpression? TryGetItemDefaultValue(IType collectionType, ITreeNode context)
    {
        if (collectionType is IDeclaredType declaredType && declaredType.TryGetGenericParameterTypes() is [{ } itemType])
        {
            Debug.Assert(CSharpLanguage.Instance is { });

            return DefaultValueUtil.GetClrDefaultValue(itemType, CSharpLanguage.Instance, context);
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

        if (IsIndexableCollection(type, invokedExpression) || type.IsString())
        {
            consumer.AddHighlighting(
                new UseIndexerSuggestion(
                    "Use the indexer.",
                    invocationExpression,
                    invokedExpression,
                    indexArgument.Value.GetText(),
                    type.IsGenericArray(invocationExpression) || type.IsString()));

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

        if (IsIndexableCollection(type, invokedExpression) || type.IsString())
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

        if (IsIndexableCollection(type, invokedExpression) || type.IsString())
        {
            consumer.AddHighlighting(new UseIndexerSuggestion("Use the indexer.", invocationExpression, invokedExpression, "0", true));

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

        if (IsIndexableCollection(type, invokedExpression) || type.IsString())
        {
            if (invocationExpression.GetCSharpLanguageLevel() >= CSharpLanguageLevel.CSharp110
                && invokedExpression.QualifierExpression.IsNotNullHere(nullableReferenceTypesDataFlowAnalysisRunSynchronizer))
            {
                consumer.AddHighlighting(
                    new UseLinqListPatternSuggestion(
                        "Use list pattern.",
                        invocationExpression,
                        invokedExpression,
                        ListPatternSuggestionKind.FirstOrDefault,
                        defaultValueArgument?.Value.GetText()
                        ?? TryGetItemDefaultValue(invokedExpression.QualifierExpression.Type(), invokedExpression)?.GetText()));
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

        if (IsIndexableCollection(type, invokedExpression) || type.IsString())
        {
            if (invocationExpression.GetCSharpLanguageLevel() >= CSharpLanguageLevel.CSharp80)
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

        if (IsIndexableCollection(type, invokedExpression) || type.IsString())
        {
            if (invocationExpression.GetCSharpLanguageLevel() >= CSharpLanguageLevel.CSharp110
                && invokedExpression.QualifierExpression.IsNotNullHere(nullableReferenceTypesDataFlowAnalysisRunSynchronizer))
            {
                consumer.AddHighlighting(
                    new UseLinqListPatternSuggestion(
                        "Use list pattern.",
                        invocationExpression,
                        invokedExpression,
                        ListPatternSuggestionKind.LastOrDefault,
                        defaultValueArgument?.Value.GetText()
                        ?? TryGetItemDefaultValue(invokedExpression.QualifierExpression.Type(), invokedExpression)?.GetText()));
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

            if ((IsIndexableCollection(type, invokedExpression) || type.IsString())
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

            if ((IsIndexableCollection(type, invokedExpression) || type.IsString())
                && invokedExpression.QualifierExpression.IsNotNullHere(nullableReferenceTypesDataFlowAnalysisRunSynchronizer))
            {
                consumer.AddHighlighting(
                    new UseSwitchExpressionSuggestion(
                        "Use switch expression.",
                        invocationExpression,
                        invokedExpression,
                        defaultValueArgument?.Value.GetText()
                        ?? TryGetItemDefaultValue(invokedExpression.QualifierExpression.Type(), invokedExpression)?.GetText()));
            }
        }
    }

    protected override void Run(IInvocationExpression element, ElementProblemAnalyzerData data, IHighlightingConsumer consumer)
    {
        if (!element.IsUsedAsStatement()
            && element is { InvokedExpression: IReferenceExpression { QualifierExpression: { }, Reference: var reference } invokedExpression }
            && reference.Resolve().DeclaredElement is IMethod
            {
                IsExtensionMethod: true, AccessibilityDomain.DomainType: AccessibilityDomain.AccessibilityDomainType.PUBLIC,
            } method
            && method.ContainingType.IsClrType(PredefinedType.ENUMERABLE_CLASS))
        {
            switch (method.ShortName)
            {
                case nameof(Enumerable.ElementAt):
                    switch (method.TypeParameters, method.Parameters, element.Arguments)
                    {
                        case ([_], [_, { Type: IDeclaredType indexType }], [{ Value: { } } indexArgument])
                            when indexType.IsInt() || indexType.IsSystemIndex():

                            AnalyzeElementAt(consumer, element, invokedExpression, indexArgument);
                            break;
                    }
                    break;

                case nameof(Enumerable.ElementAtOrDefault):
                    switch (method.TypeParameters, method.Parameters, element.Arguments)
                    {
                        case ([_], [_, { Type: IDeclaredType indexType }], _) when indexType.IsInt() || indexType.IsSystemIndex():
                            AnalyzeElementAtOrDefault(consumer, element, invokedExpression);
                            break;
                    }
                    break;

                case nameof(Enumerable.First):
                    switch (method.TypeParameters, method.Parameters, element.Arguments)
                    {
                        case ([_], [_], []):
                            AnalyzeFirst(consumer, element, invokedExpression);
                            break;
                    }
                    break;

                case nameof(Enumerable.FirstOrDefault):
                    switch (method.TypeParameters, method.Parameters, element.Arguments)
                    {
                        case ([_], [_], []):
                            AnalyzeFirstOrDefault(consumer, element, invokedExpression);
                            break;

                        case ([var typeParameter], [_, { Type: IDeclaredType defaultValueType }], [{ Value: { } } defaultValueArgument])
                            when typeParameter.Equals(defaultValueType.GetTypeElement()):

                            AnalyzeFirstOrDefault(consumer, element, invokedExpression, defaultValueArgument);
                            break;
                    }
                    break;

                case nameof(Enumerable.Last):
                    switch (method.TypeParameters, method.Parameters, element.Arguments)
                    {
                        case ([_], [_], []):
                            AnalyzeLast(consumer, element, invokedExpression);
                            break;
                    }
                    break;

                case nameof(Enumerable.LastOrDefault):
                    switch (method.TypeParameters, method.Parameters, element.Arguments)
                    {
                        case ([_], [_], []):
                            AnalyzeLastOrDefault(consumer, element, invokedExpression);
                            break;

                        case ([var typeParameter], [_, { Type: IDeclaredType defaultValueType }], [{ Value: { } } defaultValueArgument])
                            when typeParameter.Equals(defaultValueType.GetTypeElement()):

                            AnalyzeLastOrDefault(consumer, element, invokedExpression, defaultValueArgument);
                            break;
                    }
                    break;

                case nameof(Enumerable.LongCount):
                    switch (method.TypeParameters, method.Parameters, element.Arguments)
                    {
                        case ([_], [_], []):
                            AnalyzeLongCount(consumer, element, invokedExpression);
                            break;
                    }
                    break;

                case nameof(Enumerable.Single):
                    switch (method.TypeParameters, method.Parameters, element.Arguments)
                    {
                        case ([_], [_], []):
                            AnalyzeSingle(consumer, element, invokedExpression);
                            break;
                    }
                    break;

                case nameof(Enumerable.SingleOrDefault):
                    switch (method.TypeParameters, method.Parameters, element.Arguments)
                    {
                        case ([_], [_], []):
                            AnalyzeSingleOrDefault(consumer, element, invokedExpression);
                            break;

                        case ([var typeParameter], [_, { Type: IDeclaredType defaultValueType }], [{ Value: { } } defaultValueArgument])
                            when typeParameter.Equals(defaultValueType.GetTypeElement()):

                            AnalyzeSingleOrDefault(consumer, element, invokedExpression, defaultValueArgument);
                            break;
                    }
                    break;
            }
        }
    }
}