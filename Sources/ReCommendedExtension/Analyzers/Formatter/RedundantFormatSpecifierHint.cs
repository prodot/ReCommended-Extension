using JetBrains.Application.Progress;
using JetBrains.DocumentModel;
using JetBrains.ProjectModel;
using JetBrains.ReSharper.Feature.Services.Daemon;
using JetBrains.ReSharper.Feature.Services.Daemon.Attributes;
using JetBrains.ReSharper.Feature.Services.QuickFixes;
using JetBrains.ReSharper.Psi.CSharp;
using JetBrains.ReSharper.Psi.ExtensionsAPI.Tree;
using JetBrains.ReSharper.Psi.Tree;
using JetBrains.ReSharper.Resources.Shell;
using JetBrains.TextControl;
using JetBrains.Util;
using ReCommendedExtension.Extensions;

namespace ReCommendedExtension.Analyzers.Formatter;

[RegisterConfigurableSeverity(
    SeverityId,
    null,
    HighlightingGroupIds.CodeRedundancy,
    "The format specifier is redundant" + ZoneMarker.Suffix,
    "",
    Severity.SUGGESTION)]
[ConfigurableSeverityHighlighting(
    SeverityId,
    CSharpLanguage.Name,
    AttributeId = AnalysisHighlightingAttributeIds.DEADCODE,
    OverlapResolve = OverlapResolveKind.DEADCODE)]
public sealed class RedundantFormatSpecifierHint(string message) : Highlighting(message)
{
    const string SeverityId = "RedundantFormatSpecifier";

    public required FormatElement FormatElement { get; init; }

    public override DocumentRange CalculateRange()
    {
        switch (FormatElement)
        {
            case { Insert: { } insert }: return insert.FormatSpecifier.GetDocumentRange(); // the ':' character already included

            case { FormatStringExpression: { } formatStringExpression, FormatItem: { } formatItem }:
                var documentRange = formatStringExpression.GetDocumentRange();

                return new DocumentRange(
                    documentRange.Document,
                    new TextRange(
                        documentRange.StartOffset.Offset + formatItem.FormatStringRange.StartOffset - 1, // to include the ':' character
                        documentRange.StartOffset.Offset + formatItem.FormatStringRange.StartOffset + formatItem.FormatStringRange.Length));

            case { Argument: { } argument }: return argument.GetDocumentRange();
        }

        throw new NotSupportedException();
    }

    [QuickFix]
    public sealed class Fix(RedundantFormatSpecifierHint highlighting) : QuickFixBase
    {
        public override bool IsAvailable(IUserDataHolder cache) => true;

        public override string Text => "Remove format specifier";

        protected override Action<ITextControl> ExecutePsiTransaction(ISolution solution, IProgressIndicator progress)
        {
            switch (highlighting.FormatElement)
            {
                case { Insert: { } insert }:
                    using (WriteLockCookie.Create())
                    {
                        ModificationUtil.DeleteChild(insert.FormatSpecifier);
                    }
                    break;

                case { Argument: { } argument }:
                    using (WriteLockCookie.Create())
                    {
                        argument.Remove();
                    }
                    break;
            }

            return _ =>
            {
                if (highlighting.FormatElement is { FormatStringExpression: { }, FormatItem: { } })
                {
                    using (WriteLockCookie.Create())
                    {
                        var documentRange = highlighting.CalculateRange();

                        documentRange.Document.DeleteText(documentRange.TextRange);
                    }
                }
            };
        }
    }
}