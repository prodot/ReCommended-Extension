using JetBrains.Application.Progress;
using JetBrains.ProjectModel;
using JetBrains.ReSharper.Feature.Services.QuickFixes;
using JetBrains.ReSharper.Resources.Shell;
using JetBrains.TextControl;
using JetBrains.Util;

namespace ReCommendedExtension.Analyzers.Formatter;

[QuickFix]
public sealed class ReplaceTypeCastWithFormatSpecifierFix(ReplaceTypeCastWithFormatSpecifierSuggestion highlighting) : QuickFixBase
{
    public override bool IsAvailable(IUserDataHolder cache) => true;

    public override string Text
        => $"Replace with '{highlighting.Expression.GetText().TrimToSingleLineWithMaxLength(120)}:{highlighting.FormatSpecifier}'";

    protected override Action<ITextControl>? ExecutePsiTransaction(ISolution solution, IProgressIndicator progress)
        => _ =>
        {
            using (WriteLockCookie.Create())
            {
                var documentRange = highlighting.CalculateRange();

                documentRange.Document.ReplaceText(documentRange.TextRange, $"{highlighting.Expression.GetText()}:{highlighting.FormatSpecifier}");
            }
        };
}