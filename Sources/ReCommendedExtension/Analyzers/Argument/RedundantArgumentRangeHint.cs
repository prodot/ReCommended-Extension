using JetBrains.Application.Progress;
using JetBrains.ProjectModel;
using JetBrains.ReSharper.Feature.Services.Daemon;
using JetBrains.ReSharper.Feature.Services.Daemon.Attributes;
using JetBrains.ReSharper.Feature.Services.QuickFixes;
using JetBrains.ReSharper.Psi.CSharp;
using JetBrains.ReSharper.Psi.CSharp.Tree;
using JetBrains.ReSharper.Resources.Shell;
using JetBrains.TextControl;
using JetBrains.Util;
using ReCommendedExtension.Extensions;

namespace ReCommendedExtension.Analyzers.Argument;

[RegisterConfigurableSeverity(
    SeverityId,
    null,
    HighlightingGroupIds.CodeRedundancy,
    "The argument range is redundant" + ZoneMarker.Suffix,
    "",
    Severity.SUGGESTION)]
[ConfigurableSeverityHighlighting(
    SeverityId,
    CSharpLanguage.Name,
    AttributeId = AnalysisHighlightingAttributeIds.DEADCODE,
    OverlapResolve = OverlapResolveKind.DEADCODE)]
public sealed class RedundantArgumentRangeHint(string message) : MultipleHighlightings(message)
{
    const string SeverityId = "RedundantArgumentRange";

    public required IReadOnlyList<ICSharpArgument> Arguments { get; init; }

    [QuickFix]
    public sealed class Fix(RedundantArgumentRangeHint highlighting) : QuickFixBase
    {
        public override bool IsAvailable(IUserDataHolder cache) => true;

        public override string Text => "Remove arguments";

        protected override Action<ITextControl>? ExecutePsiTransaction(ISolution solution, IProgressIndicator progress)
        {
            using (WriteLockCookie.Create())
            {
                foreach (var argument in highlighting.Arguments)
                {
                    argument.Remove();
                }
            }

            return null;
        }
    }
}