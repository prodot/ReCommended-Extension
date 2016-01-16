using System;
using System.Collections.Generic;
using System.Diagnostics;
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
        public static FieldContractInfo TryCreate([NotNull] IFieldDeclaration declaration, [NotNull] Func<IType, bool> isAvailableForType)
        {
            if (!declaration.IsStatic)
            {
                var classLikeDeclaration = declaration.GetContainingTypeDeclaration() as IClassLikeDeclaration;

                Debug.Assert(declaration.DeclaredElement != null);

                if (classLikeDeclaration != null && isAvailableForType(declaration.DeclaredElement.Type))
                {
                    return new FieldContractInfo(declaration, classLikeDeclaration, declaration.DeclaredElement.Type);
                }
            }

            return null;
        }

        [NotNull]
        readonly IFieldDeclaration declaration;

        [NotNull]
        readonly IClassLikeDeclaration classLikeDeclaration;

        FieldContractInfo([NotNull] IFieldDeclaration declaration, [NotNull] IClassLikeDeclaration classLikeDeclaration, [NotNull] IType type)
            : base(ContractKind.Invariant, type)
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
            Debug.Assert(provider.PsiModule != null);

            var contractInvariantMethodDeclaration = classLikeDeclaration.EnsureContractInvariantMethod(provider.PsiModule);

            if (contractInvariantMethodDeclaration.Body != null)
            {
                var factory = CSharpElementFactory.GetInstance(provider.PsiModule);

                var expression = factory.CreateExpression("$0", declaration.DeclaredElement);

                ICSharpStatement firstNonContractStatement;

                AddContract(
                    ContractKind.Invariant,
                    contractInvariantMethodDeclaration.Body,
                    provider.PsiModule,
                    () => getContractExpression(expression),
                    out firstNonContractStatement);
                firstNonContractStatements = firstNonContractStatement != null ? new[] { firstNonContractStatement } : null;
            }
            else
            {
                firstNonContractStatements = null;
            }
        }
    }
}