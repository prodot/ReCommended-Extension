using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.Contracts;
using JetBrains.Annotations;
using JetBrains.ReSharper.Feature.Services.CSharp.Analyses.Bulbs;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.CSharp;
using JetBrains.ReSharper.Psi.CSharp.Tree;
using JetBrains.ReSharper.Psi.Tree;
using JetBrains.ReSharper.Psi.Util;

namespace ReCommendedExtension.ContextActions.CodeContracts.Internal
{
    internal sealed class OperatorContractInfo : ContractInfo
    {
        public static OperatorContractInfo TryCreate(
            [NotNull] IOperatorDeclaration declaration,
            TreeTextRange selectedTreeRange,
            [NotNull] Func<IType, bool> isAvailableForType)
        {
            if (declaration.GetNameRange().Contains(selectedTreeRange) && declaration.ArrowClause == null)
            {
                var operatorElement = declaration.DeclaredElement;

                Debug.Assert(operatorElement != null);

                if (CanAcceptContracts(operatorElement) && isAvailableForType(operatorElement.ReturnType))
                {
                    return new OperatorContractInfo(declaration, operatorElement.ReturnType);
                }
            }

            return null;
        }

        [NotNull]
        readonly IOperatorDeclaration declaration;

        OperatorContractInfo([NotNull] IOperatorDeclaration declaration, [NotNull] IType type) : base(ContractKind.Ensures, type)
            => this.declaration = declaration;

        public override string GetContractIdentifierForUI() => "result";

        public override void AddContracts(
            ICSharpContextActionDataProvider provider,
            Func<IExpression, IExpression> getContractExpression,
            out ICollection<ICSharpStatement> firstNonContractStatements)
        {
            if (declaration.Body != null)
            {
                var factory = CSharpElementFactory.GetInstance(declaration);

                var contractType = TypeElementUtil.GetTypeElementByClrName(PredefinedType.CONTRACT_FQN, provider.PsiModule);

                Debug.Assert(declaration.DeclaredElement != null);

                var expression = factory.CreateExpression(
                    string.Format("$0.{0}<$1>()", nameof(Contract.Result)),
                    contractType,
                    declaration.DeclaredElement.ReturnType);

                AddContract(
                    ContractKind.Ensures,
                    declaration.Body,
                    provider.PsiModule,
                    () => getContractExpression(expression),
                    out var firstNonContractStatement);
                firstNonContractStatements = firstNonContractStatement != null ? new[] { firstNonContractStatement } : null;
            }
            else
            {
                firstNonContractStatements = null;
            }
        }
    }
}