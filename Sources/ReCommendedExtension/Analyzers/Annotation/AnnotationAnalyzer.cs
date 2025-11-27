using System.ComponentModel;
using JetBrains.Metadata.Reader.API;
using JetBrains.ReSharper.Feature.Services.Daemon;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.CodeAnnotations;
using JetBrains.ReSharper.Psi.ControlFlow;
using JetBrains.ReSharper.Psi.CSharp;
using JetBrains.ReSharper.Psi.CSharp.DeclaredElements;
using JetBrains.ReSharper.Psi.CSharp.Impl;
using JetBrains.ReSharper.Psi.CSharp.Tree;
using JetBrains.ReSharper.Psi.CSharp.Util;
using JetBrains.ReSharper.Psi.Modules;
using JetBrains.ReSharper.Psi.Tree;
using JetBrains.ReSharper.Psi.Util;
using ReCommendedExtension.Extensions;
using ReCommendedExtension.Extensions.NumberInfos;

namespace ReCommendedExtension.Analyzers.Annotation;

[ElementProblemAnalyzer(
    typeof(IAttributesOwnerDeclaration),
    HighlightingTypes =
    [
        typeof(RedundantNullableAnnotationHint),
        typeof(RedundantAnnotationSuggestion),
        typeof(NotAllowedAnnotationWarning),
        typeof(MissingAnnotationWarning),
        typeof(MissingSuppressionJustificationWarning),
        typeof(ConflictingAnnotationWarning),
        typeof(ConditionalAnnotationHint),
        typeof(InvalidValueRangeBoundaryWarning),
        typeof(RedundantAnnotationArgumentSuggestion),
    ])]
public sealed class AnnotationAnalyzer(CodeAnnotationsCache codeAnnotationsCache) : ElementProblemAnalyzer<IAttributesOwnerDeclaration>
{
    enum NullabilityAnnotationCase
    {
        AsyncMethod,
        IteratorMethod,
        Other,
        Override,
    }

    sealed record NullabilityAttributeMark
    {
        public required CodeAnnotationNullableValue AnnotationNullableValue { get; init; }

        public required IAttribute Attribute { get; init; }
    }

    enum PurityOrDisposabilityKind
    {
        Pure,
        MustUseReturnValue,
        MustDisposeResource,
        MustDisposeResourceFalse,
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
        if (declaration.OverridesInheritedMember)
        {
            return false;
        }

        // excluding local function (C# 8 or less)
        if (declaration.IsOnLocalFunctionWithUnsupportedAttributes)
        {
            return false;
        }

        // excluding lambda expressions (C# 9 or less)
        if (declaration.IsOnLambdaExpressionWithUnsupportedAttributes)
        {
            return false;
        }

        // excluding anonymous methods
        if (declaration.IsOnAnonymousMethodWithUnsupportedAttributes)
        {
            return false;
        }

        // excluding members of non-reference types (value, nullable value, unspecified generic types)
        if (declaration is ITypeOwnerDeclaration typeOwner)
        {
            // first check if declaration is a IMethodDeclaration and its TypeUsage is null
            // (otherwise the Type property throws the NullReferenceException)
            if (typeOwner is IMethodDeclaration { TypeUsage: null })
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
    static NullabilityAnnotationCase? TryGetNullabilityAnnotationCase(IAttributesOwnerDeclaration declaration)
    {
        if (CanContainNullnessAttributes(declaration))
        {
            return declaration switch
            {
                IMethodDeclaration { IsIterator: true } => NullabilityAnnotationCase.IteratorMethod,
                IMethodDeclaration { IsAsync: true } => NullabilityAnnotationCase.AsyncMethod,

                _ => NullabilityAnnotationCase.Other,
            };
        }

        if (declaration.OverridesInheritedMember)
        {
            return NullabilityAnnotationCase.Override;
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
                && attribute.TryGetArgumentsInDeclarationOrder() is
                [
                    { Value.AsStringConstant: { } category }, { Value.AsStringConstant: { } checkId },
                ]
                && (attribute.PropertyAssignments.FirstOrDefault(p => p.PropertyNameIdentifier.Name == nameof(SuppressMessageAttribute.Justification))
                        is not { Source.AsStringConstant: { } suppressMessageJustification }
                    || string.IsNullOrWhiteSpace(suppressMessageJustification)))
            {
                consumer.AddHighlighting(
                    new MissingSuppressionJustificationWarning($"Suppression justification is missing for {category}:{checkId}.")
                    {
                        AttributesOwnerDeclaration = attributesOwnerDeclaration, Attribute = attribute,
                    });
            }

            if (excludeFromCodeCoverageJustificationPropertyExists
                && attributeType.IsClrType(ClrTypeNames.ExcludeFromCodeCoverageAttribute)
                && (attribute.PropertyAssignments.FirstOrDefault(p => p.PropertyNameIdentifier.Name == "Justification") is not // todo: use nameof(ExcludeFromCodeCoverageAttribute.Justification)
                    {
                        Source.AsStringConstant: { } excludeFromCodeCoverageJustification,
                    }
                    || string.IsNullOrWhiteSpace(excludeFromCodeCoverageJustification)))
            {
                consumer.AddHighlighting(
                    new MissingSuppressionJustificationWarning("Justification is missing for the exclusion from code coverage.")
                    {
                        AttributesOwnerDeclaration = attributesOwnerDeclaration, Attribute = attribute,
                    });
            }
        }
    }

    [Pure]
    static bool ExcludeFromCodeCoverageJustificationPropertyExists(IPsiModule psiModule)
        => ClrTypeNames.ExcludeFromCodeCoverageAttribute.TryGetTypeElement(psiModule) is { } attributeType
            && attributeType.Properties.Any(property => property is { IsStatic: false, ShortName: "Justification" }); // todo: use nameof(ExcludeFromCodeCoverageAttribute.Justification)

    static void AnalyzeNullableAnnotations(IHighlightingConsumer consumer, IAttributesOwnerDeclaration attributesOwnerDeclaration)
    {
        Debug.Assert(attributesOwnerDeclaration.IsNullableAnnotationsContextEnabled());

        switch (attributesOwnerDeclaration)
        {
            case IMethodDeclaration
            {
                DeclaredElement.ReturnType: { Classify: TypeClassification.REFERENCE_TYPE, NullableAnnotation: NullableAnnotation.Annotated },
                TypeUsage: INullableTypeUsage nullableTypeUsage,
                NameIdentifier.Name: var methodName,
            } methodDeclaration when methodDeclaration.IsIterator || methodDeclaration.IsAsync:

                // R# doesn't seem to cover multi-line async methods that return "Task?"

                consumer.AddHighlighting(
                    new RedundantNullableAnnotationHint($"Return type of '{methodName}' can be made non-nullable.")
                    {
                        NullableTypeUsage = nullableTypeUsage,
                    });
                break;

            case ILocalFunctionDeclaration
            {
                DeclaredElement.ReturnType: { Classify: TypeClassification.REFERENCE_TYPE, NullableAnnotation: NullableAnnotation.Annotated },
                TypeUsage: INullableTypeUsage nullableTypeUsage,
                NameIdentifier.Name: var localFunctionName,
            } localFunction when localFunction.IsIterator || localFunction.IsAsync:

                consumer.AddHighlighting(
                    new RedundantNullableAnnotationHint($"Return type of '{localFunctionName}' can be made non-nullable.")
                    {
                        NullableTypeUsage = nullableTypeUsage,
                    });
                break;
        }
    }

