using JetBrains.Application.Progress;
using JetBrains.ProjectModel;
using JetBrains.ReSharper.Feature.Services.QuickFixes;
using JetBrains.ReSharper.Psi.ExtensionsAPI.Tree;
using JetBrains.ReSharper.Resources.Shell;
using JetBrains.TextControl;
using JetBrains.Util;
using ReCommendedExtension.Extensions;

namespace ReCommendedExtension.Analyzers.Formatter;

[QuickFix]
public sealed class RemoveFormatSpecifierFix(RedundantFormatSpecifierHint highlighting) : QuickFixBase
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