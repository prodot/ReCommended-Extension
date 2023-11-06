using System.Diagnostics.Contracts;
using JetBrains.ReSharper.Feature.Services.CSharp.ContextActions;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.CSharp;
using JetBrains.ReSharper.Psi.CSharp.Tree;
using JetBrains.ReSharper.Psi.Tree;
using JetBrains.ReSharper.Psi.Util;

namespace ReCommendedExtension.ContextActions.CodeContracts.Internal;

internal sealed record PropertyContractInfo : ContractInfo
{
    public static PropertyContractInfo? TryCreate(
        IPropertyDeclaration declaration,
        TreeTextRange selectedTreeRange,
        Func<IType, bool> isAvailableForType)
    {
        if (declaration.GetNameRange().Contains(selectedTreeRange)
            && declaration.ArrowClause is not { }
            && declaration.AccessorDeclarations.Any(accessorDeclaration => accessorDeclaration.ArrowClause is not { }))
        {
            var property = declaration.DeclaredElement;
            Debug.Assert(property is { });

            if (CanAcceptContracts(property) && isAvailableForType(property.Type))
            {
                var contractKind = declaration.IsAuto
                    ? declaration.IsStatic ? null as ContractKind? : ContractKind.Invariant
                    : property.IsReadable
                        ? property.IsWritable ? ContractKind.RequiresAndEnsures : ContractKind.Ensures
                        : property.IsWritable
                            ? ContractKind.Requires
                            : null;

                if (contractKind is { } c)
                {
                    return new PropertyContractInfo(c, declaration, property.Type);
                }
            }
        }

        return null;
    }

    public static PropertyContractInfo? TryCreate(
        IIndexerDeclaration declaration,
        TreeTextRange selectedTreeRange,
        Func<IType, bool> isAvailableForType)
    {
        if (declaration.GetNameRange().Contains(selectedTreeRange)
            && declaration.ArrowClause is not { }
            && declaration.AccessorDeclarations.Any(accessorDeclaration => accessorDeclaration.ArrowClause is not { }))
        {
            var property = declaration.DeclaredElement;
            Debug.Assert(property is { });

            if (CanAcceptContracts(property) && isAvailableForType(property.Type))
            {
                var contractKind = property.IsReadable
                    ? property.IsWritable ? ContractKind.RequiresAndEnsures : ContractKind.Ensures
                    : property.IsWritable
                        ? ContractKind.Requires
                        : null as ContractKind?;

                if (contractKind is { } c)
                {
                    return new PropertyContractInfo(c, declaration, property.Type);
                }
            }
        }

        return null;
    }

    readonly IAccessorOwnerDeclaration declaration;

    PropertyContractInfo(ContractKind contractKind, IAccessorOwnerDeclaration declaration, IType type) : base(contractKind, type)
    {
        Debug.Assert(contractKind is ContractKind.Requires or ContractKind.Ensures or ContractKind.RequiresAndEnsures or ContractKind.Invariant);

        this.declaration = declaration;
    }

    public override string GetContractIdentifierForUI() => declaration.DeclaredName;

    public override void AddContracts(
        ICSharpContextActionDataProvider provider,
        Func<IExpression, IExpression> getContractExpression,
        out ICollection<ICSharpStatement>? firstNonContractStatements)
    {
        var factory = CSharpElementFactory.GetInstance(declaration);

        var propertyDeclaration = declaration as IPropertyDeclaration;
        if (propertyDeclaration is { IsAuto: true })
        {
            var classLikeDeclaration = (IClassLikeDeclaration?)declaration.GetContainingTypeDeclaration();
            Debug.Assert(classLikeDeclaration is { });

            var contractInvariantMethodDeclaration = classLikeDeclaration.EnsureContractInvariantMethod(provider.PsiModule);

            if (contractInvariantMethodDeclaration.Body is { })
            {
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

            return;
        }

        TreeNodeCollection<IAccessorDeclaration> accessorDeclarations;

        var containingTypeDeclaration = declaration.GetContainingTypeDeclaration();
        Debug.Assert(containingTypeDeclaration is { });

        if (declaration.IsAbstract || containingTypeDeclaration.IsAbstract)
        {
            var overriddenAccessorOwnerDeclaration = null as IAccessorOwnerDeclaration;

            var contractClassDeclaration = containingTypeDeclaration.EnsureContractClass(provider.PsiModule);

            if (propertyDeclaration is { })
            {
                overriddenAccessorOwnerDeclaration = propertyDeclaration.EnsureOverriddenPropertyInContractClass(contractClassDeclaration);
            }

            if (declaration is IIndexerDeclaration indexerDeclaration)
            {
                overriddenAccessorOwnerDeclaration = indexerDeclaration.EnsureOverriddenIndexerInContractClass(contractClassDeclaration);
            }

            Debug.Assert(overriddenAccessorOwnerDeclaration is { });

            accessorDeclarations = overriddenAccessorOwnerDeclaration.AccessorDeclarations;
        }
        else
        {
            accessorDeclarations = declaration.AccessorDeclarations;
        }

        firstNonContractStatements = new List<ICSharpStatement>(2);

        foreach (var accessorDeclaration in accessorDeclarations)
        {
            if (accessorDeclaration.Body is { })
            {
                switch (accessorDeclaration.Kind)
                {
                    case AccessorKind.GETTER:
                    {
                        var contractType = TypeElementUtil.GetTypeElementByClrName(PredefinedType.CONTRACT_FQN, provider.PsiModule);

                        var resultExpression = factory.CreateExpression($"$0.{nameof(Contract.Result)}<$1>()", contractType, Type);

                        AddContract(
                            ContractKind.Ensures,
                            accessorDeclaration.Body,
                            provider.PsiModule,
                            () => getContractExpression(resultExpression),
                            out var firstNonContractStatement);

                        if (firstNonContractStatement is { })
                        {
                            firstNonContractStatements.Add(firstNonContractStatement);
                        }
                        break;
                    }

                    case AccessorKind.SETTER:
                    {
                        var valueExpression = factory.CreateExpression("value");

                        AddContract(
                            ContractKind.Requires,
                            accessorDeclaration.Body,
                            provider.PsiModule,
                            () => getContractExpression(valueExpression),
                            out var firstNonContractStatement);

                        if (firstNonContractStatement is { })
                        {
                            firstNonContractStatements.Add(firstNonContractStatement);
                        }
                        break;
                    }
                }
            }
        }

        if (firstNonContractStatements.Count == 0)
        {
            firstNonContractStatements = null;
        }
    }
}