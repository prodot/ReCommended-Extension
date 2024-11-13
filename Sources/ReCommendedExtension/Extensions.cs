using JetBrains.Metadata.Reader.API;
using JetBrains.ProjectModel;
using JetBrains.ProjectModel.Properties.Flavours;
using JetBrains.ReSharper.Feature.Services.Daemon;
using JetBrains.ReSharper.Feature.Services.LinqTools;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.CodeAnnotations;
using JetBrains.ReSharper.Psi.CodeStyle;
using JetBrains.ReSharper.Psi.ControlFlow;
using JetBrains.ReSharper.Psi.CSharp;
using JetBrains.ReSharper.Psi.CSharp.CodeStyle.Suggestions;
using JetBrains.ReSharper.Psi.CSharp.ControlFlow;
using JetBrains.ReSharper.Psi.CSharp.Conversions;
using JetBrains.ReSharper.Psi.CSharp.Impl.ControlFlow.NullableAnalysis;
using JetBrains.ReSharper.Psi.CSharp.Impl.ControlFlow.NullableAnalysis.Runner;
using JetBrains.ReSharper.Psi.CSharp.Parsing;
using JetBrains.ReSharper.Psi.CSharp.Tree;
using JetBrains.ReSharper.Psi.CSharp.Util;
using JetBrains.ReSharper.Psi.ExtensionsAPI.Tree;
using JetBrains.ReSharper.Psi.Modules;
using JetBrains.ReSharper.Psi.Tree;
using JetBrains.ReSharper.Psi.Util;
using JetBrains.Util;

namespace ReCommendedExtension;

