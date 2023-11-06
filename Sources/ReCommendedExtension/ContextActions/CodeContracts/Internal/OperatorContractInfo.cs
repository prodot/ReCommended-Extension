using System.Diagnostics.Contracts;
using JetBrains.ReSharper.Feature.Services.CSharp.ContextActions;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.CSharp;
using JetBrains.ReSharper.Psi.CSharp.Tree;
using JetBrains.ReSharper.Psi.Tree;
using JetBrains.ReSharper.Psi.Util;

namespace ReCommendedExtension.ContextActions.CodeContracts.Internal;

internal sealed record OperatorContractInfo : ContractInfo
{
    public static OperatorContractInfo? TryCreate(
        IOperatorDeclaration declaration,
        TreeTextRange selectedTreeRange,
        Func<IType, bool> isAvailableForType)
    {
        if (declaration.GetNameRange().Contains(selectedTreeRange) && declaration.ArrowClause is not { })
        {
            var operatorElement = declaration.DeclaredElement;
            Debug.Assert(operatorElement is { });

            if (CanAcceptContracts(operatorElement) && isAvailableForType(operatorElement.ReturnType))
            {
                return new OperatorContractInfo(declaration, operatorElement.ReturnType);
            }
        }

        return null;
    }

    readonly IOperatorDeclaration declaration;

    OperatorContractInfo(IOperatorDeclaration declaration, IType type) : base(ContractKind.Ensures, type) => this.declaration = declaration;

    public override string GetContractIdentifierForUI() => "result";

    public override void AddContracts(
        ICSharpContextActionDataProvider provider,
        Func<IExpression, IExpression> getContractExpression,
        out ICollection<ICSharpStatement>? firstNonContractStatements)
    {
        if (declaration.Body is { })
        {
            Debug.Assert(declaration.DeclaredElement is { });

            var factory = CSharpElementFactory.GetInstance(declaration);

            var contractType = TypeElementUtil.GetTypeElementByClrName(PredefinedType.CONTRACT_FQN, provider.PsiModule);

            var expression = factory.CreateExpression($"$0.{nameof(Contract.Result)}<$1>()", contractType, declaration.DeclaredElement.ReturnType);

            AddContract(
                ContractKind.Ensures,
                declaration.Body,
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