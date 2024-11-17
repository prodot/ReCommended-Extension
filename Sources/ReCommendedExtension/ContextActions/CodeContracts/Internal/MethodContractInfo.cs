﻿using System.Diagnostics.Contracts;
using JetBrains.ReSharper.Feature.Services.CSharp.ContextActions;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.CSharp;
using JetBrains.ReSharper.Psi.CSharp.Tree;
using JetBrains.ReSharper.Psi.Tree;

namespace ReCommendedExtension.ContextActions.CodeContracts.Internal;

internal sealed record MethodContractInfo : ContractInfo
{
    [JetBrains.Annotations.Pure]
    public static MethodContractInfo? TryCreate(IMethodDeclaration declaration, TreeTextRange selectedTreeRange, Func<IType, bool> isAvailableForType)
    {
        if (declaration.GetNameRange().Contains(selectedTreeRange) && declaration.ArrowClause is not { })
        {
            var method = declaration.DeclaredElement;

            if (method is { } && CanAcceptContracts(method) && !method.ReturnType.IsVoid() && isAvailableForType(method.ReturnType))
            {
                return new MethodContractInfo(declaration, method.ReturnType);
            }
        }

        return null;
    }

    readonly IMethodDeclaration declaration;

    MethodContractInfo(IMethodDeclaration declaration, IType type) : base(ContractKind.Ensures, type) => this.declaration = declaration;

    public override string GetContractIdentifierForUI() => "result";

    public override void AddContracts(
        ICSharpContextActionDataProvider provider,
        Func<IExpression, IExpression> getContractExpression,
        out ICollection<ICSharpStatement>? firstNonContractStatements)
    {
        var factory = CSharpElementFactory.GetInstance(declaration);

        IBlock body;

        var containingTypeDeclaration = declaration.GetContainingTypeDeclaration();
        Debug.Assert(containingTypeDeclaration is { });

        if (declaration.IsAbstract || containingTypeDeclaration.IsAbstract || containingTypeDeclaration is IInterfaceDeclaration)
        {
            var contractClassDeclaration = EnsureContractClass(containingTypeDeclaration, provider.PsiModule);

            var overriddenMethodDeclaration = EnsureOverriddenMethodInContractClass(declaration, contractClassDeclaration);

            body = overriddenMethodDeclaration.Body;
        }
        else
        {
            body = declaration.Body;
        }

        if (body is { })
        {
            var contractType = PredefinedType.CONTRACT_FQN.TryGetTypeElement(provider.PsiModule);

            var declaredElement = declaration.DeclaredElement;
            Debug.Assert(declaredElement is { });

            var expression = factory.CreateExpression($"$0.{nameof(Contract.Result)}<$1>()", contractType, declaredElement.ReturnType);

            AddContract(
                ContractKind.Ensures,
                body,
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