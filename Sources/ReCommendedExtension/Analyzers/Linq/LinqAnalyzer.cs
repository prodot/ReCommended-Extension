using JetBrains.ReSharper.Feature.Services.Daemon;
using JetBrains.ReSharper.Feature.Services.Util;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.CSharp;
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
        typeof(UsePropertySuggestion),
        typeof(SuspiciousElementAccessWarning),
    ])]
public sealed class LinqAnalyzer : ElementProblemAnalyzer<IInvocationExpression>
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
    /// <c>list.ElementAt(0)</c> → <c>list is [var first, ..] ? first : throw new InvalidOperationException(...)</c> (C# 11)<para/>
    /// <c>list.ElementAt(^1)</c> → <c>list is [.., var last] ? last : throw new InvalidOperationException(...)</c> (C# 11)<para/>
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

        if (IsIndexableCollection(type, invokedExpression))
        {
            consumer.AddHighlighting(new UseIndexerSuggestion("Use indexer", invocationExpression, invokedExpression, indexArgument.Value.GetText()));

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

        if (IsIndexableCollection(type, invokedExpression))
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
    /// <c>list.First()</c> → <c>list is [var first, ..] ? first : throw new InvalidOperationException(...)</c> (C# 11)<para/>
    /// <c>set.First()</c> → ⚠️<para/>
    /// <c>set.First(Func&lt;T, bool&gt;)</c> → ⚠️
    /// </remarks>
    static void AnalyzeFirst(
        IHighlightingConsumer consumer,
        IInvocationExpression invocationExpression,
        IReferenceExpression invokedExpression,
        bool hasPredicateArgument)
    {
        Debug.Assert(invokedExpression.QualifierExpression is { });

        var type = invokedExpression.QualifierExpression.Type();

        if (IsIndexableCollection(type, invokedExpression))
        {
            if (!hasPredicateArgument)
            {
                consumer.AddHighlighting(new UseIndexerSuggestion("Use indexer", invocationExpression, invokedExpression, "0"));
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
    /// <c>list.FirstOrDefault(T)</c> → <c>list is [var first, ..] ? first : defaultValue</c> (C# 11)<para/>
    /// <c>set.FirstOrDefault()</c> → ⚠️<para/>
    /// <c>set.FirstOrDefault(T)</c> → ⚠️<para/>
    /// <c>set.FirstOrDefault(Func&lt;T, bool&gt;)</c> → ⚠️<para/>
    /// <c>set.FirstOrDefault(Func&lt;T, bool&gt;, T)</c> → ⚠️
    /// </remarks>
    static void AnalyzeFirstOrDefault(
        IHighlightingConsumer consumer,
        IInvocationExpression invocationExpression,
        IReferenceExpression invokedExpression,
        bool hasPredicateArgument,
        ICSharpArgument? defaultValueArgument = null)
    {
        Debug.Assert(invokedExpression.QualifierExpression is { });
        Debug.Assert(defaultValueArgument is null or { Value: { } });

        var type = invokedExpression.QualifierExpression.Type();

        if (IsIndexableCollection(type, invokedExpression))
        {
            if (!hasPredicateArgument && invocationExpression.GetCSharpLanguageLevel() >= CSharpLanguageLevel.CSharp110)
            {
                consumer.AddHighlighting(
                    new UseLinqListPatternSuggestion(
                        "Use list pattern",
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
    /// <c>list.Last()</c> → <c>list is [.., var last] ? last : throw new InvalidOperationException(...)</c> (C# 11)<para/>
    /// <c>set.Last()</c> → ⚠️<para/>
    /// <c>set.Last(Func&lt;T, bool&gt;)</c> → ⚠️
    /// </remarks>
    static void AnalyzeLast(
        IHighlightingConsumer consumer,
        IInvocationExpression invocationExpression,
        IReferenceExpression invokedExpression,
        bool hasPredicateArgument)
    {
        Debug.Assert(invokedExpression.QualifierExpression is { });

        var type = invokedExpression.QualifierExpression.Type();

        if (IsIndexableCollection(type, invokedExpression))
        {
            if (!hasPredicateArgument && invocationExpression.GetCSharpLanguageLevel() >= CSharpLanguageLevel.CSharp80)
            {
                consumer.AddHighlighting(new UseIndexerSuggestion("Use indexer", invocationExpression, invokedExpression, "^1"));
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
    /// <c>list.LastOrDefault(T)</c> → <c>list is [.., var last] ? last : defaultValue</c> (C# 11)<para/>
    /// <c>set.LastOrDefault()</c> → ⚠️<para/>
    /// <c>set.LastOrDefault(T)</c> → ⚠️<para/>
    /// <c>set.LastOrDefault(Func&lt;T, bool&gt;)</c> → ⚠️<para/>
    /// <c>set.LastOrDefault(Func&lt;T, bool&gt;, T)</c> → ⚠️
    /// </remarks>
    static void AnalyzeLastOrDefault(
        IHighlightingConsumer consumer,
        IInvocationExpression invocationExpression,
        IReferenceExpression invokedExpression,
        bool hasPredicateArgument,
        ICSharpArgument? defaultValueArgument = null)
    {
        Debug.Assert(invokedExpression.QualifierExpression is { });
        Debug.Assert(defaultValueArgument is null or { Value: { } });

        var type = invokedExpression.QualifierExpression.Type();

        if (IsIndexableCollection(type, invokedExpression))
        {
            if (!hasPredicateArgument && invocationExpression.GetCSharpLanguageLevel() >= CSharpLanguageLevel.CSharp110)
            {
                consumer.AddHighlighting(
                    new UseLinqListPatternSuggestion(
                        "Use list pattern",
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
                new UsePropertySuggestion(
                    $"Use the '{nameof(string.Length)}' property",
                    invocationExpression,
                    invokedExpression,
                    nameof(string.Length)));

            return;
        }

        if (IsCollection(type, invokedExpression))
        {
            consumer.AddHighlighting(
                new UsePropertySuggestion(
                    $"Use the '{nameof(ICollection<int>.Count)}' property",
                    invocationExpression,
                    invokedExpression,
                    nameof(ICollection<int>.Count)));
        }
    }

    /// <remarks>
    /// <c>list.Single()</c> → <c>list is [var item] ? item : throw new InvalidOperationException(...)</c> (C# 11)
    /// </remarks>
    static void AnalyzeSingle(IHighlightingConsumer consumer, IInvocationExpression invocationExpression, IReferenceExpression invokedExpression)
    {
        Debug.Assert(invokedExpression.QualifierExpression is { });

        if (invocationExpression.GetCSharpLanguageLevel() >= CSharpLanguageLevel.CSharp110
            && IsIndexableCollection(invokedExpression.QualifierExpression.Type(), invokedExpression))
        {
            consumer.AddHighlighting(
                new UseLinqListPatternSuggestion("Use list pattern", invocationExpression, invokedExpression, ListPatternSuggestionKind.Single));
        }
    }

    /// <remarks>
    /// <c>list.SingleOrDefault()</c> → <c>list switch { [] => default, [var item] => item, _ => throw new InvalidOperationException(...) }</c> (C# 11)<para/>
    /// <c>list.SingleOrDefault(T)</c> → <c>list switch { [] => defaultValue, [var item] => item, _ => throw new InvalidOperationException(...) }</c> (C# 11)
    /// </remarks>
    static void AnalyzeSingleOrDefault(
        IHighlightingConsumer consumer,
        IInvocationExpression invocationExpression,
        IReferenceExpression invokedExpression,
        ICSharpArgument? defaultValueArgument = null)
    {
        Debug.Assert(invokedExpression.QualifierExpression is { });
        Debug.Assert(defaultValueArgument is null or { Value: { } });

        if (invocationExpression.GetCSharpLanguageLevel() >= CSharpLanguageLevel.CSharp110 // introduced list patterns
            && IsIndexableCollection(invokedExpression.QualifierExpression.Type(), invokedExpression))
        {
            consumer.AddHighlighting(
                new UseSwitchExpressionSuggestion(
                    "Use switch expression",
                    invocationExpression,
                    invokedExpression,
                    defaultValueArgument?.Value.GetText()
                    ?? TryGetItemDefaultValue(invokedExpression.QualifierExpression.Type(), invokedExpression)?.GetText()));
        }
    }

    protected override void Run(IInvocationExpression element, ElementProblemAnalyzerData data, IHighlightingConsumer consumer)
    {
        if (!element.IsUsedAsStatement()
            && element is { InvokedExpression: IReferenceExpression { QualifierExpression: { }, Reference: var reference } invokedExpression }
            && reference.Resolve().DeclaredElement is IMethod method
            && method.ContainingType.IsClrType(PredefinedType.ENUMERABLE_CLASS)
            && method is { IsExtensionMethod: true, AccessibilityDomain.DomainType: AccessibilityDomain.AccessibilityDomainType.PUBLIC })
        {
            switch (method)
            {
                // ElementAt
                case { ShortName: nameof(Enumerable.ElementAt), TypeParameters: [_], Parameters: [_, { Type: IDeclaredType indexType }] }
                    when (indexType.IsInt() || indexType.IsSystemIndex()) && element.Arguments is [{ Value: { } } indexArgument]:
                    AnalyzeElementAt(consumer, element, invokedExpression, indexArgument);
                    break;

                case { ShortName: nameof(Enumerable.ElementAtOrDefault), TypeParameters: [_], Parameters: [_, { Type: IDeclaredType indexType }] }
                    when indexType.IsInt() || indexType.IsSystemIndex():
                    AnalyzeElementAtOrDefault(consumer, element, invokedExpression);
                    break;

                // First
                case { ShortName: nameof(Enumerable.First), TypeParameters: [_], Parameters: [_] }:
                    AnalyzeFirst(consumer, element, invokedExpression, false);
                    break;

                case { ShortName: nameof(Enumerable.First), TypeParameters: [_], Parameters: [_, { Type: IDeclaredType predicateType }] }
                    when predicateType.IsClrType(PredefinedType.FUNC2_FQN):
                    AnalyzeFirst(consumer, element, invokedExpression, true);
                    break;

                // FirstOrDefault
                case { ShortName: nameof(Enumerable.FirstOrDefault), TypeParameters: [_], Parameters: [_] }:
                    AnalyzeFirstOrDefault(consumer, element, invokedExpression, false);
                    break;

                case
                {
                    ShortName: nameof(Enumerable.FirstOrDefault),
                    TypeParameters: [var typeParameter],
                    Parameters: [_, { Type: IDeclaredType defaultValueType }],
                } when typeParameter.Equals(defaultValueType.GetTypeElement()) && element.Arguments is [{ Value: { } } defaultValueArgument]:
                    AnalyzeFirstOrDefault(consumer, element, invokedExpression, false, defaultValueArgument);
                    break;

                case { ShortName: nameof(Enumerable.FirstOrDefault), TypeParameters: [_], Parameters: [_, { Type: IDeclaredType predicateType }] }
                    when predicateType.IsClrType(PredefinedType.FUNC2_FQN):
                    AnalyzeFirstOrDefault(consumer, element, invokedExpression, true);
                    break;

                case
                {
                    ShortName: nameof(Enumerable.FirstOrDefault),
                    TypeParameters: [var typeParameter],
                    Parameters: [_, { Type: IDeclaredType predicateType }, { Type: IDeclaredType defaultValueType }],
                } when predicateType.IsClrType(PredefinedType.FUNC2_FQN) && typeParameter.Equals(defaultValueType.GetTypeElement()):
                    AnalyzeFirstOrDefault(consumer, element, invokedExpression, true);
                    break;

                // Last
                case { ShortName: nameof(Enumerable.Last), TypeParameters: [_], Parameters: [_] }:
                    AnalyzeLast(consumer, element, invokedExpression, false);
                    break;

                case { ShortName: nameof(Enumerable.Last), TypeParameters: [_], Parameters: [_, { Type: IDeclaredType predicateType }] }
                    when predicateType.IsClrType(PredefinedType.FUNC2_FQN):
                    AnalyzeLast(consumer, element, invokedExpression, true);
                    break;

                case { ShortName: nameof(Enumerable.LastOrDefault), TypeParameters: [_], Parameters: [_] }:
                    AnalyzeLastOrDefault(consumer, element, invokedExpression, false);
                    break;

                // LastOrDefault
                case
                {
                    ShortName: nameof(Enumerable.LastOrDefault),
                    TypeParameters: [var typeParameter],
                    Parameters: [_, { Type: IDeclaredType defaultValueType }],
                } when typeParameter.Equals(defaultValueType.GetTypeElement()) && element.Arguments is [{ Value: { } } defaultValueArgument]:
                    AnalyzeLastOrDefault(consumer, element, invokedExpression, false, defaultValueArgument);
                    break;

                case { ShortName: nameof(Enumerable.LastOrDefault), TypeParameters: [_], Parameters: [_, { Type: IDeclaredType predicateType }] }
                    when predicateType.IsClrType(PredefinedType.FUNC2_FQN):
                    AnalyzeLastOrDefault(consumer, element, invokedExpression, true);
                    break;

                case
                {
                    ShortName: nameof(Enumerable.LastOrDefault),
                    TypeParameters: [var typeParameter],
                    Parameters: [_, { Type: IDeclaredType predicateType }, { Type: IDeclaredType defaultValueType }],
                } when predicateType.IsClrType(PredefinedType.FUNC2_FQN) && typeParameter.Equals(defaultValueType.GetTypeElement()):
                    AnalyzeLastOrDefault(consumer, element, invokedExpression, true);
                    break;

                // LongCount
                case { ShortName: nameof(Enumerable.LongCount), TypeParameters: [_], Parameters: [_] }:
                    AnalyzeLongCount(consumer, element, invokedExpression);
                    break;

                // Single
                case { ShortName: nameof(Enumerable.Single), TypeParameters: [_], Parameters: [_] }:
                    AnalyzeSingle(consumer, element, invokedExpression);
                    break;

                // SingleOrDefault
                case { ShortName: nameof(Enumerable.SingleOrDefault), TypeParameters: [_], Parameters: [_] }:
                    AnalyzeSingleOrDefault(consumer, element, invokedExpression);
                    break;

                case { ShortName: nameof(Enumerable.SingleOrDefault), TypeParameters: [_], Parameters: [_, { Type: IDeclaredType defaultValueType }] }
                    when method.TypeParameters[0].Equals(defaultValueType.GetTypeElement())
                    && element.Arguments is [{ Value: { } } defaultValueArgument]:
                    AnalyzeSingleOrDefault(consumer, element, invokedExpression, defaultValueArgument);
                    break;
            }
        }
    }
}