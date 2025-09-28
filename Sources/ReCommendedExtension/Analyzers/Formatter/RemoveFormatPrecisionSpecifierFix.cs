using JetBrains.Application.Progress;
using JetBrains.ProjectModel;
using JetBrains.ReSharper.Feature.Services.QuickFixes;
using JetBrains.ReSharper.Psi.CSharp.Tree;
using JetBrains.ReSharper.Resources.Shell;
using JetBrains.TextControl;
using JetBrains.Util;

namespace ReCommendedExtension.Analyzers.Formatter;

[QuickFix]
public sealed class RemoveFormatPrecisionSpecifierFix(RedundantFormatPrecisionSpecifierHint highlighting) : QuickFixBase
{
    public override bool IsAvailable(IUserDataHolder cache)
        => highlighting.FormatElement is { Insert: { } }
            or { FormatStringExpression: { }, FormatItem: { } }
            or { Argument.Value: ICSharpLiteralExpression or IInterpolatedStringExpression };

    public override string Text => "Remove format precision specifier";

    protected override Action<ITextControl> ExecutePsiTransaction(ISolution solution, IProgressIndicator progress)
        => _ =>
        {
            using (WriteLockCookie.Create())
            {
                var documentRange = highlighting.CalculateRange();

                documentRange.Document.DeleteText(documentRange.TextRange);
            }
        };
}