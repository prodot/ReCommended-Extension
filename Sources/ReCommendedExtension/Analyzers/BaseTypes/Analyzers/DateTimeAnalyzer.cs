using JetBrains.ReSharper.Feature.Services.Daemon;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.CSharp.Tree;
using ReCommendedExtension.Extensions;
using ReCommendedExtension.Extensions.MethodFinding;

namespace ReCommendedExtension.Analyzers.BaseTypes.Analyzers;

/// <remarks>
/// C# language version checks are only done when a quick fix would require it.
/// </remarks>
[ElementProblemAnalyzer(typeof(ICSharpExpression), HighlightingTypes = [typeof(UseDateTimePropertySuggestion)])]
public sealed class DateTimeAnalyzer : ElementProblemAnalyzer<ICSharpExpression>
{
    /// <remarks>
    /// <c>DateTime.Now.Date</c> → <c>DateTime.Today</c>
    /// </remarks>
    static void AnalyzeDate(IHighlightingConsumer consumer, IReferenceExpression referenceExpression)
    {
        if (!referenceExpression.IsPropertyAssignment()
            && !referenceExpression.IsWithinNameofExpression()
            && referenceExpression.QualifierExpression is IReferenceExpression
            {
                Reference: var reference, QualifierExpression: var qualifierExpression,
            }
            && reference.Resolve().DeclaredElement is IProperty
            {
                AccessibilityDomain.DomainType: AccessibilityDomain.AccessibilityDomainType.PUBLIC,
                IsStatic: true,
                ShortName: nameof(DateTime.Now),
            } property
            && property.ContainingType.IsClrType(PredefinedType.DATETIME_FQN)
            && PredefinedType.DATETIME_FQN.HasProperty(
                new PropertySignature { Name = nameof(DateTime.Today), IsStatic = true },
                referenceExpression.GetPsiModule()))
        {
            consumer.AddHighlighting(
                new UseDateTimePropertySuggestion(
                    $"Use the '{nameof(DateTime.Today)}' property.",
                    reference,
                    qualifierExpression,
                    referenceExpression,
                    nameof(DateTime.Today)));
        }
    }

    protected override void Run(ICSharpExpression element, ElementProblemAnalyzerData data, IHighlightingConsumer consumer)
    {
        switch (element)
        {
            case IReferenceExpression { Reference: var reference } referenceExpression
                when reference.Resolve().DeclaredElement is IProperty
                {
                    AccessibilityDomain.DomainType: AccessibilityDomain.AccessibilityDomainType.PUBLIC, IsStatic: false,
                } property
                && property.ContainingType.IsClrType(PredefinedType.DATETIME_FQN):

                switch (property.ShortName)
                {
                    case nameof(DateTime.Date): AnalyzeDate(consumer, referenceExpression); break;
                }
                break;
        }
    }
}