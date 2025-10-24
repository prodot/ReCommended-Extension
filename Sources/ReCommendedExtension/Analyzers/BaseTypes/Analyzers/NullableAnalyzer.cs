using JetBrains.ReSharper.Feature.Services.Daemon;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.CSharp.Tree;
using JetBrains.ReSharper.Psi.Util;
using ReCommendedExtension.Extensions;

namespace ReCommendedExtension.Analyzers.BaseTypes.Analyzers;

/// <remarks>
/// C# language version checks are only done when a quick fix would require it.
/// </remarks>
[ElementProblemAnalyzer(
    typeof(IReferenceExpression),
    HighlightingTypes = [typeof(UseNullableHasValueAlternativeSuggestion), typeof(ReplaceNullableValueWithTypeCastSuggestion)])]
public sealed class NullableAnalyzer : ElementProblemAnalyzer<IReferenceExpression>
{
    /// <remarks>
    /// <c>nullable.HasValue</c> → <c>nullable is { }</c> (C# 7) or <c>nullable is not null</c> (C# 9) or <c>nullable != null</c><para/>
    /// <c>nullable.HasValue</c> → <c>nullable is { }</c> (C# 7) or <c>nullable is (_, _, ...)</c> (C# 8) or <c>nullable is not null</c> (C# 9) or <c>nullable != null</c>
    /// </remarks>
    static void AnalyzeHasValue(IHighlightingConsumer consumer, IReferenceExpression referenceExpression)
    {
        if (!referenceExpression.IsPropertyAssignment() && !referenceExpression.IsWithinNameofExpression())
        {
            consumer.AddHighlighting(new UseNullableHasValueAlternativeSuggestion("Use pattern or null check.", referenceExpression));
        }
    }

    /// <remarks>
    /// <c>nullable.Value</c> → <c>(T)nullable</c>
    /// </remarks>
    static void AnalyzeValue(IHighlightingConsumer consumer, IReferenceExpression referenceExpression)
    {
        Debug.Assert(referenceExpression.QualifierExpression is { });

        if (!referenceExpression.IsPropertyAssignment()
            && !referenceExpression.IsWithinNameofExpression()
            && !referenceExpression.QualifierExpression.Type().Unlift().IsValueTuple(out _))
        {
            consumer.AddHighlighting(new ReplaceNullableValueWithTypeCastSuggestion("Use type cast.", referenceExpression));
        }
    }

    protected override void Run(IReferenceExpression element, ElementProblemAnalyzerData data, IHighlightingConsumer consumer)
    {
        if (element is { Reference: var reference, QualifierExpression: { } })
        {
            switch (reference.Resolve().DeclaredElement)
            {
                case IProperty { AccessibilityDomain.DomainType: AccessibilityDomain.AccessibilityDomainType.PUBLIC, IsStatic: false } property
                    when property.ContainingType.IsNullableOfT():

                    switch (property.ShortName)
                    {
                        case nameof(Nullable<int>.HasValue): AnalyzeHasValue(consumer, element); break;

                        case nameof(Nullable<int>.Value): AnalyzeValue(consumer, element); break;
                    }
                    break;
            }
        }
    }
}