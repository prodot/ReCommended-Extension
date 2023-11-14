using JetBrains.Application.Progress;
using JetBrains.ProjectModel;
using JetBrains.ReSharper.Feature.Services.QuickFixes;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Resources.Shell;
using JetBrains.TextControl;
using JetBrains.Util;

namespace ReCommendedExtension.Analyzers.InterfaceImplementation;

[QuickFix]
public sealed class ImplementEqualityOperatorsInterfaceFix : QuickFixBase
{
    readonly ImplementEqualityOperatorsSuggestion highlighting;

    public ImplementEqualityOperatorsInterfaceFix(ImplementEqualityOperatorsForClassesSuggestion highlighting) => this.highlighting = highlighting;

    public ImplementEqualityOperatorsInterfaceFix(ImplementEqualityOperatorsForStructsSuggestion highlighting) => this.highlighting = highlighting;

    public ImplementEqualityOperatorsInterfaceFix(ImplementEqualityOperatorsForRecordsSuggestion highlighting) => this.highlighting = highlighting;

    public override bool IsAvailable(IUserDataHolder cache) => true;

    public override string Text
        => $"Declare IEqualityOperators<{highlighting.Declaration.DeclaredName}, {highlighting.Declaration.DeclaredName}, bool>";

    protected override Action<ITextControl> ExecutePsiTransaction(ISolution solution, IProgressIndicator progress)
    {
        using (WriteLockCookie.Create())
        {
            var psiModule = highlighting.Declaration.GetPsiModule();

            var equalityOperatorsInterface = EquatableAnalyzer.TryGetEqualityOperatorsInterface(psiModule);
            Debug.Assert(equalityOperatorsInterface is { });

            Debug.Assert(highlighting.Declaration.DeclaredElement is { });

            var type = TypeFactory.CreateType(highlighting.Declaration.DeclaredElement);

            highlighting.Declaration.AddSuperInterface(
                TypeFactory.CreateType(
                    equalityOperatorsInterface,
                    new IType[] { type, type, TypeFactory.CreateTypeByCLRName(PredefinedType.BOOLEAN_FQN, psiModule) }),
                false);
        }

        return _ => { };
    }
}