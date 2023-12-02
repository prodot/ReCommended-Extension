using System.Diagnostics.Contracts;
using JetBrains.Application.Settings;
using JetBrains.Metadata.Reader.API;
using JetBrains.ProjectModel;
using JetBrains.ProjectModel.Properties.Flavours;
using JetBrains.ReSharper.Feature.Services.Daemon;
using JetBrains.ReSharper.Feature.Services.LinqTools;
using JetBrains.ReSharper.Intentions.Util;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.ControlFlow;
using JetBrains.ReSharper.Psi.CSharp;
using JetBrains.ReSharper.Psi.CSharp.Conversions;
using JetBrains.ReSharper.Psi.CSharp.Parsing;
using JetBrains.ReSharper.Psi.CSharp.Tree;
using JetBrains.ReSharper.Psi.CSharp.Util;
using JetBrains.ReSharper.Psi.Modules;
using JetBrains.ReSharper.Psi.Resolve;
using JetBrains.ReSharper.Psi.Tree;
using JetBrains.ReSharper.Psi.Util;
using JetBrains.Util;

namespace ReCommendedExtension;

internal static class Extensions
{
    static readonly string contractClassFullName = GetClrTypeName(typeof(Contract));

    static readonly HashSet<string> wellKnownUnitTestingAssemblyNames = new(StringComparer.OrdinalIgnoreCase)
    {
        "Microsoft.VisualStudio.TestPlatform.TestFramework", @"nunit.framework", "xunit.core",
    };

    static readonly Version msTest14MinFileVersion = new(14, 0, 3021, 1);

    [JetBrains.Annotations.Pure]
    static string GetClrTypeName(Type type)
    {
        var fullName = type.FullName;
        Debug.Assert(fullName is { });

        return fullName;
    }

    static string? TryGetMemberName(this IExpressionStatement expressionStatement, string classFullName)
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

    static string GetSuggestedContractClassName(ICSharpTypeDeclaration typeDeclaration)
    {
        var suggestedContractClassName = $"{typeDeclaration.DeclaredName}Contract";

        if (typeDeclaration is IInterfaceDeclaration { DeclaredName: ['I', _, ..] })
        {
            suggestedContractClassName = suggestedContractClassName.Remove(0, 1);
        }

        return suggestedContractClassName;
    }

    public static bool OverridesInheritedMember(this IDeclaration declaration)
    {
        if (!declaration.IsValid())
        {
            return false;
        }

        if (declaration.DeclaredElement is IOverridableMember overridableMember && overridableMember.GetImmediateSuperMembers().Any())
        {
            return true;
        }

        if (declaration is { DeclaredElement: IParameter { ContainingParametersOwner: IOverridableMember parameterOverridableMember } }
            && parameterOverridableMember.GetImmediateSuperMembers().Any())
        {
            return true;
        }

        return false;
    }

    public static IEnumerable<T> WithoutObsolete<T>(this IEnumerable<T> fields) where T : class, IAttributesOwner
        => from field in fields where !field.HasAttributeInstance(PredefinedType.OBSOLETE_ATTRIBUTE_CLASS, false) select field;

    public static string? TryGetContractName(this IExpressionStatement expressionStatement)
        => expressionStatement.TryGetMemberName(contractClassFullName);

