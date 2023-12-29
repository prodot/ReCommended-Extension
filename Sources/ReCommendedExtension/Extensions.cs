using JetBrains.Application.Settings;
using JetBrains.Metadata.Reader.API;
using JetBrains.ProjectModel;
using JetBrains.ProjectModel.Properties.Flavours;
using JetBrains.ReSharper.Feature.Services.Daemon;
using JetBrains.ReSharper.Feature.Services.LinqTools;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.CodeAnnotations;
using JetBrains.ReSharper.Psi.ControlFlow;
using JetBrains.ReSharper.Psi.CSharp;
using JetBrains.ReSharper.Psi.CSharp.Conversions;
using JetBrains.ReSharper.Psi.CSharp.Parsing;
using JetBrains.ReSharper.Psi.CSharp.Tree;
using JetBrains.ReSharper.Psi.CSharp.Util;
using JetBrains.ReSharper.Psi.Modules;
using JetBrains.ReSharper.Psi.Tree;
using JetBrains.ReSharper.Psi.Util;
using JetBrains.Util;

namespace ReCommendedExtension;

internal static class Extensions
{
    static readonly HashSet<string> wellKnownUnitTestingAssemblyNames = new(StringComparer.OrdinalIgnoreCase)
    {
        "Microsoft.VisualStudio.TestPlatform.TestFramework", @"nunit.framework", "xunit.core",
    };

    [Pure]
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

    [Pure]
    public static IEnumerable<T> WithoutObsolete<T>(this IEnumerable<T> fields) where T : class, IAttributesOwner
        => from field in fields where !field.HasAttributeInstance(PredefinedType.OBSOLETE_ATTRIBUTE_CLASS, false) select field;

    [Pure]
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

    [Pure]
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

    [Pure]
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

    [Pure]
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

    [MustUseReturnValue]
    public static ValueAnalysisMode GetValueAnalysisMode(this ElementProblemAnalyzerData data)
        => data.SettingsStore.GetValue<HighlightingSettings, ValueAnalysisMode>(s => s.ValueAnalysisMode);

    [Pure]
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

    [Pure]
    public static void Deconstruct<K, V>(this KeyValuePair<K, V> pair, out K key, out V value)
    {
        key = pair.Key;
        value = pair.Value;
    }

    [Pure]
    public static bool IsOnLocalFunctionWithUnsupportedAttributes(this IAttributesOwnerDeclaration attributesOwnerDeclaration)
    {
        if (attributesOwnerDeclaration.GetCSharpLanguageLevel() >= CSharpLanguageLevel.CSharp90)
        {
            return false;
        }

        return attributesOwnerDeclaration.DeclaredElement is IParameter parameter && parameter.ContainingParametersOwner.IsLocalFunction()
            || attributesOwnerDeclaration.DeclaredElement is ILocalFunctionDeclaration;
    }

    [Pure]
    public static bool IsOnLambdaExpressionWithUnsupportedAttributes(this IAttributesOwnerDeclaration attributesOwnerDeclaration)
    {
        if (attributesOwnerDeclaration.GetCSharpLanguageLevel() >= CSharpLanguageLevel.CSharp100)
        {
            return false;
        }

        return attributesOwnerDeclaration.DeclaredElement is IParameter { ContainingParametersOwner: ILambdaExpression } or ILambdaExpression;
    }

    [Pure]
    public static bool IsOnAnonymousMethodWithUnsupportedAttributes(this IAttributesOwnerDeclaration attributesOwnerDeclaration)
        => attributesOwnerDeclaration.DeclaredElement is IParameter { ContainingParametersOwner: IAnonymousMethodExpression }
            or IAnonymousMethodExpression;

