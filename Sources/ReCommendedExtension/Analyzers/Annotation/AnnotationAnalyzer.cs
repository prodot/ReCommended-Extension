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
using JetBrains.ReSharper.Psi.Tree;
using JetBrains.ReSharper.Psi.Util;

namespace ReCommendedExtension.Analyzers.Annotation
{
    [ElementProblemAnalyzer(
        typeof(IAttributesOwnerDeclaration),
        HighlightingTypes = new[]
        {
            typeof(RedundantAnnotationSuggestion), typeof(NotAllowedAnnotationWarning), typeof(MissingAnnotationWarning),
            typeof(MissingSuppressionJustificationWarning), typeof(ConflictingAnnotationWarning), typeof(ConditionalAnnotationHint),
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

        [Pure]
        static bool CanContainNullnessAttributes([NotNull] IAttributesOwnerDeclaration declaration)
        {
            // excluding type, constant, enum member, property/indexer/event accessor, event, type parameter declarations
            if (declaration is ICSharpTypeDeclaration ||
                declaration is IConstantDeclaration ||
                declaration is IEnumMemberDeclaration ||
                declaration is IAccessorDeclaration ||
                declaration is IEventDeclaration ||
                declaration is ITypeParameterDeclaration ||
                declaration is IConstructorDeclaration)
            {
                return false;
            }

            // excluding overridden members
            if (declaration.OverridesInheritedMember())
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
                where categoryConstantValue != null &&
                    checkIdConstantValue != null &&
                    categoryConstantValue.IsString() &&
                    checkIdConstantValue.IsString()
                let justificationConstantValue = attributeInstance.NamedParameter(nameof(SuppressMessageAttribute.Justification)).ConstantValue
                where justificationConstantValue == null ||
                    !justificationConstantValue.IsString() ||
                    string.IsNullOrWhiteSpace((string)justificationConstantValue.Value)
                select new { Attribute = attribute, Category = (string)categoryConstantValue.Value, CheckId = (string)checkIdConstantValue.Value };

            foreach (var suppressMessageAttribute in suppressMessageAttributes)
            {
                Debug.Assert(suppressMessageAttribute != null);

                consumer.AddHighlighting(
                    new MissingSuppressionJustificationWarning(
                        attributesOwnerDeclaration,
                        suppressMessageAttribute.Attribute,
                        string.Format(
                            "Suppression justification is missing for {0}:{1}.",
                            suppressMessageAttribute.Category,
                            suppressMessageAttribute.CheckId)));
            }
        }

        static void AnalyzeConflictingPurityAnnotations(
            [NotNull] IHighlightingConsumer consumer,
            [NotNull] IAttributesOwnerDeclaration attributesOwnerDeclaration)
        {
            var groupings = (
                from attribute in attributesOwnerDeclaration.Attributes
                let shortName = attribute.GetAttributeInstance().GetAttributeType().GetClrName().ShortName
                where shortName == PureAnnotationProvider.PureAttributeShortName ||
                    shortName == MustUseReturnValueAnnotationProvider.MustUseReturnValueAttributeShortName
                group attribute by shortName).ToList();
            if (groupings.Count > 1)
            {
                foreach (var grouping in groupings)
                {
                    Debug.Assert(grouping != null);

                    var shortName = grouping.Key;

                    Debug.Assert(
                        shortName == PureAnnotationProvider.PureAttributeShortName ||
                        shortName == MustUseReturnValueAnnotationProvider.MustUseReturnValueAttributeShortName);

                    var conflictingAnnotation = shortName == PureAnnotationProvider.PureAttributeShortName
                        ? MustUseReturnValueAnnotationProvider.MustUseReturnValueAttributeShortName
                        : PureAnnotationProvider.PureAttributeShortName;

                    foreach (var attribute in grouping)
                    {
                        Debug.Assert(attribute != null);

                        consumer.AddHighlighting(
                            new ConflictingAnnotationWarning(
                                attributesOwnerDeclaration,
                                attribute,
                                string.Format("Annotation conflicts with '{0}' annotation.", conflictingAnnotation)));
                    }
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
                    where attributeInstance.AssertNotNull().PositionParameterCount == 1
                    let constantValue = attributeInstance.PositionParameter(0).ConstantValue
                    where constantValue != null && constantValue.IsString()
                    let condition = (string)constantValue.Value
                    where !string.IsNullOrEmpty(condition)
                    select condition).ToList()
                where conditions.Count > 0
                select new { Attribute = attribute, Conditions = conditions };

            foreach (var conditionalAttribute in conditionalAttributes)
            {
                Debug.Assert(conditionalAttribute != null);

                consumer.AddHighlighting(
                    new ConditionalAnnotationHint(
                        attributesOwnerDeclaration,
                        conditionalAttribute.Attribute,
                        conditionalAttribute.Conditions.Count == 1
                            ? string.Format("Attribute will be ignored if the '{0}' condition is not defined.", conditionalAttribute.Conditions[0])
                            : string.Format(
                                "Attribute will be ignored if none of the following conditions is defined: {0}.",
                                string.Join(", ", from condition in conditionalAttribute.Conditions orderby condition select $"'{condition}'"))));
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

        void AnalyzeReSharperAnnotations([NotNull] IHighlightingConsumer consumer, [NotNull] IAttributesOwnerDeclaration element)
        {
            Debug.Assert(element.IsNullableAnnotationsContextEnabled());

            foreach (var attribute in element.AttributesEnumerable)
            {
                Debug.Assert(attribute != null);

                var attributeInstance = attribute.GetAttributeInstance();
                if (nullnessProvider.IsNullableAttribute(attributeInstance) ||
                    containerElementNullnessProvider.IsContainerElementNullableAttribute(attributeInstance))
                {
                    consumer.AddHighlighting(
                        new RedundantAnnotationSuggestion(
                            element,
                            attribute,
                            "Annotation is redundant because the nullable annotation context is enabled."));
                }
            }
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
                    var nonNullAnnotationAttributeType = codeAnnotationsConfiguration.GetAttributeTypeForElement(
                        attributesOwnerDeclaration,
                        NullnessProvider.NotNullAttributeShortName);
                    if (nonNullAnnotationAttributeType != null)
                    {
                        consumer.AddHighlighting(
                            new MissingAnnotationWarning(
                                string.Format(
                                    "Declared element can never be null by default, but is not annotated with '{0}'.",
                                    NullnessProvider.NotNullAttributeShortName),
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
                            var nonNullAnnotationAttributeType = codeAnnotationsConfiguration.GetAttributeTypeForElement(
                                attributesOwnerDeclaration,
                                NullnessProvider.NotNullAttributeShortName);
                            var canBeNullAnnotationAttributeType = codeAnnotationsConfiguration.GetAttributeTypeForElement(
                                attributesOwnerDeclaration,
                                NullnessProvider.CanBeNullAttributeShortName);
                            if (nonNullAnnotationAttributeType != null || canBeNullAnnotationAttributeType != null)
                            {
                                consumer.AddHighlighting(
                                    new MissingAnnotationWarning(
                                        string.Format(
                                            @"Declared element is nullable, but is not annotated with '{0}' or '{1}'.",
                                            NullnessProvider.NotNullAttributeShortName,
                                            NullnessProvider.CanBeNullAttributeShortName),
                                        attributesOwnerDeclaration));
                            }
                            break;
                        }
                    }
                    break;

                case ValueAnalysisMode.PESSIMISTIC:
                    foreach (var attributeMark in GetAttributeMarks(attributesOwnerDeclaration))
                    {
                        if (attributeMark != null && attributeMark.AnnotationNullableValue == CodeAnnotationNullableValue.CAN_BE_NULL)
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

        void AnalyzeNotAllowedItemNotNull(
            [NotNull] IHighlightingConsumer consumer,
            [NotNull] IAttributesOwnerDeclaration attributesOwnerDeclaration)
        {
            Debug.Assert(!attributesOwnerDeclaration.IsNullableAnnotationsContextEnabled());

            var itemNotNullAttribute = attributesOwnerDeclaration.Attributes.FirstOrDefault(
                attribute
                    => containerElementNullnessProvider.GetContainerElementNullableAttributeMark(attribute.AssertNotNull().GetAttributeInstance()) ==
                    CodeAnnotationNullableValue.NOT_NULL);
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
                            string.Format(
                                "Annotation is not allowed because the declared element must be an {0}<T> (or its descendant), " +
                                "or a generic task-like type, or a {1}<T>.",
                                nameof(IEnumerable<int>),
                                nameof(Lazy<int>))));
                }
            }
        }

        protected override void Run(IAttributesOwnerDeclaration element, ElementProblemAnalyzerData data, IHighlightingConsumer consumer)
        {
            if (element.IsNullableAnnotationsContextEnabled())
            {
                AnalyzeReSharperAnnotations(consumer, element);
            }
            else
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

            // attributes annotated as [Conditional]
            AnalyzeConditional(consumer, element);
        }
    }
}