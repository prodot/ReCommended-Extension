using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.Contracts;
using System.Linq;
using JetBrains.Annotations;
using JetBrains.ReSharper.Feature.Services.CSharp.Analyses.Bulbs;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.CSharp;
using JetBrains.ReSharper.Psi.CSharp.Tree;
using JetBrains.ReSharper.Psi.Impl.Types;
using JetBrains.ReSharper.Psi.Tree;

namespace ReCommendedExtension.ContextActions.CodeContracts.Internal
{
    internal sealed class ParameterContractInfo : ContractInfo
    {
        static void AddContractForRequires(
            [NotNull] ICSharpContextActionDataProvider provider,
            [NotNull] Func<IExpression, IExpression> getContractExpression,
            [NotNull] IParameter parameter,
            [NotNull] IBlock body,
            out ICSharpStatement firstNonContractStatement)
        {
            var factory = CSharpElementFactory.GetInstance(body);

            var parameterExpression = factory.CreateExpression("$0", parameter);

            AddContract(
                ContractKind.Requires, body, provider.PsiModule, () => getContractExpression(parameterExpression), out firstNonContractStatement);
        }

        static void AddContractForEnsures(
            [NotNull] ICSharpContextActionDataProvider provider,
            [NotNull] Func<IExpression, IExpression> getContractExpression,
            [NotNull] IParameter parameter,
            [NotNull] IBlock body,
            out ICSharpStatement firstNonContractStatement)
        {
            var factory = CSharpElementFactory.GetInstance(body);

            var contractType = new DeclaredTypeFromCLRName(ClrTypeNames.Contract, provider.PsiModule).GetTypeElement();

            var parameterExpression = factory.CreateExpression("$0", parameter);

            var expression = factory.CreateExpression(
                string.Format("$0.{0}(out $1)", nameof(Contract.ValueAtReturn)),
                contractType,
                parameterExpression);

            AddContract(ContractKind.Ensures, body, provider.PsiModule, () => getContractExpression(expression), out firstNonContractStatement);
        }

        public static ParameterContractInfo TryCreate([NotNull] IParameterDeclaration declaration, [NotNull] Func<IType, bool> isAvailableForType)
        {
            if (declaration.PathToRoot().OfType<IExpressionBodyOwnerDeclaration>().FirstOrDefault()?.ArrowClause != null)
            {
                return null;
            }

            var parameter = declaration.DeclaredElement;
            var typeMember = parameter?.ContainingParametersOwner as ITypeMember;
            if (typeMember != null && CanAcceptContracts(typeMember) && isAvailableForType(parameter.Type))
            {
                ContractKind contractKind;
                switch (parameter.Kind)
                {
                    case ParameterKind.VALUE:
                        contractKind = ContractKind.Requires;
                        break;

                    case ParameterKind.REFERENCE:
                        contractKind = ContractKind.RequiresAndEnsures;
                        break;

                    case ParameterKind.OUTPUT:
                        contractKind = ContractKind.Ensures;
                        break;

                    default:
                        return null;
                }

                return new ParameterContractInfo(contractKind, declaration, parameter.Type);
            }

            return null;
        }

        [NotNull]
        readonly IParameterDeclaration declaration;

        ParameterContractInfo(ContractKind contractKind, [NotNull] IParameterDeclaration declaration, [NotNull] IType type) : base(contractKind, type)
        {
            Debug.Assert(
                contractKind == ContractKind.Requires || contractKind == ContractKind.Ensures || contractKind == ContractKind.RequiresAndEnsures);

            this.declaration = declaration;
        }

        void AddContract(
            [NotNull] ICSharpContextActionDataProvider provider,
            [NotNull] Func<IExpression, IExpression> getContractExpression,
            [NotNull] IParameter parameter,
            [NotNull] IBlock body,
            out ICSharpStatement firstNonContractStatement)
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

                default:
                    throw new NotSupportedException();
            }
        }

        public override string GetContractIdentifierForUI() => declaration.DeclaredName;

        public override void AddContracts(
            ICSharpContextActionDataProvider provider,
            Func<IExpression, IExpression> getContractExpression,
            out ICollection<ICSharpStatement> firstNonContractStatements)
        {
            var parameter = declaration.DeclaredElement;

            Debug.Assert(parameter != null);

            var function = parameter.ContainingParametersOwner as IFunction;
            var functionDeclaration =
                function?.GetDeclarationsIn(provider.SourceFile).FirstOrDefault(d => Equals(d.AssertNotNull().DeclaredElement, function)) as
                    ICSharpFunctionDeclaration;
            if (functionDeclaration != null)
            {
                IBlock body;

                var methodDeclaration = functionDeclaration as IMethodDeclaration;
                if (methodDeclaration != null && methodDeclaration.IsAbstract)
                {
                    var containingTypeDeclaration = methodDeclaration.GetContainingTypeDeclaration();

                    Debug.Assert(containingTypeDeclaration != null);

                    var contractClassDeclaration = containingTypeDeclaration.EnsureContractClass(provider.PsiModule);

                    var overriddenMethodDeclaration = methodDeclaration.EnsureOverriddenMethodInContractClass(contractClassDeclaration);

                    body = overriddenMethodDeclaration.Body;
                }
                else
                {
                    body = functionDeclaration.Body;
                }

                if (body != null)
                {
                    ICSharpStatement firstNonContractStatement;
                    AddContract(provider, getContractExpression, parameter, body, out firstNonContractStatement);
                    firstNonContractStatements = firstNonContractStatement != null ? new[] { firstNonContractStatement } : null;
                }
                else
                {
                    firstNonContractStatements = null;
                }

                return;
            }

            var property = parameter.ContainingParametersOwner as IProperty;
            var indexerDeclaration =
                property?.GetDeclarationsIn(provider.SourceFile).FirstOrDefault(d => Equals(d.AssertNotNull().DeclaredElement, property)) as
                    IIndexerDeclaration;
            if (indexerDeclaration != null)
            {
                IEnumerable<IAccessorDeclaration> accessorDeclarations;

                if (indexerDeclaration.IsAbstract)
                {
                    var containingTypeDeclaration = indexerDeclaration.GetContainingTypeDeclaration();

                    Debug.Assert(containingTypeDeclaration != null);

                    var contractClassDeclaration = containingTypeDeclaration.EnsureContractClass(provider.PsiModule);

                    var overriddenIndexerDeclaration = indexerDeclaration.EnsureOverriddenIndexerInContractClass(contractClassDeclaration);

                    accessorDeclarations = overriddenIndexerDeclaration.AccessorDeclarations;
                }
                else
                {
                    accessorDeclarations = indexerDeclaration.AccessorDeclarations;
                }

                firstNonContractStatements = new List<ICSharpStatement>(2);
                foreach (var accessorDeclaration in accessorDeclarations)
                {
                    Debug.Assert(accessorDeclaration != null);

                    if (accessorDeclaration.Body != null)
                    {
                        ICSharpStatement firstNonContractStatement;
                        AddContract(
                            provider,
                            getContractExpression,
                            parameter,
                            accessorDeclaration.Body,
                            out firstNonContractStatement);
                        if (firstNonContractStatement != null)
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

            firstNonContractStatements = null;
        }
    }
}