using JetBrains.ReSharper.Feature.Services.Daemon;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.CodeAnnotations;
using JetBrains.ReSharper.Psi.ControlFlow;
using JetBrains.ReSharper.Psi.CSharp;
using JetBrains.ReSharper.Psi.CSharp.Impl;
using JetBrains.ReSharper.Psi.CSharp.Tree;
using JetBrains.ReSharper.Psi.CSharp.Util;
using JetBrains.ReSharper.Psi.Modules;
using JetBrains.ReSharper.Psi.Tree;
using JetBrains.ReSharper.Psi.Util;

namespace ReCommendedExtension.Analyzers.Annotation;

[ElementProblemAnalyzer(
    typeof(IAttributesOwnerDeclaration),
    HighlightingTypes =
    [
        typeof(RedundantAnnotationSuggestion), typeof(NotAllowedAnnotationWarning), typeof(MissingAnnotationWarning),
        typeof(MissingSuppressionJustificationWarning), typeof(ConflictingAnnotationWarning), typeof(ConditionalAnnotationHint),
        typeof(InvalidValueRangeBoundaryWarning), typeof(MissingAttributeUsageAnnotationWarning), typeof(MissingNotNullWhenAnnotationSuggestion),
    ])]
public sealed class AnnotationAnalyzer(CodeAnnotationsCache codeAnnotationsCache, CodeAnnotationsConfiguration codeAnnotationsConfiguration)
    : ElementProblemAnalyzer<IAttributesOwnerDeclaration>
{
    enum AnnotationCase
    {
        AsyncMethod,
        IteratorMethod,
        Other,
        Override,
    }

    sealed record AttributeMark
    {
        public required CodeAnnotationNullableValue AnnotationNullableValue { get; init; }

        public required IAttribute Attribute { get; init; }
    }

    sealed record NumericRange
    {
        static readonly NumericRange int32 = new(int.MinValue, int.MaxValue);

        static readonly NumericRange int64 = new(long.MinValue, long.MaxValue);

        static readonly NumericRange @byte = new(byte.MinValue, byte.MaxValue);

        static readonly NumericRange int16 = new(short.MinValue, short.MaxValue);

        static readonly NumericRange uint32 = new(uint.MinValue, uint.MaxValue);

        static readonly NumericRange uint64 = new(ulong.MinValue, ulong.MaxValue);

        static readonly NumericRange uint16 = new(ushort.MinValue, ushort.MaxValue);

        static readonly NumericRange @sbyte = new(sbyte.MinValue, sbyte.MaxValue);

        [Pure]
        public static NumericRange? TryGetFor(IType type)
            => type switch
            {
                var t when t.IsInt() => int32,
                var t when t.IsLong() => int64,
                var t when t.IsByte() => @byte,
                var t when t.IsShort() => int16,
                var t when t.IsUint() => uint32,
                var t when t.IsUlong() => uint64,
                var t when t.IsUshort() => uint16,
                var t when t.IsSbyte() => @sbyte,

                _ => null,
            };

        NumericRange(decimal minValue, decimal maxValue)
        {
            Debug.Assert(minValue < maxValue);

            MinValue = minValue;
            MaxValue = maxValue;
        }

        public bool IsSigned => MinValue < 0;

        public decimal MinValue { get; }

        public decimal MaxValue { get; }
    }

    [Pure]
    static bool CanContainNullnessAttributes(IAttributesOwnerDeclaration declaration)
    {
        // excluding type, constant, enum member, property/indexer/event accessor, event, type parameter declarations
        if (declaration is ICSharpTypeDeclaration
            or IConstantDeclaration
            or IEnumMemberDeclaration
            or IAccessorDeclaration
            or IEventDeclaration
            or ITypeParameterDeclaration
            or IConstructorDeclaration)
        {
            return false;
        }

        // excluding overridden members
        if (declaration.OverridesInheritedMember())
        {
            return false;
        }

        // excluding local function (C# 8 or less)
        if (declaration.IsOnLocalFunctionWithUnsupportedAttributes())
        {
            return false;
        }

        // excluding lambda expressions (C# 9 or less)
        if (declaration.IsOnLambdaExpressionWithUnsupportedAttributes())
        {
            return false;
        }

        // excluding anonymous methods
        if (declaration.IsOnAnonymousMethodWithUnsupportedAttributes())
        {
            return false;
        }

        // excluding members of non-reference types (value, nullable value, unspecified generic types)
        if (declaration is ITypeOwnerDeclaration typeOwner)
        {
            // first check if declaration is a IMethodDeclaration and its TypeUsage is null
            // (otherwise the Type property throws the NullReferenceException)
            if (typeOwner is IMethodDeclaration { TypeUsage: not { } })
            {
                return false;
            }

            if (typeOwner.Type.Classify != TypeClassification.REFERENCE_TYPE)
            {
                return false;
            }
        }

        return true;
    }

    [Pure]
    static AnnotationCase? TryGetAnnotationCase(IAttributesOwnerDeclaration declaration)
    {
        if (CanContainNullnessAttributes(declaration))
        {
            return declaration switch
            {
                IMethodDeclaration { IsIterator: true } => AnnotationCase.IteratorMethod,
                IMethodDeclaration { IsAsync: true } => AnnotationCase.AsyncMethod,

                _ => AnnotationCase.Other,
            };
        }

        if (declaration.OverridesInheritedMember())
        {
            return AnnotationCase.Override;
        }

        return null;
    }

    static void AnalyzeMissingSuppressionJustification(IHighlightingConsumer consumer, IAttributesOwnerDeclaration attributesOwnerDeclaration)
    {
        if (attributesOwnerDeclaration.IsDeclaredInTestProject())
        {
            return;
        }

        var excludeFromCodeCoverageJustificationPropertyExists =
            ExcludeFromCodeCoverageJustificationPropertyExists(attributesOwnerDeclaration.GetPsiModule());

        foreach (var attribute in attributesOwnerDeclaration.Attributes)
        {
            var attributeType = attribute.GetAttributeType();

            if (attributeType.IsClrType(ClrTypeNames.SuppressMessageAttribute)
                && attribute.Arguments is
                [
                    { Value.ConstantValue: { Kind: ConstantValueKind.String, StringValue: var category } },
                    { Value.ConstantValue: { Kind: ConstantValueKind.String, StringValue: var checkId } },
                ]
                && (attribute.PropertyAssignments.FirstOrDefault(p => p.PropertyNameIdentifier.Name == nameof(SuppressMessageAttribute.Justification))
                        ?.Source is not { ConstantValue: { Kind: ConstantValueKind.String, StringValue: var suppressMessageJustification } }
                    || string.IsNullOrWhiteSpace(suppressMessageJustification)))
            {
                consumer.AddHighlighting(
                    new MissingSuppressionJustificationWarning(
                        attributesOwnerDeclaration,
                        attribute,
                        $"Suppression justification is missing for {category}:{checkId}."));
            }

            if (excludeFromCodeCoverageJustificationPropertyExists
                && attributeType.IsClrType(ClrTypeNames.ExcludeFromCodeCoverageAttribute)
                && (attribute.PropertyAssignments.FirstOrDefault(p => p.PropertyNameIdentifier.Name == "Justification")?.Source is
                        not // todo: use nameof(ExcludeFromCodeCoverageAttribute.Justification)
                        {
                            ConstantValue: { Kind: ConstantValueKind.String, StringValue: var excludeFromCodeCoverageJustification },
                        }
                    || string.IsNullOrWhiteSpace(excludeFromCodeCoverageJustification)))
            {
                consumer.AddHighlighting(
                    new MissingSuppressionJustificationWarning(
                        attributesOwnerDeclaration,
                        attribute,
                        "Justification is missing for the exclusion from code coverage."));
            }
        }
    }

    [Pure]
    static bool ExcludeFromCodeCoverageJustificationPropertyExists(IPsiModule psiModule)
        => ClrTypeNames.ExcludeFromCodeCoverageAttribute.TryGetTypeElement(psiModule) is { } attributeType
            && attributeType.Properties.Any(property => property is { IsStatic: false, ShortName: "Justification" }); // todo: use nameof(ExcludeFromCodeCoverageAttribute.Justification)

    static void AnalyzeConflictingPurityAnnotations(IHighlightingConsumer consumer, IAttributesOwnerDeclaration attributesOwnerDeclaration)
    {
        if (attributesOwnerDeclaration is not IMethodDeclaration)
        {
            return;
        }

        [Pure]
        static string GetOtherName(string name)
            => name switch
            {
                nameof(PureAttribute) => nameof(MustUseReturnValueAttribute),
                nameof(MustUseReturnValueAttribute) => nameof(PureAttribute),

                _ => throw new NotSupportedException(),
            };

        var attributes = null as List<(string name, IAttribute)>;

        foreach (var attribute in attributesOwnerDeclaration.Attributes)
        {
            var name = attribute.GetAttributeType().GetTypeElement()?.ShortName;

            if (name is nameof(PureAttribute) or nameof(MustUseReturnValueAttribute))
            {
                switch (attributes)
                {
                    case null:
                        attributes = new List<(string name, IAttribute)> { (name, attribute) };
                        break;

                    case [(nameof(PureAttribute), _), ..] when name == nameof(PureAttribute):
                    case [(nameof(MustUseReturnValueAttribute), _), ..] when name == nameof(MustUseReturnValueAttribute):
                        attributes.Add((name, attribute));
                        break;

                    case [(nameof(PureAttribute), _), ..] when name == nameof(MustUseReturnValueAttribute):
                    case [(nameof(MustUseReturnValueAttribute), _), ..] when name == nameof(PureAttribute):
                        foreach (var (_, a) in attributes)
                        {
                            consumer.AddHighlighting(
                                new ConflictingAnnotationWarning(
                                    attributesOwnerDeclaration,
                                    a,
                                    $"Annotation conflicts with [{name[..^"Attribute".Length]}]."));
                        }
                        attributes.Clear();

                        consumer.AddHighlighting(
                            new ConflictingAnnotationWarning(
                                attributesOwnerDeclaration,
                                attribute,
                                $"Annotation conflicts with [{GetOtherName(name)[..^"Attribute".Length]}]."));
                        break;

                    case []:
                        consumer.AddHighlighting(
                            new ConflictingAnnotationWarning(
                                attributesOwnerDeclaration,
                                attribute,
                                $"Annotation conflicts with [{GetOtherName(name)[..^"Attribute".Length]}]."));
                        break;
                }
            }
        }
    }

    static void AnalyzeNumericRangeAnnotations(IHighlightingConsumer consumer, IAttributesOwnerDeclaration attributesOwnerDeclaration)
    {
        var type = null as IType;
        var numericRange = null as NumericRange;

        foreach (var attribute in attributesOwnerDeclaration.Attributes)
        {
            var name = attribute.GetAttributeType().GetTypeElement()?.ShortName;

            if (name is nameof(NonNegativeValueAttribute) or nameof(ValueRangeAttribute))
            {
                if (attributesOwnerDeclaration is IConstantDeclaration)
                {
                    consumer.AddHighlighting(
                        new RedundantAnnotationSuggestion(
                            attributesOwnerDeclaration,
                            attribute,
                            "Annotation is redundant because the declared element is a constant."));
                    continue;
                }

                type ??= attributesOwnerDeclaration.DeclaredElement switch
                {
                    IMethod method => method.ReturnType,
                    IProperty property => property.Type,
                    IField field => field.Type,
                    IParameter parameter => parameter.Type,
                    IDelegate delegateType => delegateType.InvokeMethod.ReturnType,

                    _ => null,
                };

                if (type is not { })
                {
                    continue;
                }

                numericRange ??= NumericRange.TryGetFor(type);

                if (numericRange is not { })
                {
                    consumer.AddHighlighting(
                        new NotAllowedAnnotationWarning(
                            attributesOwnerDeclaration,
                            attribute,
                            "Annotation is not valid because the type of the declared element is not an integral numeric type."));
                    continue;
                }

                switch (name)
                {
                    case nameof(NonNegativeValueAttribute):
                        if (!numericRange.IsSigned)
                        {
                            consumer.AddHighlighting(
                                new RedundantAnnotationSuggestion(
                                    attributesOwnerDeclaration,
                                    attribute,
                                    "Annotation is redundant because the declared element can never be negative by default."));
                        }
                        break;

                    case nameof(ValueRangeAttribute):
                        decimal from, to;

                        switch (attribute.Arguments)
                        {
                            case
                            [
                                {
                                    Value.ConstantValue:
                                    {
                                        Kind: ConstantValueKind.Int
                                        or ConstantValueKind.Uint
                                        or ConstantValueKind.Long
                                        or ConstantValueKind.Ulong
                                        or ConstantValueKind.Sbyte
                                        or ConstantValueKind.Byte
                                        or ConstantValueKind.Short
                                        or ConstantValueKind.Ushort,
                                    } constantValue,
                                },
                            ]:
                                from = to = constantValue.ToDecimalUnchecked();
                                break;

                            case
                            [
                                {
                                    Value.ConstantValue:
                                    {
                                        Kind: ConstantValueKind.Int
                                        or ConstantValueKind.Uint
                                        or ConstantValueKind.Long
                                        or ConstantValueKind.Ulong
                                        or ConstantValueKind.Sbyte
                                        or ConstantValueKind.Byte
                                        or ConstantValueKind.Short
                                        or ConstantValueKind.Ushort,
                                    } fromConstantValue,
                                },
                                {
                                    Value.ConstantValue:
                                    {
                                        Kind: ConstantValueKind.Int
                                        or ConstantValueKind.Uint
                                        or ConstantValueKind.Long
                                        or ConstantValueKind.Ulong
                                        or ConstantValueKind.Sbyte
                                        or ConstantValueKind.Byte
                                        or ConstantValueKind.Short
                                        or ConstantValueKind.Ushort,
                                    } toConstantValue,
                                },
                            ]:
                                (from, to) = (fromConstantValue.ToDecimalUnchecked(), toConstantValue.ToDecimalUnchecked());
                                break;

                            default: continue;
                        }

                        if (from > to)
                        {
                            consumer.AddHighlighting(
                                new NotAllowedAnnotationWarning(
                                    attributesOwnerDeclaration,
                                    attribute,
                                    "Annotation is not valid because the 'from' value is greater than the 'to' value."));
                            continue;
                        }

                        if (to < numericRange.MinValue || from > numericRange.MaxValue)
                        {
                            consumer.AddHighlighting(
                                new NotAllowedAnnotationWarning(
                                    attributesOwnerDeclaration,
                                    attribute,
                                    "Annotation is not valid because the declared element can never be in the range."));
                            continue;
                        }

                        if (from < numericRange.MinValue && to < numericRange.MaxValue)
                        {
                            Debug.Assert(from < to);
                            Debug.Assert(CSharpLanguage.Instance is { });

                            consumer.AddHighlighting(
                                new InvalidValueRangeBoundaryWarning(
                                    attribute.ConstructorArgumentExpressions[0],
                                    ValueRangeBoundary.Lower,
                                    type,
                                    numericRange.IsSigned,
                                    numericRange.IsSigned
                                        ? $"The 'from' value is less than the '{type.GetPresentableName(CSharpLanguage.Instance)}.{nameof(int.MinValue)}'."
                                        : "The 'from' value is negative."));
                            continue;
                        }

                        if (from > numericRange.MinValue && to > numericRange.MaxValue)
                        {
                            Debug.Assert(from < to);
                            Debug.Assert(CSharpLanguage.Instance is { });

                            consumer.AddHighlighting(
                                new InvalidValueRangeBoundaryWarning(
                                    attribute.ConstructorArgumentExpressions[1],
                                    ValueRangeBoundary.Higher,
                                    type,
                                    numericRange.IsSigned,
                                    $"The 'to' value is greater than the '{type.GetPresentableName(CSharpLanguage.Instance)}.{nameof(int.MaxValue)}'."));
                            continue;
                        }

                        if (from <= numericRange.MinValue && to >= numericRange.MaxValue)
                        {
                            consumer.AddHighlighting(
                                new RedundantAnnotationSuggestion(
                                    attributesOwnerDeclaration,
                                    attribute,
                                    "Annotation is redundant because the declared element is always in the range by default."));
                        }

                        break;
                }
            }
        }
    }

    static void AnalyzeMissingAttributeUsageAnnotations(IHighlightingConsumer consumer, IAttributesOwnerDeclaration attributesOwnerDeclaration)
    {
        [Pure]
        static bool AnyBaseClassAnnotated(IClassDeclaration classDeclaration)
        {
            for (var baseType = classDeclaration.SuperTypes.FirstOrDefault();
                baseType is { } && baseType.IsClassType();
                baseType = baseType.GetSuperTypes().FirstOrDefault())
            {
                if (baseType.IsClrType(PredefinedType.ATTRIBUTE_FQN))
                {
                    return false;
                }

                if (baseType.GetTypeElement() is { } baseTypeElement
                    && baseTypeElement.HasAttributeInstance(PredefinedType.ATTRIBUTE_USAGE_ATTRIBUTE_CLASS, false))
                {
                    return true;
                }
            }

            return false;
        }

        if (attributesOwnerDeclaration is IClassDeclaration { IsAbstract: false } classDeclaration
            && classDeclaration.DeclaredElement.IsAttribute()
            && classDeclaration.Attributes.All(a => !a.GetAttributeType().IsClrType(PredefinedType.ATTRIBUTE_USAGE_ATTRIBUTE_CLASS))
            && !AnyBaseClassAnnotated(classDeclaration))
        {
            consumer.AddHighlighting(
                new MissingAttributeUsageAnnotationWarning(
                    attributesOwnerDeclaration,
                    $"Annotate the attribute with [{nameof(AttributeUsageAttribute)[..^"Attribute".Length]}]."));
        }
    }

    static void AnalyzeMissingNotNullWhenAnnotations(IHighlightingConsumer consumer, IAttributesOwnerDeclaration attributesOwnerDeclaration)
    {
        if (attributesOwnerDeclaration.IsNullableAnnotationsContextEnabled()
            && attributesOwnerDeclaration is IParameterDeclaration parameterDeclaration
            && !parameterDeclaration.IsInsideClosure()
            && parameterDeclaration.GetContainingTypeMemberDeclarationIgnoringClosures() is IMethodDeclaration
            {
                DeclaredName: nameof(IEquatable<int>.Equals), ParameterDeclarations: [var p], DeclaredElement: { },
            } methodDeclaration
            && p == parameterDeclaration
            && methodDeclaration.DeclaredElement.ReturnType.IsBool()
            && methodDeclaration.GetContainingTypeDeclaration() is IClassDeclaration { DeclaredElement: { } } classDeclaration
            && parameterDeclaration.Type.Equals(TypeFactory.CreateType(classDeclaration.DeclaredElement))
            && attributesOwnerDeclaration.Attributes.All(a => !a.GetAttributeType().IsClrType(ClrTypeNames.NotNullWhenAttribute)))
        {
            // we do not verify if the type implements the IEquatable<T> interface: in any case, the parameter should not be null when such a method
            // returns true

            consumer.AddHighlighting(
                new MissingNotNullWhenAnnotationSuggestion(
                    attributesOwnerDeclaration,
                    $"Annotate the parameter with [{nameof(NotNullWhenAttribute)[..^"Attribute".Length]}(true)]."));
        }
    }

    static void AnalyzeConditional(IHighlightingConsumer consumer, IAttributesOwnerDeclaration attributesOwnerDeclaration)
    {
        var conditions = null as List<string>;

        foreach (var attribute in attributesOwnerDeclaration.Attributes)
        {
            if (attribute.GetAttributeType().GetTypeElement() is { } typeElement)
            {
                foreach (var attributeAnnotation in typeElement.GetAttributeInstances(PredefinedType.CONDITIONAL_ATTRIBUTE_CLASS, true))
                {
                    if (attributeAnnotation.PositionParameterCount == 1
                        && attributeAnnotation.PositionParameter(0).ConstantValue is
                        {
                            Kind: ConstantValueKind.String, StringValue: [_, ..] condition,
                        })
                    {
                        conditions ??= [];
                        conditions.Add(condition);
                    }
                }

                switch (conditions)
                {
                    case [var singleCondition]:
                        consumer.AddHighlighting(
                            new ConditionalAnnotationHint(
                                attributesOwnerDeclaration,
                                attribute,
                                $"Attribute will be ignored if the '{singleCondition}' condition is not defined."));
                        conditions.Clear();
                        break;

                    case [_, _, ..]:
                        consumer.AddHighlighting(
                            new ConditionalAnnotationHint(
                                attributesOwnerDeclaration,
                                attribute,
                                $"Attribute will be ignored if none of the following conditions is defined: {
                                    string.Join(", ", from c in conditions orderby c select $"'{c}'")
                                }."));
                        conditions.Clear();
                        break;
                }
            }
        }
    }

    readonly NullnessProvider nullnessProvider = codeAnnotationsCache.GetProvider<NullnessProvider>();
    readonly ContainerElementNullnessProvider containerElementNullnessProvider = codeAnnotationsCache.GetProvider<ContainerElementNullnessProvider>();

    [Pure]
    IEnumerable<AttributeMark?> GetAttributeMarks(IAttributesOwnerDeclaration declaration)
    {
        var markFound = false;

        foreach (var attribute in declaration.Attributes)
        {
            if (nullnessProvider.GetNullableAttributeMark(attribute.GetAttributeInstance()) is { } mark)
            {
                yield return new AttributeMark { AnnotationNullableValue = mark, Attribute = attribute };

                markFound = true;
            }
        }

        if (!markFound)
        {
            yield return null;
        }
    }

    void AnalyzeAsyncMethod(IHighlightingConsumer consumer, IAttributesOwnerDeclaration attributesOwnerDeclaration)
    {
        Debug.Assert(!attributesOwnerDeclaration.IsNullableAnnotationsContextEnabled());

        foreach (var attributeMark in GetAttributeMarks(attributesOwnerDeclaration))
        {
            switch (attributeMark)
            {
                case { AnnotationNullableValue: CodeAnnotationNullableValue.NOT_NULL }:
                    consumer.AddHighlighting(
                        new RedundantAnnotationSuggestion(
                            attributesOwnerDeclaration,
                            attributeMark.Attribute,
                            "Annotation is redundant because the declared element can never be null by default."));
                    break;

                case { AnnotationNullableValue: CodeAnnotationNullableValue.CAN_BE_NULL }:
                    consumer.AddHighlighting(
                        new NotAllowedAnnotationWarning(
                            attributesOwnerDeclaration,
                            attributeMark.Attribute,
                            "Annotation is not valid because the declared element can never be null by default."));
                    break;
            }
        }
    }

    void AnalyzeIteratorMethod(IHighlightingConsumer consumer, IAttributesOwnerDeclaration attributesOwnerDeclaration)
    {
        Debug.Assert(!attributesOwnerDeclaration.IsNullableAnnotationsContextEnabled());

        foreach (var attributeMark in GetAttributeMarks(attributesOwnerDeclaration))
        {
            if (attributeMark is { })
            {
                if (attributeMark.AnnotationNullableValue == CodeAnnotationNullableValue.CAN_BE_NULL)
                {
                    consumer.AddHighlighting(
                        new NotAllowedAnnotationWarning(
                            attributesOwnerDeclaration,
                            attributeMark.Attribute,
                            "Annotation is not valid because the declared element can never be null by default."));
                }
            }
            else
            {
                const string notNullAttributeShortName = NullnessProvider.NotNullAttributeShortName;

                if (codeAnnotationsConfiguration.GetAttributeTypeForElement(attributesOwnerDeclaration, notNullAttributeShortName) is { })
                {
                    consumer.AddHighlighting(
                        new MissingAnnotationWarning(
                            $"Declared element can never be null by default, but is not annotated with '{notNullAttributeShortName}'.",
                            attributesOwnerDeclaration));
                }
                break;
            }
        }
    }

    void AnalyzeOther(ValueAnalysisMode valueAnalysisMode, IHighlightingConsumer consumer, IAttributesOwnerDeclaration attributesOwnerDeclaration)
    {
        Debug.Assert(!attributesOwnerDeclaration.IsNullableAnnotationsContextEnabled());

        switch (valueAnalysisMode)
        {
            case ValueAnalysisMode.OPTIMISTIC:
                foreach (var attributeMark in GetAttributeMarks(attributesOwnerDeclaration))
                {
                    if (attributeMark is not { })
                    {
                        const string notNullAttributeShortName = NullnessProvider.NotNullAttributeShortName;
                        const string canBeNullAttributeShortName = NullnessProvider.CanBeNullAttributeShortName;

                        var nonNullAnnotationAttributeType = codeAnnotationsConfiguration.GetAttributeTypeForElement(
                            attributesOwnerDeclaration,
                            notNullAttributeShortName);
                        var canBeNullAnnotationAttributeType = codeAnnotationsConfiguration.GetAttributeTypeForElement(
                            attributesOwnerDeclaration,
                            canBeNullAttributeShortName);

                        if (nonNullAnnotationAttributeType is { } || canBeNullAnnotationAttributeType is { })
                        {
                            consumer.AddHighlighting(
                                new MissingAnnotationWarning(
                                    $"Declared element is nullable, but is not annotated with '{notNullAttributeShortName}' or '{canBeNullAttributeShortName}'.",
                                    attributesOwnerDeclaration));
                        }
                        break;
                    }
                }
                break;

            case ValueAnalysisMode.PESSIMISTIC:
                foreach (var attributeMark in GetAttributeMarks(attributesOwnerDeclaration))
                {
                    if (attributeMark is { AnnotationNullableValue: CodeAnnotationNullableValue.CAN_BE_NULL })
                    {
                        consumer.AddHighlighting(
                            new RedundantAnnotationSuggestion(
                                attributesOwnerDeclaration,
                                attributeMark.Attribute,
                                "Annotation is redundant because the declared element can be null by default."));
                    }
                }
                break;
        }
    }

    void AnalyzeOverride(IHighlightingConsumer consumer, IAttributesOwnerDeclaration attributesOwnerDeclaration)
    {
        Debug.Assert(!attributesOwnerDeclaration.IsNullableAnnotationsContextEnabled());

        foreach (var attributeMark in GetAttributeMarks(attributesOwnerDeclaration))
        {
            if (attributeMark is { })
            {
                consumer.AddHighlighting(
                    new NotAllowedAnnotationWarning(
                        attributesOwnerDeclaration,
                        attributeMark.Attribute,
                        "Annotation is not allowed because the declared element overrides or implements the inherited member."));
            }
        }
    }

    void AnalyzeNotAllowedItemNotNull(IHighlightingConsumer consumer, IAttributesOwnerDeclaration attributesOwnerDeclaration)
    {
        Debug.Assert(!attributesOwnerDeclaration.IsNullableAnnotationsContextEnabled());

        var itemNotNullAttribute = attributesOwnerDeclaration.Attributes.FirstOrDefault(
            attribute => containerElementNullnessProvider.GetContainerElementNullableAttributeMark(attribute.GetAttributeInstance())
                == CodeAnnotationNullableValue.NOT_NULL);
        if (itemNotNullAttribute is { })
        {
            if (attributesOwnerDeclaration.OverridesInheritedMember())
            {
                consumer.AddHighlighting(
                    new NotAllowedAnnotationWarning(
                        attributesOwnerDeclaration,
                        itemNotNullAttribute,
                        "Annotation is not allowed because the declared element overrides or implements the inherited member."));
                return;
            }

            var type = attributesOwnerDeclaration.DeclaredElement switch
            {
                IMethod method => method.ReturnType,
                IParameter parameter => parameter.Type,
                IProperty property => property.Type,
                IDelegate delegateType => delegateType.InvokeMethod.ReturnType,
                IField field => field.Type,

                _ => null,
            };

            if (type is { })
            {
                if (type.IsGenericEnumerableOrDescendant() || type.IsGenericArray(attributesOwnerDeclaration))
                {
                    var elementType = CollectionTypeUtil.ElementTypeByCollectionType(type, attributesOwnerDeclaration, false);
                    if (elementType is { Classify: not TypeClassification.REFERENCE_TYPE })
                    {
                        consumer.AddHighlighting(
                            new NotAllowedAnnotationWarning(
                                attributesOwnerDeclaration,
                                itemNotNullAttribute,
                                "Annotation is not allowed because the declared element type is not a reference type."));
                    }

                    return;
                }

                if (type.GetTasklikeUnderlyingType(attributesOwnerDeclaration) is { } resultType)
                {
                    if (resultType.Classify != TypeClassification.REFERENCE_TYPE)
                    {
                        consumer.AddHighlighting(
                            new NotAllowedAnnotationWarning(
                                attributesOwnerDeclaration,
                                itemNotNullAttribute,
                                "Annotation is not allowed because the declared task result type is not a reference type."));
                    }

                    return;
                }

                if (type.IsLazy())
                {
                    if (type.GetGenericUnderlyingType(PredefinedType.LAZY_FQN.TryGetTypeElement(attributesOwnerDeclaration.GetPsiModule())) is
                        {
                            Classify: not TypeClassification.REFERENCE_TYPE,
                        })
                    {
                        consumer.AddHighlighting(
                            new NotAllowedAnnotationWarning(
                                attributesOwnerDeclaration,
                                itemNotNullAttribute,
                                "Annotation is not allowed because the declared lazy value type is not a reference type."));
                    }

                    return;
                }

                consumer.AddHighlighting(
                    new NotAllowedAnnotationWarning(
                        attributesOwnerDeclaration,
                        itemNotNullAttribute,
                        $"Annotation is not allowed because the declared element must be an {nameof(IEnumerable<int>)}<T> (or its descendant), or a generic task-like type, or a {nameof(Lazy<int>)}<T>."));
            }
        }
    }

    protected override void Run(IAttributesOwnerDeclaration element, ElementProblemAnalyzerData data, IHighlightingConsumer consumer)
    {
        if (!element.IsNullableAnnotationsContextEnabled())
        {
            // [NotNull], [CanBeNull] annotations
            switch (TryGetAnnotationCase(element))
            {
                case AnnotationCase.AsyncMethod:
                    AnalyzeAsyncMethod(consumer, element);
                    break;

                case AnnotationCase.IteratorMethod:
                    AnalyzeIteratorMethod(consumer, element);
                    break;

                case AnnotationCase.Other:
                    AnalyzeOther(data.GetValueAnalysisMode(), consumer, element);
                    break;

                case AnnotationCase.Override:
                    AnalyzeOverride(consumer, element);
                    break;
            }

            // [ItemNotNull] annotations
            AnalyzeNotAllowedItemNotNull(consumer, element);
        }

        // [SuppressMessage] annotations
        AnalyzeMissingSuppressionJustification(consumer, element);

        // [Pure] and [MustUseReturnValue] annotations
        AnalyzeConflictingPurityAnnotations(consumer, element);

        // [NonNegativeValue] and [ValueRange(...)] annotations
        AnalyzeNumericRangeAnnotations(consumer, element);

        // [AttributeUsage] annotations
        AnalyzeMissingAttributeUsageAnnotations(consumer, element);

        // [NotNullWhen(true)] annotations
        AnalyzeMissingNotNullWhenAnnotations(consumer, element);

        // attributes annotated as [Conditional]
        AnalyzeConditional(consumer, element);
    }
}