    static void AnalyzePurityAndDisposability(IHighlightingConsumer consumer, IAttributesOwnerDeclaration element)
    {
        [Pure]
        static bool IsPureAttribute(IAttributeInstance attribute) => attribute.GetAttributeShortName() == nameof(PureAttribute);

        [Pure]
        static bool IsMustUseReturnValueAttribute(IAttributeInstance attribute)
            => attribute.GetAttributeShortName() == nameof(MustUseReturnValueAttribute);

        [Pure]
        static bool IsMustDisposeResourceAttribute(IAttributeInstance attribute)
            => attribute.GetAttributeShortName() == nameof(MustDisposeResourceAttribute)
                && (attribute.PositionParameterCount == 0
                    || attribute.PositionParameterCount == 1
                    && attribute.PositionParameter(0).ConstantValue is { Kind: ConstantValueKind.Bool, BoolValue: true });

        [Pure]
        static bool IsMustDisposeResourceFalseAttribute(IAttributeInstance attribute)
            => attribute.GetAttributeShortName() == nameof(MustDisposeResourceAttribute)
                && attribute.PositionParameterCount == 1
                && attribute.PositionParameter(0).ConstantValue is { Kind: ConstantValueKind.Bool, BoolValue: false };

        [Pure]
        static bool IsMustDisposeResourceTrueAttribute(IAttributeInstance attribute)
            => attribute.GetAttributeShortName() == nameof(MustDisposeResourceAttribute)
                && attribute.PositionParameterCount == 1
                && attribute.PositionParameter(0).ConstantValue is { Kind: ConstantValueKind.Bool, BoolValue: true };

        [Pure]
        static IAttributeInstance? TryGetAnnotation(IAttributesSet attributesSet, PurityOrDisposabilityKind kind)
            => attributesSet
                .GetAttributeInstances(AttributesSource.Self)
                .FirstOrDefault(
                    a => kind switch
                    {
                        PurityOrDisposabilityKind.Pure => IsPureAttribute(a),
                        PurityOrDisposabilityKind.MustUseReturnValue => IsMustUseReturnValueAttribute(a),
                        PurityOrDisposabilityKind.MustDisposeResource => IsMustDisposeResourceAttribute(a),
                        PurityOrDisposabilityKind.MustDisposeResourceFalse => IsMustDisposeResourceFalseAttribute(a),

                        _ => throw new NotSupportedException(),
                    });

        [Pure]
        static bool IsAnnotated(IAttributesSet attributesSet, PurityOrDisposabilityKind kind) => TryGetAnnotation(attributesSet, kind) is { };

        [Pure]
        static IAttributeInstance? TryGetAnnotationAnyOf(
            IAttributesSet attributesSet,
            PurityOrDisposabilityKind kind1,
            PurityOrDisposabilityKind kind2)
        {
            Debug.Assert(kind1 != kind2);

            return attributesSet
                .GetAttributeInstances(AttributesSource.Self)
                .FirstOrDefault(a => kind1 switch
                    {
                        PurityOrDisposabilityKind.MustDisposeResource => IsMustDisposeResourceAttribute(a),
                        PurityOrDisposabilityKind.MustDisposeResourceFalse => IsMustDisposeResourceFalseAttribute(a),

                        _ => throw new NotSupportedException(),
                    }
                    || kind2 switch
                    {
                        PurityOrDisposabilityKind.MustDisposeResource => IsMustDisposeResourceAttribute(a),
                        PurityOrDisposabilityKind.MustDisposeResourceFalse => IsMustDisposeResourceFalseAttribute(a),

                        _ => throw new NotSupportedException(),
                    });
        }

        [Pure]
        static bool IsAnnotatedWithAnyOf(IAttributesSet attributesSet, PurityOrDisposabilityKind kind1, PurityOrDisposabilityKind kind2)
            => TryGetAnnotationAnyOf(attributesSet, kind1, kind2) is { };

        [Pure]
        static bool IsAnyBaseTypeAnnotated(ITypeElement type, PurityOrDisposabilityKind kind)
            => type.GetAllSuperTypeElements().Any(baseType => IsAnnotated(baseType, kind));

        [Pure]
        static bool IsAnyBaseMethodAnnotated(IMethod method, PurityOrDisposabilityKind kind)
            => method.GetAllSuperMembers().Any(baseMethod => IsAnnotated(baseMethod.Member, kind));

        [Pure]
        static bool IsMethodOverridenWithOtherReturnType(IMethod method)
            => method.IsOverride
                && method.GetImmediateSuperMembers().FirstOrDefault() is { Member: IMethod baseMethod }
                && !TypeEqualityComparer.Default.Equals(baseMethod.ReturnType, method.ReturnType);

        [Pure]
        static bool IsParameterOfAnyBaseMethodAnnotated(IParameter parameter, PurityOrDisposabilityKind kind)
        {
            if (parameter.ContainingParametersOwner is IMethod method)
            {
                var parameterIndex = method.Parameters.IndexOf(parameter);

                foreach (var member in method.GetAllSuperMembers())
                {
                    var baseMethod = (IMethod)member.Member;

                    if (baseMethod.Parameters.ElementAtOrDefault(parameterIndex) is { } baseMethodParameter
                        && TypeEqualityComparer.Default.Equals(parameter.Type, baseMethodParameter.Type)
                        && parameter.Kind == baseMethodParameter.Kind
                        && IsAnnotated(baseMethodParameter, kind))
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        [Pure]
        static string GetTypeDescription(IClassLikeDeclaration typeDeclaration)
            => typeDeclaration switch
            {
                IClassDeclaration => "class",
                IStructDeclaration => "struct",
                IRecordDeclaration => "record",

                _ => throw new NotSupportedException(),
            };

        [Pure]
        string? TryGetAttributeNameIfAnnotationProvided(PurityOrDisposabilityKind kind)
        {
            var attributeName = kind switch
            {
                PurityOrDisposabilityKind.Pure => nameof(PureAttribute),
                PurityOrDisposabilityKind.MustUseReturnValue => nameof(MustUseReturnValueAttribute),
                PurityOrDisposabilityKind.MustDisposeResource or PurityOrDisposabilityKind.MustDisposeResourceFalse => nameof(
                    MustDisposeResourceAttribute),

                _ => throw new NotSupportedException(),
            };

            return element.IsAnnotationProvided(attributeName) ? attributeName : null;
        }

        void HighlightMissing(PurityOrDisposabilityKind kind, string message)
        {
            if (TryGetAttributeNameIfAnnotationProvided(kind) is { })
            {
                consumer.AddHighlighting(new MissingAnnotationWarning(message, element));
            }
        }

        void HighlightRedundant(PurityOrDisposabilityKind kind, string message)
        {
            if (TryGetAttributeNameIfAnnotationProvided(kind) is { } attributeName
                && element.Attributes.FirstOrDefault(a => a.GetAttributeInstance().GetAttributeShortName() == attributeName) is { } attribute)
            {
                consumer.AddHighlighting(new RedundantAnnotationSuggestion(message) { AttributesOwnerDeclaration = element, Attribute = attribute });
            }
        }

        void HighlightNotAllowed(PurityOrDisposabilityKind kind, string message)
        {
            if (TryGetAttributeNameIfAnnotationProvided(kind) is { } attributeName
                && element.Attributes.FirstOrDefault(a => a.GetAttributeInstance().GetAttributeShortName() == attributeName) is { } attribute)
            {
                consumer.AddHighlighting(new NotAllowedAnnotationWarning(message) { AttributesOwnerDeclaration = element, Attribute = attribute });
            }
        }

        void HighlightConflicting(PurityOrDisposabilityKind kind, PurityOrDisposabilityKind otherKind, string message)
        {
            Debug.Assert(kind != otherKind);

            if (TryGetAttributeNameIfAnnotationProvided(kind) is { } attributeName
                && TryGetAttributeNameIfAnnotationProvided(otherKind) is { }
                && element.Attributes.FirstOrDefault(a => a.GetAttributeInstance().GetAttributeShortName() == attributeName) is { } attribute)
            {
                consumer.AddHighlighting(new ConflictingAnnotationWarning(message) { AttributesOwnerDeclaration = element, Attribute = attribute });
            }
        }

        void HighlightRedundantMustDisposeResourceTrueArgument(string message)
        {
            if (TryGetAttributeNameIfAnnotationProvided(PurityOrDisposabilityKind.MustDisposeResource) is { } attributeName
                && element.Attributes.FirstOrDefault(a => a.GetAttributeInstance().GetAttributeShortName() == attributeName) is { } attribute
                && attribute.TryGetArgumentsInDeclarationOrder() is [{ } argument])
            {
                consumer.AddHighlighting(
                    new RedundantAnnotationArgumentSuggestion(message, argument) { AttributesOwnerDeclaration = element, Attribute = attribute });
            }
        }

        switch (element)
        {
            case IClassLikeDeclaration { DeclaredElement: { } type } typeDeclaration
                and (IClassDeclaration or IRecordDeclaration { IsStruct: false }):
            {
                if (type.IsDisposable)
                {
                    if (!IsAnnotatedWithAnyOf(type, PurityOrDisposabilityKind.MustDisposeResource, PurityOrDisposabilityKind.MustDisposeResourceFalse)
                        && !IsAnyBaseTypeAnnotated(type, PurityOrDisposabilityKind.MustDisposeResource))
                    {
                        var typeDescription = GetTypeDescription(typeDeclaration).WithFirstCharacterUpperCased();
                        var name = nameof(MustDisposeResourceAttribute).WithoutSuffix();

                        HighlightMissing(
                            PurityOrDisposabilityKind.MustDisposeResource,
                            $"{typeDescription} is disposable, but not annotated with [{name}] or [{name}(false)].");
                    }

                    if (TryGetAnnotation(type, PurityOrDisposabilityKind.MustDisposeResource) is { } annotation)
                    {
                        if (IsAnyBaseTypeAnnotated(type, PurityOrDisposabilityKind.MustDisposeResource))
                        {
                            var typeDescription = GetTypeDescription(typeDeclaration);
                            var name = nameof(MustDisposeResourceAttribute).WithoutSuffix();

                            HighlightRedundant(
                                PurityOrDisposabilityKind.MustDisposeResource,
                                $"Annotation is redundant because a base {typeDescription} is already annotated with [{name}].");
                        }
                        else
                        {
                            if (IsMustDisposeResourceTrueAttribute(annotation))
                            {
                                HighlightRedundantMustDisposeResourceTrueArgument(
                                    $"Passing 'true' to the [{nameof(MustDisposeResourceAttribute).WithoutSuffix()}] annotation is redundant.");
                            }
                        }
                    }
                }
                else
                {
                    if (IsAnnotatedWithAnyOf(type, PurityOrDisposabilityKind.MustDisposeResource, PurityOrDisposabilityKind.MustDisposeResourceFalse))
                    {
                        var typeDescription = GetTypeDescription(typeDeclaration);

                        HighlightNotAllowed(
                            PurityOrDisposabilityKind.MustDisposeResource,
                            $"Annotation is not valid because the {typeDescription} is not disposable.");
                    }
                }
                break;
            }

            case IClassLikeDeclaration { DeclaredElement: { } type } typeDeclaration
                and (IStructDeclaration { IsByRefLike: false } or IRecordDeclaration { IsStruct: true }):
            {
                if (type.IsDisposable)
                {
                    if (element.MustDisposeResourceAttributeSupportsStructs())
                    {
                        if (!IsAnnotatedWithAnyOf(
                            type,
                            PurityOrDisposabilityKind.MustDisposeResource,
                            PurityOrDisposabilityKind.MustDisposeResourceFalse))
                        {
                            var typeDescription = GetTypeDescription(typeDeclaration).WithFirstCharacterUpperCased();
                            var name = nameof(MustDisposeResourceAttribute).WithoutSuffix();

                            HighlightMissing(
                                PurityOrDisposabilityKind.MustDisposeResource,
                                $"{typeDescription} is disposable, but not annotated with [{name}] or [{name}(false)].");
                        }

                        if (TryGetAnnotation(type, PurityOrDisposabilityKind.MustDisposeResource) is { } annotation
                            && IsMustDisposeResourceTrueAttribute(annotation))
                        {
                            HighlightRedundantMustDisposeResourceTrueArgument(
                                $"Passing 'true' to the [{nameof(MustDisposeResourceAttribute).WithoutSuffix()}] annotation is redundant.");
                        }
                    }
                    else
                    {
                        if (type.Constructors.All(c => c.IsImplicit))
                        {
                            var typeDescription = GetTypeDescription(typeDeclaration).WithFirstCharacterUpperCased();
                            var name = nameof(MustDisposeResourceAttribute).WithoutSuffix();

                            HighlightMissing(
                                PurityOrDisposabilityKind.MustDisposeResource,
                                $"{typeDescription} is disposable, but it doesn't have any constructor to be annotated with [{name}] or [{name}(false)].");
                        }
                    }
                }
                else
                {
                    if (IsAnnotatedWithAnyOf(type, PurityOrDisposabilityKind.MustDisposeResource, PurityOrDisposabilityKind.MustDisposeResourceFalse)
                        && element.MustDisposeResourceAttributeSupportsStructs())
                    {
                        var typeDescription = GetTypeDescription(typeDeclaration);

                        HighlightNotAllowed(
                            PurityOrDisposabilityKind.MustDisposeResource,
                            $"Annotation is not valid because the {typeDescription} is not disposable.");
                    }
                }
                break;
            }

            case IStructDeclaration { IsByRefLike: true, DeclaredElement: { } type }:
            {
                if (type.IsDisposable || type.HasDisposeMethods)
                {
                    if (element.MustDisposeResourceAttributeSupportsStructs())
                    {
                        if (!IsAnnotatedWithAnyOf(
                            type,
                            PurityOrDisposabilityKind.MustDisposeResource,
                            PurityOrDisposabilityKind.MustDisposeResourceFalse))
                        {
                            var name = nameof(MustDisposeResourceAttribute).WithoutSuffix();

                            HighlightMissing(
                                PurityOrDisposabilityKind.MustDisposeResource,
                                $"Struct is disposable, but not annotated with [{name}] or [{name}(false)].");
                        }

                        if (TryGetAnnotation(type, PurityOrDisposabilityKind.MustDisposeResource) is { } annotation
                            && IsMustDisposeResourceTrueAttribute(annotation))
                        {
                            HighlightRedundantMustDisposeResourceTrueArgument(
                                $"Passing 'true' to the [{nameof(MustDisposeResourceAttribute).WithoutSuffix()}] annotation is redundant.");
                        }
                    }
                    else
                    {
                        if (type.Constructors.All(c => c.IsImplicit))
                        {
                            var name = nameof(MustDisposeResourceAttribute).WithoutSuffix();

                            HighlightMissing(
                                PurityOrDisposabilityKind.MustDisposeResource,
                                $"Struct is disposable, but it doesn't have any constructor to be annotated with [{name}] or [{name}(false)].");
                        }
                    }
                }
                else
                {
                    if (IsAnnotatedWithAnyOf(type, PurityOrDisposabilityKind.MustDisposeResource, PurityOrDisposabilityKind.MustDisposeResourceFalse)
                        && element.MustDisposeResourceAttributeSupportsStructs())
                    {
                        HighlightNotAllowed(
                            PurityOrDisposabilityKind.MustDisposeResource,
                            "Annotation is not valid because the struct is not disposable.");
                    }
                }
                break;
            }

            case ICSharpTypeMemberDeclaration { DeclaredElement: { } constructor } constructorDeclaration
                and (IConstructorDeclaration or IPrimaryConstructorDeclaration):
            {
                switch (constructorDeclaration.GetContainingTypeDeclaration())
                {
                    case IClassLikeDeclaration { DeclaredElement: { } type } typeDeclaration
                        and (IClassDeclaration or IStructDeclaration { IsByRefLike: false } or IRecordDeclaration):
                    {
                        if (type.IsDisposable)
                        {
                            if (!IsAnnotatedWithAnyOf(
                                    constructor,
                                    PurityOrDisposabilityKind.MustDisposeResource,
                                    PurityOrDisposabilityKind.MustDisposeResourceFalse)
                                && IsAnnotated(type, PurityOrDisposabilityKind.MustDisposeResourceFalse)
                                && (element.MustDisposeResourceAttributeSupportsStructs()
                                    || typeDeclaration is IClassDeclaration or IRecordDeclaration { IsStruct: false }))
                            {
                                var typeDescription = GetTypeDescription(typeDeclaration);
                                var name = nameof(MustDisposeResourceAttribute).WithoutSuffix();

                                HighlightMissing(
                                    PurityOrDisposabilityKind.MustDisposeResource,
                                    $"Constructor of the disposable {typeDescription} (with the [{name}(false)] annotation) is not annotated with [{name}] or [{name}(false)].");
                            }

                            if (TryGetAnnotation(constructor, PurityOrDisposabilityKind.MustDisposeResource) is { } annotation)
                            {
                                if ((IsAnnotated(type, PurityOrDisposabilityKind.MustDisposeResource)
                                        || IsAnyBaseTypeAnnotated(type, PurityOrDisposabilityKind.MustDisposeResource))
                                    && (element.MustDisposeResourceAttributeSupportsStructs()
                                        || typeDeclaration is IClassDeclaration or IRecordDeclaration { IsStruct: false }))
                                {
                                    var typeDescription = GetTypeDescription(typeDeclaration);
                                    var name = nameof(MustDisposeResourceAttribute).WithoutSuffix();

                                    switch (typeDeclaration)
                                    {
                                        case IClassDeclaration or IRecordDeclaration { IsStruct: false }:
                                            HighlightRedundant(
                                                PurityOrDisposabilityKind.MustDisposeResource,
                                                $"Annotation is redundant because the {typeDescription} or a base {typeDescription} is already annotated with [{name}].");
                                            break;

                                        case IStructDeclaration or IRecordDeclaration { IsStruct: true }:
                                            HighlightRedundant(
                                                PurityOrDisposabilityKind.MustDisposeResource,
                                                $"Annotation is redundant because the {typeDescription} is already annotated with [{name}].");
                                            break;
                                    }
                                }
                                else
                                {
                                    if (IsMustDisposeResourceTrueAttribute(annotation))
                                    {
                                        HighlightRedundantMustDisposeResourceTrueArgument(
                                            $"Passing 'true' to the [{nameof(MustDisposeResourceAttribute).WithoutSuffix()}] annotation is redundant.");
                                    }
                                }
                            }

                            if (!element.MustDisposeResourceAttributeSupportsStructs()
                                && !IsAnnotatedWithAnyOf(
                                    constructor,
                                    PurityOrDisposabilityKind.MustDisposeResource,
                                    PurityOrDisposabilityKind.MustDisposeResourceFalse)
                                && typeDeclaration is IStructDeclaration or IRecordDeclaration { IsStruct: true })
                            {
                                var typeDescription = GetTypeDescription(typeDeclaration);
                                var name = nameof(MustDisposeResourceAttribute).WithoutSuffix();

                                HighlightMissing(
                                    PurityOrDisposabilityKind.MustDisposeResource,
                                    $"Constructor of the disposable {typeDescription} is not annotated with [{name}] or [{name}(false)].");
                            }
                        }
                        else
                        {
                            if (IsAnnotatedWithAnyOf(
                                constructor,
                                PurityOrDisposabilityKind.MustDisposeResource,
                                PurityOrDisposabilityKind.MustDisposeResourceFalse))
                            {
                                var typeDescription = GetTypeDescription(typeDeclaration);

                                HighlightNotAllowed(
                                    PurityOrDisposabilityKind.MustDisposeResource,
                                    $"Annotation is not valid because the {typeDescription} is not disposable.");
                            }
                        }
                        break;
                    }

                    case IStructDeclaration { IsByRefLike: true, DeclaredElement: { } type }:
                    {
                        if (type.IsDisposable || type.HasDisposeMethods)
                        {
                            if (!IsAnnotatedWithAnyOf(
                                    constructor,
                                    PurityOrDisposabilityKind.MustDisposeResource,
                                    PurityOrDisposabilityKind.MustDisposeResourceFalse)
                                && IsAnnotated(type, PurityOrDisposabilityKind.MustDisposeResourceFalse)
                                && element.MustDisposeResourceAttributeSupportsStructs())
                            {
                                var name = nameof(MustDisposeResourceAttribute).WithoutSuffix();

                                HighlightMissing(
                                    PurityOrDisposabilityKind.MustDisposeResource,
                                    $"Constructor of the disposable struct (with the [{name}(false)] annotation) is not annotated with [{name}] or [{name}(false)].");
                            }

                            if (TryGetAnnotation(constructor, PurityOrDisposabilityKind.MustDisposeResource) is { } annotation)
                            {
                                if (IsAnnotated(type, PurityOrDisposabilityKind.MustDisposeResource)
                                    && element.MustDisposeResourceAttributeSupportsStructs())
                                {
                                    var name = nameof(MustDisposeResourceAttribute).WithoutSuffix();

                                    HighlightRedundant(
                                        PurityOrDisposabilityKind.MustDisposeResource,
                                        $"Annotation is redundant because the struct is already annotated with [{name}].");
                                }
                                else
                                {
                                    if (IsMustDisposeResourceTrueAttribute(annotation))
                                    {
                                        HighlightRedundantMustDisposeResourceTrueArgument(
                                            $"Passing 'true' to the [{nameof(MustDisposeResourceAttribute).WithoutSuffix()}] annotation is redundant.");
                                    }
                                }
                            }

                            if (!element.MustDisposeResourceAttributeSupportsStructs()
                                && !IsAnnotatedWithAnyOf(
                                    constructor,
                                    PurityOrDisposabilityKind.MustDisposeResource,
                                    PurityOrDisposabilityKind.MustDisposeResourceFalse))
                            {
                                var name = nameof(MustDisposeResourceAttribute).WithoutSuffix();

                                HighlightMissing(
                                    PurityOrDisposabilityKind.MustDisposeResource,
                                    $"Constructor of the disposable struct is not annotated with [{name}] or [{name}(false)].");
                            }
                        }
                        else
                        {
                            if (TryGetAnnotationAnyOf(
                                    constructor,
                                    PurityOrDisposabilityKind.MustDisposeResource,
                                    PurityOrDisposabilityKind.MustDisposeResourceFalse) is { })
                            {
                                HighlightNotAllowed(
                                    PurityOrDisposabilityKind.MustDisposeResource,
                                    "Annotation is not valid because the struct is not disposable.");
                            }
                        }
                        break;
                    }
                }
                break;
            }

            case ICSharpParametersOwnerDeclaration { DeclaredElement: IAttributesSet methodOrLocalFunction }
                and (IMethodDeclaration or ILocalFunctionDeclaration):
            {
                var (returnType, functionDescription) = methodOrLocalFunction switch
                {
                    IMethod method => (method.ReturnType, "method"),
                    ILocalFunction localFunction => (localFunction.ReturnType, "local function"),

                    _ => throw new NotSupportedException(),
                };

                if (returnType.IsDisposable() || returnType.IsTasklikeOfDisposable(element))
                {
                    if (IsAnnotated(methodOrLocalFunction, PurityOrDisposabilityKind.Pure))
                    {
                        HighlightNotAllowed(
                            PurityOrDisposabilityKind.Pure,
                            $"Annotation is not valid because the {functionDescription} return type is disposable.");
                    }

                    if (IsAnnotated(methodOrLocalFunction, PurityOrDisposabilityKind.MustUseReturnValue))
                    {
                        HighlightNotAllowed(
                            PurityOrDisposabilityKind.MustUseReturnValue,
                            $"Annotation is not valid because the {functionDescription} return type is disposable.");
                    }

                    if (TryGetAnnotation(methodOrLocalFunction, PurityOrDisposabilityKind.MustDisposeResource) is { } annotation)
                    {
                        var highlighted = false;

                        if (methodOrLocalFunction is IMethod method)
                        {
                            if (IsMethodOverridenWithOtherReturnType(method))
                            {
                                var name = nameof(MustDisposeResourceAttribute).WithoutSuffix();

                                HighlightNotAllowed(
                                    PurityOrDisposabilityKind.MustDisposeResource,
                                    $"Overriden method return type becomes disposable, the only valid annotation is [{name}(false)].");

                                highlighted = true;
                            }

                            if (!highlighted && IsAnyBaseMethodAnnotated(method, PurityOrDisposabilityKind.MustDisposeResource))
                            {
                                var name = nameof(MustDisposeResourceAttribute).WithoutSuffix();

                                HighlightRedundant(
                                    PurityOrDisposabilityKind.MustDisposeResource,
                                    $"Annotation is redundant because a base method is already annotated with [{name}].");

                                highlighted = true;
                            }
                        }

                        if (!highlighted && IsMustDisposeResourceTrueAttribute(annotation))
                        {
                            HighlightRedundantMustDisposeResourceTrueArgument(
                                $"Passing 'true' to the [{nameof(MustDisposeResourceAttribute).WithoutSuffix()}] annotation is redundant.");
                        }
                    }

                    if (!IsAnnotatedWithAnyOf(
                        methodOrLocalFunction,
                        PurityOrDisposabilityKind.MustDisposeResource,
                        PurityOrDisposabilityKind.MustDisposeResourceFalse))
                    {
                        if (methodOrLocalFunction is IMethod m2 && IsMethodOverridenWithOtherReturnType(m2))
                        {
                            var name = nameof(MustDisposeResourceAttribute).WithoutSuffix();

                            HighlightMissing(
                                PurityOrDisposabilityKind.MustDisposeResource,
                                $"Overriden method, which return type becomes disposable, is not annotated with [{name}(false)].");
                        }
                        else
                        {
                            if (methodOrLocalFunction is IMethod method
                                && !IsAnyBaseMethodAnnotated(method, PurityOrDisposabilityKind.MustDisposeResource)
                                || methodOrLocalFunction is ILocalFunction)
                            {
                                var name = nameof(MustDisposeResourceAttribute).WithoutSuffix();

                                HighlightMissing(
                                    PurityOrDisposabilityKind.MustDisposeResource,
                                    $"{functionDescription.WithFirstCharacterUpperCased()} with the disposable return type is not annotated with [{name}] or [{name}(false)].");
                            }
                        }
                    }
                }
                else
                {
                    if (IsAnnotated(methodOrLocalFunction, PurityOrDisposabilityKind.Pure))
                    {
                        if (methodOrLocalFunction is IMethod method)
                        {
                            if (IsAnyBaseMethodAnnotated(method, PurityOrDisposabilityKind.Pure))
                            {
                                var name = nameof(PureAttribute).WithoutSuffix();

                                HighlightRedundant(
                                    PurityOrDisposabilityKind.Pure,
                                    $"Annotation is redundant because a base method is already annotated with [{name}].");
                            }

                            if (IsAnyBaseMethodAnnotated(method, PurityOrDisposabilityKind.MustUseReturnValue))
                            {
                                var name = nameof(MustUseReturnValueAttribute).WithoutSuffix();

                                HighlightConflicting(
                                    PurityOrDisposabilityKind.Pure,
                                    PurityOrDisposabilityKind.MustUseReturnValue,
                                    $"Annotation is conflicting because a base method is already annotated with [{name}].");
                            }
                        }

                        if (IsAnnotated(methodOrLocalFunction, PurityOrDisposabilityKind.MustUseReturnValue))
                        {
                            var name = nameof(MustUseReturnValueAttribute).WithoutSuffix();

                            HighlightConflicting(
                                PurityOrDisposabilityKind.Pure,
                                PurityOrDisposabilityKind.MustUseReturnValue,
                                $"Annotation is conflicting because the {functionDescription} is also annotated with [{name}].");
                        }
                    }

                    if (IsAnnotated(methodOrLocalFunction, PurityOrDisposabilityKind.MustUseReturnValue))
                    {
                        if (methodOrLocalFunction is IMethod method)
                        {
                            if (IsAnyBaseMethodAnnotated(method, PurityOrDisposabilityKind.Pure))
                            {
                                var name = nameof(PureAttribute).WithoutSuffix();

                                HighlightConflicting(
                                    PurityOrDisposabilityKind.MustUseReturnValue,
                                    PurityOrDisposabilityKind.Pure,
                                    $"Annotation is conflicting because a base method is already annotated with [{name}].");
                            }

                            if (IsAnyBaseMethodAnnotated(method, PurityOrDisposabilityKind.MustUseReturnValue))
                            {
                                var name = nameof(MustUseReturnValueAttribute).WithoutSuffix();

                                HighlightRedundant(
                                    PurityOrDisposabilityKind.MustUseReturnValue,
                                    $"Annotation is redundant because a base method is already annotated with [{name}].");
                            }
                        }

                        if (IsAnnotated(methodOrLocalFunction, PurityOrDisposabilityKind.Pure))
                        {
                            var name = nameof(PureAttribute).WithoutSuffix();

                            HighlightConflicting(
                                PurityOrDisposabilityKind.MustUseReturnValue,
                                PurityOrDisposabilityKind.Pure,
                                $"Annotation is conflicting because the {functionDescription} is also annotated with [{name}].");
                        }
                    }

                    if (IsAnnotatedWithAnyOf(
                        methodOrLocalFunction,
                        PurityOrDisposabilityKind.MustDisposeResource,
                        PurityOrDisposabilityKind.MustDisposeResourceFalse))
                    {
                        // also highlights the [MustDisposeResource(false)] annotation

                        HighlightNotAllowed(
                            PurityOrDisposabilityKind.MustDisposeResource,
                            $"Annotation is not valid because the {functionDescription} return type is not disposable.");
                    }
                }
                break;
            }

            case IParameterDeclaration { DeclaredElement: { Kind: ParameterKind.REFERENCE or ParameterKind.OUTPUT } parameter }:
            {
                if (parameter.Type.IsDisposable() || parameter.Type.IsTasklikeOfDisposable(element))
                {
                    if (TryGetAnnotation(parameter, PurityOrDisposabilityKind.MustDisposeResource) is { } annotation)
                    {
                        if (IsParameterOfAnyBaseMethodAnnotated(parameter, PurityOrDisposabilityKind.MustDisposeResource))
                        {
                            var name = nameof(MustDisposeResourceAttribute).WithoutSuffix();

                            HighlightRedundant(
                                PurityOrDisposabilityKind.MustDisposeResource,
                                $"Annotation is redundant because the parameter of a base method is already annotated with [{name}].");
                        }
                        else
                        {
                            if (IsMustDisposeResourceTrueAttribute(annotation))
                            {
                                HighlightRedundantMustDisposeResourceTrueArgument(
                                    $"Passing 'true' to the [{nameof(MustDisposeResourceAttribute).WithoutSuffix()}] annotation is redundant.");
                            }
                        }
                    }

                    if (!IsAnnotatedWithAnyOf(
                            parameter,
                            PurityOrDisposabilityKind.MustDisposeResource,
                            PurityOrDisposabilityKind.MustDisposeResourceFalse)
                        && !IsParameterOfAnyBaseMethodAnnotated(parameter, PurityOrDisposabilityKind.MustDisposeResource))
                    {
                        var name = nameof(MustDisposeResourceAttribute).WithoutSuffix();

                        HighlightMissing(
                            PurityOrDisposabilityKind.MustDisposeResource,
                            $"Parameter is disposable, but not annotated with [{name}] or [{name}(false)].");
                    }
                }
                else
                {
                    if (IsAnnotatedWithAnyOf(
                        parameter,
                        PurityOrDisposabilityKind.MustDisposeResource,
                        PurityOrDisposabilityKind.MustDisposeResourceFalse))
                    {
                        HighlightNotAllowed(
                            PurityOrDisposabilityKind.MustDisposeResource,
                            "Annotation is not valid because the parameter is not disposable.");
                    }
                }

                break;
            }
        }
    }

    static void AnalyzeDisposalHandling(IHighlightingConsumer consumer, IAttributesOwnerDeclaration element)
    {
        [Pure]
        static bool IsAnnotated(IAttributesSet attributesSet)
            => attributesSet
                .GetAttributeInstances(AttributesSource.Self)
                .Any(a => a.GetAttributeShortName() == nameof(HandlesResourceDisposalAttribute));

        [Pure]
        static bool IsAnyBaseMethodAnnotated(IMethod method) => method.GetAllSuperMembers().Any(baseMethod => IsAnnotated(baseMethod.Member));

        [Pure]
        static bool IsParameterOfAnyBaseMethodAnnotated(IParameter parameter)
        {
            if (parameter.ContainingParametersOwner is IMethod method)
            {
                var parameterIndex = method.Parameters.IndexOf(parameter);

                foreach (var member in method.GetAllSuperMembers())
                {
                    var baseMethod = (IMethod)member.Member;

                    if (baseMethod.Parameters.ElementAtOrDefault(parameterIndex) is { } baseMethodParameter
                        && TypeEqualityComparer.Default.Equals(parameter.Type, baseMethodParameter.Type)
                        && parameter.Kind == baseMethodParameter.Kind
                        && IsAnnotated(baseMethodParameter))
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        [Pure]
        static bool IsAnyBasePropertyAnnotated(IProperty property)
            => property.GetAllSuperMembers().Any(baseProperty => IsAnnotated(baseProperty.Member));

        [Pure]
        IAttribute? TryGetAttributeIfAnnotationProvided()
            => element.IsAnnotationProvided(nameof(HandlesResourceDisposalAttribute))
                ? element.Attributes.FirstOrDefault(a => a.GetAttributeInstance().GetAttributeShortName() == nameof(HandlesResourceDisposalAttribute))
                : null;

        void HighlightNotAllowed(string message)
        {
            if (TryGetAttributeIfAnnotationProvided() is { } attribute)
            {
                consumer.AddHighlighting(new NotAllowedAnnotationWarning(message) { AttributesOwnerDeclaration = element, Attribute = attribute });
            }
        }

        void HighlightRedundant(string message)
        {
            if (TryGetAttributeIfAnnotationProvided() is { } attribute)
            {
                consumer.AddHighlighting(new NotAllowedAnnotationWarning(message) { AttributesOwnerDeclaration = element, Attribute = attribute });
            }
        }

        switch (element)
        {
            case IMethodDeclaration { DeclaredElement: { ContainingType: { } } method }:
                if (IsAnnotated(method))
                {
                    if (method.IsStatic)
                    {
                        HighlightNotAllowed("Annotation is not valid for static methods.");
                    }

                    if (method.GetAccessRights() is AccessRights.PRIVATE or AccessRights.NONE)
                    {
                        HighlightNotAllowed("Annotation is not valid for private methods.");
                    }

                    if (method.ContainingType is { IsDisposable: true } or IStruct { IsByRefLike: true })
                    {
                        if (method.IsDisposeMethod)
                        {
                            HighlightRedundant($"Annotation is redundant for '{nameof(IDisposable.Dispose)}' methods.");
                        }

                        if (method.IsDisposeAsyncMethod)
                        {
                            HighlightRedundant("Annotation is redundant for 'DisposeAsync' methods."); // todo: use nameof(IAsyncDisposable.DisposeAsync)
                        }

                        if (method.ContainingType is IStruct { IsByRefLike: true })
                        {
                            if (method.IsDisposeMethodByConvention)
                            {
                                HighlightRedundant("Annotation is redundant for 'Dispose' methods.");
                            }

                            if (method.IsDisposeAsyncMethodByConvention)
                            {
                                HighlightRedundant("Annotation is redundant for 'DisposeAsync' methods.");
                            }
                        }

                        if (IsAnyBaseMethodAnnotated(method))
                        {
                            var name = nameof(HandlesResourceDisposalAttribute).WithoutSuffix();

                            HighlightRedundant($"Annotation is redundant because a base method is already annotated with [{name}].");
                        }
                    }
                    else
                    {
                        HighlightNotAllowed("Annotation is not valid for methods of non-disposable types.");
                    }
                }
                break;

            case IParameterDeclaration { DeclaredElement: { } parameter }:
                if (IsAnnotated(parameter))
                {
                    if (parameter.Kind is ParameterKind.VALUE or ParameterKind.INPUT or ParameterKind.READONLY_REFERENCE or ParameterKind.REFERENCE)
                    {
                        if (parameter.Type.IsDisposable())
                        {
                            if (IsParameterOfAnyBaseMethodAnnotated(parameter))
                            {
                                var name = nameof(HandlesResourceDisposalAttribute).WithoutSuffix();

                                HighlightRedundant(
                                    $"Annotation is redundant because the parameter of a base method is already annotated with [{name}].");
                            }
                        }
                        else
                        {
                            HighlightNotAllowed("Annotation is not valid because the parameter is not disposable.");
                        }
                    }
                    else
                    {
                        HighlightNotAllowed("Annotation is not valid for non-input parameters.");
                    }
                }
                break;

            case IPropertyDeclaration { DeclaredElement: { } property }:
                if (IsAnnotated(property))
                {
                    if (property.Type.IsDisposable())
                    {
                        if (IsAnyBasePropertyAnnotated(property))
                        {
                            var name = nameof(HandlesResourceDisposalAttribute).WithoutSuffix();

                            HighlightRedundant($"Annotation is redundant because a base property is already annotated with [{name}].");
                        }
                    }
                    else
                    {
                        HighlightNotAllowed("Annotation is not valid because the property is not disposable.");
                    }
                }
                break;

            case IFieldDeclaration { DeclaredElement: { } field }:
                if (IsAnnotated(field) && !field.Type.IsDisposable())
                {
                    HighlightNotAllowed("Annotation is not valid because the field is not disposable.");
                }
                break;
        }
    }

    static void AnalyzeNumericRangeAnnotations(IHighlightingConsumer consumer, IAttributesOwnerDeclaration attributesOwnerDeclaration)
    {
        if (!attributesOwnerDeclaration.IsAnnotationProvided(nameof(NonNegativeValueAttribute))
            && !attributesOwnerDeclaration.IsAnnotationProvided(nameof(ValueRangeAttribute)))
        {
            return;
        }

        var type = null as IType;
        var numberInfo = null as NumberInfo;

        foreach (var attribute in attributesOwnerDeclaration.Attributes)
        {
            var name = attribute.GetAttributeType().GetTypeElement()?.ShortName;

            if (name is nameof(NonNegativeValueAttribute) or nameof(ValueRangeAttribute))
            {
                if (attributesOwnerDeclaration is IConstantDeclaration)
                {
                    consumer.AddHighlighting(
                        new RedundantAnnotationSuggestion("Annotation is redundant because the declared element is a constant.")
                        {
                            AttributesOwnerDeclaration = attributesOwnerDeclaration, Attribute = attribute,
                        });
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

                if (type == null)
                {
                    continue;
                }

                numberInfo ??= NumberInfo.TryGet(type);

                if (numberInfo is not { IntegralMinMaxValues: var (minValue, maxValue) })
                {
                    consumer.AddHighlighting(
                        new NotAllowedAnnotationWarning(
                            "Annotation is not valid because the type of the declared element is not an integral numeric type.")
                        {
                            AttributesOwnerDeclaration = attributesOwnerDeclaration, Attribute = attribute,
                        });
                    continue;
                }

                switch (name)
                {
                    case nameof(NonNegativeValueAttribute):
                        if (minValue >= 0)
                        {
                            consumer.AddHighlighting(
                                new RedundantAnnotationSuggestion(
                                    "Annotation is redundant because the declared element can never be negative by default.")
                                {
                                    AttributesOwnerDeclaration = attributesOwnerDeclaration, Attribute = attribute,
                                });
                        }
                        break;

                    case nameof(ValueRangeAttribute):
                        decimal from, to;

                        switch (attribute.TryGetArgumentsInDeclarationOrder())
                        {
                            case [{ Value.AsInt64Constant: { } value }]: from = to = value; break;
                            case [{ Value.AsUInt64Constant: { } value }]: from = to = value; break;

                            case [{ Value.AsInt64Constant: { } fromValue }, { Value.AsInt64Constant: { } toValue }]:
                                (from, to) = (fromValue, toValue);
                                break;

                            case [{ Value.AsUInt64Constant: { } fromValue }, { Value.AsUInt64Constant: { } toValue }]:
                                (from, to) = (fromValue, toValue);
                                break;

                            default: continue;
                        }

                        if (from > to)
                        {
                            consumer.AddHighlighting(
                                new NotAllowedAnnotationWarning("Annotation is not valid because the 'from' value is greater than the 'to' value.")
                                {
                                    AttributesOwnerDeclaration = attributesOwnerDeclaration, Attribute = attribute,
                                });
                            continue;
                        }

                        if (to < minValue || from > maxValue)
                        {
                            consumer.AddHighlighting(
                                new NotAllowedAnnotationWarning("Annotation is not valid because the declared element can never be in the range.")
                                {
                                    AttributesOwnerDeclaration = attributesOwnerDeclaration, Attribute = attribute,
                                });
                            continue;
                        }

                        if (from < minValue && to < maxValue)
                        {
                            Debug.Assert(from < to);
                            Debug.Assert(CSharpLanguage.Instance is { });

                            consumer.AddHighlighting(
                                new InvalidValueRangeBoundaryWarning(
                                    minValue < 0
                                        ? $"The 'from' value is less than the '{type.GetPresentableName(CSharpLanguage.Instance)}.{nameof(int.MinValue)}'."
                                        : "The 'from' value is negative.")
                                {
                                    PositionParameter = attribute.ConstructorArgumentExpressions[0],
                                    Boundary = ValueRangeBoundary.Lower,
                                    Type = type,
                                    TypeIsSigned = minValue < 0,
                                });
                            continue;
                        }

                        if (from > minValue && to > maxValue)
                        {
                            Debug.Assert(from < to);
                            Debug.Assert(CSharpLanguage.Instance is { });

                            consumer.AddHighlighting(
                                new InvalidValueRangeBoundaryWarning(
                                    $"The 'to' value is greater than the '{type.GetPresentableName(CSharpLanguage.Instance)}.{nameof(int.MaxValue)}'.")
                                {
                                    PositionParameter = attribute.ConstructorArgumentExpressions[1],
                                    Boundary = ValueRangeBoundary.Higher,
                                    Type = type,
                                    TypeIsSigned = minValue < 0,
                                });
                            continue;
                        }

                        if (from <= minValue && to >= maxValue)
                        {
                            consumer.AddHighlighting(
                                new RedundantAnnotationSuggestion(
                                    "Annotation is redundant because the declared element is always in the range by default.")
                                {
                                    AttributesOwnerDeclaration = attributesOwnerDeclaration, Attribute = attribute,
                                });
                        }

                        break;
                }
            }
        }
    }

    static void AnalyzeMissingAttributeUsageAnnotations(IHighlightingConsumer consumer, IAttributesOwnerDeclaration attributesOwnerDeclaration)
    {
        [Pure]
        static bool AnyBaseClassAnnotated(IClass type)
        {
            for (var baseType = type.GetBaseClassType(); baseType is { }; baseType = baseType.GetSuperTypes().FirstOrDefault())
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

        if (attributesOwnerDeclaration is IClassDeclaration { DeclaredElement: { IsAbstract: false } type }
            && type.IsAttribute()
            && !type.HasAttributeInstance(PredefinedType.ATTRIBUTE_USAGE_ATTRIBUTE_CLASS, false)
            && !AnyBaseClassAnnotated(type))
        {
            consumer.AddHighlighting(
                new MissingAnnotationWarning(
                    $"Annotate the attribute with [{nameof(AttributeUsageAttribute).WithoutSuffix()}].",
                    attributesOwnerDeclaration));
        }
    }

    static void AnalyzeMissingEditorBrowsableAnnotations(IHighlightingConsumer consumer, IAttributesOwnerDeclaration attributesOwnerDeclaration)
    {
        if (attributesOwnerDeclaration is IMethodDeclaration { DeclaredElement: { ShortName: "Deconstruct" } method }
            && method.ReturnType.IsVoid()
            && (method is { IsStatic: false, Parameters: [_, _, ..] } && method.Parameters.All(p => p.Kind == ParameterKind.OUTPUT)
                || method is { IsExtensionMethod: true, Parameters: [_, _, _, ..] }
                && method.Parameters.Skip(1).All(p => p.Kind == ParameterKind.OUTPUT))
            && !method.HasAttributeInstance(PredefinedType.EDITOR_BROWSABLE_ATTRIBUTE_CLASS, true))
        {
            consumer.AddHighlighting(
                new MissingAnnotationWarning(
                    $"Annotate the deconstruction method with [{
                        nameof(EditorBrowsableAttribute).WithoutSuffix()
                    }({
                        nameof(EditorBrowsableState)
                    }.{
                        nameof(EditorBrowsableState.Never)
                    })].",
                    attributesOwnerDeclaration));
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
                            new ConditionalAnnotationHint($"Attribute will be ignored if the '{singleCondition}' condition is not defined.")
                            {
                                AttributesOwnerDeclaration = attributesOwnerDeclaration, Attribute = attribute,
                            });
                        conditions.Clear();
                        break;

                    case [_, _, ..]:
                        consumer.AddHighlighting(
                            new ConditionalAnnotationHint(
                                $"Attribute will be ignored if none of the following conditions is defined: {
                                    string.Join(", ", from c in conditions orderby c select $"'{c}'")
                                }.") { AttributesOwnerDeclaration = attributesOwnerDeclaration, Attribute = attribute });
                        conditions.Clear();
                        break;
                }
            }
        }
    }

    readonly Lazy<NullnessProvider> nullnessProvider = codeAnnotationsCache.GetLazyProvider<NullnessProvider>();

    readonly Lazy<ContainerElementNullnessProvider> containerElementNullnessProvider =
        codeAnnotationsCache.GetLazyProvider<ContainerElementNullnessProvider>();

    [Pure]
    IEnumerable<NullabilityAttributeMark?> GetAttributeMarks(IAttributesOwnerDeclaration declaration)
    {
        var markFound = false;

        foreach (var attribute in declaration.Attributes)
        {
            if (nullnessProvider.Value.GetNullableAttributeMark(attribute.GetAttributeInstance()) is { } mark)
            {
                yield return new NullabilityAttributeMark { AnnotationNullableValue = mark, Attribute = attribute };

                markFound = true;
            }
        }

        if (!markFound)
        {
            yield return null;
        }
    }

    void AnalyzeNullabilityForAsyncMethod(IHighlightingConsumer consumer, IAttributesOwnerDeclaration attributesOwnerDeclaration)
    {
        Debug.Assert(!attributesOwnerDeclaration.IsNullableAnnotationsContextEnabled());

        foreach (var attributeMark in GetAttributeMarks(attributesOwnerDeclaration))
        {
            switch (attributeMark)
            {
                case { AnnotationNullableValue: CodeAnnotationNullableValue.NOT_NULL }
                    when attributesOwnerDeclaration.IsAnnotationProvided(nameof(NotNullAttribute)):

                    consumer.AddHighlighting(
                        new RedundantAnnotationSuggestion("Annotation is redundant because the declared element can never be null by default.")
                        {
                            AttributesOwnerDeclaration = attributesOwnerDeclaration, Attribute = attributeMark.Attribute,
                        });
                    break;

                case { AnnotationNullableValue: CodeAnnotationNullableValue.CAN_BE_NULL }
                    when attributesOwnerDeclaration.IsAnnotationProvided(nameof(CanBeNullAttribute)):

                    consumer.AddHighlighting(
                        new NotAllowedAnnotationWarning("Annotation is not valid because the declared element can never be null by default.")
                        {
                            AttributesOwnerDeclaration = attributesOwnerDeclaration, Attribute = attributeMark.Attribute,
                        });
                    break;
            }
        }
    }

    void AnalyzeNullabilityForIteratorMethod(IHighlightingConsumer consumer, IAttributesOwnerDeclaration attributesOwnerDeclaration)
    {
        Debug.Assert(!attributesOwnerDeclaration.IsNullableAnnotationsContextEnabled());

        foreach (var attributeMark in GetAttributeMarks(attributesOwnerDeclaration))
        {
            if (attributeMark is { })
            {
                if (attributeMark.AnnotationNullableValue == CodeAnnotationNullableValue.CAN_BE_NULL
                    && attributesOwnerDeclaration.IsAnnotationProvided(nameof(CanBeNullAttribute)))
                {
                    consumer.AddHighlighting(
                        new NotAllowedAnnotationWarning("Annotation is not valid because the declared element can never be null by default.")
                        {
                            AttributesOwnerDeclaration = attributesOwnerDeclaration, Attribute = attributeMark.Attribute,
                        });
                }
            }
            else
            {
                if (attributesOwnerDeclaration.IsAnnotationProvided(nameof(NotNullAttribute)))
                {
                    consumer.AddHighlighting(
                        new MissingAnnotationWarning(
                            $"Declared element can never be null by default, but is not annotated with '{nameof(NotNullAttribute)}'.",
                            attributesOwnerDeclaration));
                }
                break;
            }
        }
    }

    void AnalyzeNullabilityForOthers(
        ValueAnalysisMode valueAnalysisMode,
        IHighlightingConsumer consumer,
        IAttributesOwnerDeclaration attributesOwnerDeclaration)
    {
        Debug.Assert(!attributesOwnerDeclaration.IsNullableAnnotationsContextEnabled());

        switch (valueAnalysisMode)
        {
            case ValueAnalysisMode.OPTIMISTIC:
                foreach (var attributeMark in GetAttributeMarks(attributesOwnerDeclaration))
                {
                    if (attributeMark == null)
                    {
                        if (attributesOwnerDeclaration.IsAnnotationProvided(nameof(NotNullAttribute))
                            && attributesOwnerDeclaration.IsAnnotationProvided(nameof(CanBeNullAttribute)))
                        {
                            consumer.AddHighlighting(
                                new MissingAnnotationWarning(
                                    $"Declared element is nullable, but is not annotated with '{nameof(NotNullAttribute)}' or '{nameof(CanBeNullAttribute)}'.",
                                    attributesOwnerDeclaration));
                        }
                        break;
                    }
                }
                break;

            case ValueAnalysisMode.PESSIMISTIC:
                foreach (var attributeMark in GetAttributeMarks(attributesOwnerDeclaration))
                {
                    if (attributeMark is { AnnotationNullableValue: CodeAnnotationNullableValue.CAN_BE_NULL }
                        && attributesOwnerDeclaration.IsAnnotationProvided(nameof(CanBeNullAttribute)))
                    {
                        consumer.AddHighlighting(
                            new RedundantAnnotationSuggestion("Annotation is redundant because the declared element can be null by default.")
                            {
                                AttributesOwnerDeclaration = attributesOwnerDeclaration, Attribute = attributeMark.Attribute,
                            });
                    }
                }
                break;
        }
    }

    void AnalyzeNullabilityForOverrides(IHighlightingConsumer consumer, IAttributesOwnerDeclaration attributesOwnerDeclaration)
    {
        Debug.Assert(!attributesOwnerDeclaration.IsNullableAnnotationsContextEnabled());

        if (!attributesOwnerDeclaration.IsAnnotationProvided(nameof(NotNullAttribute))
            && !attributesOwnerDeclaration.IsAnnotationProvided(nameof(CanBeNullAttribute)))
        {
            return;
        }

        foreach (var attributeMark in GetAttributeMarks(attributesOwnerDeclaration))
        {
            if (attributeMark is { })
            {
                consumer.AddHighlighting(
                    new NotAllowedAnnotationWarning(
                        "Annotation is not allowed because the declared element overrides or implements the inherited member.")
                    {
                        AttributesOwnerDeclaration = attributesOwnerDeclaration, Attribute = attributeMark.Attribute,
                    });
            }
        }
    }

    void AnalyzeNotAllowedItemNotNull(IHighlightingConsumer consumer, IAttributesOwnerDeclaration attributesOwnerDeclaration)
    {
        Debug.Assert(!attributesOwnerDeclaration.IsNullableAnnotationsContextEnabled());

        if (!attributesOwnerDeclaration.IsAnnotationProvided(nameof(ItemNotNullAttribute)))
        {
            return;
        }

        var itemNotNullAttribute = attributesOwnerDeclaration.Attributes.FirstOrDefault(attribute
            => containerElementNullnessProvider.Value.GetContainerElementNullableAttributeMark(attribute.GetAttributeInstance())
            == CodeAnnotationNullableValue.NOT_NULL);
        if (itemNotNullAttribute is { })
        {
            if (attributesOwnerDeclaration.OverridesInheritedMember)
            {
                consumer.AddHighlighting(
                    new NotAllowedAnnotationWarning(
                        "Annotation is not allowed because the declared element overrides or implements the inherited member.")
                    {
                        AttributesOwnerDeclaration = attributesOwnerDeclaration, Attribute = itemNotNullAttribute,
                    });
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
                if (type.IsGenericIEnumerableOrDescendant() || type.IsGenericArrayOfAnyRank())
                {
                    var elementType = CollectionTypeUtil.ElementTypeByCollectionType(type, attributesOwnerDeclaration, false);
                    if (elementType is { Classify: not TypeClassification.REFERENCE_TYPE })
                    {
                        consumer.AddHighlighting(
                            new NotAllowedAnnotationWarning("Annotation is not allowed because the declared element type is not a reference type.")
                            {
                                AttributesOwnerDeclaration = attributesOwnerDeclaration, Attribute = itemNotNullAttribute,
                            });
                    }

                    return;
                }

                if (type.GetTasklikeUnderlyingType(attributesOwnerDeclaration) is { } resultType)
                {
                    if (resultType.Classify != TypeClassification.REFERENCE_TYPE)
                    {
                        consumer.AddHighlighting(
                            new NotAllowedAnnotationWarning(
                                "Annotation is not allowed because the declared task result type is not a reference type.")
                            {
                                AttributesOwnerDeclaration = attributesOwnerDeclaration, Attribute = itemNotNullAttribute,
                            });
                    }

                    return;
                }

                if (type.IsLazy())
                {
                    if (TypesUtil.GetTypeArgumentValue(type, 0) is { Classify: not TypeClassification.REFERENCE_TYPE })
                    {
                        consumer.AddHighlighting(
                            new NotAllowedAnnotationWarning("Annotation is not allowed because the declared lazy value type is not a reference type.")
                            {
                                AttributesOwnerDeclaration = attributesOwnerDeclaration, Attribute = itemNotNullAttribute,
                            });
                    }

                    return;
                }

                consumer.AddHighlighting(
                    new NotAllowedAnnotationWarning(
                        $"Annotation is not allowed because the declared element must be an {
                            nameof(IEnumerable<>)
                        }<T> (or its descendant), or a generic task-like type, or a {
                            nameof(Lazy<>)
                        }<T>.") { AttributesOwnerDeclaration = attributesOwnerDeclaration, Attribute = itemNotNullAttribute });
            }
        }
    }

    protected override void Run(IAttributesOwnerDeclaration element, ElementProblemAnalyzerData data, IHighlightingConsumer consumer)
    {
        if (element.IsNullableAnnotationsContextEnabled())
        {
            // ? annotations
            AnalyzeNullableAnnotations(consumer, element);
        }
        else
        {
            // [NotNull], [CanBeNull] annotations
            switch (TryGetNullabilityAnnotationCase(element))
            {
                case NullabilityAnnotationCase.AsyncMethod:
                    AnalyzeNullabilityForAsyncMethod(consumer, element);
                    break;

                case NullabilityAnnotationCase.IteratorMethod:
                    AnalyzeNullabilityForIteratorMethod(consumer, element);
                    break;

                case NullabilityAnnotationCase.Other:
                    AnalyzeNullabilityForOthers(data.GetValueAnalysisMode(), consumer, element);
                    break;

                case NullabilityAnnotationCase.Override:
                    AnalyzeNullabilityForOverrides(consumer, element);
                    break;
            }

            // [ItemNotNull] annotations
            AnalyzeNotAllowedItemNotNull(consumer, element);
        }

        // [SuppressMessage] annotations
        AnalyzeMissingSuppressionJustification(consumer, element);

        // [Pure], [MustUseReturnValue], [MustDisposeResource], and [MustDisposeResource(false)] annotations
        AnalyzePurityAndDisposability(consumer, element);

        // [HandlesResourceDisposal] annotations
        AnalyzeDisposalHandling(consumer, element);

        // [NonNegativeValue] and [ValueRange(...)] annotations
        AnalyzeNumericRangeAnnotations(consumer, element);

        // [AttributeUsage] annotations
        AnalyzeMissingAttributeUsageAnnotations(consumer, element);

        // [EditorBrowsable] annotations
        AnalyzeMissingEditorBrowsableAnnotations(consumer, element);

        // attributes annotated as [Conditional]
        AnalyzeConditional(consumer, element);
    }
}