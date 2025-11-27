using JetBrains.Application.Progress;
using JetBrains.ProjectModel;
using JetBrains.ReSharper.Feature.Services.Daemon;
using JetBrains.ReSharper.Feature.Services.QuickFixes;
using JetBrains.ReSharper.Psi.CSharp;
using JetBrains.ReSharper.Psi.CSharp.Tree;
using JetBrains.ReSharper.Psi.ExtensionsAPI.Tree;
using JetBrains.ReSharper.Resources.Shell;
using JetBrains.TextControl;
using JetBrains.Util;
using ReCommendedExtension.Extensions;

namespace ReCommendedExtension.Analyzers.MemberInvocation;

[RegisterConfigurableSeverity(SeverityId, null, HighlightingGroupIds.BestPractice, "Use another method" + ZoneMarker.Suffix, "", Severity.SUGGESTION)]
[ConfigurableSeverityHighlighting(SeverityId, CSharpLanguage.Name)]
public sealed class UseOtherMethodSuggestion(string message) : MultipleHighlightings(message)
{
    const string SeverityId = "UseOtherMethod";

    public required ICSharpExpression Qualifier { get; init; }

    public required ReplacedMethodInvocation ReplacedMethodInvocation { get; init; }

    [QuickFix]
    public sealed class Fix(UseOtherMethodSuggestion highlighting) : QuickFixBase
    {
        public override bool IsAvailable(IUserDataHolder cache) => true;

        public override string Text
        {
            get
            {
                var negation = highlighting.ReplacedMethodInvocation.Replacement.IsNegated ? "!" : "";
                var arguments = string.Join(", ", highlighting.ReplacedMethodInvocation.Replacement.Arguments);

                return $"Replace with '{negation}{highlighting.ReplacedMethodInvocation.Name}({arguments.TrimToSingleLineWithMaxLength(120)})'";
            }
        }

        protected override Action<ITextControl>? ExecutePsiTransaction(ISolution solution, IProgressIndicator progress)
        {
            using (WriteLockCookie.Create())
            {
                var factory = CSharpElementFactory.GetInstance(highlighting.ReplacedMethodInvocation.Replacement.OriginalExpression);

                var negation = highlighting.ReplacedMethodInvocation.Replacement.IsNegated ? "!" : "";
                var arguments = string.Join(", ", highlighting.ReplacedMethodInvocation.Replacement.Arguments);

                ModificationUtil
                    .ReplaceChild(
                        highlighting.ReplacedMethodInvocation.Replacement.OriginalExpression,
                        factory.CreateExpression($"({negation}$0.{highlighting.ReplacedMethodInvocation.Name}({arguments}))", highlighting.Qualifier))
                    .TryRemoveParentheses(factory);
            }

            return null;
        }
    }
}