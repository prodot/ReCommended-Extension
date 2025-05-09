using JetBrains.Application.Progress;
using JetBrains.ProjectModel;
using JetBrains.ReSharper.Feature.Services.QuickFixes;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.CSharp;
using JetBrains.ReSharper.Psi.ExtensionsAPI.Tree;
using JetBrains.ReSharper.Resources.Shell;
using JetBrains.TextControl;
using JetBrains.Util;

namespace ReCommendedExtension.Analyzers.BaseTypes;

[QuickFix]
public sealed class PassOtherFormatSpecifierFix(PassOtherFormatSpecifierSuggestion highlighting) : QuickFixBase
{
    public override bool IsAvailable(IUserDataHolder cache) => true;

    public override string Text => $"Replace with '{highlighting.Replacement}'";

    protected override Action<ITextControl> ExecutePsiTransaction(ISolution solution, IProgressIndicator progress)
    {
        if (highlighting.FormatArgument is { })
        {
            using (WriteLockCookie.Create())
            {
                var factory = CSharpElementFactory.GetInstance(highlighting.FormatArgument);

                ModificationUtil.ReplaceChild(
                    highlighting.FormatArgument,
                    factory.CreateArgument(
                        ParameterKind.UNKNOWN,
                        highlighting.FormatArgument.NameIdentifier?.Name,
                        factory.CreateExpression($"\"{highlighting.Replacement}\"")));
            }
        }

        return _ =>
        {
            if (highlighting.Insert is { })
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