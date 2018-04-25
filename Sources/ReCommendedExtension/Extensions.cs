using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Diagnostics.Contracts;
using System.Linq;
using JetBrains.Annotations;
using JetBrains.Application.Settings;
using JetBrains.ReSharper.Feature.Services.Daemon;
using JetBrains.ReSharper.Feature.Services.LinqTools;
using JetBrains.ReSharper.Intentions.Util;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.ControlFlow;
using JetBrains.ReSharper.Psi.CSharp;
using JetBrains.ReSharper.Psi.CSharp.Conversions;
using JetBrains.ReSharper.Psi.CSharp.Parsing;
using JetBrains.ReSharper.Psi.CSharp.Tree;
using JetBrains.ReSharper.Psi.Modules;
using JetBrains.ReSharper.Psi.Resolve;
using JetBrains.ReSharper.Psi.Tree;
using JetBrains.ReSharper.Psi.Util;

namespace ReCommendedExtension
{
    internal static class Extensions
    {
        [NotNull]
        static readonly string contractClassFullName = typeof(Contract).FullName.AssertNotNull();

        static string TryGetMemberName([NotNull] this IExpressionStatement expressionStatement, [NotNull] string classFullName)
            => (expressionStatement.Expression as IInvocationExpression)?.InvokedExpression is IReferenceExpression referenceExpression &&
                ((referenceExpression.QualifierExpression as IReferenceExpression)?.Reference.Resolve().DeclaredElement as IClass)?.GetClrName()
                .FullName ==
                classFullName
                    ? referenceExpression.Reference.GetName()
                    : null;

