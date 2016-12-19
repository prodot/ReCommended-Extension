using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.Contracts;
using JetBrains.Annotations;
using JetBrains.ReSharper.Feature.Services.CSharp.Analyses.Bulbs;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.CSharp;
using JetBrains.ReSharper.Psi.CSharp.Tree;
using JetBrains.ReSharper.Psi.Impl.Types;
using JetBrains.ReSharper.Psi.Tree;

namespace ReCommendedExtension.ContextActions.CodeContracts.Internal
{
    internal sealed class MethodContractInfo : ContractInfo
    {
        public static MethodContractInfo TryCreate(
            [NotNull] IMethodDeclaration declaration, TreeTextRange selectedTreeRange, [NotNull] Func<IType, bool> isAvailableForType)
        {
            if (declaration.GetNameRange().Contains(selectedTreeRange) && declaration.ArrowExpression == null)
            {
                var method = declaration.DeclaredElement;

                if (method != null && CanAcceptContracts(method) && !method.ReturnType.IsVoid() && isAvailableForType(method.ReturnType))
                {
                    return new MethodContractInfo(declaration, method.ReturnType);
                }
            }

            return null;
        }

        [NotNull]
        readonly IMethodDeclaration declaration;

        MethodContractInfo([NotNull] IMethodDeclaration declaration, [NotNull] IType type) : base(ContractKind.Ensures, type)
        {
            this.declaration = declaration;
        }

        public override string GetContractIdentifierForUI() => "result";

        public override void AddContracts(
            ICSharpContextActionDataProvider provider,
            Func<IExpression, IExpression> getContractExpression,
            out ICollection<ICSharpStatement> firstNonContractStatements)
        {
            var factory = CSharpElementFactory.GetInstance(provider.PsiModule);

            IBlock body;

            if (declaration.IsAbstract)
            {
                var containingTypeDeclaration = declaration.GetContainingTypeDeclaration();

                Debug.Assert(containingTypeDeclaration != null);

                var contractClassDeclaration = containingTypeDeclaration.EnsureContractClass(provider.PsiModule);

                var overriddenMethodDeclaration = declaration.EnsureOverriddenMethodInContractClass(contractClassDeclaration, provider.PsiModule);

                body = overriddenMethodDeclaration.Body;
            }
            else
            {
                body = declaration.Body;
            }

            if (body != null)
            {
                var contractType = new DeclaredTypeFromCLRName(ClrTypeNames.Contract, provider.PsiModule).GetTypeElement();

                var declaredElement = declaration.DeclaredElement;

                Debug.Assert(declaredElement != null);

                var expression = factory.CreateExpression(
                    string.Format("$0.{0}<$1>()", nameof(Contract.Result)),
                    contractType,
                    declaredElement.ReturnType);

                ICSharpStatement firstNonContractStatement;

                AddContract(ContractKind.Ensures, body, provider.PsiModule, () => getContractExpression(expression), out firstNonContractStatement);

                firstNonContractStatements = firstNonContractStatement != null ? new[] { firstNonContractStatement } : null;
            }
            else
            {
                firstNonContractStatements = null;
            }
        }
    }
}