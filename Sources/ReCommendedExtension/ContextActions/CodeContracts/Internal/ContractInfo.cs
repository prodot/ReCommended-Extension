using System.Diagnostics.Contracts;
using JetBrains.ReSharper.Feature.Services.CSharp.ContextActions;
using JetBrains.ReSharper.Intentions.Util;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.CSharp;
using JetBrains.ReSharper.Psi.CSharp.Tree;
using JetBrains.ReSharper.Psi.Modules;
using JetBrains.ReSharper.Psi.Resolve;
using JetBrains.ReSharper.Psi.Tree;
using JetBrains.ReSharper.Psi.Util;
using JetBrains.Util;

namespace ReCommendedExtension.ContextActions.CodeContracts.Internal;

internal abstract record ContractInfo
{
    sealed record ContractStatementInfo
    {
        static readonly string contractClassFullName = typeof(Contract).FullName!;

        [JetBrains.Annotations.Pure]
        static string? TryGetMemberName(IExpressionStatement expressionStatement, string classFullName)
            => expressionStatement.Expression is IInvocationExpression
                {
                    InvokedExpression: IReferenceExpression
                    {
                        QualifierExpression: IReferenceExpression { Reference: { } reference },
                    } referenceExpression,
                }
                && reference.Resolve().DeclaredElement is IClass @class
                && @class.GetClrName().FullName == classFullName
                    ? referenceExpression.Reference.GetName()
                    : null;