internal static class Extensions
{
    static readonly HashSet<string> wellKnownUnitTestingAssemblyNames = new(StringComparer.OrdinalIgnoreCase)
    {
        "Microsoft.VisualStudio.TestPlatform.TestFramework", "nunit.framework", "xunit.core",
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
    public static IType? TryGetTargetType(this IExpression expression)
    {
        var targetType = expression.GetImplicitlyConvertedTo();

        if (targetType.IsUnknown)
        {
            return null;
        }

        switch (expression.Parent)
        {
            case IReferenceExpression referenceExpression when referenceExpression.IsExtensionMethodInvocation():
            case IQueryFirstFrom or IQueryParameterPlatform:
                return null;
        }

        return targetType;
    }

    [Pure]
    public static bool IsGenericArray(this IType type, ITreeNode context)
        => CollectionTypeUtil.ElementTypeByCollectionType(type, context, false) is { } elementType
            && type.IsImplicitlyConvertibleTo(
                TypeFactory.CreateArrayType(elementType, 1, NullableAnnotation.Unknown),
                context.GetTypeConversionRule());

    [Pure]
    public static bool IsGenericArrayOfAnyRank(this IType type, ITreeNode context)
    {
        if (CollectionTypeUtil.ElementTypeByCollectionType(type, context, false) is { } elementType)
        {
            for (var i = 1; i <= 16; i++)
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
    public static bool IsGenericArrayOf(this IType type, IClrTypeName elementTypeName, ITreeNode context)
        => CollectionTypeUtil.ElementTypeByCollectionType(type, context, false) is { } elementType
            && elementType.IsClrType(elementTypeName)
            && type.IsImplicitlyConvertibleTo(
                TypeFactory.CreateArrayType(elementType, 1, NullableAnnotation.Unknown),
                context.GetTypeConversionRule());

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
            var elementTypes = new IType?[typeElement.TypeParametersCount];

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
    public static bool IsDefaultValueOf([NotNullWhen(true)] this ITreeNode? element, IType type)
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
    {
        var disposableInterface = PredefinedType.IDISPOSABLE_FQN.TryGetTypeElement(method.Module);
        var disposeMethod = disposableInterface?.Methods.FirstOrDefault(m => m.ShortName == nameof(IDisposable.Dispose));

        return method.ContainingType is { }
            && method.ContainingType.IsDescendantOf(disposableInterface)
            && disposeMethod is { }
            && method.OverridesOrImplements(disposeMethod);
    }

    [Pure]
    public static bool IsDisposeAsyncMethod(this IMethod method)
    {
        var asyncDisposableInterface = PredefinedType.IASYNCDISPOSABLE_FQN.TryGetTypeElement(method.Module);
        var disposeAsyncMethod = asyncDisposableInterface?.Methods.FirstOrDefault(m => m.ShortName == "DisposeAsync"); // todo: use nameof(IAsyncDisposable.DisposeAsync)

        return method.ContainingType is { }
            && method.ContainingType.IsDescendantOf(asyncDisposableInterface)
            && disposeAsyncMethod is { }
            && method.OverridesOrImplements(disposeAsyncMethod);
    }

    [Pure]
    public static bool IsDisposeMethodByConvention(this IMethod method)
        => method is { ShortName: "Dispose", IsStatic: false, TypeParameters: [], Parameters: [] }
            && method.ReturnType.IsVoid()
            && method.GetAccessRights() is AccessRights.INTERNAL or AccessRights.PUBLIC;

    [Pure]
    public static bool IsDisposeAsyncMethodByConvention(this IMethod method)
        => method is { ShortName: "DisposeAsync", IsStatic: false, TypeParameters: [], Parameters: [] }
            && method.ReturnType.IsValueTask()
            && method.GetAccessRights() is AccessRights.INTERNAL or AccessRights.PUBLIC;

    [Pure]
    public static bool HasDisposeMethods(this IStruct type)
    {
        Debug.Assert(type.IsByRefLike);

        return type.Methods.Any(
            method => method.IsDisposeMethodByConvention()
                || method.IsDisposeAsyncMethodByConvention()
                || method.GetAttributeInstances(false).Any(attribute => attribute.GetAttributeShortName() == nameof(HandlesResourceDisposalAttribute))
                && !method.IsStatic
                && method.GetAccessRights() is AccessRights.INTERNAL or AccessRights.PUBLIC);
    }

    [Pure]
    public static bool IsDisposable(this IType type, ITreeNode context)
    {
        if (type.GetTypeElement() is { } typeElement)
        {
            var psiModule = context.GetPsiModule();

            return typeElement.IsDisposable(psiModule) && !type.IsTask() && !type.IsGenericTask()
                || typeElement.IsNullableOfT()
                && TypesUtil.GetTypeArgumentValue(type, 0).GetTypeElement() is { } structType
                && structType.IsDisposable(psiModule)
                || typeElement is IStruct { IsByRefLike: true } s && s.HasDisposeMethods();
        }

        return false;
    }

    [Pure]
    public static bool IsTasklikeOfDisposable(this IType type, ITreeNode context)
    {
        if (type.IsTasklike(context)
            && type.GetTasklikeUnderlyingType(context) is { } awaitedType
            && awaitedType.GetTypeElement() is { } awaitedTypeElement)
        {
            var psiModule = context.GetPsiModule();

            return awaitedTypeElement.IsDisposable(psiModule) && !awaitedType.IsTask() && !awaitedType.IsGenericTask()
                || awaitedTypeElement.IsNullableOfT()
                && TypesUtil.GetTypeArgumentValue(awaitedType, 0).GetTypeElement() is { } structType
                && structType.IsDisposable(psiModule);
        }

        return false;
    }

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

    [Pure]
    public static bool IsUsedAsStatement(this IInvocationExpression invocationExpression)
        => invocationExpression.Parent is IExpressionStatement or IForInitializer or IForIterator;

    [Pure]
    public static bool IsDefaultValue([NotNullWhen(true)] this ICSharpExpression? expression)
        => expression is { } && expression.IsDefaultValueOf(expression.Type());

    [Pure]
    public static string? TryGetStringConstant(this ICSharpExpression? expression)
        => expression switch
        {
            IConstantValueOwner { ConstantValue: { Kind: ConstantValueKind.String, StringValue: var value } } => value,

            IReferenceExpression { Reference: var reference } when reference.Resolve().DeclaredElement is IField
                {
                    ShortName: nameof(string.Empty),
                    IsStatic: true,
                    IsReadonly: true,
                    AccessibilityDomain.DomainType: AccessibilityDomain.AccessibilityDomainType.PUBLIC,
                } field
                && field.ContainingType.IsSystemString() => "",

            _ => null,
        };

    [Pure]
    public static char? TryGetCharConstant(this ICSharpExpression? expression)
        => expression is IConstantValueOwner { ConstantValue: { Kind: ConstantValueKind.Char, CharValue: var value } } ? value : null;

    [Pure]
    public static int? TryGetInt32Constant(this ICSharpExpression? expression)
        => expression is IConstantValueOwner { ConstantValue: { Kind: ConstantValueKind.Int, IntValue: var value } } ? value : null;

    [Pure]
    public static StringComparison? TryGetStringComparisonConstant(this ICSharpExpression? expression)
        => expression is IConstantValueOwner { ConstantValue: { Kind: ConstantValueKind.Enum, Type: var enumType } constantValue }
            && enumType.IsClrType(PredefinedType.STRING_COMPARISON_CLASS)
                ? (StringComparison)constantValue.IntValue
                : null;

    [Pure]
    public static StringSplitOptions? TryGetStringSplitOptionsConstant(this ICSharpExpression? expression)
        => expression is IConstantValueOwner { ConstantValue: { Kind: ConstantValueKind.Enum, Type: var enumType } constantValue }
            && enumType.IsClrType(ClrTypeNames.StringSplitOptions)
                ? (StringSplitOptions)constantValue.IntValue
                : null;

    public static void TryRemoveParentheses(this ICSharpExpression expression, CSharpElementFactory factory)
    {
        if (expression is IParenthesizedExpression parenthesizedExpression
            && CodeStyleUtil.SuggestStyle<IRedundantParenthesesCodeStyleSuggestion>(expression, LanguageManager.Instance, null) is
            {
                NeedsToRemove: true,
            })
        {
            ModificationUtil.ReplaceChild(expression, factory.CreateExpression("$0", parenthesizedExpression.Expression));
        }
    }

    public static void TryRemoveRangeIndexParentheses(this ICSharpExpression expression, CSharpElementFactory factory)
    {
        if (expression is IElementAccessExpression { Arguments: [{ Value: IRangeExpression rangeExpression }] })
        {
            rangeExpression.LeftOperand?.TryRemoveParentheses(factory);
            rangeExpression.RightOperand?.TryRemoveParentheses(factory);
        }
    }

    [Pure]
    public static bool IsPrintable(this char c) => char.IsLetterOrDigit(c) || char.IsWhiteSpace(c) || char.IsSymbol(c) || char.IsPunctuation(c);

    [Pure]
    public static CSharpCompilerNullableInspector? TryGetCSharpCompilerNullableInspector(
        this NullableReferenceTypesDataFlowAnalysisRunSynchronizer nullableReferenceTypesDataFlowAnalysisRunSynchronizer,
        ICSharpTreeNode treeNode)
        => (CSharpCompilerNullableInspector?)nullableReferenceTypesDataFlowAnalysisRunSynchronizer.RunNullableAnalysisAndGetResults(
            treeNode,
            null!, // wrong [NotNull] annotation in R# code
            ValueAnalysisMode.OFF,
            false);

    [Pure]
    public static CSharpControlFlowNullReferenceState GetExpressionNullReferenceStateByNullableContext(
        this CSharpCompilerNullableInspector? nullabilityInspector,
        ICSharpExpression expression)
    {
        var type = expression.Type();
        if (expression.IsDefaultValueOf(type))
        {
            switch (type.Classify)
            {
                case TypeClassification.VALUE_TYPE:
                    return type.IsNullable() ? CSharpControlFlowNullReferenceState.NULL : CSharpControlFlowNullReferenceState.NOT_NULL;

                case TypeClassification.REFERENCE_TYPE: return CSharpControlFlowNullReferenceState.NULL;

                case TypeClassification.UNKNOWN: return CSharpControlFlowNullReferenceState.UNKNOWN; // unconstrained generic type

                default: goto case TypeClassification.UNKNOWN;
            }
        }

        if (expression.GetContainingNode<ICSharpClosure>() is { } closure)
        {
            nullabilityInspector = nullabilityInspector?.GetClosureAnalysisResult(closure) as CSharpCompilerNullableInspector;
        }

        if (nullabilityInspector?.ControlFlowGraph.GetLeafElementsFor(expression).LastOrDefault()?.Exits.FirstOrDefault() is { } edge)
        {
            var nullableContext = nullabilityInspector.GetContext(edge);

            return nullableContext?.ExpressionAnnotation switch
            {
                NullableAnnotation.NotAnnotated or NullableAnnotation.NotNullable => CSharpControlFlowNullReferenceState.NOT_NULL,

                NullableAnnotation.RuntimeNotNullable when expression is IObjectCreationExpression
                    || expression.Parent is not IReferenceExpression { Reference: var reference } // the nullability detection doesn't work well for extension method invocations
                    || reference.Resolve().DeclaredElement is not IMethod { IsExtensionMethod: true } => CSharpControlFlowNullReferenceState.NOT_NULL,

                NullableAnnotation.Annotated or NullableAnnotation.Nullable => CSharpControlFlowNullReferenceState.MAY_BE_NULL, // todo: distinguish if the expression is "null" or just "may be null" here

                _ => CSharpControlFlowNullReferenceState.UNKNOWN,
            };
        }

        return CSharpControlFlowNullReferenceState.UNKNOWN;
    }

    [Pure]
    public static bool HasQualifierExpressionNotNull(
        this IReferenceExpression referenceExpression,
        NullableReferenceTypesDataFlowAnalysisRunSynchronizer nullableReferenceTypesDataFlowAnalysisRunSynchronizer)
        => referenceExpression.IsNullableWarningsContextEnabled()
            && referenceExpression.QualifierExpression is { } qualifierExpression
            && nullableReferenceTypesDataFlowAnalysisRunSynchronizer.TryGetCSharpCompilerNullableInspector(qualifierExpression) is { } inspector
            && inspector.GetExpressionNullReferenceStateByNullableContext(qualifierExpression) == CSharpControlFlowNullReferenceState.NOT_NULL;
}