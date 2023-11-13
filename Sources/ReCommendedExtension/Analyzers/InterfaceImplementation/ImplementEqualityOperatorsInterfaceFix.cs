using JetBrains.ReSharper.Feature.Services.QuickFixes;
using JetBrains.ReSharper.Psi;

namespace ReCommendedExtension.Analyzers.InterfaceImplementation;

public abstract class ImplementEqualityOperatorsInterfaceFix : QuickFixBase
{
    private protected ImplementEqualityOperatorsInterfaceFix(ImplementEqualityOperatorsSuggestion highlighting) => Highlighting = highlighting;

    private protected ImplementEqualityOperatorsSuggestion Highlighting { get; }

    private protected void AddBaseInterfaceDeclaration()
    {
        var psiModule = Highlighting.Declaration.GetPsiModule();

        var equalityOperatorsInterface = EquatableAnalyzer.TryGetEqualityOperatorsInterface(psiModule);
        Debug.Assert(equalityOperatorsInterface is { });

        Debug.Assert(Highlighting.Declaration.DeclaredElement is { });

        var type = TypeFactory.CreateType(Highlighting.Declaration.DeclaredElement);

        Highlighting.Declaration.AddSuperInterface(
            TypeFactory.CreateType(
                equalityOperatorsInterface,
                new IType[] { type, type, TypeFactory.CreateTypeByCLRName(PredefinedType.BOOLEAN_FQN, psiModule) }),
            false);
    }
}