        [JetBrains.Annotations.Pure]
        public static IList<ContractStatementInfo> CreateContractStatementInfos(IBlock body)
        {
            var list = new List<ContractStatementInfo>();

            foreach (var statement in body.Statements)
            {
                if (statement is IExpressionStatement expressionStatement)
                {
                    switch (TryGetMemberName(expressionStatement, contractClassFullName))
                    {
                        case nameof(Contract.Requires):
                            list.Add(new ContractStatementInfo { ContractKind = ContractKind.Requires, Statement = expressionStatement });
                            continue;

                        case nameof(Contract.Ensures):
                            list.Add(new ContractStatementInfo { ContractKind = ContractKind.Ensures, Statement = expressionStatement });
                            continue;

                        case nameof(Contract.EnsuresOnThrow):
                            list.Add(new ContractStatementInfo { ContractKind = ContractKind.EnsuresOnThrow, Statement = expressionStatement });
                            continue;

                        case nameof(Contract.Invariant):
                            list.Add(new ContractStatementInfo { ContractKind = ContractKind.Invariant, Statement = expressionStatement });
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

    [JetBrains.Annotations.Pure]
    protected static bool CanAcceptContracts(ITypeMember typeMember)
        => !typeMember.IsExtern && (typeMember is not IOverridableMember overridableMember || !overridableMember.GetImmediateSuperMembers().Any());

    [JetBrains.Annotations.Pure]
    static ICSharpStatement CreateContractStatement(
        ContractKind contractKind,
        IPsiModule psiModule,
        IExpression contractExpression)
    {
        var factory = CSharpElementFactory.GetInstance(contractExpression);

        var contractType = PredefinedType.CONTRACT_FQN.TryGetTypeElement(psiModule);

        return contractKind switch
        {
            ContractKind.Requires => factory.CreateStatement($"$0.{nameof(Contract.Requires)}($1);", contractType, contractExpression),
            ContractKind.Ensures => factory.CreateStatement($"$0.{nameof(Contract.Ensures)}($1);", contractType, contractExpression),
            ContractKind.Invariant => factory.CreateStatement($"$0.{nameof(Contract.Invariant)}($1);", contractType, contractExpression),

            _ => throw new ArgumentOutOfRangeException(nameof(contractKind)),
        };
    }

    [JetBrains.Annotations.Pure]
    static string GetSuggestedContractClassName(ICSharpTypeDeclaration typeDeclaration)
    {
        var suggestedContractClassName = $"{typeDeclaration.DeclaredName}Contract";

        if (typeDeclaration is IInterfaceDeclaration { DeclaredName: ['I', _, ..] })
        {
            suggestedContractClassName = suggestedContractClassName[1..];
        }

        return suggestedContractClassName;
    }

    static void CopyTypeParameterConstraints<P>(
        CSharpElementFactory factory,
        TreeNodeCollection<P> source,
        TreeNodeCollection<P> destination,
        [InstantHandle] Action<ITypeParameterConstraintsClause> addClause) where P : class, ITypeParameterDeclaration
    {
        var typeParameterMap = new Dictionary<ITypeParameter, IType>();
        for (var i = 0; i < source.Count; i++)
        {
            var typeParameterDeclaration = destination[i];
            var originalTypeParameter = source[i].DeclaredElement;

            Debug.Assert(typeParameterDeclaration.DeclaredElement is { });

            typeParameterMap.Add(originalTypeParameter, TypeFactory.CreateType(typeParameterDeclaration.DeclaredElement));
        }

        var newSubstitution = EmptySubstitution.INSTANCE.Extend(typeParameterMap);
        for (var i = 0; i < source.Count; i++)
        {
            var typeParameter = source[i].DeclaredElement;
            var typeParameterDeclaration = destination[i];

            Debug.Assert(typeParameter is { });

            if (factory.CreateTypeParameterConstraintsClause(typeParameter, newSubstitution, typeParameterDeclaration.DeclaredName) is { } clause)
            {
                addClause(clause);
            }
        }
    }

    protected static IClassDeclaration EnsureContractClass(ICSharpTypeDeclaration typeDeclaration, IPsiModule psiModule)
    {
        var factory = CSharpElementFactory.GetInstance(typeDeclaration);

        var contractClassDeclaration = null as IClassDeclaration;

        var attributeInstance = typeDeclaration.DeclaredElement?.GetAttributeInstances(ClrTypeNames.ContractClassAttribute, false).FirstOrDefault();
        if (attributeInstance is { PositionParameterCount: > 0 }
            && attributeInstance.PositionParameter(0).TypeValue.GetTypeElement<IClass>() is { } typeElement)
        {
            contractClassDeclaration = typeElement.GetDeclarations().FirstOrDefault() as IClassDeclaration;
        }

        if (contractClassDeclaration is { })
        {
            return contractClassDeclaration;
        }

        var typeParameters = typeDeclaration.TypeParameters is not []
            ? $"<{string.Join(", ", from typeParameter in typeDeclaration.TypeParameters select typeParameter.DeclaredName)}>"
            : "";
        var typeParametersForAttribute = typeDeclaration.TypeParameters is not []
            ? $"<{new string(',', typeDeclaration.TypeParameters.Count - 1)}>"
            : "";

        contractClassDeclaration = (IClassDeclaration)factory.CreateTypeMemberDeclaration(
            string.Format(
                @"[$0(typeof($1{2}))] abstract class {0}{1} : $1{1} {{ }}",
                GetSuggestedContractClassName(typeDeclaration),
                typeParameters,
                typeParametersForAttribute),
            ClrTypeNames.ContractClassForAttribute.TryGetTypeElement(psiModule),
            typeDeclaration.DeclaredElement);

        CopyTypeParameterConstraints(
            factory,
            typeDeclaration.TypeParameters,
            contractClassDeclaration.TypeParameters,
            clause => contractClassDeclaration.AddTypeParameterConstraintsClauseBefore(clause, null));

        var attributeTypeParameters = contractClassDeclaration.TypeParameters.Any()
            ? $"<{new string(',', contractClassDeclaration.TypeParameters.Count - 1)}>"
            : "";
        var typeofExpression = (ITypeofExpression)factory.CreateExpression(
            $"typeof($0{attributeTypeParameters})",
            contractClassDeclaration.DeclaredElement);

        // todo: the generated "typeof" expression doesn't contain generics: "<,>"
        var contractClassAttributeTypeElement = ClrTypeNames.ContractClassAttribute.TryGetTypeElement(psiModule);
        Debug.Assert(contractClassAttributeTypeElement is { });
        var attribute = factory.CreateAttribute(
            contractClassAttributeTypeElement,
            [new AttributeValue(typeofExpression.ArgumentType)],
            Array.Empty<Pair<string, AttributeValue>>());

        typeDeclaration.AddAttributeAfter(attribute, null);

        if (typeDeclaration.GetContainingTypeDeclaration() is IClassLikeDeclaration parentTypeDeclaration)
        {
            contractClassDeclaration.SetAccessRights(AccessRights.PRIVATE);

            contractClassDeclaration = parentTypeDeclaration.AddClassMemberDeclaration(contractClassDeclaration);
        }
        else
        {
            if (typeDeclaration.GetContainingNamespaceDeclaration() is { } parentNamespaceDeclaration)
            {
                contractClassDeclaration.SetAccessRights(AccessRights.INTERNAL);

                contractClassDeclaration =
                    (IClassDeclaration)parentNamespaceDeclaration.AddTypeDeclarationAfter(contractClassDeclaration, typeDeclaration);
            }
        }

        ContextActionUtils.FormatWithDefaultProfile(contractClassDeclaration);

        return contractClassDeclaration;
    }

    protected static IMethodDeclaration EnsureOverriddenMethodInContractClass(
        IMethodDeclaration methodDeclaration,
        IClassDeclaration contractClassDeclaration)
    {
        var factory = CSharpElementFactory.GetInstance(methodDeclaration);

        var declaredElement = methodDeclaration.DeclaredElement;

        // todo: find a better way to compare instances (than using hash codes)

        var overriddenMethodDeclaration =
        (
            from d in contractClassDeclaration.MethodDeclarations
            where d.DeclaredElement is { }
                && d
                    .DeclaredElement.GetImmediateSuperMembers()
                    .Any(overridableMemberInstance => overridableMemberInstance.GetHashCode() == declaredElement.GetHashCode())
            select d).FirstOrDefault();

        if (overriddenMethodDeclaration is { })
        {
            return overriddenMethodDeclaration;
        }

        Debug.Assert(declaredElement is { });

        var typeParameters = methodDeclaration.TypeParameterDeclarations.Any()
            ? $"<{string.Join(", ", from typeParameter in methodDeclaration.TypeParameterDeclarations select typeParameter.DeclaredName)}>"
            : "";

        var returnStatement = declaredElement.ReturnType.IsVoid() ? "" : " return default($0); ";
        overriddenMethodDeclaration = (IMethodDeclaration)factory.CreateTypeMemberDeclaration(
            $"$0 {methodDeclaration.DeclaredName}{typeParameters}() {{{returnStatement}}}",
            declaredElement.ReturnType);
        overriddenMethodDeclaration.SetAccessRights(
            methodDeclaration.GetContainingTypeDeclaration() is IInterfaceDeclaration
                ? AccessRights.PUBLIC
                : methodDeclaration.GetAccessRights());
        overriddenMethodDeclaration.SetOverride(methodDeclaration.GetContainingTypeDeclaration() is IClassDeclaration);

        foreach (var parameterDeclaration in methodDeclaration.ParameterDeclarations)
        {
            var parameter = parameterDeclaration.DeclaredElement;

            overriddenMethodDeclaration.AddParameterDeclarationBefore(
                factory.CreateParameterDeclaration(
                    null,
                    parameter.Kind,
                    parameter.IsParameterArray,
                    parameter.IsVarArg,
                    parameterDeclaration.Type,
                    parameterDeclaration.DeclaredName,
                    null),
                null);
        }

        CopyTypeParameterConstraints(
            factory,
            methodDeclaration.TypeParameterDeclarations,
            overriddenMethodDeclaration.TypeParameterDeclarations,
            clause => overriddenMethodDeclaration.AddTypeParameterConstraintsClauseBefore(clause, null));

        overriddenMethodDeclaration = contractClassDeclaration.AddClassMemberDeclaration(overriddenMethodDeclaration);

        ContextActionUtils.FormatWithDefaultProfile(overriddenMethodDeclaration);

        return overriddenMethodDeclaration;
    }

    protected static IIndexerDeclaration EnsureOverriddenIndexerInContractClass(
        IIndexerDeclaration indexerDeclaration,
        IClassDeclaration contractClassDeclaration)
    {
        var factory = CSharpElementFactory.GetInstance(indexerDeclaration);

        // todo: find a better way to compare instances (than using hash codes)
        var overriddenIndexerDeclaration = (
            from d in contractClassDeclaration.IndexerDeclarations
            where d.DeclaredElement is { }
                && d
                    .DeclaredElement.GetImmediateSuperMembers()
                    .Any(overridableMemberInstance => overridableMemberInstance.GetHashCode() == indexerDeclaration.DeclaredElement.GetHashCode())
            select d).FirstOrDefault();

        if (overriddenIndexerDeclaration is { })
        {
            return overriddenIndexerDeclaration;
        }

        Debug.Assert(indexerDeclaration.DeclaredElement is { });

        var getter = indexerDeclaration.DeclaredElement.IsReadable ? " get { return default($0); } " : "";
        var setter = indexerDeclaration.DeclaredElement.IsWritable ? " set { } " : "";
        overriddenIndexerDeclaration = (IIndexerDeclaration)factory.CreateTypeMemberDeclaration(
            $"$0 this[] {{{getter}{setter}}}",
            indexerDeclaration.DeclaredElement.Type);

        overriddenIndexerDeclaration.SetAccessRights(
            indexerDeclaration.GetContainingTypeDeclaration() is IInterfaceDeclaration
                ? AccessRights.PUBLIC
                : indexerDeclaration.GetAccessRights());
        overriddenIndexerDeclaration.SetOverride(indexerDeclaration.GetContainingTypeDeclaration() is IClassDeclaration);

        foreach (var parameterDeclaration in indexerDeclaration.ParameterDeclarations)
        {
            var parameter = parameterDeclaration.DeclaredElement;

            overriddenIndexerDeclaration.AddParameterDeclarationBefore(
                factory.CreateParameterDeclaration(
                    null,
                    parameter.Kind,
                    parameter.IsParameterArray,
                    parameter.IsVarArg,
                    parameterDeclaration.Type,
                    parameterDeclaration.DeclaredName,
                    null),
                null);
        }

        overriddenIndexerDeclaration = contractClassDeclaration.AddClassMemberDeclaration(overriddenIndexerDeclaration);

        ContextActionUtils.FormatWithDefaultProfile(overriddenIndexerDeclaration);

        return overriddenIndexerDeclaration;
    }

    protected static IPropertyDeclaration EnsureOverriddenPropertyInContractClass(
        IPropertyDeclaration propertyDeclaration,
        IClassDeclaration contractClassDeclaration)
    {
        var factory = CSharpElementFactory.GetInstance(propertyDeclaration);

        // todo: find a better way to compare instances (than using hash codes)
        var overriddenPropertyDeclaration = (
            from d in contractClassDeclaration.PropertyDeclarations
            where d.DeclaredElement is { }
                && d
                    .DeclaredElement.GetImmediateSuperMembers()
                    .Any(overridableMemberInstance => overridableMemberInstance.GetHashCode() == propertyDeclaration.DeclaredElement.GetHashCode())
            select d).FirstOrDefault();

        if (overriddenPropertyDeclaration is { })
        {
            return overriddenPropertyDeclaration;
        }

        Debug.Assert(propertyDeclaration.DeclaredElement is { });

        var getter = propertyDeclaration.DeclaredElement.IsReadable ? " get { return default($0); } " : "";
        var setter = propertyDeclaration.DeclaredElement.IsWritable ? " set { } " : "";
        overriddenPropertyDeclaration = (IPropertyDeclaration)factory.CreateTypeMemberDeclaration(
            $"$0 {propertyDeclaration.DeclaredName} {{{getter}{setter}}}",
            propertyDeclaration.DeclaredElement.Type);

        overriddenPropertyDeclaration.SetAccessRights(
            propertyDeclaration.GetContainingTypeDeclaration() is IInterfaceDeclaration
                ? AccessRights.PUBLIC
                : propertyDeclaration.GetAccessRights());
        overriddenPropertyDeclaration.SetOverride(propertyDeclaration.GetContainingTypeDeclaration() is IClassDeclaration);

        overriddenPropertyDeclaration = contractClassDeclaration.AddClassMemberDeclaration(overriddenPropertyDeclaration);

        ContextActionUtils.FormatWithDefaultProfile(overriddenPropertyDeclaration);

        return overriddenPropertyDeclaration;
    }

    protected static IMethodDeclaration EnsureContractInvariantMethod(IClassLikeDeclaration classLikeDeclaration, IPsiModule psiModule)
    {
        var factory = CSharpElementFactory.GetInstance(classLikeDeclaration);

        var contractInvariantMethodDeclaration = classLikeDeclaration.MethodDeclarations.FirstOrDefault(
            methodDeclaration =>
            {
                Debug.Assert(methodDeclaration.DeclaredElement is { });

                return methodDeclaration.DeclaredElement.HasAttributeInstance(ClrTypeNames.ContractInvariantMethodAttribute, false);
            });

        if (contractInvariantMethodDeclaration is { })
        {
            return contractInvariantMethodDeclaration;
        }

        contractInvariantMethodDeclaration = (IMethodDeclaration)factory.CreateTypeMemberDeclaration(
            "[$0] private void ObjectInvariant() { }",
            ClrTypeNames.ContractInvariantMethodAttribute.TryGetTypeElement(psiModule));
        contractInvariantMethodDeclaration = classLikeDeclaration.AddClassMemberDeclaration(contractInvariantMethodDeclaration);

        ContextActionUtils.FormatWithDefaultProfile(contractInvariantMethodDeclaration);

        return contractInvariantMethodDeclaration;
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

    [JetBrains.Annotations.Pure]
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

    [JetBrains.Annotations.Pure]
    public string GetContractKindForUI()
        => ContractKind switch
        {
            ContractKind.Requires => "requires",
            ContractKind.Ensures => "ensures",
            ContractKind.RequiresAndEnsures => "requires & ensures",
            ContractKind.Invariant => "invariant",

            _ => throw new NotSupportedException(),
        };

    [JetBrains.Annotations.Pure]
    public abstract string GetContractIdentifierForUI();

    public abstract void AddContracts(
        ICSharpContextActionDataProvider provider,
        Func<IExpression, IExpression> getContractExpression,
        out ICollection<ICSharpStatement>? firstNonContractStatements);
}