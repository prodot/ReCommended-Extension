using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.Contracts;
using System.Linq;
using JetBrains.Annotations;
using JetBrains.ReSharper.Feature.Services.CSharp.Analyses.Bulbs;
using JetBrains.ReSharper.Intentions.Util;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.CSharp;
using JetBrains.ReSharper.Psi.CSharp.Tree;
using JetBrains.ReSharper.Psi.Modules;
using JetBrains.ReSharper.Psi.Tree;
using JetBrains.ReSharper.Psi.Util;

namespace ReCommendedExtension.ContextActions.CodeContracts.Internal
{
    internal abstract class ContractInfo
    {
        sealed class ContractStatementInfo
        {
            [NotNull]
            [ItemNotNull]
            public static IList<ContractStatementInfo> CreateContractStatementInfos([NotNull] IBlock body)
            {
                var list = new List<ContractStatementInfo>();

                foreach (var statement in body.StatementsEnumerable)
                {
                    if (statement is IExpressionStatement expressionStatement)
                    {
                        switch (expressionStatement.TryGetContractName())
                        {
                            case nameof(Contract.Requires):
                                list.Add(new ContractStatementInfo(ContractKind.Requires, expressionStatement));
                                continue;

                            case nameof(Contract.Ensures):
                                list.Add(new ContractStatementInfo(ContractKind.Ensures, expressionStatement));
                                continue;

                            case nameof(Contract.EnsuresOnThrow):
                                list.Add(new ContractStatementInfo(ContractKind.EnsuresOnThrow, expressionStatement));
                                continue;

                            case nameof(Contract.Invariant):
                                list.Add(new ContractStatementInfo(ContractKind.Invariant, expressionStatement));
                                continue;
                        }

                        break;
                    }
                }

                return list;
            }

            ContractStatementInfo(ContractKind contractKind, [NotNull] ICSharpStatement statement)
            {
                ContractKind = contractKind;
                Statement = statement;
            }

            public ContractKind ContractKind { get; }

            [NotNull]
            public ICSharpStatement Statement { get; }
        }

        protected static bool CanAcceptContracts([NotNull] ITypeMember typeMember)
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

        [NotNull]
        static ICSharpStatement CreateContractStatement(
            ContractKind contractKind,
            [NotNull] IPsiModule psiModule,
            [NotNull] IExpression contractExpression)
        {
            var factory = CSharpElementFactory.GetInstance(contractExpression);

            var contractType = TypeElementUtil.GetTypeElementByClrName(PredefinedType.CONTRACT_FQN, psiModule);

            switch (contractKind)
            {
                case ContractKind.Requires:
                    return factory.CreateStatement(string.Format("$0.{0}($1);", nameof(Contract.Requires)), contractType, contractExpression);

                case ContractKind.Ensures:
                    return factory.CreateStatement(string.Format("$0.{0}($1);", nameof(Contract.Ensures)), contractType, contractExpression);

                case ContractKind.Invariant:
                    return factory.CreateStatement(string.Format("$0.{0}($1);", nameof(Contract.Invariant)), contractType, contractExpression);

                default: throw new ArgumentOutOfRangeException(nameof(contractKind));
            }
        }

