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
    internal sealed class PropertyContractInfo : ContractInfo
    {
        [CanBeNull]
        public static PropertyContractInfo TryCreate(
            [NotNull] IPropertyDeclaration declaration,
            TreeTextRange selectedTreeRange,
            [NotNull] Func<IType, bool> isAvailableForType)
        {
            if (declaration.GetNameRange().Contains(selectedTreeRange) &&
                declaration.ArrowClause == null &&
                declaration.AccessorDeclarations.Any(accessorDeclaration => accessorDeclaration.AssertNotNull().ArrowClause == null))
            {
                var property = declaration.DeclaredElement;

                Debug.Assert(property != null);

                if (CanAcceptContracts(property) && isAvailableForType(property.Type))
                {
                    var contractKind = declaration.IsAuto
                        ? declaration.IsStatic
                            ? (ContractKind?)null
                            : ContractKind.Invariant
                        : property.IsReadable
                            ? property.IsWritable
                                ? ContractKind.RequiresAndEnsures
                                : ContractKind.Ensures
                            : property.IsWritable
                                ? (ContractKind?)ContractKind.Requires
                                : null;
                    if (contractKind != null)
                    {
                        return new PropertyContractInfo((ContractKind)contractKind, declaration, property.Type);
                    }
                }
            }

            return null;
        }

        [CanBeNull]
        public static PropertyContractInfo TryCreate(
            [NotNull] IIndexerDeclaration declaration,
            TreeTextRange selectedTreeRange,
            [NotNull] Func<IType, bool> isAvailableForType)
        {
            if (declaration.GetNameRange().Contains(selectedTreeRange) &&
                declaration.ArrowClause == null &&
                declaration.AccessorDeclarations.Any(accessorDeclaration => accessorDeclaration.AssertNotNull().ArrowClause == null))
            {
                var property = declaration.DeclaredElement;

                Debug.Assert(property != null);

                if (CanAcceptContracts(property) && isAvailableForType(property.Type))
                {
                    var contractKind = property.IsReadable
                        ? property.IsWritable
                            ? ContractKind.RequiresAndEnsures
                            : ContractKind.Ensures
                        : property.IsWritable
                            ? (ContractKind?)ContractKind.Requires
                            : null;
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

        PropertyContractInfo(ContractKind contractKind, [NotNull] IAccessorOwnerDeclaration declaration, [NotNull] IType type) : base(
            contractKind,
            type)
        {
            Debug.Assert(
                contractKind == ContractKind.Requires ||
                contractKind == ContractKind.Ensures ||
                contractKind == ContractKind.RequiresAndEnsures ||
                contractKind == ContractKind.Invariant);

            this.declaration = declaration;
        }

        public override string GetContractIdentifierForUI() => declaration.DeclaredName;

        public override void AddContracts(
            ICSharpContextActionDataProvider provider,
            Func<IExpression, IExpression> getContractExpression,
            out ICollection<ICSharpStatement> firstNonContractStatements)
        {
            var factory = CSharpElementFactory.GetInstance(declaration);

            var propertyDeclaration = declaration as IPropertyDeclaration;
            if (propertyDeclaration != null && propertyDeclaration.IsAuto)
            {
                var classLikeDeclaration = (IClassLikeDeclaration)declaration.GetContainingTypeDeclaration();

                Debug.Assert(classLikeDeclaration != null);

                var contractInvariantMethodDeclaration = classLikeDeclaration.EnsureContractInvariantMethod(provider.PsiModule);

                if (contractInvariantMethodDeclaration.Body != null)
                {
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

                return;
            }

            TreeNodeCollection<IAccessorDeclaration> accessorDeclarations;

            if (declaration.IsAbstract)
            {
                IAccessorOwnerDeclaration overriddenAccessorOwnerDeclaration = null;

                var containingTypeDeclaration = declaration.GetContainingTypeDeclaration();

                Debug.Assert(containingTypeDeclaration != null);

                var contractClassDeclaration = containingTypeDeclaration.EnsureContractClass(provider.PsiModule);

                if (propertyDeclaration != null)
                {
                    overriddenAccessorOwnerDeclaration = propertyDeclaration.EnsureOverriddenPropertyInContractClass(contractClassDeclaration);
                }

                if (declaration is IIndexerDeclaration indexerDeclaration)
                {
                    overriddenAccessorOwnerDeclaration = indexerDeclaration.EnsureOverriddenIndexerInContractClass(contractClassDeclaration);
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
                            var contractType = TypeElementUtil.GetTypeElementByClrName(PredefinedType.CONTRACT_FQN, provider.PsiModule);

                            var resultExpression = factory.CreateExpression(
                                string.Format("$0.{0}<$1>()", nameof(Contract.Result)),
                                contractType,
                                Type);

                            AddContract(
                                ContractKind.Ensures,
                                accessorDeclaration.Body,
                                provider.PsiModule,
                                () => getContractExpression(resultExpression),
                                out var firstNonContractStatement);

                            if (firstNonContractStatement != null)
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