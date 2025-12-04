using JetBrains.Application.Progress;
using JetBrains.ProjectModel;
using JetBrains.ReSharper.Feature.Services.Daemon;
using JetBrains.ReSharper.Feature.Services.Daemon.Attributes;
using JetBrains.ReSharper.Feature.Services.QuickFixes;
using JetBrains.ReSharper.Psi.CSharp;
using JetBrains.ReSharper.Psi.CSharp.Tree;
using JetBrains.ReSharper.Psi.ExtensionsAPI.Tree;
using JetBrains.ReSharper.Resources.Shell;
using JetBrains.TextControl;
using JetBrains.Util;
using ReCommendedExtension.Extensions;

namespace ReCommendedExtension.Analyzers.Linq;

[RegisterConfigurableSeverity(
    SeverityId,
    null,
    HighlightingGroupIds.CodeRedundancy,
    "The LINQ query is redundant" + ZoneMarker.Suffix,
    "",
    Severity.SUGGESTION)]
[ConfigurableSeverityHighlighting(
    SeverityId,
    CSharpLanguage.Name,
    AttributeId = AnalysisHighlightingAttributeIds.DEADCODE,
    OverlapResolve = OverlapResolveKind.DEADCODE)]
public sealed class RedundantLinqQueryHint(string message) : MultipleHighlightings(message)
{
    const string SeverityId = "RedundantLinqQuery";

    public required IQueryExpression QueryExpression { get; init; }

    public required ICSharpExpression Expression { get; init; }

    [QuickFix]
    public sealed class Fix(RedundantLinqQueryHint highlighting) : QuickFixBase
    {
        public override bool IsAvailable(IUserDataHolder cache) => true;

        public override string Text => "Remove LINQ query";

        protected override Action<ITextControl>? ExecutePsiTransaction(ISolution solution, IProgressIndicator progress)
        {
            using (WriteLockCookie.Create())
            {
                var factory = CSharpElementFactory.GetInstance(highlighting.QueryExpression);

                var replacedExpression = ModificationUtil.ReplaceChild(
                    highlighting.QueryExpression,
                    factory.CreateExpression("$0", highlighting.Expression));

                if (replacedExpression.Parent is ICSharpExpression parent)
                {
                    parent.TryRemoveParentheses(factory);
                }
            }

            return null;
        }
    }
}