        static void CopyTypeParameterConstraints<P>(
            [NotNull] CSharpElementFactory factory,
            TreeNodeCollection<P> source,
            TreeNodeCollection<P> destination,
            [InstantHandle][NotNull] Action<ITypeParameterConstraintsClause> addClause) where P : class, ITypeParameterDeclaration
        {
            var typeParameterMap = new Dictionary<ITypeParameter, IType>();
            for (var i = 0; i < source.Count; i++)
            {
                Debug.Assert(source[i] != null);

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
                Debug.Assert(source[i] != null);

                var typeParameter = source[i].DeclaredElement;
                var typeParameterDeclaration = destination[i];

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

            if (typeDeclaration is IInterfaceDeclaration &&
                typeDeclaration.DeclaredName.StartsWith("I", StringComparison.Ordinal) &&
                typeDeclaration.DeclaredName.Length > 1)
            {
                suggestedContractClassName = suggestedContractClassName.Remove(0, 1);
            }

            return suggestedContractClassName;
        }

        [DebuggerStepThrough]
        [NotNull]
        internal static T AssertNotNull<T>(this T value) where T : class
        {
            Debug.Assert(value != null);

            return value;
        }

        internal static bool OverridesInheritedMember([NotNull] this IDeclaration declaration)
        {
            if (!declaration.IsValid())
            {
                return false;
            }

            if (declaration.DeclaredElement is IOverridableMember overridableMember && overridableMember.GetImmediateSuperMembers().Any())
            {
                return true;
            }

            if ((declaration.DeclaredElement as IParameter)?.ContainingParametersOwner is IOverridableMember parameterOverridableMember &&
                parameterOverridableMember.GetImmediateSuperMembers().Any())
            {
                return true;
            }

            return false;
        }

        [NotNull]
        [ItemNotNull]
        internal static IEnumerable<T> WithoutObsolete<T>([NotNull][ItemNotNull] this IEnumerable<T> fields) where T : class, IAttributesOwner
            => from field in fields where !field.HasAttributeInstance(PredefinedType.OBSOLETE_ATTRIBUTE_CLASS, false) select field;

        internal static string TryGetContractName([NotNull] this IExpressionStatement expressionStatement)
            => expressionStatement.TryGetMemberName(contractClassFullName);

        [NotNull]
        internal static IClassDeclaration EnsureContractClass([NotNull] this ICSharpTypeDeclaration typeDeclaration, [NotNull] IPsiModule psiModule)
        {
            var factory = CSharpElementFactory.GetInstance(typeDeclaration);

            IClassDeclaration contractClassDeclaration = null;

            var attributeInstance = typeDeclaration.DeclaredElement?.GetAttributeInstances(ClrTypeNames.ContractClassAttribute, false)
                .FirstOrDefault();
            if (attributeInstance != null && attributeInstance.PositionParameterCount > 0)
            {
                var typeElement = attributeInstance.PositionParameter(0).TypeValue.GetTypeElement<IClass>();
                if (typeElement != null)
                {
                    contractClassDeclaration = typeElement.GetDeclarations().FirstOrDefault() as IClassDeclaration;
                }
            }

            if (contractClassDeclaration == null)
            {
                var typeParameters = typeDeclaration.TypeParametersEnumerable.Any()
                    ? $"<{string.Join(", ", from typeParameter in typeDeclaration.TypeParametersEnumerable select typeParameter.DeclaredName)}>"
                    : "";
                var typeParametersForAttribute = typeDeclaration.TypeParametersEnumerable.Any()
                    ? $"<{new string(',', typeDeclaration.TypeParametersEnumerable.Count() - 1)}>"
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
                    clause => contractClassDeclaration.AddTypeParameterConstraintsClauseBefore(clause.AssertNotNull(), null));

                var attributeTypeParameters = contractClassDeclaration.TypeParametersEnumerable.Any()
                    ? $"<{new string(',', contractClassDeclaration.TypeParametersEnumerable.Count() - 1)}>"
                    : "";
                var typeofExpression = (ITypeofExpression)factory.CreateExpression(
                    string.Format(@"typeof($0{0})", attributeTypeParameters),
                    contractClassDeclaration.DeclaredElement);

                // todo: the generated "typeof" expression doesn't contain generics: "<,>"
                var contractClassAttributeTypeElement = TypeElementUtil.GetTypeElementByClrName(ClrTypeNames.ContractClassAttribute, psiModule);
                var attribute = factory.CreateAttribute(
                    contractClassAttributeTypeElement,
                    new[] { new AttributeValue(typeofExpression.ArgumentType) },
                    new JetBrains.Util.Pair<string, AttributeValue>[] { });

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
        internal static IMethodDeclaration EnsureOverriddenMethodInContractClass(
            [NotNull] this IMethodDeclaration methodDeclaration,
            [NotNull] IClassDeclaration contractClassDeclaration)
        {
            var factory = CSharpElementFactory.GetInstance(methodDeclaration);

            var declaredElement = methodDeclaration.DeclaredElement;

            Debug.Assert(declaredElement != null);

            // todo: find a better way to compare instances (than using hash codes)

            var overriddenMethodDeclaration =
            (
                from d in contractClassDeclaration.MethodDeclarationsEnumerable
                where d.DeclaredElement != null &&
                    d.DeclaredElement.GetImmediateSuperMembers()
                        .Any(overridableMemberInstance => overridableMemberInstance.GetHashCode() == declaredElement.GetHashCode())
                select d).FirstOrDefault();

            if (overriddenMethodDeclaration == null)
            {
                var typeParameters = methodDeclaration.TypeParameterDeclarationsEnumerable.Any()
                    ? string.Format(
                        "<{0}>",
                        string.Join(
                            ", ",
                            from typeParameter in methodDeclaration.TypeParameterDeclarationsEnumerable select typeParameter.DeclaredName))
                    : "";

                overriddenMethodDeclaration = (IMethodDeclaration)factory.CreateTypeMemberDeclaration(
                    string.Format(
                        "$0 {0}{1}() {{{2}}}",
                        methodDeclaration.DeclaredName,
                        typeParameters,
                        declaredElement.ReturnType.IsVoid() ? "" : " return default($0); "),
                    declaredElement.ReturnType);
                overriddenMethodDeclaration.SetAccessRights(methodDeclaration.GetAccessRights());
                overriddenMethodDeclaration.SetOverride(methodDeclaration.GetContainingTypeDeclaration() is IClassDeclaration);

                foreach (var parameterDeclaration in methodDeclaration.ParameterDeclarationsEnumerable)
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
                    clause => overriddenMethodDeclaration.AddTypeParameterConstraintsClauseBefore(clause.AssertNotNull(), null));

                overriddenMethodDeclaration = contractClassDeclaration.AddClassMemberDeclaration(overriddenMethodDeclaration);

                ContextActionUtils.FormatWithDefaultProfile(overriddenMethodDeclaration);
            }

            return overriddenMethodDeclaration;
        }

        [NotNull]
        internal static IIndexerDeclaration EnsureOverriddenIndexerInContractClass(
            [NotNull] this IIndexerDeclaration indexerDeclaration,
            [NotNull] IClassDeclaration contractClassDeclaration)
        {
            var factory = CSharpElementFactory.GetInstance(indexerDeclaration);

            // todo: find a better way to compare instances (than using hash codes)
            var overriddenIndexerDeclaration = (
                from d in contractClassDeclaration.IndexerDeclarationsEnumerable
                where d.DeclaredElement != null &&
                    d.DeclaredElement.GetImmediateSuperMembers()
                        .Any(
                            overridableMemberInstance => overridableMemberInstance.GetHashCode() ==
                                indexerDeclaration.DeclaredElement.AssertNotNull().GetHashCode())
                select d).FirstOrDefault();

            if (overriddenIndexerDeclaration == null)
            {
                Debug.Assert(indexerDeclaration.DeclaredElement != null);

                overriddenIndexerDeclaration = (IIndexerDeclaration)factory.CreateTypeMemberDeclaration(
                    string.Format(
                        "$0 this[] {{{0}{1}}}",
                        indexerDeclaration.DeclaredElement.IsReadable ? " get { return default($0); } " : "",
                        indexerDeclaration.DeclaredElement.IsWritable ? " set { } " : ""),
                    indexerDeclaration.DeclaredElement.Type);
                overriddenIndexerDeclaration.SetAccessRights(indexerDeclaration.GetAccessRights());
                overriddenIndexerDeclaration.SetOverride(indexerDeclaration.GetContainingTypeDeclaration() is IClassDeclaration);

                foreach (var parameterDeclaration in indexerDeclaration.ParameterDeclarationsEnumerable)
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
        internal static IPropertyDeclaration EnsureOverriddenPropertyInContractClass(
            [NotNull] this IPropertyDeclaration propertyDeclaration,
            [NotNull] IClassDeclaration contractClassDeclaration)
        {
            var factory = CSharpElementFactory.GetInstance(propertyDeclaration);

            // todo: find a better way to compare instances (than using hash codes)
            var overriddenPropertyDeclaration = (
                from d in contractClassDeclaration.PropertyDeclarationsEnumerable
                where d.DeclaredElement != null &&
                    d.DeclaredElement.GetImmediateSuperMembers()
                        .Any(
                            overridableMemberInstance => overridableMemberInstance.GetHashCode() ==
                                propertyDeclaration.DeclaredElement.AssertNotNull().GetHashCode())
                select d).FirstOrDefault();

            if (overriddenPropertyDeclaration == null)
            {
                Debug.Assert(propertyDeclaration.DeclaredElement != null);

                overriddenPropertyDeclaration = (IPropertyDeclaration)factory.CreateTypeMemberDeclaration(
                    string.Format(
                        "$0 {0} {{{1}{2}}}",
                        propertyDeclaration.DeclaredName,
                        propertyDeclaration.DeclaredElement.IsReadable ? " get { return default($0); } " : "",
                        propertyDeclaration.DeclaredElement.IsWritable ? " set { } " : ""),
                    propertyDeclaration.DeclaredElement.Type);
                overriddenPropertyDeclaration.SetAccessRights(propertyDeclaration.GetAccessRights());
                overriddenPropertyDeclaration.SetOverride(propertyDeclaration.GetContainingTypeDeclaration() is IClassDeclaration);

                overriddenPropertyDeclaration = contractClassDeclaration.AddClassMemberDeclaration(overriddenPropertyDeclaration);

                ContextActionUtils.FormatWithDefaultProfile(overriddenPropertyDeclaration);
            }

            return overriddenPropertyDeclaration;
        }

        [NotNull]
        internal static IMethodDeclaration EnsureContractInvariantMethod(
            [NotNull] this IClassLikeDeclaration classLikeDeclaration,
            [NotNull] IPsiModule psiModule)
        {
            var factory = CSharpElementFactory.GetInstance(classLikeDeclaration);

            var contractInvariantMethodDeclaration = classLikeDeclaration.MethodDeclarationsEnumerable.FirstOrDefault(
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

        internal static bool IsGenericArray([NotNull] this IType type, [NotNull] ITreeNode context)
        {
            var elementType = CollectionTypeUtil.ElementTypeByCollectionType(type, context);
            if (elementType != null)
            {
                for (var i = 1; i < 16; i++)
                {
                    if (type.IsImplicitlyConvertibleTo(TypeFactory.CreateArrayType(elementType, i), context.GetTypeConversionRule()))
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        internal static bool IsGenericEnumerableOrDescendant([NotNull] this IType type)
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
        internal static bool IsDefaultValueOf([NotNull] this ITreeNode element, [NotNull] IType type)
        {
            switch (element)
            {
                case ICSharpLiteralExpression literalExpression when literalExpression.Literal?.GetTokenType() == CSharpTokenType.DEFAULT_KEYWORD:
                case IDefaultExpression defaultExpression when Equals(defaultExpression.Type(), type):
                    return true;
            }

            if (type.IsUnconstrainedGenericType())
            {
                // unconstrained generic type
            }
            else if (type.IsValueType())
            {
                // value type (non-nullable and nullable)

                switch (element)
                {
                    case IConstantValueOwner constantValueOwner when type.IsNullable()
                        ? constantValueOwner.ConstantValue.IsNull()
                        : constantValueOwner.ConstantValue.IsDefaultValue(type, element):

                    case IObjectCreationExpression objectCreationExpression
                        when Equals(objectCreationExpression.Type(), type) && objectCreationExpression.Arguments.Count == 0:

                    case IAsExpression asExpression when asExpression.Operand != null &&
                        asExpression.Operand.ConstantValue.IsNull() &&
                        Equals(asExpression.Operand.Type(), type) &&
                        type.IsNullable():
                        return true;
                }
            }
            else
            {
                // reference type

                switch (element)
                {
                    case IConstantValueOwner constantValueOwner when constantValueOwner.ConstantValue.IsNull() ||
                        constantValueOwner.ConstantValue.IsDefaultValue(type, element):

                    case IAsExpression asExpression when asExpression.Operand != null &&
                        asExpression.Operand.ConstantValue.IsNull() &&
                        Equals(asExpression.Operand.Type(), type):
                        return true;
                }
            }

            return false;
        }

        internal static ValueAnalysisMode GetValueAnalysisMode([NotNull] this ElementProblemAnalyzerData data)
            => data.SettingsStore.GetValue<HighlightingSettings, ValueAnalysisMode>(s => s.ValueAnalysisMode);

        /// <summary>
        /// Returns the object creation expression <c>new EventHandler(</c><paramref name="argument"/><c>)</c> if used in the pattern
        /// <c>new EventHandler(</c><paramref name="argument"/><c>)</c> or <c>null</c>.
        /// </summary>
        static IObjectCreationExpression TryGetDelegateCreation([NotNull] ICSharpArgument argument)
        {
            var argumentList = argument.Parent as IArgumentList;

            if (argumentList?.ArgumentsEnumerable.Count() != 1)
            {
                return null;
            }

            return argumentList.Parent as IObjectCreationExpression;
        }

        internal static bool IsEventTarget([NotNull] this IReference reference)
        {
            var treeNode = reference.GetTreeNode();

            if (treeNode.Parent is IAssignmentExpression assignmentExpression)
            {
                return assignmentExpression.IsEventSubscriptionOrUnSubscription();
            }

            if (treeNode.Parent is ICSharpArgument argument)
            {
                var delegateCreation = TryGetDelegateCreation(argument);
                if (delegateCreation != null)
                {
                    assignmentExpression = delegateCreation.Parent as IAssignmentExpression;
                    if (assignmentExpression != null)
                    {
                        return assignmentExpression.IsEventSubscriptionOrUnSubscription();
                    }
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
        internal static bool IsEventSubscriptionOrUnSubscription([NotNull] this IAssignmentExpression assignmentExpression)
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

        internal static bool IsVoidMethodDeclaration([NotNull] this ILocalFunctionDeclaration localFunctionDeclaration)
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
        internal static bool IsVoidMethodDeclaration([NotNull] this IMethodDeclaration methodDeclaration)
        {
            var predefinedTypeName = (methodDeclaration.TypeUsage as IPredefinedTypeUsage)?.ScalarPredefinedTypeName;

            if (predefinedTypeName?.TypeKeyword == null)
            {
                return false;
            }

            return predefinedTypeName.TypeKeyword.GetTokenType() == CSharpTokenType.VOID_KEYWORD;
        }

        /// <remarks>
        /// This method (<c>CSharpDaemonUtil.IsUnderAnonymousMethod</c>) has been removed from ReSharper 10 SDK.
        /// </remarks>
        internal static bool IsUnderAnonymousMethod(this ITreeNode element)
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
        [SuppressMessage("ReSharper", "AssignNullToNotNullAttribute", Justification = "The method was imported.")]
        internal static IList<JetBrains.Util.Pair<IType, IType>> GetKeyValueTypesForGenericDictionary([NotNull] this IDeclaredType declaredType)
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
            var localList = new JetBrains.Util.LocalList<JetBrains.Util.Pair<IType, IType>>();
            foreach (var substitution1 in ancestorSubstitution)
            {
                var substitution2 = declaredType.GetSubstitution().Apply(substitution1);
                var typeParameters = typeElement2.TypeParameters;
                var first = substitution2[typeParameters[0]];
                var second = substitution2[typeParameters[1]];
                localList.Add(JetBrains.Util.Pair.Of(first, second));
            }

            return localList.ResultingList();
        }
    }
}