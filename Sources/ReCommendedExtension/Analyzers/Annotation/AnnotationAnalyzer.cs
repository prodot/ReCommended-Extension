using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using JetBrains.Annotations;
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
using JetBrains.Util;

namespace ReCommendedExtension.Analyzers.Annotation
{
    [ElementProblemAnalyzer(
        typeof(IAttributesOwnerDeclaration),
        HighlightingTypes = new[]
        {
            typeof(RedundantAnnotationSuggestion), typeof(NotAllowedAnnotationWarning), typeof(MissingAnnotationWarning),
            typeof(MissingSuppressionJustificationWarning), typeof(ConflictingAnnotationWarning), typeof(ConditionalAnnotationHint),
            typeof(InvalidValueRangeBoundaryWarning),
        })]
    public sealed class AnnotationAnalyzer : ElementProblemAnalyzer<IAttributesOwnerDeclaration>
    {
        enum AnnotationCase
        {
            AsyncMethod,
            IteratorMethod,
            Other,
            Override,
        }

        sealed class AttributeMark
        {
            public AttributeMark(CodeAnnotationNullableValue annotationNullableValue, [NotNull] IAttribute attribute)
            {
                AnnotationNullableValue = annotationNullableValue;
                Attribute = attribute;
            }

            public CodeAnnotationNullableValue AnnotationNullableValue { get; }

            [NotNull]
            public IAttribute Attribute { get; }
        }

        sealed class NumericRange
        {
            [NotNull]
            static readonly NumericRange int32 = new NumericRange(int.MinValue, int.MaxValue);

            [NotNull]
            static readonly NumericRange int64 = new NumericRange(long.MinValue, long.MaxValue);

            [NotNull]
            static readonly NumericRange @byte = new NumericRange(byte.MinValue, byte.MaxValue);

            [NotNull]
            static readonly NumericRange int16 = new NumericRange(short.MinValue, short.MaxValue);

            [NotNull]
            static readonly NumericRange uint32 = new NumericRange(uint.MinValue, uint.MaxValue);

            [NotNull]
            static readonly NumericRange uint64 = new NumericRange(ulong.MinValue, ulong.MaxValue);

            [NotNull]
            static readonly NumericRange uint16 = new NumericRange(ushort.MinValue, ushort.MaxValue);

            [NotNull]
            static readonly NumericRange @sbyte = new NumericRange(sbyte.MinValue, sbyte.MaxValue);

            [Pure]
            [CanBeNull]
            public static NumericRange TryGetFor([NotNull] IType type)
            {
                switch (type)
                {
                    case var t when t.IsInt(): return int32;
                    case var t when t.IsLong(): return int64;
                    case var t when t.IsByte(): return @byte;
                    case var t when t.IsShort(): return int16;
                    case var t when t.IsUint(): return uint32;
                    case var t when t.IsUlong(): return uint64;
                    case var t when t.IsUshort(): return uint16;
                    case var t when t.IsSbyte(): return @sbyte;
                    default: return null;
                }
            }

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
        [CanBeNull]
        static IType TryGetTypeForNumericRange([NotNull] IAttributesOwnerDeclaration attributesOwnerDeclaration)
        {
            switch (attributesOwnerDeclaration.DeclaredElement)
            {
                case IMethod method: return method.ReturnType;

                case IProperty property: return property.Type;

                case IField field: return field.Type;

                case IParameter parameter: return parameter.Type;

                case IDelegate delegateType: return delegateType.InvokeMethod.ReturnType;
            }

            return null;
        }

        [Pure]
        static decimal? TryGetAsDecimal([NotNull] ConstantValue constantValue)
        {
            switch (constantValue)
            {
                case var v when v.IsLong(): return v.LongValue;
                case var v when v.IsUlong(): return v.UlongValue;

                default: return null;
            }
        }

