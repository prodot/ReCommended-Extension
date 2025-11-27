using JetBrains.Application.Progress;
using JetBrains.DocumentModel;
using JetBrains.ProjectModel;
using JetBrains.ReSharper.Feature.Services.Daemon;
using JetBrains.ReSharper.Feature.Services.QuickFixes;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.CSharp;
using JetBrains.ReSharper.Psi.ExtensionsAPI.Tree;
using JetBrains.ReSharper.Psi.Tree;
using JetBrains.ReSharper.Resources.Shell;
using JetBrains.TextControl;
using JetBrains.Util;

namespace ReCommendedExtension.Analyzers.Formatter;

[RegisterConfigurableSeverity(
    SeverityId,
    null,
    HighlightingGroupIds.BestPractice,
    "Pass other format specifier" + ZoneMarker.Suffix,
    "",
    Severity.SUGGESTION)]
[ConfigurableSeverityHighlighting(SeverityId, CSharpLanguage.Name)]
public sealed class PassOtherFormatSpecifierSuggestion(string message) : Highlighting(message)
{
    const string SeverityId = "PassOtherFormatSpecifier";

    public required FormatElement FormatElement { get; init; }

    public required string Replacement { get; init; }

    public override DocumentRange CalculateRange()
    {
        switch (FormatElement)
        {
            case { Insert: { } insert }: return insert.FormatSpecifier.GetDocumentRange().ExtendLeft(-1); // to exclude the ':' character

            case { FormatStringExpression: { } formatStringExpression, FormatItem: { } formatItem }:
                var documentRange = formatStringExpression.GetDocumentRange();

                return new DocumentRange(
                    documentRange.Document,
                    new TextRange(
                        documentRange.StartOffset.Offset + formatItem.FormatStringRange.StartOffset, // the ':' character already excluded
                        documentRange.StartOffset.Offset + formatItem.FormatStringRange.StartOffset + formatItem.FormatStringRange.Length));

            case { Argument: { } argument }: return argument.Value.GetDocumentRange();
        }

        throw new NotSupportedException();
    }

    [QuickFix]
    public sealed class Fix(PassOtherFormatSpecifierSuggestion highlighting) : QuickFixBase
    {
        public override bool IsAvailable(IUserDataHolder cache) => true;

        public override string Text => $"Replace with '{highlighting.Replacement}'";

        protected override Action<ITextControl> ExecutePsiTransaction(ISolution solution, IProgressIndicator progress)
        {
            if (highlighting.FormatElement.Argument is { } argument)
            {
                using (WriteLockCookie.Create())
                {
                    var factory = CSharpElementFactory.GetInstance(argument);

                    ModificationUtil.ReplaceChild(
                        argument,
                        factory.CreateArgument(
                            ParameterKind.UNKNOWN,
                            argument.NameIdentifier?.Name,
                            factory.CreateExpression($"\"{highlighting.Replacement}\"")));
                }
            }

            return _ =>
            {
                if (highlighting.FormatElement is { Insert: { } } or { FormatStringExpression: { }, FormatItem: { } })
                {
                    using (WriteLockCookie.Create())
                    {
                        var documentRange = highlighting.CalculateRange();

                        documentRange.Document.ReplaceText(documentRange.TextRange, highlighting.Replacement);
                    }
                }
            };
        }
    }
}