    [Pure]
    public static bool IsDisposable(this ITypeElement type, IPsiModule psiModule)
        => type.IsClrType(PredefinedType.IDISPOSABLE_FQN)
            || type.IsClrType(PredefinedType.IASYNCDISPOSABLE_FQN)
            || type.IsDescendantOf(PredefinedType.IDISPOSABLE_FQN.TryGetTypeElement(psiModule))
            || type.IsDescendantOf(PredefinedType.IASYNCDISPOSABLE_FQN.TryGetTypeElement(psiModule));

    [Pure]
    public static bool IsDisposeMethod(this IMethod method)
        => method is
            {
                ShortName: "Dispose",
                IsStatic: false,
                AccessibilityDomain.DomainType: AccessibilityDomain.AccessibilityDomainType.INTERNAL
                or AccessibilityDomain.AccessibilityDomainType.PUBLIC,
                TypeParameters: [],
                Parameters: [],
            }
            && method.ReturnType.IsVoid();

    [Pure]
    public static bool IsDisposeAsyncMethod(this IMethod method)
        => method is
            {
                ShortName: "DisposeAsync",
                IsStatic: false,
                AccessibilityDomain.DomainType: AccessibilityDomain.AccessibilityDomainType.INTERNAL
                or AccessibilityDomain.AccessibilityDomainType.PUBLIC,
                TypeParameters: [],
                Parameters: [],
            }
            && method.ReturnType.IsValueTask();

    [Pure]
    public static bool HasDisposeMethods(this IStruct type)
    {
        Debug.Assert(type.IsByRefLike);

        return type.Methods.Any(
            method => method.IsDisposeMethod()
                || method.IsDisposeAsyncMethod()
                || method.GetAttributeInstances(false).Any(attribute => attribute.GetAttributeShortName() == nameof(HandlesResourceDisposalAttribute))
                && method is
                {
                    IsStatic: false,
                    AccessibilityDomain.DomainType: AccessibilityDomain.AccessibilityDomainType.INTERNAL
                    or AccessibilityDomain.AccessibilityDomainType.PUBLIC,
                });
    }

    [Pure]
    public static bool IsDisposable(this IType type, ITreeNode context)
        => type.GetTypeElement() is { } typeElement
            && (typeElement.IsDisposable(context.GetPsiModule()) && !type.IsTask() && !type.IsGenericTask()
                || type is IDeclaredType declaredType && declaredType.GetTypeElement() is IStruct { IsByRefLike: true } s && s.HasDisposeMethods());

    [Pure]
    public static bool IsTasklikeOfIsDisposable(this IType type, ITreeNode context)
        => type.IsTasklike(context)
            && type.GetTasklikeUnderlyingType(context).GetTypeElement() is { } awaitedTypeElement
            && awaitedTypeElement.IsDisposable(context.GetPsiModule())
            && !awaitedTypeElement.Type().IsTask()
            && !awaitedTypeElement.Type().IsGenericTask();

    [Pure]
    public static ITypeElement? TryGetAnnotationAttributeType(this IAttributesOwnerDeclaration attributesOwnerDeclaration, string attributeShortName)
        => attributesOwnerDeclaration
            .GetPsiServices()
            .GetComponent<CodeAnnotationsConfiguration>()
            .GetAttributeTypeForElement(attributesOwnerDeclaration, attributeShortName);

    [Pure]
    public static bool IsAnnotationProvided(this IAttributesOwnerDeclaration attributesOwnerDeclaration, string attributeShortName)
        => attributesOwnerDeclaration.TryGetAnnotationAttributeType(attributeShortName) is { };

    [Pure]
    public static string WithoutSuffix(this string attributeShortName)
    {
        Debug.Assert(attributeShortName.EndsWith("Attribute", StringComparison.Ordinal));

        return attributeShortName[..^"Attribute".Length];
    }

    [Pure]
    public static string WithFirstCharacterUpperCased(this string value)
    {
        Debug.Assert(value is [>= 'a' and <= 'z', ..]);

        return $"{value[0].ToUpperFast().ToString()}{value[1..]}";
    }
}