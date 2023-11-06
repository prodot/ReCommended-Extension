using JetBrains.ReSharper.Feature.Services.CSharp.ContextActions;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.CSharp;
using JetBrains.ReSharper.Psi.CSharp.Tree;
using JetBrains.ReSharper.Psi.Tree;

namespace ReCommendedExtension.ContextActions.CodeContracts.Internal;

internal sealed record FieldContractInfo : ContractInfo
{
    public static FieldContractInfo? TryCreate(IFieldDeclaration declaration, Func<IType, bool> isAvailableForType)
    {
        if (declaration is { IsStatic: false, DeclaredElement: { } }
            && declaration.GetContainingTypeDeclaration() is IClassLikeDeclaration classLikeDeclaration
            && isAvailableForType(declaration.DeclaredElement.Type))
        {
            return new FieldContractInfo(declaration, classLikeDeclaration, declaration.DeclaredElement.Type);
        }

        return null;
    }

    readonly IFieldDeclaration declaration;
    readonly IClassLikeDeclaration classLikeDeclaration;

    FieldContractInfo(IFieldDeclaration declaration, IClassLikeDeclaration classLikeDeclaration, IType type) : base(ContractKind.Invariant, type)
    {
        this.declaration = declaration;
        this.classLikeDeclaration = classLikeDeclaration;
    }

    public override string GetContractIdentifierForUI() => declaration.DeclaredName;

    public override void AddContracts(
        ICSharpContextActionDataProvider provider,
        Func<IExpression, IExpression> getContractExpression,
        out ICollection<ICSharpStatement>? firstNonContractStatements)
    {
        var contractInvariantMethodDeclaration = classLikeDeclaration.EnsureContractInvariantMethod(provider.PsiModule);

        if (contractInvariantMethodDeclaration.Body is { })
        {
            var factory = CSharpElementFactory.GetInstance(declaration);

            var expression = factory.CreateExpression("$0", declaration.DeclaredElement);

            AddContract(
                ContractKind.Invariant,
                contractInvariantMethodDeclaration.Body,
                provider.PsiModule,
                () => getContractExpression(expression),
                out var firstNonContractStatement);
            firstNonContractStatements = firstNonContractStatement is { } ? new[] { firstNonContractStatement } : null;
        }
        else
        {
            firstNonContractStatements = null;
        }
    }
}