using System.Diagnostics.Contracts;
using JetBrains.ReSharper.Feature.Services.CSharp.ContextActions;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.CSharp;
using JetBrains.ReSharper.Psi.CSharp.Tree;
using JetBrains.ReSharper.Psi.Tree;

namespace ReCommendedExtension.ContextActions.CodeContracts.Internal;

internal sealed record ParameterContractInfo : ContractInfo
{
    static void AddContractForRequires(
        ICSharpContextActionDataProvider provider,
        Func<IExpression, IExpression> getContractExpression,
        IParameter parameter,
        IBlock body,
        out ICSharpStatement? firstNonContractStatement)
    {
        var factory = CSharpElementFactory.GetInstance(body);

        var parameterExpression = factory.CreateExpression("$0", parameter);

        AddContract(ContractKind.Requires, body, provider.PsiModule, () => getContractExpression(parameterExpression), out firstNonContractStatement);
    }

    static void AddContractForEnsures(
        ICSharpContextActionDataProvider provider,
        Func<IExpression, IExpression> getContractExpression,
        IParameter parameter,
        IBlock body,
        out ICSharpStatement? firstNonContractStatement)
    {
        var factory = CSharpElementFactory.GetInstance(body);

        var contractType = PredefinedType.CONTRACT_FQN.TryGetTypeElement(provider.PsiModule);

        var parameterExpression = factory.CreateExpression("$0", parameter);

        var expression = factory.CreateExpression($"$0.{nameof(Contract.ValueAtReturn)}(out $1)", contractType, parameterExpression);

        AddContract(ContractKind.Ensures, body, provider.PsiModule, () => getContractExpression(expression), out firstNonContractStatement);
    }

    [JetBrains.Annotations.Pure]
    public static ParameterContractInfo? TryCreate(IParameterDeclaration declaration, Func<IType, bool> isAvailableForType)
    {
        var expressionBodyOwnerDeclaration = declaration.PathToRoot().OfType<IExpressionBodyOwnerDeclaration>().FirstOrDefault();
        if (expressionBodyOwnerDeclaration is { ArrowClause: { } }
            or IPropertyDeclaration { AccessorDeclarations: [] or [{ ArrowClause: { } }] or [{ ArrowClause: { } }, { ArrowClause: { } }] }
            or IIndexerDeclaration { AccessorDeclarations: [] or [{ ArrowClause: { } }] or [{ ArrowClause: { } }, { ArrowClause: { } }] })
        {
            return null;
        }

        var parameter = declaration.DeclaredElement;

        if (parameter.ContainingParametersOwner is ITypeMember typeMember && CanAcceptContracts(typeMember) && isAvailableForType(parameter.Type))
        {
            var contractKind = parameter.Kind switch
            {
                ParameterKind.VALUE => ContractKind.Requires,
                ParameterKind.REFERENCE => ContractKind.RequiresAndEnsures,
                ParameterKind.OUTPUT => ContractKind.Ensures,

                _ => null as ContractKind?,
            };

            if (contractKind is { } c)
            {
                return new ParameterContractInfo(c, declaration, parameter.Type);
            }
        }

        return null;
    }

    readonly IParameterDeclaration declaration;

    ParameterContractInfo(ContractKind contractKind, IParameterDeclaration declaration, IType type) : base(contractKind, type)
    {
        Debug.Assert(contractKind is ContractKind.Requires or ContractKind.Ensures or ContractKind.RequiresAndEnsures);

        this.declaration = declaration;
    }

    void AddContract(
        ICSharpContextActionDataProvider provider,
        Func<IExpression, IExpression> getContractExpression,
        IParameter parameter,
        IBlock body,
        out ICSharpStatement? firstNonContractStatement)
    {
        switch (ContractKind)
        {
            case ContractKind.Requires:
                AddContractForRequires(provider, getContractExpression, parameter, body, out firstNonContractStatement);
                break;

            case ContractKind.Ensures:
                AddContractForEnsures(provider, getContractExpression, parameter, body, out firstNonContractStatement);
                break;

            case ContractKind.RequiresAndEnsures:
                AddContractForEnsures(provider, getContractExpression, parameter, body, out firstNonContractStatement);

                AddContractForRequires(provider, getContractExpression, parameter, body, out _);
                break;

            default: throw new NotSupportedException();
        }
    }

    public override string GetContractIdentifierForUI() => declaration.DeclaredName;

    public override void AddContracts(
        ICSharpContextActionDataProvider provider,
        Func<IExpression, IExpression> getContractExpression,
        out ICollection<ICSharpStatement>? firstNonContractStatements)
    {
        var parameter = declaration.DeclaredElement;

        switch (parameter.ContainingParametersOwner)
        {
            case IFunction function
                when function.GetDeclarationsIn(provider.SourceFile).FirstOrDefault(d => Equals(d.DeclaredElement, function)) is
                    ICSharpFunctionDeclaration functionDeclaration:
            {
                IBlock body;

                if (functionDeclaration is IMethodDeclaration methodDeclaration
                    && (methodDeclaration.IsAbstract || methodDeclaration.GetContainingTypeDeclaration() is { IsAbstract: true }))
                {
                    var containingTypeDeclaration = methodDeclaration.GetContainingTypeDeclaration();
                    Debug.Assert(containingTypeDeclaration is { });

                    var contractClassDeclaration = EnsureContractClass(containingTypeDeclaration, provider.PsiModule);

                    var overriddenMethodDeclaration = EnsureOverriddenMethodInContractClass(methodDeclaration, contractClassDeclaration);

                    body = overriddenMethodDeclaration.Body;
                }
                else
                {
                    body = functionDeclaration.Body;
                }

                if (body is { })
                {
                    AddContract(provider, getContractExpression, parameter, body, out var firstNonContractStatement);
                    firstNonContractStatements = firstNonContractStatement is { } ? new[] { firstNonContractStatement } : null;
                }
                else
                {
                    firstNonContractStatements = null;
                }

                return;
            }

            case IProperty property
                when property.GetDeclarationsIn(provider.SourceFile).FirstOrDefault(d => Equals(d.DeclaredElement, property)) is
                    IIndexerDeclaration indexerDeclaration:
            {
                TreeNodeCollection<IAccessorDeclaration> accessorDeclarations;

                var containingTypeDeclaration = indexerDeclaration.GetContainingTypeDeclaration();
                Debug.Assert(containingTypeDeclaration is { });

                if (indexerDeclaration.IsAbstract || containingTypeDeclaration.IsAbstract)
                {
                    var contractClassDeclaration = EnsureContractClass(containingTypeDeclaration, provider.PsiModule);

                    var overriddenIndexerDeclaration = EnsureOverriddenIndexerInContractClass(indexerDeclaration, contractClassDeclaration);

                    accessorDeclarations = overriddenIndexerDeclaration.AccessorDeclarations;
                }
                else
                {
                    accessorDeclarations = indexerDeclaration.AccessorDeclarations;
                }

                firstNonContractStatements = new List<ICSharpStatement>(2);
                foreach (var accessorDeclaration in accessorDeclarations)
                {
                    if (accessorDeclaration.Body is { })
                    {
                        AddContract(provider, getContractExpression, parameter, accessorDeclaration.Body, out var firstNonContractStatement);
                        if (firstNonContractStatement is { })
                        {
                            firstNonContractStatements.Add(firstNonContractStatement);
                        }
                    }
                }

                if (firstNonContractStatements.Count == 0)
                {
                    firstNonContractStatements = null;
                }

                return;
            }

            default:
                firstNonContractStatements = null;
                break;
        }
    }
}