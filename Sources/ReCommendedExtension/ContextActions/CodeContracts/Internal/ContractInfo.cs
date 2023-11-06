using System.Diagnostics.Contracts;
using JetBrains.ReSharper.Feature.Services.CSharp.ContextActions;
using JetBrains.ReSharper.Intentions.Util;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.CSharp;
using JetBrains.ReSharper.Psi.CSharp.Tree;
using JetBrains.ReSharper.Psi.Modules;
using JetBrains.ReSharper.Psi.Tree;
using JetBrains.ReSharper.Psi.Util;

namespace ReCommendedExtension.ContextActions.CodeContracts.Internal;

internal abstract record ContractInfo
{
    sealed record ContractStatementInfo
    {
        public static IList<ContractStatementInfo> CreateContractStatementInfos(IBlock body)
        {
            var list = new List<ContractStatementInfo>();

            foreach (var statement in body.Statements)
            {
                if (statement is IExpressionStatement expressionStatement)
                {
                    switch (expressionStatement.TryGetContractName())
                    {
                        case nameof(Contract.Requires):
                            list.Add(new ContractStatementInfo { ContractKind = ContractKind.Requires,Statement = expressionStatement});
                            continue;

                        case nameof(Contract.Ensures):
                            list.Add(new ContractStatementInfo { ContractKind = ContractKind.Ensures, Statement = expressionStatement });
                            continue;

                        case nameof(Contract.EnsuresOnThrow):
                            list.Add(new ContractStatementInfo { ContractKind = ContractKind.EnsuresOnThrow, Statement = expressionStatement });
                            continue;

                        case nameof(Contract.Invariant):
                            list.Add(new ContractStatementInfo { ContractKind = ContractKind.Invariant,Statement = expressionStatement});
                            continue;
                    }

                    break;
                }
            }

            return list;
        }

        public required ContractKind ContractKind { get; init; }

        public required ICSharpStatement Statement { get; init; }
    }

    protected static bool CanAcceptContracts(ITypeMember typeMember)
    {
        if (typeMember.IsExtern)
        {
            return false;
        }

        if (typeMember is IOverridableMember overridableMember && overridableMember.GetImmediateSuperMembers().Any())
        {
            return false;
        }

        return true;
    }

    static ICSharpStatement CreateContractStatement(
        ContractKind contractKind,
        IPsiModule psiModule,
        IExpression contractExpression)
    {
        var factory = CSharpElementFactory.GetInstance(contractExpression);

        var contractType = TypeElementUtil.GetTypeElementByClrName(PredefinedType.CONTRACT_FQN, psiModule);

        return contractKind switch
        {
            ContractKind.Requires => factory.CreateStatement($"$0.{nameof(Contract.Requires)}($1);", contractType, contractExpression),
            ContractKind.Ensures => factory.CreateStatement($"$0.{nameof(Contract.Ensures)}($1);", contractType, contractExpression),
            ContractKind.Invariant => factory.CreateStatement($"$0.{nameof(Contract.Invariant)}($1);", contractType, contractExpression),

            _ => throw new ArgumentOutOfRangeException(nameof(contractKind)),
        };
    }