        [Pure]
        static bool CanContainNullnessAttributes([NotNull] IAttributesOwnerDeclaration declaration)
        {
            // excluding type, constant, enum member, property/indexer/event accessor, event, type parameter declarations
            if (declaration is ICSharpTypeDeclaration
                || declaration is IConstantDeclaration
                || declaration is IEnumMemberDeclaration
                || declaration is IAccessorDeclaration
                || declaration is IEventDeclaration
                || declaration is ITypeParameterDeclaration
                || declaration is IConstructorDeclaration)
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

            // excluding members of non-reference types (value, nullable value, unspecified generic types)
            if (declaration is ITypeOwnerDeclaration typeOwner)
            {
                // first check if declaration is a IMethodDeclaration and its TypeUsage is null
                // (otherwise the Type property throws the NullReferenceException)
                if (typeOwner is IMethodDeclaration methodDeclaration && methodDeclaration.TypeUsage == null)
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
        static AnnotationCase? TryGetAnnotationCase([NotNull] IAttributesOwnerDeclaration declaration)
        {
            if (CanContainNullnessAttributes(declaration))
            {
                if (declaration is IMethodDeclaration methodDeclaration)
                {
                    if (methodDeclaration.IsIterator)
                    {
                        return AnnotationCase.IteratorMethod;
                    }

                    if (methodDeclaration.IsAsync)
                    {
                        return AnnotationCase.AsyncMethod;
                    }
                }

                return AnnotationCase.Other;
            }

            if (declaration.OverridesInheritedMember())
            {
                return AnnotationCase.Override;
            }

            return null;
        }

        [Pure]
        [CanBeNull]
        static IType TryGetTypeForIfCanBeAnnotatedWithItemNotNull([NotNull] IAttributesOwnerDeclaration attributesOwnerDeclaration)
        {
            switch (attributesOwnerDeclaration.DeclaredElement)
            {
                case IMethod method: return method.ReturnType;

                case IParameter parameter: return parameter.Type;

                case IProperty property: return property.Type;

                case IDelegate delegateType: return delegateType.InvokeMethod.ReturnType;

                case IField field: return field.Type;

                default: return null;
            }
        }

        static void AnalyzeMissingSuppressionJustification(
            [NotNull] IHighlightingConsumer consumer,
            [NotNull] IAttributesOwnerDeclaration attributesOwnerDeclaration)
        {
            if (attributesOwnerDeclaration.IsDeclaredInTestProject())
            {
                return;
            }

            var suppressMessageAttributes =
                from attribute in attributesOwnerDeclaration.Attributes
                let attributeInstance = attribute.GetAttributeInstance()
                where Equals(attributeInstance.GetClrName(), ClrTypeNames.SuppressMessageAttribute) && attributeInstance.PositionParameterCount == 2
                let categoryConstantValue = attributeInstance.PositionParameter(0).ConstantValue
                let checkIdConstantValue = attributeInstance.PositionParameter(1).ConstantValue
                where categoryConstantValue.IsString() && checkIdConstantValue.IsString()
                let justificationConstantValue = attributeInstance.NamedParameter(nameof(SuppressMessageAttribute.Justification)).ConstantValue
                where !justificationConstantValue.IsString() || string.IsNullOrWhiteSpace(justificationConstantValue.StringValue)
                select new { Attribute = attribute, Category = categoryConstantValue.StringValue, CheckId = checkIdConstantValue.StringValue };

            foreach (var suppressMessageAttribute in suppressMessageAttributes)
            {
                consumer.AddHighlighting(
                    new MissingSuppressionJustificationWarning(
                        attributesOwnerDeclaration,
                        suppressMessageAttribute.Attribute,
                        $"Suppression justification is missing for {suppressMessageAttribute.Category}:{suppressMessageAttribute.CheckId}."));
            }

            if (!ExcludeFromCodeCoverageJustificationPropertyExists(attributesOwnerDeclaration.GetPsiModule()))
            {
                return;
            }

            var excludeFromCodeCoverageAttributes =
                from attribute in attributesOwnerDeclaration.Attributes
                let attributeInstance = attribute.GetAttributeInstance()
                where Equals(attributeInstance.GetClrName(), ClrTypeNames.ExcludeFromCodeCoverageAttribute)
                let justificationConstantValue =
                    attributeInstance.NamedParameter("Justification")
                        .ConstantValue // todo: use nameof(ExcludeFromCodeCoverageAttribute.Justification)
                where !justificationConstantValue.IsString() || string.IsNullOrWhiteSpace(justificationConstantValue.StringValue)
                select attribute;

            foreach (var excludeFromCodeCoverageAttribute in excludeFromCodeCoverageAttributes)
            {
                consumer.AddHighlighting(
                    new MissingSuppressionJustificationWarning(
                        attributesOwnerDeclaration,
                        excludeFromCodeCoverageAttribute,
                        "Justification is missing for the exclusion from code coverage."));
            }
        }

        [Pure]
        static bool ExcludeFromCodeCoverageJustificationPropertyExists([NotNull] IPsiModule psiModule)
        {
            var attributeType = TypeElementUtil.GetTypeElementByClrName(ClrTypeNames.ExcludeFromCodeCoverageAttribute.GetPersistent(), psiModule);
            return attributeType != null
                && attributeType.Properties.Any(
                    property => !property.IsStatic
                        && property.ShortName == "Justification"); // todo: use nameof(ExcludeFromCodeCoverageAttribute.Justification)
        }

        static void AnalyzeConflictingPurityAnnotations(
            [NotNull] IHighlightingConsumer consumer,
            [NotNull] IAttributesOwnerDeclaration attributesOwnerDeclaration)
        {
            var groupings = (
                from attribute in attributesOwnerDeclaration.Attributes
                let shortName = attribute.GetAttributeInstance().GetAttributeType().GetClrName().ShortName
                where shortName == nameof(PureAttribute) || shortName == nameof(MustUseReturnValueAttribute)
                group attribute by shortName).ToList();
            if (groupings.Count > 1)
            {
                foreach (var (shortName, attributes) in groupings)
                {
                    Debug.Assert(shortName == nameof(PureAttribute) || shortName == nameof(MustUseReturnValueAttribute));

                    var conflictingAnnotation = shortName == nameof(PureAttribute) ? nameof(MustUseReturnValueAttribute) : nameof(PureAttribute);

                    foreach (var attribute in attributes)
                    {
                        Debug.Assert(attribute != null);

                        consumer.AddHighlighting(
                            new ConflictingAnnotationWarning(
                                attributesOwnerDeclaration,
                                attribute,
                                $"Annotation conflicts with '{conflictingAnnotation}' annotation."));
                    }
                }
            }
        }

        static void AnalyzeNumericRangeAnnotations(
            [NotNull] IHighlightingConsumer consumer,
            [NotNull] IAttributesOwnerDeclaration attributesOwnerDeclaration)
        {
            var type = null as IType;
            var numericRange = null as NumericRange;

            var nonNegativeValueAttributes =
                from attribute in attributesOwnerDeclaration.Attributes
                where attribute.GetAttributeInstance().GetAttributeType().GetClrName().ShortName == nameof(NonNegativeValueAttribute)
                select attribute;

            foreach (var attribute in nonNegativeValueAttributes)
            {
                Debug.Assert(attribute != null);

                if (type == null)
                {
                    type = TryGetTypeForNumericRange(attributesOwnerDeclaration);
                }
                if (type == null)
                {
                    continue;
                }

                if (numericRange == null)
                {
                    numericRange = NumericRange.TryGetFor(type);
                }

                if (numericRange == null)
                {
                    consumer.AddHighlighting(
                        new NotAllowedAnnotationWarning(
                            attributesOwnerDeclaration,
                            attribute,
                            "Annotation is not valid because the type of the declared element is not an integral numeric type."));
                    continue;
                }

                if (!numericRange.IsSigned)
                {
                    consumer.AddHighlighting(
                        new RedundantAnnotationSuggestion(
                            attributesOwnerDeclaration,
                            attribute,
                            "Annotation is redundant because the declared element can never be negative by default."));
                    continue;
                }

                if (attributesOwnerDeclaration.DeclaredElement is IField field && field.IsConstant)
                {
                    consumer.AddHighlighting(
                        new RedundantAnnotationSuggestion(
                            attributesOwnerDeclaration,
                            attribute,
                            "Annotation is redundant because the declared element is a constant."));
                }
            }

            var valueRangeAttributes =
                from attribute in attributesOwnerDeclaration.Attributes
                where attribute.GetAttributeInstance().GetAttributeType().GetClrName().ShortName == nameof(ValueRangeAttribute)
                select attribute;

            foreach (var attribute in valueRangeAttributes)
            {
                Debug.Assert(attribute != null);

                if (type == null)
                {
                    type = TryGetTypeForNumericRange(attributesOwnerDeclaration);
                }
                if (type == null)
                {
                    continue;
                }

                if (numericRange == null)
                {
                    numericRange = NumericRange.TryGetFor(type);
                }

                if (numericRange == null)
                {
                    consumer.AddHighlighting(
                        new NotAllowedAnnotationWarning(
                            attributesOwnerDeclaration,
                            attribute,
                            "Annotation is not valid because the type of the declared element is not an integral numeric type."));
                    continue;
                }

                decimal from, to;

                var instance = attribute.GetAttributeInstance();
                switch (instance.PositionParameterCount)
                {
                    case 1:
                    {
                        var value = TryGetAsDecimal(instance.PositionParameter(0).ConstantValue);
                        if (value == null)
                        {
                            continue;
                        }

                        from = to = (decimal)value;

                        break;
                    }

                    case 2:
                    {
                        var value = TryGetAsDecimal(instance.PositionParameter(0).ConstantValue);
                        if (value == null)
                        {
                            continue;
                        }

                        from = (decimal)value;

                        value = TryGetAsDecimal(instance.PositionParameter(1).ConstantValue);
                        if (value == null)
                        {
                            continue;
                        }

                        to = (decimal)value;

                        if (from > to)
                        {
                            consumer.AddHighlighting(
                                new NotAllowedAnnotationWarning(
                                    attributesOwnerDeclaration,
                                    attribute,
                                    "Annotation is not valid because the 'from' value is greater than the 'to' value."));
                            continue;
                        }

                        break;
                    }

                    default: continue;
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
                    Debug.Assert(attribute.ConstructorArgumentExpressions[0] != null);
                    Debug.Assert(CSharpLanguage.Instance != null);

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
                    Debug.Assert(attribute.ConstructorArgumentExpressions[1] != null);
                    Debug.Assert(CSharpLanguage.Instance != null);

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
                    continue;
                }

                if (attributesOwnerDeclaration.DeclaredElement is IField field && field.IsConstant)
                {
                    consumer.AddHighlighting(
                        new RedundantAnnotationSuggestion(
                            attributesOwnerDeclaration,
                            attribute,
                            "Annotation is redundant because the declared element is a constant."));
                }
            }
        }

        static void AnalyzeConditional([NotNull] IHighlightingConsumer consumer, [NotNull] IAttributesOwnerDeclaration attributesOwnerDeclaration)
        {
            var conditionalAttributes =
                from attribute in attributesOwnerDeclaration.Attributes
                let typeElement = attribute.GetAttributeInstance().GetAttributeType().GetTypeElement()
                where typeElement != null
                let conditions =
                (
                    from attributeInstance in typeElement.GetAttributeInstances(PredefinedType.CONDITIONAL_ATTRIBUTE_CLASS, false)
                    where attributeInstance.PositionParameterCount == 1
                    let constantValue = attributeInstance.PositionParameter(0).ConstantValue
                    where constantValue.IsString() && !string.IsNullOrEmpty(constantValue.StringValue)
                    select constantValue.StringValue).ToList()
                where conditions.Count > 0
                select new { Attribute = attribute, Conditions = conditions };

            foreach (var conditionalAttribute in conditionalAttributes)
            {
                Debug.Assert(conditionalAttribute != null);

                if (conditionalAttribute.Conditions.Count == 1)
                {
                    consumer.AddHighlighting(
                        new ConditionalAnnotationHint(
                            attributesOwnerDeclaration,
                            conditionalAttribute.Attribute,
                            $"Attribute will be ignored if the '{conditionalAttribute.Conditions[0]}' condition is not defined."));
                }
                else
                {
                    var conditions = string.Join(", ", from condition in conditionalAttribute.Conditions orderby condition select $"'{condition}'");

                    consumer.AddHighlighting(
                        new ConditionalAnnotationHint(
                            attributesOwnerDeclaration,
                            conditionalAttribute.Attribute,
                            $"Attribute will be ignored if none of the following conditions is defined: {conditions}."));
                }
            }
        }

        [NotNull]
        readonly NullnessProvider nullnessProvider;

        [NotNull]
        readonly ContainerElementNullnessProvider containerElementNullnessProvider;

        [NotNull]
        readonly CodeAnnotationsConfiguration codeAnnotationsConfiguration;

        public AnnotationAnalyzer(
            [NotNull] CodeAnnotationsCache codeAnnotationsCache,
            [NotNull] CodeAnnotationsConfiguration codeAnnotationsConfiguration)
        {
            nullnessProvider = codeAnnotationsCache.GetProvider<NullnessProvider>();
            containerElementNullnessProvider = codeAnnotationsCache.GetProvider<ContainerElementNullnessProvider>();

            this.codeAnnotationsConfiguration = codeAnnotationsConfiguration;
        }

        [Pure]
        [NotNull]
        IEnumerable<AttributeMark> GetAttributeMarks([NotNull] IAttributesOwnerDeclaration declaration)
        {
            var markFound = false;

            foreach (var attribute in declaration.Attributes)
            {
                Debug.Assert(attribute != null);

                var mark = nullnessProvider.GetNullableAttributeMark(attribute.GetAttributeInstance());
                if (mark != null)
                {
                    yield return new AttributeMark((CodeAnnotationNullableValue)mark, attribute);

                    markFound = true;
                }
            }

            if (!markFound)
            {
                yield return null;
            }
        }

        void AnalyzeAsyncMethod([NotNull] IHighlightingConsumer consumer, [NotNull] IAttributesOwnerDeclaration attributesOwnerDeclaration)
        {
            Debug.Assert(!attributesOwnerDeclaration.IsNullableAnnotationsContextEnabled());

            foreach (var attributeMark in GetAttributeMarks(attributesOwnerDeclaration))
            {
                if (attributeMark != null)
                {
                    switch (attributeMark.AnnotationNullableValue)
                    {
                        case CodeAnnotationNullableValue.NOT_NULL:
                            consumer.AddHighlighting(
                                new RedundantAnnotationSuggestion(
                                    attributesOwnerDeclaration,
                                    attributeMark.Attribute,
                                    "Annotation is redundant because the declared element can never be null by default."));
                            break;

                        case CodeAnnotationNullableValue.CAN_BE_NULL:
                            consumer.AddHighlighting(
                                new NotAllowedAnnotationWarning(
                                    attributesOwnerDeclaration,
                                    attributeMark.Attribute,
                                    "Annotation is not valid because the declared element can never be null by default."));
                            break;
                    }
                }
            }
        }

        void AnalyzeIteratorMethod([NotNull] IHighlightingConsumer consumer, [NotNull] IAttributesOwnerDeclaration attributesOwnerDeclaration)
        {
            Debug.Assert(!attributesOwnerDeclaration.IsNullableAnnotationsContextEnabled());

            foreach (var attributeMark in GetAttributeMarks(attributesOwnerDeclaration))
            {
                if (attributeMark != null)
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

                    var nonNullAnnotationAttributeType = codeAnnotationsConfiguration.GetAttributeTypeForElement(
                        attributesOwnerDeclaration,
                        notNullAttributeShortName);
                    if (nonNullAnnotationAttributeType != null)
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

        void AnalyzeOther(
            ValueAnalysisMode valueAnalysisMode,
            [NotNull] IHighlightingConsumer consumer,
            [NotNull] IAttributesOwnerDeclaration attributesOwnerDeclaration)
        {
            Debug.Assert(!attributesOwnerDeclaration.IsNullableAnnotationsContextEnabled());

            switch (valueAnalysisMode)
            {
                case ValueAnalysisMode.OPTIMISTIC:
                    foreach (var attributeMark in GetAttributeMarks(attributesOwnerDeclaration))
                    {
                        if (attributeMark == null)
                        {
                            const string notNullAttributeShortName = NullnessProvider.NotNullAttributeShortName;
                            const string canBeNullAttributeShortName = NullnessProvider.CanBeNullAttributeShortName;

                            var nonNullAnnotationAttributeType = codeAnnotationsConfiguration.GetAttributeTypeForElement(
                                attributesOwnerDeclaration,
                                notNullAttributeShortName);
                            var canBeNullAnnotationAttributeType = codeAnnotationsConfiguration.GetAttributeTypeForElement(
                                attributesOwnerDeclaration,
                                canBeNullAttributeShortName);
                            if (nonNullAnnotationAttributeType != null || canBeNullAnnotationAttributeType != null)
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
                        if (attributeMark?.AnnotationNullableValue == CodeAnnotationNullableValue.CAN_BE_NULL)
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

        void AnalyzeOverride([NotNull] IHighlightingConsumer consumer, [NotNull] IAttributesOwnerDeclaration attributesOwnerDeclaration)
        {
            Debug.Assert(!attributesOwnerDeclaration.IsNullableAnnotationsContextEnabled());

            foreach (var attributeMark in GetAttributeMarks(attributesOwnerDeclaration))
            {
                if (attributeMark != null)
                {
                    consumer.AddHighlighting(
                        new NotAllowedAnnotationWarning(
                            attributesOwnerDeclaration,
                            attributeMark.Attribute,
                            "Annotation is not allowed because the declared element overrides or implements the inherited member."));
                }
            }
        }

        void AnalyzeNotAllowedItemNotNull([NotNull] IHighlightingConsumer consumer, [NotNull] IAttributesOwnerDeclaration attributesOwnerDeclaration)
        {
            Debug.Assert(!attributesOwnerDeclaration.IsNullableAnnotationsContextEnabled());

            var itemNotNullAttribute = attributesOwnerDeclaration.Attributes.FirstOrDefault(
                attribute => containerElementNullnessProvider.GetContainerElementNullableAttributeMark(attribute.GetAttributeInstance())
                    == CodeAnnotationNullableValue.NOT_NULL);
            if (itemNotNullAttribute != null)
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

                var type = TryGetTypeForIfCanBeAnnotatedWithItemNotNull(attributesOwnerDeclaration);
                if (type != null)
                {
                    if (type.IsGenericEnumerableOrDescendant() || type.IsGenericArray(attributesOwnerDeclaration))
                    {
                        var elementType = CollectionTypeUtil.ElementTypeByCollectionType(type, attributesOwnerDeclaration, false);
                        if (elementType != null && elementType.Classify != TypeClassification.REFERENCE_TYPE)
                        {
                            consumer.AddHighlighting(
                                new NotAllowedAnnotationWarning(
                                    attributesOwnerDeclaration,
                                    itemNotNullAttribute,
                                    "Annotation is not allowed because the declared element type is not a reference type."));
                        }
                        return;
                    }

                    var resultType = type.GetTasklikeUnderlyingType(attributesOwnerDeclaration);
                    if (resultType != null)
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
                        var typeElement = TypeElementUtil.GetTypeElementByClrName(PredefinedType.LAZY_FQN, attributesOwnerDeclaration.GetPsiModule());
                        var valueType = type.GetGenericUnderlyingType(typeElement);
                        if (valueType != null)
                        {
                            if (valueType.Classify != TypeClassification.REFERENCE_TYPE)
                            {
                                consumer.AddHighlighting(
                                    new NotAllowedAnnotationWarning(
                                        attributesOwnerDeclaration,
                                        itemNotNullAttribute,
                                        "Annotation is not allowed because the declared lazy value type is not a reference type."));
                            }
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

            // attributes annotated as [Conditional]
            AnalyzeConditional(consumer, element);
        }
    }
}