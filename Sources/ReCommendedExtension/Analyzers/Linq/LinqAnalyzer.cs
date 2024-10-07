using JetBrains.ReSharper.Feature.Services.Daemon;
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
        typeof(UseListPatternSuggestion),
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

    static void AnalyzeElementAt(
        IHighlightingConsumer consumer,
        IInvocationExpression invocationExpression,
        IReferenceExpression invokedExpression,
        ICSharpArgument index)
    {
        Debug.Assert(invokedExpression.QualifierExpression is { });

        var type = invokedExpression.QualifierExpression.Type();

        if (IsIndexableCollection(type, invokedExpression))
        {
            consumer.AddHighlighting(new UseIndexerSuggestion("Use indexer", invocationExpression, invokedExpression, index.GetText()));

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

    static void AnalyzeFirstOrDefault(
        IHighlightingConsumer consumer,
        IInvocationExpression invocationExpression,
        IReferenceExpression invokedExpression,
        bool hasPredicateArgument,
        ICSharpArgument? defaultValueArgument = null)
    {
        Debug.Assert(invokedExpression.QualifierExpression is { });

        var type = invokedExpression.QualifierExpression.Type();

        if (IsIndexableCollection(type, invokedExpression))
        {
            if (!hasPredicateArgument && invocationExpression.GetCSharpLanguageLevel() >= CSharpLanguageLevel.CSharp110) // introduced list patterns
            {
                consumer.AddHighlighting(
                    new UseListPatternSuggestion(
                        "Use list pattern",
                        invocationExpression,
                        invokedExpression,
                        ListPatternSuggestionKind.FirstOrDefault,
                        defaultValueArgument?.GetText()));
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
            if (!hasPredicateArgument && invocationExpression.GetCSharpLanguageLevel() >= CSharpLanguageLevel.CSharp80) // introduced indices
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

    static void AnalyzeLastOrDefault(
        IHighlightingConsumer consumer,
        IInvocationExpression invocationExpression,
        IReferenceExpression invokedExpression,
        bool hasPredicateArgument,
        ICSharpArgument? defaultValueArgument = null)
    {
        Debug.Assert(invokedExpression.QualifierExpression is { });

        var type = invokedExpression.QualifierExpression.Type();

        if (IsIndexableCollection(type, invokedExpression))
        {
            if (!hasPredicateArgument && invocationExpression.GetCSharpLanguageLevel() >= CSharpLanguageLevel.CSharp110) // introduced list patterns
            {
                consumer.AddHighlighting(
                    new UseListPatternSuggestion(
                        "Use list pattern",
                        invocationExpression,
                        invokedExpression,
                        ListPatternSuggestionKind.LastOrDefault,
                        defaultValueArgument?.GetText()));
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

            // return;
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

    static void AnalyzeSingle(IHighlightingConsumer consumer, IInvocationExpression invocationExpression, IReferenceExpression invokedExpression)
    {
        Debug.Assert(invokedExpression.QualifierExpression is { });

        if (invocationExpression.GetCSharpLanguageLevel() >= CSharpLanguageLevel.CSharp110 // introduced list patterns
            && IsIndexableCollection(invokedExpression.QualifierExpression.Type(), invokedExpression))
        {
            consumer.AddHighlighting(
                new UseListPatternSuggestion("Use list pattern", invocationExpression, invokedExpression, ListPatternSuggestionKind.Single));
        }
    }

    static void AnalyzeSingleOrDefault(
        IHighlightingConsumer consumer,
        IInvocationExpression invocationExpression,
        IReferenceExpression invokedExpression,
        ICSharpArgument? defaultValueArgument = null)
    {
        Debug.Assert(invokedExpression.QualifierExpression is { });

        if (invocationExpression.GetCSharpLanguageLevel() >= CSharpLanguageLevel.CSharp110 // introduced list patterns
            && IsIndexableCollection(invokedExpression.QualifierExpression.Type(), invokedExpression))
        {
            consumer.AddHighlighting(
                new UseSwitchExpressionSuggestion("Use switch expression", invocationExpression, invokedExpression, defaultValueArgument?.GetText()));
        }
    }

    protected override void Run(IInvocationExpression element, ElementProblemAnalyzerData data, IHighlightingConsumer consumer)
    {
        if (element is
            {
                Parent: { } and not (IExpressionStatement or IForInitializer or IForIterator),
                InvokedExpression: IReferenceExpression { QualifierExpression: { }, Reference: var reference } invokedExpression,
            }
            && reference.Resolve().DeclaredElement is IMethod method
            && method.ContainingType.IsClrType(PredefinedType.ENUMERABLE_CLASS)
            && method is { IsExtensionMethod: true, AccessibilityDomain.DomainType: AccessibilityDomain.AccessibilityDomainType.PUBLIC })
        {
            switch (method)
            {
                case { ShortName: nameof(Enumerable.ElementAt), TypeParameters: [_], Parameters: [_, { Type: IDeclaredType indexType }] }
                    when indexType.IsInt() || indexType.IsSystemIndex():
                    AnalyzeElementAt(consumer, element, invokedExpression, element.Arguments[0]);
                    break;

                case { ShortName: nameof(Enumerable.ElementAtOrDefault), TypeParameters: [_], Parameters: [_, { Type: IDeclaredType indexType }] }
                    when indexType.IsInt() || indexType.IsSystemIndex():
                    AnalyzeElementAtOrDefault(consumer, element, invokedExpression);
                    break;

                case { ShortName: nameof(Enumerable.First), TypeParameters: [_], Parameters: [_] }:
                    AnalyzeFirst(consumer, element, invokedExpression, false);
                    break;

                case { ShortName: nameof(Enumerable.First), TypeParameters: [_], Parameters: [_, { Type: IDeclaredType predicateType }] }
                    when predicateType.IsClrType(PredefinedType.FUNC2_FQN):
                    AnalyzeFirst(consumer, element, invokedExpression, true);
                    break;

                case { ShortName: nameof(Enumerable.FirstOrDefault), TypeParameters: [_], Parameters: [_] }:
                    AnalyzeFirstOrDefault(consumer, element, invokedExpression, false);
                    break;

                case
                {
                    ShortName: nameof(Enumerable.FirstOrDefault),
                    TypeParameters: [var typeParameter],
                    Parameters: [_, { Type: IDeclaredType defaultValueType }],
                } when typeParameter.Equals(defaultValueType.GetTypeElement()):
                    AnalyzeFirstOrDefault(consumer, element, invokedExpression, false, element.Arguments[0]);
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

                case
                {
                    ShortName: nameof(Enumerable.LastOrDefault),
                    TypeParameters: [var typeParameter],
                    Parameters: [_, { Type: IDeclaredType defaultValueType }],
                } when typeParameter.Equals(defaultValueType.GetTypeElement()):
                    AnalyzeLastOrDefault(consumer, element, invokedExpression, false, element.Arguments[0]);
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

                case { ShortName: nameof(Enumerable.LongCount), TypeParameters: [_], Parameters: [_] }:
                    AnalyzeLongCount(consumer, element, invokedExpression);
                    break;

                case { ShortName: nameof(Enumerable.Single), TypeParameters: [_], Parameters: [_] }:
                    AnalyzeSingle(consumer, element, invokedExpression);
                    break;

                case { ShortName: nameof(Enumerable.SingleOrDefault), TypeParameters: [_], Parameters: [_] }:
                    AnalyzeSingleOrDefault(consumer, element, invokedExpression);
                    break;

                case { ShortName: nameof(Enumerable.SingleOrDefault), TypeParameters: [_], Parameters: [_, { Type: IDeclaredType defaultValueType }] }
                    when method.TypeParameters[0].Equals(defaultValueType.GetTypeElement()):
                    AnalyzeSingleOrDefault(consumer, element, invokedExpression, element.Arguments[0]);
                    break;
            }
        }
    }
}