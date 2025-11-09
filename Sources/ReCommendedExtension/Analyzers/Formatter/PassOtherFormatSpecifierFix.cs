using JetBrains.Application.Progress;
using JetBrains.ProjectModel;
using JetBrains.ReSharper.Feature.Services.QuickFixes;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.CSharp;
using JetBrains.ReSharper.Psi.ExtensionsAPI.Tree;
using JetBrains.ReSharper.Resources.Shell;
using JetBrains.TextControl;
using JetBrains.Util;

namespace ReCommendedExtension.Analyzers.Formatter;

[QuickFix]
public sealed class PassOtherFormatSpecifierFix(PassOtherFormatSpecifierSuggestion highlighting) : QuickFixBase
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