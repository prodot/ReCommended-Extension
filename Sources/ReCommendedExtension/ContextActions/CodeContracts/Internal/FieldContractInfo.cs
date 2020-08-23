using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using JetBrains.ReSharper.Feature.Services.CSharp.Analyses.Bulbs;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.CSharp;
using JetBrains.ReSharper.Psi.CSharp.Tree;
using JetBrains.ReSharper.Psi.Tree;

namespace ReCommendedExtension.ContextActions.CodeContracts.Internal
{
    internal sealed class FieldContractInfo : ContractInfo
    {
        [CanBeNull]
        public static FieldContractInfo TryCreate([NotNull] IFieldDeclaration declaration, [NotNull] Func<IType, bool> isAvailableForType)
        {
            if (!declaration.IsStatic &&
                declaration.GetContainingTypeDeclaration() is IClassLikeDeclaration classLikeDeclaration &&
                declaration.DeclaredElement != null &&
                isAvailableForType(declaration.DeclaredElement.Type))
            {
                return new FieldContractInfo(declaration, classLikeDeclaration, declaration.DeclaredElement.Type);
            }

            return null;
        }

        [NotNull]
        readonly IFieldDeclaration declaration;

        [NotNull]
        readonly IClassLikeDeclaration classLikeDeclaration;

        FieldContractInfo([NotNull] IFieldDeclaration declaration, [NotNull] IClassLikeDeclaration classLikeDeclaration, [NotNull] IType type) : base(
            ContractKind.Invariant,
            type)
        {
            this.declaration = declaration;
            this.classLikeDeclaration = classLikeDeclaration;
        }

        public override string GetContractIdentifierForUI() => declaration.DeclaredName;

        public override void AddContracts(
            ICSharpContextActionDataProvider provider,
            Func<IExpression, IExpression> getContractExpression,
            out ICollection<ICSharpStatement> firstNonContractStatements)
        {
            var contractInvariantMethodDeclaration = classLikeDeclaration.EnsureContractInvariantMethod(provider.PsiModule);

            if (contractInvariantMethodDeclaration.Body != null)
            {
                var factory = CSharpElementFactory.GetInstance(declaration);

                var expression = factory.CreateExpression("$0", declaration.DeclaredElement);

                AddContract(
                    ContractKind.Invariant,
                    contractInvariantMethodDeclaration.Body,
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