        protected static void AddContract(
            ContractKind contractKind,
            [NotNull] IBlock body,
            [NotNull] IPsiModule psiModule,
            [NotNull] Func<IExpression> getContractExpression,
            out ICSharpStatement firstNonContractStatement)
        {
            var contractExpression = getContractExpression();

            Debug.Assert(contractExpression != null);

            var statement = CreateContractStatement(contractKind, psiModule, contractExpression);

            var contractStatements = ContractStatementInfo.CreateContractStatementInfos(body);

            switch (contractKind)
            {
                case ContractKind.Requires:
                    var lastRequiresStatement = (from s in contractStatements where s.ContractKind == ContractKind.Requires select s.Statement)
                        .LastOrDefault();
                    if (lastRequiresStatement != null)
                    {
                        statement = body.AddStatementAfter(statement, lastRequiresStatement);

                        firstNonContractStatement = null;
                    }
                    else
                    {
                        var firstEnsuresOrEnsuresOnThrowStatement =
                        (
                            from s in contractStatements
                            where s.ContractKind == ContractKind.Ensures || s.ContractKind == ContractKind.EnsuresOnThrow
                            select s.Statement).FirstOrDefault();
                        if (firstEnsuresOrEnsuresOnThrowStatement != null)
                        {
                            statement = body.AddStatementBefore(statement, firstEnsuresOrEnsuresOnThrowStatement);

                            firstNonContractStatement = null;
                        }
                        else
                        {
                            firstNonContractStatement = body.StatementsEnumerable.FirstOrDefault();
                            statement = body.AddStatementBefore(statement, firstNonContractStatement);
                        }
                    }
                    break;

                case ContractKind.Ensures:
                    var lastEnsuresOrLastRequiresStatement =
                        (from s in contractStatements where s.ContractKind == ContractKind.Ensures select s.Statement).LastOrDefault() ??
                        (from s in contractStatements where s.ContractKind == ContractKind.Requires select s.Statement).LastOrDefault();
                    if (lastEnsuresOrLastRequiresStatement != null)
                    {
                        statement = body.AddStatementAfter(statement, lastEnsuresOrLastRequiresStatement);

                        firstNonContractStatement = null;
                    }
                    else
                    {
                        var lastEnsuresOnThrowStatement =
                            (from s in contractStatements where s.ContractKind == ContractKind.EnsuresOnThrow select s.Statement).FirstOrDefault();
                        if (lastEnsuresOnThrowStatement != null)
                        {
                            statement = body.AddStatementBefore(statement, lastEnsuresOnThrowStatement);

                            firstNonContractStatement = null;
                        }
                        else
                        {
                            firstNonContractStatement = body.StatementsEnumerable.FirstOrDefault();
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

        public static ContractInfo TryCreate(
            IDeclaration declaration,
            TreeTextRange selectedTreeRange,
            [NotNull] Func<IType, bool> isAvailableForType)
        {
            switch (declaration)
            {
                case IParameterDeclaration parameterDeclaration: return ParameterContractInfo.TryCreate(parameterDeclaration, isAvailableForType);

                case IMethodDeclaration methodDeclaration:
                    return MethodContractInfo.TryCreate(methodDeclaration, selectedTreeRange, isAvailableForType);

                case IPropertyDeclaration propertyDeclaration:
                    return PropertyContractInfo.TryCreate(propertyDeclaration, selectedTreeRange, isAvailableForType);

                case IIndexerDeclaration indexerDeclaration:
                    return PropertyContractInfo.TryCreate(indexerDeclaration, selectedTreeRange, isAvailableForType);

                case IFieldDeclaration fieldDeclaration: return FieldContractInfo.TryCreate(fieldDeclaration, isAvailableForType);

                case IOperatorDeclaration operatorDeclaration:
                    return OperatorContractInfo.TryCreate(operatorDeclaration, selectedTreeRange, isAvailableForType);

                default: return null;
            }
        }

        protected ContractInfo(ContractKind contractKind, [NotNull] IType type)
        {
            Debug.Assert(
                contractKind == ContractKind.Requires ||
                contractKind == ContractKind.Ensures ||
                contractKind == ContractKind.RequiresAndEnsures ||
                contractKind == ContractKind.Invariant);

            ContractKind = contractKind;
            Type = type;
        }

        protected ContractKind ContractKind { get; }

        [NotNull]
        protected IType Type { get; }

        [NotNull]
        public string GetContractKindForUI()
        {
            switch (ContractKind)
            {
                case ContractKind.Requires: return "requires";

                case ContractKind.Ensures: return "ensures";

                case ContractKind.RequiresAndEnsures: return "requires & ensures";

                case ContractKind.Invariant: return "invariant";

                default: throw new NotSupportedException();
            }
        }

        [NotNull]
        public abstract string GetContractIdentifierForUI();

        public abstract void AddContracts(
            [NotNull] ICSharpContextActionDataProvider provider,
            [NotNull] Func<IExpression, IExpression> getContractExpression,
            [ItemNotNull] out ICollection<ICSharpStatement> firstNonContractStatements);
    }
}