using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.Contracts;
using System.Linq;
using JetBrains.Annotations;
using JetBrains.ReSharper.Feature.Services.CSharp.ContextActions;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.CSharp;
using JetBrains.ReSharper.Psi.CSharp.Tree;
using JetBrains.ReSharper.Psi.Tree;
using JetBrains.ReSharper.Psi.Util;

namespace ReCommendedExtension.ContextActions.CodeContracts.Internal
{
    internal sealed class ParameterContractInfo : ContractInfo
    {
        static void AddContractForRequires(
            [NotNull] ICSharpContextActionDataProvider provider,
            [NotNull] Func<IExpression, IExpression> getContractExpression,
            [NotNull] IParameter parameter,
            [NotNull] IBlock body,
            [CanBeNull] out ICSharpStatement firstNonContractStatement)
        {
            var factory = CSharpElementFactory.GetInstance(body);

            var parameterExpression = factory.CreateExpression("$0", parameter);

            AddContract(
                ContractKind.Requires,
                body,
                provider.PsiModule,
                () => getContractExpression(parameterExpression),
                out firstNonContractStatement);
        }

        static void AddContractForEnsures(
            [NotNull] ICSharpContextActionDataProvider provider,
            [NotNull] Func<IExpression, IExpression> getContractExpression,
            [NotNull] IParameter parameter,
            [NotNull] IBlock body,
            [CanBeNull] out ICSharpStatement firstNonContractStatement)
        {
            var factory = CSharpElementFactory.GetInstance(body);

            var contractType = TypeElementUtil.GetTypeElementByClrName(PredefinedType.CONTRACT_FQN, provider.PsiModule);

            var parameterExpression = factory.CreateExpression("$0", parameter);

            var expression = factory.CreateExpression($"$0.{nameof(Contract.ValueAtReturn)}(out $1)", contractType, parameterExpression);

            AddContract(ContractKind.Ensures, body, provider.PsiModule, () => getContractExpression(expression), out firstNonContractStatement);
        }

        [CanBeNull]
        public static ParameterContractInfo TryCreate([NotNull] IParameterDeclaration declaration, [NotNull] Func<IType, bool> isAvailableForType)
        {
            var expressionBodyOwnerDeclaration = declaration.PathToRoot().OfType<IExpressionBodyOwnerDeclaration>().FirstOrDefault();
            if (expressionBodyOwnerDeclaration?.ArrowClause != null)
            {
                return null;
            }

            switch (expressionBodyOwnerDeclaration)
            {
                case IPropertyDeclaration propertyDeclaration
                    when propertyDeclaration.AccessorDeclarations.All(accessorDeclaration => accessorDeclaration.ArrowClause != null):
                case IIndexerDeclaration indexerDeclaration
                    when indexerDeclaration.AccessorDeclarations.All(accessorDeclaration => accessorDeclaration.ArrowClause != null):
                    return null;
            }

            var parameter = declaration.DeclaredElement;
            if (parameter?.ContainingParametersOwner is ITypeMember typeMember &&
                CanAcceptContracts(typeMember) &&
                isAvailableForType(parameter.Type))
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

                    default: return null;
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
            [CanBeNull] out ICSharpStatement firstNonContractStatement)
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
            out ICollection<ICSharpStatement> firstNonContractStatements)
        {
            var parameter = declaration.DeclaredElement;

            Debug.Assert(parameter != null);

            switch (parameter.ContainingParametersOwner)
            {
                case IFunction function
                    when function.GetDeclarationsIn(provider.SourceFile).FirstOrDefault(d => Equals(d.DeclaredElement, function)) is
                        ICSharpFunctionDeclaration functionDeclaration:
                {
                    IBlock body;

                    if (functionDeclaration is IMethodDeclaration methodDeclaration &&
                        (methodDeclaration.IsAbstract || methodDeclaration.GetContainingTypeDeclaration()?.IsAbstract == true))
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
                        AddContract(provider, getContractExpression, parameter, body, out var firstNonContractStatement);
                        firstNonContractStatements = firstNonContractStatement != null ? new[] { firstNonContractStatement } : null;
                    }
                    else
                    {
                        firstNonContractStatements = null;
                    }

                    return;
                }

                case IProperty property
                    when property.GetDeclarationsIn(provider.SourceFile).FirstOrDefault(d => Equals(d.DeclaredElement, property)) is
                        IIndexerDeclaration
                        indexerDeclaration:
                {
                    TreeNodeCollection<IAccessorDeclaration> accessorDeclarations;

                    var containingTypeDeclaration = indexerDeclaration.GetContainingTypeDeclaration();
                    Debug.Assert(containingTypeDeclaration != null);

                    if (indexerDeclaration.IsAbstract || containingTypeDeclaration.IsAbstract)
                    {
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
                            AddContract(provider, getContractExpression, parameter, accessorDeclaration.Body, out var firstNonContractStatement);
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

                default:
                    firstNonContractStatements = null;
                    break;
            }
        }
    }
}