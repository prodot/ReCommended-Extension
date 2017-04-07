using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using JetBrains.Annotations;
using JetBrains.ProjectModel.Properties.Flavours;
using JetBrains.ReSharper.Daemon.Stages.Dispatcher;
using JetBrains.ReSharper.Feature.Services.Daemon;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.CodeAnnotations;
using JetBrains.ReSharper.Psi.ControlFlow;
using JetBrains.ReSharper.Psi.CSharp.Impl;
using JetBrains.ReSharper.Psi.CSharp.Tree;
using JetBrains.ReSharper.Psi.CSharp.Util;
using JetBrains.ReSharper.Psi.Impl.Types;
using JetBrains.ReSharper.Psi.Tree;
using JetBrains.ReSharper.Psi.Util;
using ReCommendedExtension.Highlightings;

namespace ReCommendedExtension.Analyzers
{
    [ElementProblemAnalyzer(typeof(IAttributesOwnerDeclaration),
        HighlightingTypes =
            new[]
            {
                typeof(RedundantAnnotationHighlighting), typeof(NotAllowedAnnotationHighlighting), typeof(MissingAnnotationHighlighting),
                typeof(MissingSuppressionJustificationHighlighting), typeof(ConflictingAnnotationHighlighting),
                typeof(ConditionalAnnotationHighlighting)
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

        static bool CanContainNullnessAttributes([NotNull] IAttributesOwnerDeclaration declaration)
        {
            // excluding type, constant, enum member, property/indexer/event accessor, event, type parameter declarations
            if (declaration is ICSharpTypeDeclaration || declaration is IConstantDeclaration || declaration is IEnumMemberDeclaration ||
                declaration is IAccessorDeclaration || declaration is IEventDeclaration || declaration is ITypeParameterDeclaration ||
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

        static AnnotationCase? TryGetAnnotationCase([NotNull] IAttributesOwnerDeclaration declaration)
        {
            if (CanContainNullnessAttributes(declaration))
            {
                if (declaration is IMethodDeclaration methodDeclaration)
                {
                    if (methodDeclaration.IsAsync)
                    {
                        return AnnotationCase.AsyncMethod;
                    }

                    if (methodDeclaration.IsIterator)
                    {
                        return AnnotationCase.IteratorMethod;
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

        [NotNull]
        static IEnumerable<AttributeMark> GetAttributeMarks(
            [NotNull] NullnessProvider nullnessProvider,
            [NotNull] IAttributesOwnerDeclaration declaration)
        {
            var markFound = false;

            foreach (var attribute in declaration.AttributesEnumerable)
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

        [SuppressMessage("ReSharper", "UseNullPropagation", Justification = "Preserve code symmetry.")]
        static IType TryGetTypeForIfCanBeAnnotatedWithItemNotNull([NotNull] IAttributesOwnerDeclaration attributesOwnerDeclaration)
        {
            switch (attributesOwnerDeclaration.DeclaredElement)
            {
                case IMethod method:
                    return method.ReturnType;

                case IParameter parameter:
                    return parameter.Type;

                case IProperty property:
                    return property.Type;

                case IDelegate delegateType:
                    return delegateType.InvokeMethod.ReturnType;

                case IField field:
                    return field.Type;

                default:
                    return null;
            }
        }

        static void AnalyzeAsyncMethod(
            [NotNull] IHighlightingConsumer consumer,
            [NotNull] NullnessProvider nullnessProvider,
            [NotNull] IAttributesOwnerDeclaration attributesOwnerDeclaration)
        {
            foreach (var attributeMark in GetAttributeMarks(nullnessProvider, attributesOwnerDeclaration))
            {
                if (attributeMark != null)
                {
                    switch (attributeMark.AnnotationNullableValue)
                    {
                        case CodeAnnotationNullableValue.NOT_NULL:
                            consumer.AddHighlighting(
                                new RedundantAnnotationHighlighting(
                                    attributesOwnerDeclaration,
                                    attributeMark.Attribute,
                                    "Annotation is redundant because the declared element can never be null by default."));
                            break;

                        case CodeAnnotationNullableValue.CAN_BE_NULL:
                            consumer.AddHighlighting(
                                new NotAllowedAnnotationHighlighting(
                                    attributesOwnerDeclaration,
                                    attributeMark.Attribute,
                                    "Annotation is not valid because the declared element can never be null by default."));
                            break;
                    }
                }
            }
        }

        static void AnalyzeIteratorMethod(
            [NotNull] IHighlightingConsumer consumer,
            [NotNull] IAttributesOwnerDeclaration attributesOwnerDeclaration,
            [NotNull] NullnessProvider nullnessProvider,
            [NotNull] CodeAnnotationsConfiguration codeAnnotationsConfiguration)
        {
            foreach (var attributeMark in GetAttributeMarks(nullnessProvider, attributesOwnerDeclaration))
            {
                if (attributeMark != null)
                {
                    if (attributeMark.AnnotationNullableValue == CodeAnnotationNullableValue.CAN_BE_NULL)
                    {
                        consumer.AddHighlighting(
                            new NotAllowedAnnotationHighlighting(
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
                            new MissingAnnotationHighlighting(
                                string.Format(
                                    "Declared element can never be null by default, but is not annotated with '{0}'.",
                                    NullnessProvider.NotNullAttributeShortName),
                                attributesOwnerDeclaration));
                    }
                    break;
                }
            }
        }

        static void AnalyzeOther(
            ValueAnalysisMode valueAnalysisMode,
            [NotNull] IHighlightingConsumer consumer,
            [NotNull] NullnessProvider nullnessProvider,
            [NotNull] CodeAnnotationsConfiguration codeAnnotationsConfiguration,
            [NotNull] IAttributesOwnerDeclaration attributesOwnerDeclaration)
        {
            switch (valueAnalysisMode)
            {
                case ValueAnalysisMode.OPTIMISTIC:
                    foreach (var attributeMark in GetAttributeMarks(nullnessProvider, attributesOwnerDeclaration))
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
                                    new MissingAnnotationHighlighting(
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
                    foreach (var attributeMark in GetAttributeMarks(nullnessProvider, attributesOwnerDeclaration))
                    {
                        if (attributeMark != null && attributeMark.AnnotationNullableValue == CodeAnnotationNullableValue.CAN_BE_NULL)
                        {
                            consumer.AddHighlighting(
                                new RedundantAnnotationHighlighting(
                                    attributesOwnerDeclaration,
                                    attributeMark.Attribute,
                                    "Annotation is redundant because the declared element can be null by default."));
                        }
                    }
                    break;
            }
        }

        static void AnalyzeOverride(
            [NotNull] IHighlightingConsumer consumer,
            [NotNull] NullnessProvider nullnessProvider,
            [NotNull] IAttributesOwnerDeclaration attributesOwnerDeclaration)
        {
            foreach (var attributeMark in GetAttributeMarks(nullnessProvider, attributesOwnerDeclaration))
            {
                if (attributeMark != null)
                {
                    consumer.AddHighlighting(
                        new NotAllowedAnnotationHighlighting(
                            attributesOwnerDeclaration,
                            attributeMark.Attribute,
                            "Annotation is not allowed because the declared element overrides or implements the inherited member."));
                }
            }
        }

        static void AnalyzeNotAllowedItemNotNull(
            [NotNull] IHighlightingConsumer consumer,
            [NotNull] IAttributesOwnerDeclaration attributesOwnerDeclaration)
        {
            var itemNotNullAttribute =
                attributesOwnerDeclaration.AttributesEnumerable.FirstOrDefault(
                    attribute =>
                        attribute.AssertNotNull().GetAttributeInstance().GetAttributeType().GetClrName().ShortName ==
                        ContainerElementNullnessProvider.ItemNotNullAttributeShortName);
            if (itemNotNullAttribute != null)
            {
                if (attributesOwnerDeclaration.OverridesInheritedMember())
                {
                    consumer.AddHighlighting(
                        new NotAllowedAnnotationHighlighting(
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
                        var elementType = CollectionTypeUtil.ElementTypeByCollectionType(type, attributesOwnerDeclaration);
                        if (elementType != null)
                        {
                            if (elementType.Classify != TypeClassification.REFERENCE_TYPE)
                            {
                                consumer.AddHighlighting(
                                    new NotAllowedAnnotationHighlighting(
                                        attributesOwnerDeclaration,
                                        itemNotNullAttribute,
                                        "Annotation is not allowed because the declared element type is not a reference type."));
                            }
                        }
                        return;
                    }

                    var resultType = type.GetTasklikeUnderlyingType(attributesOwnerDeclaration);
                    if (resultType != null)
                    {
                        if (resultType.Classify != TypeClassification.REFERENCE_TYPE)
                        {
                            consumer.AddHighlighting(
                                new NotAllowedAnnotationHighlighting(
                                    attributesOwnerDeclaration,
                                    itemNotNullAttribute,
                                    "Annotation is not allowed because the declared task result type is not a reference type."));
                        }
                        return;
                    }

                    if (type.IsLazy())
                    {
                        var typeElement =
                            new DeclaredTypeFromCLRName(PredefinedType.LAZY_FQN, attributesOwnerDeclaration.GetPsiModule()).GetTypeElement();
                        var valueType = type.GetGenericUnderlyingType(typeElement);
                        if (valueType != null)
                        {
                            if (valueType.Classify != TypeClassification.REFERENCE_TYPE)
                            {
                                consumer.AddHighlighting(
                                    new NotAllowedAnnotationHighlighting(
                                        attributesOwnerDeclaration,
                                        itemNotNullAttribute,
                                        "Annotation is not allowed because the declared lazy value type is not a reference type."));
                            }
                        }
                        return;
                    }

                    consumer.AddHighlighting(
                        new NotAllowedAnnotationHighlighting(
                            attributesOwnerDeclaration,
                            itemNotNullAttribute,
                            string.Format(
                                "Annotation is not allowed because the declared element must be an {0}<T> (or its descendant), " +
                                "or a generic task-like type, or a {1}<T>.",
                                nameof(IEnumerable<int>),
                                "Lazy")));
                }
            }
        }

        static void AnalyzeMissingSuppressionJustification(
            [NotNull] IHighlightingConsumer consumer,
            [NotNull] IAttributesOwnerDeclaration attributesOwnerDeclaration)
        {
            var project = attributesOwnerDeclaration.GetProject();
            if (project != null)
            {
                if (project.HasFlavour<MsTestProjectFlavor>())
                {
                    return;
                }
            }

            var suppressMessageAttributes =
                from attribute in attributesOwnerDeclaration.AttributesEnumerable
                let attributeInstance = attribute.GetAttributeInstance()
                where Equals(attributeInstance.GetClrName(), ClrTypeNames.SuppressMessageAttribute)
                where attributeInstance.PositionParameterCount == 2
                let categoryConstantValue = attributeInstance.PositionParameter(0).ConstantValue
                let checkIdConstantValue = attributeInstance.PositionParameter(1).ConstantValue
                where categoryConstantValue != null
                where checkIdConstantValue != null
                where categoryConstantValue.IsString()
                where checkIdConstantValue.IsString()
                let justificationConstantValue = attributeInstance.NamedParameter(nameof(SuppressMessageAttribute.Justification)).ConstantValue
                where
                justificationConstantValue == null || !justificationConstantValue.IsString() ||
                string.IsNullOrWhiteSpace((string)justificationConstantValue.Value)
                select new { Attribute = attribute, Category = (string)categoryConstantValue.Value, CheckId = (string)checkIdConstantValue.Value };

            foreach (var suppressMessageAttribute in suppressMessageAttributes)
            {
                Debug.Assert(suppressMessageAttribute != null);

                consumer.AddHighlighting(
                    new MissingSuppressionJustificationHighlighting(
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
                from attribute in attributesOwnerDeclaration.AttributesEnumerable
                let shortName = attribute.GetAttributeInstance().GetAttributeType().GetClrName().ShortName
                where
                shortName == PureAnnotationProvider.PureAttributeShortName ||
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
                            new ConflictingAnnotationHighlighting(
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
                from attribute in attributesOwnerDeclaration.AttributesEnumerable
                let typeElement = attribute.GetAttributeInstance().GetAttributeType().GetTypeElement()
                where typeElement != null
                let conditions = (
                    from attributeInstance in typeElement.GetAttributeInstances(PredefinedType.CONDITIONAL_ATTRIBUTE_CLASS, false)
                    where attributeInstance.AssertNotNull().PositionParameterCount == 1
                    let constantValue = attributeInstance.PositionParameter(0).ConstantValue
                    where constantValue != null
                    where constantValue.IsString()
                    let condition = (string)constantValue.Value
                    where !string.IsNullOrEmpty(condition)
                    select condition).ToList()
                where conditions.Count > 0
                select new { Attribute = attribute, Conditions = conditions };

            foreach (var conditionalAttribute in conditionalAttributes)
            {
                Debug.Assert(conditionalAttribute != null);

                consumer.AddHighlighting(
                    new ConditionalAnnotationHighlighting(
                        attributesOwnerDeclaration,
                        conditionalAttribute.Attribute,
                        conditionalAttribute.Conditions.Count == 1
                            ? string.Format("Attribute will be ignored if the '{0}' condition is not defined.", conditionalAttribute.Conditions[0])
                            : string.Format(
                                "Attribute will be ignored if none of the following conditions is defined: {0}.",
                                string.Join(", ", from condition in conditionalAttribute.Conditions orderby condition select $"'{condition}'"))));
            }
        }

        protected override void Run(IAttributesOwnerDeclaration element, ElementProblemAnalyzerData data, IHighlightingConsumer consumer)
        {
            var psiServices = element.GetPsiServices();
            var nullnessProvider = psiServices.GetCodeAnnotationsCache().GetProvider<NullnessProvider>();
            var codeAnnotationsConfiguration = psiServices.GetComponent<CodeAnnotationsConfiguration>();

            // [NotNull], [CanBeNull] annotations
            switch (TryGetAnnotationCase(element))
            {
                case AnnotationCase.AsyncMethod:
                    AnalyzeAsyncMethod(consumer, nullnessProvider, element);
                    break;

                case AnnotationCase.IteratorMethod:
                    AnalyzeIteratorMethod(consumer, element, nullnessProvider, codeAnnotationsConfiguration);
                    break;

                case AnnotationCase.Other:
                    AnalyzeOther(data.GetValueAnalysisMode(), consumer, nullnessProvider, codeAnnotationsConfiguration, element);
                    break;

                case AnnotationCase.Override:
                    AnalyzeOverride(consumer, nullnessProvider, element);
                    break;
            }

            // [ItemNotNull] annotations
            AnalyzeNotAllowedItemNotNull(consumer, element);

            // [SuppressMessage] annotations
            AnalyzeMissingSuppressionJustification(consumer, element);

            // [Pure] and [MustUseReturnValue] annotations
            AnalyzeConflictingPurityAnnotations(consumer, element);

            // attributes annotated as [Conditional]
            AnalyzeConditional(consumer, element);
        }
    }
}