    public static IClassDeclaration EnsureContractClass(this ICSharpTypeDeclaration typeDeclaration, IPsiModule psiModule)
    {
        var factory = CSharpElementFactory.GetInstance(typeDeclaration);

        var contractClassDeclaration = null as IClassDeclaration;

        var attributeInstance = typeDeclaration.DeclaredElement?.GetAttributeInstances(ClrTypeNames.ContractClassAttribute, false).FirstOrDefault();
        if (attributeInstance is { PositionParameterCount: > 0 }
            && attributeInstance.PositionParameter(0).TypeValue.GetTypeElement<IClass>() is { } typeElement)
        {
            contractClassDeclaration = typeElement.GetDeclarations().FirstOrDefault() as IClassDeclaration;
        }

        if (contractClassDeclaration is not { })
        {
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
                TypeElementUtil.GetTypeElementByClrName(ClrTypeNames.ContractClassForAttribute, psiModule),
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
            var contractClassAttributeTypeElement = TypeElementUtil.GetTypeElementByClrName(ClrTypeNames.ContractClassAttribute, psiModule);
            Debug.Assert(contractClassAttributeTypeElement is { });
            var attribute = factory.CreateAttribute(
                contractClassAttributeTypeElement,
                new[] { new AttributeValue(typeofExpression.ArgumentType) },
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
        }

        return contractClassDeclaration;
    }

    public static IMethodDeclaration EnsureOverriddenMethodInContractClass(
        this IMethodDeclaration methodDeclaration,
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

        if (overriddenMethodDeclaration is not { })
        {
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
        }

        return overriddenMethodDeclaration;
    }

    public static IIndexerDeclaration EnsureOverriddenIndexerInContractClass(
        this IIndexerDeclaration indexerDeclaration,
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

        if (overriddenIndexerDeclaration is not { })
        {
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
        }

        return overriddenIndexerDeclaration;
    }

    public static IPropertyDeclaration EnsureOverriddenPropertyInContractClass(
        this IPropertyDeclaration propertyDeclaration,
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

        if (overriddenPropertyDeclaration is not { })
        {
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
        }

        return overriddenPropertyDeclaration;
    }

    public static IMethodDeclaration EnsureContractInvariantMethod(this IClassLikeDeclaration classLikeDeclaration, IPsiModule psiModule)
    {
        var factory = CSharpElementFactory.GetInstance(classLikeDeclaration);

        var contractInvariantMethodDeclaration = classLikeDeclaration.MethodDeclarations.FirstOrDefault(
            methodDeclaration =>
            {
                Debug.Assert(methodDeclaration.DeclaredElement is { });

                return methodDeclaration.DeclaredElement.HasAttributeInstance(ClrTypeNames.ContractInvariantMethodAttribute, false);
            });

        if (contractInvariantMethodDeclaration is not { })
        {
            contractInvariantMethodDeclaration = (IMethodDeclaration)factory.CreateTypeMemberDeclaration(
                "[$0] private void ObjectInvariant() { }",
                TypeElementUtil.GetTypeElementByClrName(ClrTypeNames.ContractInvariantMethodAttribute, psiModule));
            contractInvariantMethodDeclaration = classLikeDeclaration.AddClassMemberDeclaration(contractInvariantMethodDeclaration);

            ContextActionUtils.FormatWithDefaultProfile(contractInvariantMethodDeclaration);
        }

        return contractInvariantMethodDeclaration;
    }

    public static bool IsGenericArray(this IType type, ITreeNode context)
    {
        if (CollectionTypeUtil.ElementTypeByCollectionType(type, context, false) is { } elementType)
        {
            for (var i = 1; i < 16; i++)
            {
                if (type.IsImplicitlyConvertibleTo(
                    TypeFactory.CreateArrayType(elementType, i, NullableAnnotation.Unknown),
                    context.GetTypeConversionRule()))
                {
                    return true;
                }
            }
        }

        return false;
    }

    public static bool IsGenericEnumerableOrDescendant(this IType type)
    {
        if (type.IsGenericIEnumerable())
        {
            return true;
        }

        if (type.GetTypeElement<ITypeElement>() is { } typeElement
            && typeElement.IsDescendantOf(typeElement.Module.GetPredefinedType().GenericIEnumerable.GetTypeElement()))
        {
            return true;
        }

        return false;
    }

    [JetBrains.Annotations.Pure]
    public static IType?[]? TryGetGenericParameterTypes(this IDeclaredType declaredType)
    {
        if (declaredType.GetTypeElement() is { } typeElement)
        {
            var elementTypes = new IType?[typeElement.TypeParameters.Count];

            for (var i = 0; i < elementTypes.Length; i++)
            {
                if (CollectionTypeUtil.GetElementTypesForGenericType(declaredType, typeElement, i) is [var elementType])
                {
                    elementTypes[i] = elementType;
                }
            }

            return elementTypes;
        }

        return null;
    }

    [JetBrains.Annotations.Pure]
    public static bool IsDefaultValueOf(this ITreeNode element, IType type)
    {
        switch (element)
        {
            case ICSharpLiteralExpression literalExpression when literalExpression.Literal.GetTokenType() == CSharpTokenType.DEFAULT_KEYWORD:
            case IDefaultExpression defaultExpression when Equals(defaultExpression.Type(), type):
                return true;

            case IParenthesizedExpression { Expression: { } } parenthesizedExpression:
                return parenthesizedExpression.Expression.IsDefaultValueOf(type);
        }

        if (type.IsUnconstrainedGenericType())
        {
            // unconstrained generic type

            return false;
        }

        if (type.IsValueType())
        {
            // value type (non-nullable and nullable)

            switch (element)
            {
                case IConstantValueOwner constantValueOwner when type.IsNullable()
                    ? constantValueOwner.ConstantValue.IsNull()
                    : constantValueOwner.ConstantValue.IsDefaultValue(type, element):
                    return true;

                case IObjectCreationExpression objectCreationExpression:
                    var structType = type.GetStructType(); // null if type is a generic type

                    return Equals(objectCreationExpression.Type(), type)
                        && objectCreationExpression is { Arguments: [], Initializer: not { } or { InitializerElements: [] } }
                        && structType is { HasCustomParameterlessConstructor: false };

                case IAsExpression asExpression:
                    return asExpression is { Operand: { }, TypeOperand: { } }
                        && asExpression.Operand.ConstantValue.IsNull()
                        && Equals(CSharpTypeFactory.CreateType(asExpression.TypeOperand), type)
                        && type.IsNullable();

                default: return false;
            }
        }

        // reference type

        return element switch
        {
            IConstantValueOwner constantValueOwner when constantValueOwner.ConstantValue.IsNull()
                || constantValueOwner.ConstantValue.IsDefaultValue(type, element) => true,

            IAsExpression asExpression => asExpression.Operand is { }
                && asExpression.Operand.ConstantValue.IsNull()
                && asExpression.TypeOperand is { }
                && Equals(CSharpTypeFactory.CreateType(asExpression.TypeOperand), type),

            _ => false,
        };
    }

    public static ValueAnalysisMode GetValueAnalysisMode(this ElementProblemAnalyzerData data)
        => data.SettingsStore.GetValue<HighlightingSettings, ValueAnalysisMode>(s => s.ValueAnalysisMode);

    /// <summary>
    /// Returns the object creation expression <c>new EventHandler(</c><paramref name="argument"/><c>)</c> if used in the pattern
    /// <c>new EventHandler(</c><paramref name="argument"/><c>)</c> or <c>null</c>.
    /// </summary>
    static IObjectCreationExpression? TryGetDelegateCreation(ICSharpArgument argument)
    {
        var argumentList = argument.Parent as IArgumentList;

        if (argumentList is not { } or { Arguments: not [_] })
        {
            return null;
        }

        return argumentList.Parent as IObjectCreationExpression;
    }

    public static bool IsEventTarget(this IReference reference)
    {
        switch (reference.GetTreeNode().Parent)
        {
            case IAssignmentExpression assignmentExpression: return assignmentExpression.IsEventSubscriptionOrUnSubscription();

            case ICSharpArgument argument:
            {
                if (TryGetDelegateCreation(argument) is { Parent: IAssignmentExpression assignmentExpression })
                {
                    return assignmentExpression.IsEventSubscriptionOrUnSubscription();
                }
                break;
            }
        }

        if (reference is IReferenceToDelegateCreation referenceToDelegateCreation)
        {
            return referenceToDelegateCreation.IsEventSubscription;
        }

        return false;
    }

    /// <summary>
    /// Returns <c>true</c> if used in the pattern <c>button.Click ±= </c><paramref name="assignmentExpression"/><c>;</c>.
    /// </summary>
    public static bool IsEventSubscriptionOrUnSubscription(this IAssignmentExpression assignmentExpression)
    {
        if (assignmentExpression.AssignmentType is not (AssignmentType.PLUSEQ or AssignmentType.MINUSEQ))
        {
            return false;
        }

        if (assignmentExpression.OperatorOperands is not [_, _])
        {
            return false;
        }

        return (assignmentExpression.Dest as IReferenceExpression)?.Reference.Resolve().DeclaredElement is IEvent;
    }

    public static bool IsVoidMethodDeclaration(this ILocalFunctionDeclaration localFunctionDeclaration)
    {
        var predefinedTypeName = (localFunctionDeclaration.TypeUsage as IPredefinedTypeUsage)?.ScalarPredefinedTypeName;

        if (predefinedTypeName is not { TypeKeyword: { } })
        {
            return false;
        }

        return predefinedTypeName.TypeKeyword.GetTokenType() == CSharpTokenType.VOID_KEYWORD;
    }

    /// <remarks>
    /// This method has been removed from ReSharper 10 SDK.
    /// </remarks>
    public static bool IsVoidMethodDeclaration(this IMethodDeclaration methodDeclaration)
    {
        var predefinedTypeName = (methodDeclaration.TypeUsage as IPredefinedTypeUsage)?.ScalarPredefinedTypeName;

        if (predefinedTypeName is not { TypeKeyword: { } })
        {
            return false;
        }

        return predefinedTypeName.TypeKeyword.GetTokenType() == CSharpTokenType.VOID_KEYWORD;
    }

    public static bool IsDeclaredInTestProject(this IAttributesOwnerDeclaration attributesOwnerDeclaration)
    {
        if (attributesOwnerDeclaration.GetProject() is { } project)
        {
            if (project.HasFlavour<MsTestProjectFlavor>())
            {
                return true;
            }

            if (project
                .GetAssemblyReferences(project.GetCurrentTargetFrameworkId())
                .Any(assemblyReference => assemblyReference is { } && wellKnownUnitTestingAssemblyNames.Contains(assemblyReference.Name)))
            {
                return true;
            }
        }

        return false;
    }

    [SuppressMessage("ReSharper", "EmptyGeneralCatchClause", Justification = "The local function should return false in case of any exception.")]
    public static bool IsDeclaredInOldMsTestProject(this IAttributesOwnerDeclaration attributesOwnerDeclaration)
    {
        if (attributesOwnerDeclaration.GetProject() is { } project)
        {
            static bool IsReferenceToOldMsTestAssembly(IProjectToAssemblyReference assemblyReference)
            {
                if (assemblyReference is
                    {
                        Name: "Microsoft.VisualStudio.TestPlatform.TestFramework", ReferenceTarget.HintLocation.FileAccessPath: { },
                    })
                {
                    try
                    {
                        var fileVersion = FileVersionInfo.GetVersionInfo(assemblyReference.ReferenceTarget.HintLocation.FileAccessPath);
                        return new Version(
                                fileVersion.FileMajorPart,
                                fileVersion.FileMinorPart,
                                fileVersion.FileBuildPart,
                                fileVersion.FilePrivatePart)
                            < msTest14MinFileVersion;
                    }
                    catch { }
                }

                return false;
            }

            if (project.GetAssemblyReferences(project.GetCurrentTargetFrameworkId()).Any(IsReferenceToOldMsTestAssembly))
            {
                return true;
            }
        }

        return false;
    }

    /// <remarks>
    /// This method (<c>CSharpDaemonUtil.IsUnderAnonymousMethod</c>) has been removed from ReSharper 10 SDK.
    /// </remarks>
    public static bool IsUnderAnonymousMethod(this ITreeNode? element)
    {
        foreach (var treeNode in element.ContainingNodes())
        {
            if (treeNode is IAnonymousFunctionExpression or IQueryParameterPlatform)
            {
                return true;
            }
        }

        return false;
    }

    /// <remarks>
    /// This method (<c>CollectionTypeUtil.GetKeyValueTypesForGenericDictionary</c>) has been removed from ReSharper 10 SDK.
    /// </remarks>
    public static IList<Pair<IType, IType>>? GetKeyValueTypesForGenericDictionary(this IDeclaredType declaredType)
    {
        if (declaredType.GetTypeElement() is not { } typeElement1)
        {
            return null;
        }

        if (declaredType.Module.GetPredefinedType().GenericIDictionary.GetTypeElement() is not { } typeElement2)
        {
            return null;
        }

        if (!typeElement1.IsDescendantOf(typeElement2))
        {
            return null;
        }

        var ancestorSubstitution = typeElement1.GetAncestorSubstitution(typeElement2);
        var localList = new LocalList<Pair<IType, IType>>();
        foreach (var substitution1 in ancestorSubstitution)
        {
            var substitution2 = declaredType.GetSubstitution().Apply(substitution1);
            var typeParameters = typeElement2.TypeParameters;
            var first = substitution2[typeParameters[0]];
            var second = substitution2[typeParameters[1]];
            localList.Add(Pair.Of(first, second));
        }

        return localList.ResultingList();
    }

    [JetBrains.Annotations.Pure]
    public static void Deconstruct<K, V>(this KeyValuePair<K, V> pair, out K key, out V value)
    {
        key = pair.Key;
        value = pair.Value;
    }

    [JetBrains.Annotations.Pure]
    public static bool IsOnLocalFunctionWithUnsupportedAttributes(this IAttributesOwnerDeclaration attributesOwnerDeclaration)
    {
        if (attributesOwnerDeclaration.GetCSharpLanguageLevel() >= CSharpLanguageLevel.CSharp90)
        {
            return false;
        }

        return attributesOwnerDeclaration.DeclaredElement is IParameter parameter && parameter.ContainingParametersOwner.IsLocalFunction()
            || attributesOwnerDeclaration.DeclaredElement is ILocalFunctionDeclaration;
    }

    [JetBrains.Annotations.Pure]
    public static bool IsOnLambdaExpressionWithUnsupportedAttributes(this IAttributesOwnerDeclaration attributesOwnerDeclaration)
    {
        if (attributesOwnerDeclaration.GetCSharpLanguageLevel() >= CSharpLanguageLevel.CSharp100)
        {
            return false;
        }

        return attributesOwnerDeclaration.DeclaredElement is IParameter { ContainingParametersOwner: ILambdaExpression } or ILambdaExpression;
    }

    [JetBrains.Annotations.Pure]
    public static bool IsOnAnonymousMethodWithUnsupportedAttributes(this IAttributesOwnerDeclaration attributesOwnerDeclaration)
        => attributesOwnerDeclaration.DeclaredElement is IParameter { ContainingParametersOwner: IAnonymousMethodExpression }
            or IAnonymousMethodExpression;
}