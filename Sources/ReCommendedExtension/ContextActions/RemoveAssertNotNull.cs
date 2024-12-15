using JetBrains.Application.Progress;
using JetBrains.ProjectModel;
using JetBrains.ReSharper.Feature.Services.ContextActions;
using JetBrains.ReSharper.Feature.Services.CSharp.ContextActions;
using JetBrains.ReSharper.Psi.CSharp.Tree;
using JetBrains.ReSharper.Psi.Tree;
using JetBrains.TextControl;
using ReCommendedExtension.Analyzers.ControlFlow;

namespace ReCommendedExtension.ContextActions;

[ContextAction(
    GroupType = typeof(CSharpContextActions),
    Name = "Remove inline assertion for non-nullness" + ZoneMarker.Suffix,
    Description = "Removes inline assertion for non-nullness.")]
public sealed class RemoveAssertNotNull(ICSharpContextActionDataProvider provider) : ContextAction<IInvocationExpression>(provider)
{
    InlineAssertion? assertion;

    [MemberNotNullWhen(true, nameof(assertion))]
    protected override bool IsAvailable(IInvocationExpression selectedElement)
    {
        assertion = InlineAssertion.TryFromInvocationExpression(selectedElement);

        return assertion is { };
    }

    public override string Text
    {
        get
        {
            Debug.Assert(assertion is { });

            return $"Remove '{assertion.MethodName}()'";
        }
    }

    protected override Action<ITextControl> ExecutePsiTransaction(ISolution solution, IProgressIndicator progress)
    {
        try
        {
            Debug.Assert(assertion is { });

            // changing the tree with the ModificationUtil does not re-trigger the analysis for the current statement => changing the text
            // directly does re-trigger the analysis

            var textRange = assertion.InvocationExpression.GetDocumentRange().TextRange;
            var qualifierExpressionText = assertion.QualifierExpression.GetText();

            return textControl => textControl.Document.ReplaceText(textRange, qualifierExpressionText);
        }
        finally
        {
            assertion = null;
        }
    }
}