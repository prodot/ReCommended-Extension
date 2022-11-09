using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Threading.Tasks;
using JetBrains.Annotations;
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

namespace ReCommendedExtension
{
    internal static class Extensions
    {
        [NotNull]
        static readonly string contractClassFullName = typeof(Contract).FullName.AssertNotNull();

        [NotNull]
        [ItemNotNull]
        static readonly HashSet<string> wellKnownUnitTestingAssemblyNames = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
        {
            "Microsoft.VisualStudio.TestPlatform.TestFramework", @"nunit.framework", "xunit.core",
        };

        [NotNull]
        static readonly Version msTest14MinFileVersion = new Version(14, 0, 3021, 1);

        [CanBeNull]
        static string TryGetMemberName([NotNull] this IExpressionStatement expressionStatement, [NotNull] string classFullName)
            => (expressionStatement.Expression as IInvocationExpression)?.InvokedExpression is IReferenceExpression referenceExpression
                && ((referenceExpression.QualifierExpression as IReferenceExpression)?.Reference.Resolve().DeclaredElement as IClass)?.GetClrName()
                .FullName
                == classFullName
                    ? referenceExpression.Reference.GetName()
                    : null;

        static void CopyTypeParameterConstraints<P>(
            [NotNull] CSharpElementFactory factory,
            [ItemNotNull] TreeNodeCollection<P> source,
            TreeNodeCollection<P> destination,
            [InstantHandle][NotNull] Action<ITypeParameterConstraintsClause> addClause) where P : class, ITypeParameterDeclaration
        {
            var typeParameterMap = new Dictionary<ITypeParameter, IType>();
            for (var i = 0; i < source.Count; i++)
            {
                var typeParameterDeclaration = destination[i];
                var originalTypeParameter = source[i].DeclaredElement;

                Debug.Assert(originalTypeParameter != null);
                Debug.Assert(typeParameterDeclaration != null);
                Debug.Assert(typeParameterDeclaration.DeclaredElement != null);

                typeParameterMap.Add(originalTypeParameter, TypeFactory.CreateType(typeParameterDeclaration.DeclaredElement));
            }

            var newSubstitution = EmptySubstitution.INSTANCE.Extend(typeParameterMap);
            for (var i = 0; i < source.Count; i++)
            {
                var typeParameter = source[i].DeclaredElement;
                var typeParameterDeclaration = destination[i];

                Debug.Assert(typeParameter != null);
                Debug.Assert(typeParameterDeclaration != null);

                var clause = factory.CreateTypeParameterConstraintsClause(typeParameter, newSubstitution, typeParameterDeclaration.DeclaredName);
                if (clause != null)
                {
                    addClause(clause);
                }
            }
        }

        [NotNull]
        static string GetSuggestedContractClassName([NotNull] ICSharpTypeDeclaration typeDeclaration)
        {
            var suggestedContractClassName = typeDeclaration.DeclaredName + "Contract";

            if (typeDeclaration is IInterfaceDeclaration
                && typeDeclaration.DeclaredName.StartsWith("I", StringComparison.Ordinal)
                && typeDeclaration.DeclaredName.Length > 1)
            {
                suggestedContractClassName = suggestedContractClassName.Remove(0, 1);
            }

            return suggestedContractClassName;
        }

        [DebuggerStepThrough]
        [NotNull]
        public static T AssertNotNull<T>([CanBeNull] this T value) where T : class
        {
            Debug.Assert(value != null);

            return value;
        }

        public static bool OverridesInheritedMember([NotNull] this IDeclaration declaration)
        {
            if (!declaration.IsValid())
            {
                return false;
            }

            if (declaration.DeclaredElement is IOverridableMember overridableMember && overridableMember.GetImmediateSuperMembers().Any())
            {
                return true;
            }

            if ((declaration.DeclaredElement as IParameter)?.ContainingParametersOwner is IOverridableMember parameterOverridableMember
                && parameterOverridableMember.GetImmediateSuperMembers().Any())
            {
                return true;
            }

            return false;
        }