    protected static void AddContract(
        ContractKind contractKind,
        IBlock body,
        IPsiModule psiModule,
        Func<IExpression> getContractExpression,
        out ICSharpStatement? firstNonContractStatement)
    {
        var contractExpression = getContractExpression();

        var statement = CreateContractStatement(contractKind, psiModule, contractExpression);

        var contractStatements = ContractStatementInfo.CreateContractStatementInfos(body);

        switch (contractKind)
        {
            case ContractKind.Requires:
                var lastRequiresStatement = (from s in contractStatements where s.ContractKind == ContractKind.Requires select s.Statement)
                    .LastOrDefault();
                if (lastRequiresStatement is { })
                {
                    statement = body.AddStatementAfter(statement, lastRequiresStatement);

                    firstNonContractStatement = null;
                }
                else
                {
                    var firstEnsuresOrEnsuresOnThrowStatement =
                        (from s in contractStatements where s.ContractKind is ContractKind.Ensures or ContractKind.EnsuresOnThrow select s.Statement)
                        .FirstOrDefault();
                    if (firstEnsuresOrEnsuresOnThrowStatement is { })
                    {
                        statement = body.AddStatementBefore(statement, firstEnsuresOrEnsuresOnThrowStatement);

                        firstNonContractStatement = null;
                    }
                    else
                    {
                        firstNonContractStatement = body.Statements.FirstOrDefault();
                        statement = body.AddStatementBefore(statement, firstNonContractStatement);
                    }
                }
                break;

            case ContractKind.Ensures:
                var lastEnsuresOrLastRequiresStatement =
                    (from s in contractStatements where s.ContractKind == ContractKind.Ensures select s.Statement).LastOrDefault()
                    ?? (from s in contractStatements where s.ContractKind == ContractKind.Requires select s.Statement).LastOrDefault();
                if (lastEnsuresOrLastRequiresStatement is { })
                {
                    statement = body.AddStatementAfter(statement, lastEnsuresOrLastRequiresStatement);

                    firstNonContractStatement = null;
                }
                else
                {
                    var lastEnsuresOnThrowStatement =
                        (from s in contractStatements where s.ContractKind == ContractKind.EnsuresOnThrow select s.Statement).FirstOrDefault();
                    if (lastEnsuresOnThrowStatement is { })
                    {
                        statement = body.AddStatementBefore(statement, lastEnsuresOnThrowStatement);

                        firstNonContractStatement = null;
                    }
                    else
                    {
                        firstNonContractStatement = body.Statements.FirstOrDefault();
                        body.AddStatementBefore(statement, firstNonContractStatement);
                    }
                }
                break;

            case ContractKind.Invariant:
                var lastInvariantStatement = (from s in contractStatements where s.ContractKind == ContractKind.Invariant select s.Statement)
                    .LastOrDefault();
                statement = body.AddStatementAfter(statement, lastInvariantStatement);

                firstNonContractStatement = null;

                break;

            default: throw new ArgumentOutOfRangeException(nameof(contractKind));
        }

        ContextActionUtils.FormatWithDefaultProfile(statement);
    }

    public static ContractInfo? TryCreate(IDeclaration? declaration, TreeTextRange selectedTreeRange, Func<IType, bool> isAvailableForType)
        => declaration switch
        {
            IParameterDeclaration parameterDeclaration => ParameterContractInfo.TryCreate(parameterDeclaration, isAvailableForType),
            IMethodDeclaration methodDeclaration => MethodContractInfo.TryCreate(methodDeclaration, selectedTreeRange, isAvailableForType),
            IPropertyDeclaration propertyDeclaration => PropertyContractInfo.TryCreate(propertyDeclaration, selectedTreeRange, isAvailableForType),
            IIndexerDeclaration indexerDeclaration => PropertyContractInfo.TryCreate(indexerDeclaration, selectedTreeRange, isAvailableForType),
            IFieldDeclaration fieldDeclaration => FieldContractInfo.TryCreate(fieldDeclaration, isAvailableForType),
            IOperatorDeclaration operatorDeclaration => OperatorContractInfo.TryCreate(operatorDeclaration, selectedTreeRange, isAvailableForType),

            _ => null,
        };

    protected ContractInfo(ContractKind contractKind, IType type)
    {
        Debug.Assert(contractKind is ContractKind.Requires or ContractKind.Ensures or ContractKind.RequiresAndEnsures or ContractKind.Invariant);

        ContractKind = contractKind;
        Type = type;
    }

    protected ContractKind ContractKind { get; }

    protected IType Type { get; }

    public string GetContractKindForUI()
        => ContractKind switch
        {
            ContractKind.Requires => "requires",
            ContractKind.Ensures => "ensures",
            ContractKind.RequiresAndEnsures => "requires & ensures",
            ContractKind.Invariant => "invariant",

            _ => throw new NotSupportedException(),
        };

    public abstract string GetContractIdentifierForUI();

    public abstract void AddContracts(
        ICSharpContextActionDataProvider provider,
        Func<IExpression, IExpression> getContractExpression,
        out ICollection<ICSharpStatement>? firstNonContractStatements);
}