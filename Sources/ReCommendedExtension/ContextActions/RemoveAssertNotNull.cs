using System;
using System.Diagnostics;
using JetBrains.Annotations;
using JetBrains.Application.Progress;
using JetBrains.ProjectModel;
using JetBrains.ReSharper.Feature.Services.ContextActions;
using JetBrains.ReSharper.Feature.Services.CSharp.Analyses.Bulbs;
using JetBrains.ReSharper.Psi.CSharp.Tree;
using JetBrains.ReSharper.Psi.Tree;
using JetBrains.TextControl;
using ReCommendedExtension.Analyzers.ControlFlow;

namespace ReCommendedExtension.ContextActions
{
    [ContextAction(
        Group = "C#",
        Name = "Remove inline assertion for non-nullness" + ZoneMarker.Suffix,
        Description = "Removes inline assertion for non-nullness.")]
    public sealed class RemoveAssertNotNull : ContextActionBase
    {
        [NotNull]
        readonly ICSharpContextActionDataProvider provider;

        [CanBeNull]
        InlineAssertion assertion;

        public RemoveAssertNotNull([NotNull] ICSharpContextActionDataProvider provider) => this.provider = provider;

        public override string Text
        {
            get
            {
                Debug.Assert(assertion != null);

                return $"Remove '{assertion.MethodName}()'";
            }
        }

        public override bool IsAvailable(JetBrains.Util.IUserDataHolder cache)
        {
            var invocationExpression = provider.GetSelectedElement<IInvocationExpression>(true, false);

            assertion = invocationExpression != null ? InlineAssertion.TryFromInvocationExpression(invocationExpression) : null;

            return assertion != null;
        }

        protected override Action<ITextControl> ExecutePsiTransaction(ISolution solution, IProgressIndicator progress)
        {
            Debug.Assert(assertion != null);

            try
            {
                // changing the tree with the ModificationUtil does not re-trigger the analysis for the current statement => changing the text
                // directly does re-trigger the analysis

                var textRange = assertion.InvocationExpression.GetDocumentRange().TextRange;
                var qualifierExpressionText = assertion.QualifierExpression.GetText();

                return textControl =>
                {
                    Debug.Assert(textControl != null);

                    textControl.Document.ReplaceText(textRange, qualifierExpressionText);
                };
            }
            finally
            {
                assertion = null;
            }
        }
    }
}