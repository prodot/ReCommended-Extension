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
    internal sealed class PropertyContractInfo : ContractInfo
    {
        public static PropertyContractInfo TryCreate(
            [NotNull] IPropertyDeclaration declaration, TreeOffset treeOffset, [NotNull] Func<IType, bool> isAvailableForType)
        {
            if (declaration.GetNameRange().Contains(treeOffset) && declaration.ArrowExpression == null)
            {
                var property = declaration.DeclaredElement;

                Debug.Assert(property != null);

                if (CanAcceptContracts(property) && isAvailableForType(property.Type))
                {
                    var contractKind = declaration.IsAuto
                                           ? (declaration.IsStatic ? (ContractKind?)null : ContractKind.Invariant)
                                           : property.IsReadable
                                                 ? (property.IsWritable ? ContractKind.RequiresAndEnsures : ContractKind.Ensures)
                                                 : (property.IsWritable ? (ContractKind?)ContractKind.Requires : null);
                    if (contractKind != null)
                    {
                        return new PropertyContractInfo((ContractKind)contractKind, declaration, property.Type);
                    }
                }
            }

            return null;
        }

        public static PropertyContractInfo TryCreate(
            [NotNull] IIndexerDeclaration declaration, TreeOffset treeOffset, [NotNull] Func<IType, bool> isAvailableForType)
        {
            if (declaration.GetNameRange().Contains(treeOffset) && declaration.ArrowExpression == null)
            {
                var property = declaration.DeclaredElement;

                Debug.Assert(property != null);

                if (CanAcceptContracts(property) && isAvailableForType(property.Type))
                {
                    var contractKind = property.IsReadable
                                           ? (property.IsWritable ? ContractKind.RequiresAndEnsures : ContractKind.Ensures)
                                           : (property.IsWritable ? (ContractKind?)ContractKind.Requires : null);
                    if (contractKind != null)
                    {
                        return new PropertyContractInfo((ContractKind)contractKind, declaration, property.Type);
                    }
                }
            }

            return null;
        }

        [NotNull]
        readonly IAccessorOwnerDeclaration declaration;

        PropertyContractInfo(ContractKind contractKind, [NotNull] IAccessorOwnerDeclaration declaration, [NotNull] IType type)
            : base(contractKind, type)
        {
            Debug.Assert(
                contractKind == ContractKind.Requires || contractKind == ContractKind.Ensures || contractKind == ContractKind.RequiresAndEnsures ||
                contractKind == ContractKind.Invariant);

            this.declaration = declaration;
        }

        public override string GetContractIdentifierForUI() => declaration.DeclaredName;

        public override void AddContracts(
            ICSharpContextActionDataProvider provider,
            Func<IExpression, IExpression> getContractExpression,
            out ICollection<ICSharpStatement> firstNonContractStatements)
        {
            var factory = CSharpElementFactory.GetInstance(provider.PsiModule);

            var propertyDeclaration = declaration as IPropertyDeclaration;
            if (propertyDeclaration != null && propertyDeclaration.IsAuto)
            {
                var classLikeDeclaration = (IClassLikeDeclaration)declaration.GetContainingTypeDeclaration();

                Debug.Assert(classLikeDeclaration != null);

                var contractInvariantMethodDeclaration = classLikeDeclaration.EnsureContractInvariantMethod(provider.PsiModule);

                if (contractInvariantMethodDeclaration.Body != null)
                {
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

                return;
            }

            IEnumerable<IAccessorDeclaration> accessorDeclarations;

            if (declaration.IsAbstract)
            {
                IAccessorOwnerDeclaration overriddenAccessorOwnerDeclaration = null;

                var containingTypeDeclaration = declaration.GetContainingTypeDeclaration();

                Debug.Assert(containingTypeDeclaration != null);

                var contractClassDeclaration = containingTypeDeclaration.EnsureContractClass(provider.PsiModule);

                if (propertyDeclaration != null)
                {
                    overriddenAccessorOwnerDeclaration = propertyDeclaration.EnsureOverriddenPropertyInContractClass(
                        contractClassDeclaration, provider.PsiModule);
                }

                var indexerDeclaration = declaration as IIndexerDeclaration;
                if (indexerDeclaration != null)
                {
                    overriddenAccessorOwnerDeclaration = indexerDeclaration.EnsureOverriddenIndexerInContractClass(
                        contractClassDeclaration, provider.PsiModule);
                }

                Debug.Assert(overriddenAccessorOwnerDeclaration != null);

                accessorDeclarations = overriddenAccessorOwnerDeclaration.AccessorDeclarations;
            }
            else
            {
                accessorDeclarations = declaration.AccessorDeclarations;
            }

            firstNonContractStatements = new List<ICSharpStatement>(2);

            foreach (var accessorDeclaration in accessorDeclarations)
            {
                Debug.Assert(accessorDeclaration != null);

                if (accessorDeclaration.Body != null)
                {
                    switch (accessorDeclaration.Kind)
                    {
                        case AccessorKind.GETTER:
                        {
                            var contractType = new DeclaredTypeFromCLRName(ClrTypeNames.Contract, provider.PsiModule).GetTypeElement();

                            var resultExpression = factory.CreateExpression(
                                string.Format("$0.{0}<$1>()", nameof(Contract.Result)),
                                contractType,
                                Type);

                            ICSharpStatement firstNonContractStatement;
                            AddContract(
                                ContractKind.Ensures,
                                accessorDeclaration.Body,
                                provider.PsiModule,
                                () => getContractExpression(resultExpression),
                                out firstNonContractStatement);

                            if (firstNonContractStatement != null)
                            {
                                firstNonContractStatements.Add(firstNonContractStatement);
                            }
                            break;
                        }

                        case AccessorKind.SETTER:
                        {
                            var valueExpression = factory.CreateExpression("value");

                            ICSharpStatement firstNonContractStatement;
                            AddContract(
                                ContractKind.Requires,
                                accessorDeclaration.Body,
                                provider.PsiModule,
                                () => getContractExpression(valueExpression),
                                out firstNonContractStatement);

                            if (firstNonContractStatement != null)
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
}