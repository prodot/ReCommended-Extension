using JetBrains.Application.Progress;
using JetBrains.ProjectModel;
using JetBrains.ReSharper.Feature.Services.QuickFixes;
using JetBrains.ReSharper.Psi.CSharp.Tree;
using JetBrains.ReSharper.Resources.Shell;
using JetBrains.TextControl;
using JetBrains.Util;

namespace ReCommendedExtension.Analyzers.BaseTypes;

[QuickFix]
public sealed class RemoveFormatPrecisionSpecifierFix(RedundantFormatPrecisionSpecifierHint highlighting) : QuickFixBase
{
    public override bool IsAvailable(IUserDataHolder cache)
        => highlighting.FormatArgument.Value is ICSharpLiteralExpression or IInterpolatedStringExpression;

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