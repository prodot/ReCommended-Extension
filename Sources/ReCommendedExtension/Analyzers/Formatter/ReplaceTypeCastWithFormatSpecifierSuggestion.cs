using JetBrains.Application.Progress;
using JetBrains.DocumentModel;
using JetBrains.ProjectModel;
using JetBrains.ReSharper.Feature.Services.Daemon;
using JetBrains.ReSharper.Feature.Services.QuickFixes;
using JetBrains.ReSharper.Psi.CSharp;
using JetBrains.ReSharper.Psi.CSharp.Tree;
using JetBrains.ReSharper.Resources.Shell;
using JetBrains.TextControl;
using JetBrains.Util;

namespace ReCommendedExtension.Analyzers.Formatter;

[RegisterConfigurableSeverity(
    SeverityId,
    null,
    HighlightingGroupIds.BestPractice,
    "Use a format specifier instead of the type cast" + ZoneMarker.Suffix,
    "",
    Severity.SUGGESTION)]
[ConfigurableSeverityHighlighting(SeverityId, CSharpLanguage.Name)]
public sealed class ReplaceTypeCastWithFormatSpecifierSuggestion(string message, IInterpolatedStringInsert insert) : Highlighting(message)
{
    const string SeverityId = "ReplaceTypeCastWithFormatSpecifier";

    public required ICSharpExpression Expression { get; init; }

    public required string FormatSpecifier { get; init; }

    public override DocumentRange CalculateRange() => insert.GetHighlightingRange();

    [QuickFix]
    public sealed class Fix(ReplaceTypeCastWithFormatSpecifierSuggestion highlighting) : QuickFixBase
    {
        public override bool IsAvailable(IUserDataHolder cache) => true;

        public override string Text
            => $"Replace with '{highlighting.Expression.GetText().TrimToSingleLineWithMaxLength(120)}:{highlighting.FormatSpecifier}'";

        protected override Action<ITextControl> ExecutePsiTransaction(ISolution solution, IProgressIndicator progress)
            => _ =>
            {
                using (WriteLockCookie.Create())
                {
                    var documentRange = highlighting.CalculateRange();

                    documentRange.Document.ReplaceText(documentRange.TextRange, $"{highlighting.Expression.GetText()}:{highlighting.FormatSpecifier}");
                }
            };
    }
}