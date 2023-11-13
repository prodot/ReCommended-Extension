using JetBrains.Application.Progress;
using JetBrains.ProjectModel;
using JetBrains.ReSharper.Feature.Services.QuickFixes;
using JetBrains.ReSharper.Resources.Shell;
using JetBrains.TextControl;
using JetBrains.Util;

namespace ReCommendedExtension.Analyzers.InterfaceImplementation;

[QuickFix]
public sealed class MarkAsImplementingEqualityOperatorsInterfaceFix : ImplementEqualityOperatorsInterfaceFix
{
    public MarkAsImplementingEqualityOperatorsInterfaceFix(ImplementEqualityOperatorsSuggestion highlighting) : base(highlighting) { }

    public override bool IsAvailable(IUserDataHolder cache) => Highlighting.OperatorsAvailable;

    public override string Text
    {
        get
        {
            var type = Highlighting.Declaration.DeclaredName;

            return $"Mark as implementing IEqualityOperators<{type}, {type}, bool>";
        }
    }

    protected override Action<ITextControl> ExecutePsiTransaction(ISolution solution, IProgressIndicator progress)
    {
        using (WriteLockCookie.Create())
        {
            AddBaseInterfaceDeclaration();
        }

        return _ => { };
    }
}