        [NotNull]
        [ItemNotNull]
        public static IEnumerable<T> WithoutObsolete<T>([NotNull][ItemNotNull] this IEnumerable<T> fields) where T : class, IAttributesOwner
            => from field in fields where !field.HasAttributeInstance(PredefinedType.OBSOLETE_ATTRIBUTE_CLASS, false) select field;

        [CanBeNull]
        public static string TryGetContractName([NotNull] this IExpressionStatement expressionStatement)
            => expressionStatement.TryGetMemberName(contractClassFullName);

        [NotNull]
        public static IClassDeclaration EnsureContractClass([NotNull] this ICSharpTypeDeclaration typeDeclaration, [NotNull] IPsiModule psiModule)
        {
            var factory = CSharpElementFactory.GetInstance(typeDeclaration);

            IClassDeclaration contractClassDeclaration = null;

            var attributeInstance = typeDeclaration.DeclaredElement?.GetAttributeInstances(ClrTypeNames.ContractClassAttribute, false)
                .FirstOrDefault();
            if (attributeInstance?.PositionParameterCount > 0)
            {
                var typeElement = attributeInstance.PositionParameter(0).TypeValue.GetTypeElement<IClass>();
                if (typeElement != null)
                {
                    contractClassDeclaration = typeElement.GetDeclarations().FirstOrDefault() as IClassDeclaration;
                }
            }

            if (contractClassDeclaration == null)
            {
                var typeParameters = typeDeclaration.TypeParameters.Any()
                    ? $"<{string.Join(", ", from typeParameter in typeDeclaration.TypeParameters select typeParameter.DeclaredName)}>"
                    : "";
                var typeParametersForAttribute = typeDeclaration.TypeParameters.Any()
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
                    $@"typeof($0{attributeTypeParameters})",
                    contractClassDeclaration.DeclaredElement);

                // todo: the generated "typeof" expression doesn't contain generics: "<,>"
                var contractClassAttributeTypeElement = TypeElementUtil.GetTypeElementByClrName(ClrTypeNames.ContractClassAttribute, psiModule);
                Debug.Assert(contractClassAttributeTypeElement != null);
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
                    var parentNamespaceDeclaration = typeDeclaration.GetContainingNamespaceDeclaration();
                    if (parentNamespaceDeclaration != null)
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

        [NotNull]
        public static IMethodDeclaration EnsureOverriddenMethodInContractClass(
            [NotNull] this IMethodDeclaration methodDeclaration,
            [NotNull] IClassDeclaration contractClassDeclaration)
        {
            var factory = CSharpElementFactory.GetInstance(methodDeclaration);

            var declaredElement = methodDeclaration.DeclaredElement;

            Debug.Assert(declaredElement != null);

            // todo: find a better way to compare instances (than using hash codes)

            var overriddenMethodDeclaration =
            (
                from d in contractClassDeclaration.MethodDeclarations
                where d.DeclaredElement != null
                    && d.DeclaredElement.GetImmediateSuperMembers()
                        .Any(overridableMemberInstance => overridableMemberInstance.GetHashCode() == declaredElement.GetHashCode())
                select d).FirstOrDefault();

            if (overriddenMethodDeclaration == null)
            {
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
                    Debug.Assert(parameterDeclaration != null);

                    var parameter = parameterDeclaration.DeclaredElement;

                    Debug.Assert(parameter != null);

                    overriddenMethodDeclaration.AddParameterDeclarationBefore(
                        factory.CreateParameterDeclaration(
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

        [NotNull]
        public static IIndexerDeclaration EnsureOverriddenIndexerInContractClass(
            [NotNull] this IIndexerDeclaration indexerDeclaration,
            [NotNull] IClassDeclaration contractClassDeclaration)
        {
            var factory = CSharpElementFactory.GetInstance(indexerDeclaration);

            // todo: find a better way to compare instances (than using hash codes)
            var overriddenIndexerDeclaration = (
                from d in contractClassDeclaration.IndexerDeclarations
                where d.DeclaredElement != null
                    && d.DeclaredElement.GetImmediateSuperMembers()
                        .Any(
                            overridableMemberInstance => overridableMemberInstance.GetHashCode()
                                == indexerDeclaration.DeclaredElement.AssertNotNull().GetHashCode())
                select d).FirstOrDefault();

            if (overriddenIndexerDeclaration == null)
            {
                Debug.Assert(indexerDeclaration.DeclaredElement != null);

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
                    Debug.Assert(parameterDeclaration != null);

                    var declaredElement = parameterDeclaration.DeclaredElement;

                    Debug.Assert(declaredElement != null);

                    overriddenIndexerDeclaration.AddParameterDeclarationBefore(
                        factory.CreateParameterDeclaration(
                            declaredElement.Kind,
                            declaredElement.IsParameterArray,
                            declaredElement.IsVarArg,
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

        [NotNull]
        public static IPropertyDeclaration EnsureOverriddenPropertyInContractClass(
            [NotNull] this IPropertyDeclaration propertyDeclaration,
            [NotNull] IClassDeclaration contractClassDeclaration)
        {
            var factory = CSharpElementFactory.GetInstance(propertyDeclaration);

            // todo: find a better way to compare instances (than using hash codes)
            var overriddenPropertyDeclaration = (
                from d in contractClassDeclaration.PropertyDeclarations
                where d.DeclaredElement != null
                    && d.DeclaredElement.GetImmediateSuperMembers()
                        .Any(
                            overridableMemberInstance => overridableMemberInstance.GetHashCode()
                                == propertyDeclaration.DeclaredElement.AssertNotNull().GetHashCode())
                select d).FirstOrDefault();

            if (overriddenPropertyDeclaration == null)
            {
                Debug.Assert(propertyDeclaration.DeclaredElement != null);

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

        [NotNull]
        public static IMethodDeclaration EnsureContractInvariantMethod(
            [NotNull] this IClassLikeDeclaration classLikeDeclaration,
            [NotNull] IPsiModule psiModule)
        {
            var factory = CSharpElementFactory.GetInstance(classLikeDeclaration);

            var contractInvariantMethodDeclaration = classLikeDeclaration.MethodDeclarations.FirstOrDefault(
                methodDeclaration =>
                {
                    Debug.Assert(methodDeclaration != null);

                    var declaredElement = methodDeclaration.DeclaredElement;

                    Debug.Assert(declaredElement != null);

                    return declaredElement.HasAttributeInstance(ClrTypeNames.ContractInvariantMethodAttribute, false);
                });

            if (contractInvariantMethodDeclaration == null)
            {
                contractInvariantMethodDeclaration = (IMethodDeclaration)factory.CreateTypeMemberDeclaration(
                    "[$0] private void ObjectInvariant() { }",
                    TypeElementUtil.GetTypeElementByClrName(ClrTypeNames.ContractInvariantMethodAttribute, psiModule));
                contractInvariantMethodDeclaration = classLikeDeclaration.AddClassMemberDeclaration(contractInvariantMethodDeclaration);

                ContextActionUtils.FormatWithDefaultProfile(contractInvariantMethodDeclaration);
            }

            return contractInvariantMethodDeclaration;
        }

        public static bool IsGenericArray([NotNull] this IType type, [NotNull] ITreeNode context)
        {
            var elementType = CollectionTypeUtil.ElementTypeByCollectionType(type, context, false);
            if (elementType != null)
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

        public static bool IsGenericEnumerableOrDescendant([NotNull] this IType type)
        {
            if (type.IsGenericIEnumerable())
            {
                return true;
            }

            var typeElement = type.GetTypeElement<ITypeElement>();
            if (typeElement != null && typeElement.IsDescendantOf(typeElement.Module.GetPredefinedType().GenericIEnumerable.GetTypeElement()))
            {
                return true;
            }

            return false;
        }

        [JetBrains.Annotations.Pure]
        public static bool IsDefaultValueOf([NotNull] this ITreeNode element, [NotNull] IType type)
        {
            switch (element)
            {
                case ICSharpLiteralExpression literalExpression when literalExpression.Literal?.GetTokenType() == CSharpTokenType.DEFAULT_KEYWORD:
                case IDefaultExpression defaultExpression when Equals(defaultExpression.Type(), type):
                    return true;

                case IParenthesizedExpression parenthesizedExpression when parenthesizedExpression.Expression != null:
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
                            && objectCreationExpression.Arguments.Count == 0
                            && structType != null
                            && !structType.HasCustomParameterlessConstructor
                            && (objectCreationExpression.Initializer == null || objectCreationExpression.Initializer.InitializerElements.Count == 0);

                    case IAsExpression asExpression:
                        return asExpression.Operand != null
                            && asExpression.Operand.ConstantValue.IsNull()
                            && asExpression.TypeOperand != null
                            && Equals(CSharpTypeFactory.CreateType(asExpression.TypeOperand), type)
                            && type.IsNullable();

                    default: return false;
                }
            }

            // reference type

            switch (element)
            {
                case IConstantValueOwner constantValueOwner when constantValueOwner.ConstantValue.IsNull()
                    || constantValueOwner.ConstantValue.IsDefaultValue(type, element):
                    return true;

                case IAsExpression asExpression:
                    return asExpression.Operand != null
                        && asExpression.Operand.ConstantValue.IsNull()
                        && asExpression.TypeOperand != null
                        && Equals(CSharpTypeFactory.CreateType(asExpression.TypeOperand), type);

                default: return false;
            }
        }

        public static ValueAnalysisMode GetValueAnalysisMode([NotNull] this ElementProblemAnalyzerData data)
            => data.SettingsStore.GetValue<HighlightingSettings, ValueAnalysisMode>(s => s.ValueAnalysisMode);

        /// <summary>
        /// Returns the object creation expression <c>new EventHandler(</c><paramref name="argument"/><c>)</c> if used in the pattern
        /// <c>new EventHandler(</c><paramref name="argument"/><c>)</c> or <c>null</c>.
        /// </summary>
        [CanBeNull]
        static IObjectCreationExpression TryGetDelegateCreation([NotNull] ICSharpArgument argument)
        {
            var argumentList = argument.Parent as IArgumentList;

            if (argumentList?.Arguments.Count != 1)
            {
                return null;
            }

            return argumentList.Parent as IObjectCreationExpression;
        }

        public static bool IsEventTarget([NotNull] this IReference reference)
        {
            switch (reference.GetTreeNode().Parent)
            {
                case IAssignmentExpression assignmentExpression: return assignmentExpression.IsEventSubscriptionOrUnSubscription();

                case ICSharpArgument argument:
                {
                    var delegateCreation = TryGetDelegateCreation(argument);
                    if (delegateCreation != null && delegateCreation.Parent is IAssignmentExpression assignmentExpression)
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
        public static bool IsEventSubscriptionOrUnSubscription([NotNull] this IAssignmentExpression assignmentExpression)
        {
            switch (assignmentExpression.AssignmentType)
            {
                case AssignmentType.PLUSEQ:
                case AssignmentType.MINUSEQ:
                    break;

                default: return false;
            }

            if (assignmentExpression.OperatorOperands.Count != 2)
            {
                return false;
            }

            return (assignmentExpression.Dest as IReferenceExpression)?.Reference.Resolve().DeclaredElement is IEvent;
        }

        public static bool IsVoidMethodDeclaration([NotNull] this ILocalFunctionDeclaration localFunctionDeclaration)
        {
            var predefinedTypeName = (localFunctionDeclaration.TypeUsage as IPredefinedTypeUsage)?.ScalarPredefinedTypeName;

            if (predefinedTypeName?.TypeKeyword == null)
            {
                return false;
            }

            return predefinedTypeName.TypeKeyword.GetTokenType() == CSharpTokenType.VOID_KEYWORD;
        }

        /// <remarks>
        /// This method has been removed from ReSharper 10 SDK.
        /// </remarks>
        public static bool IsVoidMethodDeclaration([NotNull] this IMethodDeclaration methodDeclaration)
        {
            var predefinedTypeName = (methodDeclaration.TypeUsage as IPredefinedTypeUsage)?.ScalarPredefinedTypeName;

            if (predefinedTypeName?.TypeKeyword == null)
            {
                return false;
            }

            return predefinedTypeName.TypeKeyword.GetTokenType() == CSharpTokenType.VOID_KEYWORD;
        }

        public static bool IsConfigureAwaitAvailable([CanBeNull] this IUnaryExpression awaitedExpression)
        {
            var typeElement = (awaitedExpression?.Type() as IDeclaredType)?.GetTypeElement();
            if (typeElement != null)
            {
                var hasConfigureAwaitMethod = typeElement.Methods.Any(
                    method =>
                    {
                        Debug.Assert(method != null);

                        if (method.ShortName == nameof(Task.ConfigureAwait) && method.Parameters.Count == 1)
                        {
                            Debug.Assert(method.Parameters[0] != null);

                            return method.Parameters[0].Type.IsBool();
                        }

                        return false;
                    });

                if (hasConfigureAwaitMethod)
                {
                    return true;
                }
            }

            return false;
        }

        public static bool IsDeclaredInTestProject([NotNull] this IAttributesOwnerDeclaration attributesOwnerDeclaration)
        {
            var project = attributesOwnerDeclaration.GetProject();
            if (project != null)
            {
                // todo: detect <ProjectCapability Include="TestContainer" /> added by NUnit and xUnit.net (and by MSTest in the future as well) packages

                if (project.HasFlavour<MsTestProjectFlavor>())
                {
                    return true;
                }

                if (project.GetAssemblyReferences(project.GetCurrentTargetFrameworkId())
                    .Any(assemblyReference => wellKnownUnitTestingAssemblyNames.Contains(assemblyReference?.Name)))
                {
                    return true;
                }
            }

            return false;
        }

        [SuppressMessage("ReSharper", "EmptyGeneralCatchClause", Justification = "The local function should return false in case of any exception.")]
        public static bool IsDeclaredInOldMsTestProject([NotNull] this IAttributesOwnerDeclaration attributesOwnerDeclaration)
        {
            var project = attributesOwnerDeclaration.GetProject();
            if (project != null)
            {
                bool IsReferenceToOldMsTestAssembly(IProjectToAssemblyReference assemblyReference)
                {
                    if (assemblyReference?.Name == "Microsoft.VisualStudio.TestPlatform.TestFramework"
                        && assemblyReference.ReferenceTarget.HintLocation?.FileAccessPath != null)
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
        public static bool IsUnderAnonymousMethod([CanBeNull] this ITreeNode element)
        {
            foreach (var treeNode in element.ContainingNodes())
            {
                if (treeNode is IAnonymousFunctionExpression || treeNode is IQueryParameterPlatform)
                {
                    return true;
                }
            }

            return false;
        }

        /// <remarks>
        /// This method (<c>CollectionTypeUtil.GetKeyValueTypesForGenericDictionary</c>) has been removed from ReSharper 10 SDK.
        /// </remarks>
        [CanBeNull]
        public static IList<Pair<IType, IType>> GetKeyValueTypesForGenericDictionary([NotNull] this IDeclaredType declaredType)
        {
            var typeElement1 = declaredType.GetTypeElement();
            if (typeElement1 == null)
            {
                return null;
            }

            var typeElement2 = declaredType.Module.GetPredefinedType().GenericIDictionary.GetTypeElement();
            if (typeElement2 == null)
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
        public static bool IsOnLocalFunctionWithUnsupportedAttributes([NotNull] this IAttributesOwnerDeclaration attributesOwnerDeclaration)
        {
            if (attributesOwnerDeclaration.GetCSharpLanguageLevel() >= CSharpLanguageLevel.CSharp90)
            {
                return false;
            }

            switch (attributesOwnerDeclaration.DeclaredElement)
            {
                case IParameter parameter when parameter.ContainingParametersOwner.IsLocalFunction():
                case ILocalFunctionDeclaration _:
                    return true;
            }

            return false;
        }

        [JetBrains.Annotations.Pure]
        public static bool IsOnLambdaExpressionWithUnsupportedAttributes([NotNull] this IAttributesOwnerDeclaration attributesOwnerDeclaration)
        {
            if (attributesOwnerDeclaration.GetCSharpLanguageLevel() >= CSharpLanguageLevel.CSharp100)
            {
                return false;
            }

            switch (attributesOwnerDeclaration.DeclaredElement)
            {
                case IParameter parameter when parameter.ContainingParametersOwner is ILambdaExpression:
                case ILambdaExpression _:
                    return true;
            }

            return false;
        }
    }
}