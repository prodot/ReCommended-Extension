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

namespace ReCommendedExtension.Analyzers.Annotation;

[ElementProblemAnalyzer(
    typeof(IAttributesOwnerDeclaration),
    HighlightingTypes =
    [
        typeof(RedundantAnnotationSuggestion), typeof(NotAllowedAnnotationWarning), typeof(MissingAnnotationWarning),
        typeof(MissingSuppressionJustificationWarning), typeof(ConflictingAnnotationWarning), typeof(ConditionalAnnotationHint),
        typeof(InvalidValueRangeBoundaryWarning), typeof(RedundantAnnotationArgumentSuggestion),
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

        if (declaration.OverridesInheritedMember())
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
                .FirstOrDefault(
                    a => kind1 switch
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
                consumer.AddHighlighting(new MissingAnnotationWarning(element, message));
            }
        }

        void HighlightRedundant(PurityOrDisposabilityKind kind, string message)
        {
            if (TryGetAttributeNameIfAnnotationProvided(kind) is { } attributeName
                && element.Attributes.FirstOrDefault(a => a.GetAttributeInstance().GetAttributeShortName() == attributeName) is { } attribute)
            {
                consumer.AddHighlighting(new RedundantAnnotationSuggestion(element, attribute, message));
            }
        }

        void HighlightNotAllowed(PurityOrDisposabilityKind kind, string message)
        {
            if (TryGetAttributeNameIfAnnotationProvided(kind) is { } attributeName
                && element.Attributes.FirstOrDefault(a => a.GetAttributeInstance().GetAttributeShortName() == attributeName) is { } attribute)
            {
                consumer.AddHighlighting(new NotAllowedAnnotationWarning(element, attribute, message));
            }
        }

        void HighlightConflicting(PurityOrDisposabilityKind kind, PurityOrDisposabilityKind otherKind, string message)
        {
            Debug.Assert(kind != otherKind);

            if (TryGetAttributeNameIfAnnotationProvided(kind) is { } attributeName
                && TryGetAttributeNameIfAnnotationProvided(otherKind) is { }
                && element.Attributes.FirstOrDefault(a => a.GetAttributeInstance().GetAttributeShortName() == attributeName) is { } attribute)
            {
                consumer.AddHighlighting(new ConflictingAnnotationWarning(element, attribute, message));
            }
        }

        void HighlightRedundantMustDisposeResourceTrueArgument(string message)
        {
            if (TryGetAttributeNameIfAnnotationProvided(PurityOrDisposabilityKind.MustDisposeResource) is { } attributeName
                && element.Attributes.FirstOrDefault(a => a.GetAttributeInstance().GetAttributeShortName() == attributeName) is
                {
                    Arguments: [var argument],
                } attribute)
            {
                consumer.AddHighlighting(new RedundantAnnotationArgumentSuggestion(element, attribute, argument, message));
            }
        }

        switch (element)
        {
            case IClassLikeDeclaration { DeclaredElement: { } type } and (IClassDeclaration or IRecordDeclaration { IsStruct: false }):
            {
                var typeDescription = element switch
                {
                    IClassDeclaration => "class",
                    IRecordDeclaration => "record",

                    _ => throw new NotSupportedException(),
                };

                if (type.IsDisposable(element.GetPsiModule()))
                {
                    if (!IsAnnotatedWithAnyOf(type, PurityOrDisposabilityKind.MustDisposeResource, PurityOrDisposabilityKind.MustDisposeResourceFalse)
                        && !IsAnyBaseTypeAnnotated(type, PurityOrDisposabilityKind.MustDisposeResource))
                    {
                        var name = nameof(MustDisposeResourceAttribute).WithoutSuffix();

                        HighlightMissing(
                            PurityOrDisposabilityKind.MustDisposeResource,
                            $"{typeDescription.WithFirstCharacterUpperCased()} is disposable, but not annotated with [{name}] or [{name}(false)].");
                    }

                    if (TryGetAnnotation(type, PurityOrDisposabilityKind.MustDisposeResource) is { } annotation)
                    {
                        if (IsAnyBaseTypeAnnotated(type, PurityOrDisposabilityKind.MustDisposeResource))
                        {
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
                        HighlightNotAllowed(
                            PurityOrDisposabilityKind.MustDisposeResource,
                            $"Annotation is not valid because the {typeDescription} is not disposable.");
                    }
                }
                break;
            }

            case IClassLikeDeclaration { PrimaryConstructorDeclaration: not { }, ConstructorDeclarations: [], DeclaredElement: { } type }
                and (IStructDeclaration { IsByRefLike: false } or IRecordDeclaration { IsStruct: true }):
            {
                if (type.IsDisposable(element.GetPsiModule()))
                {
                    var typeDescription = element switch
                    {
                        IStructDeclaration => "Struct",
                        IRecordDeclaration => "Record",

                        _ => throw new NotSupportedException(),
                    };

                    var name = nameof(MustDisposeResourceAttribute).WithoutSuffix();

                    HighlightMissing(
                        PurityOrDisposabilityKind.MustDisposeResource,
                        $"{typeDescription} is disposable, but it doesn't have any constructor to be annotated with [{name}] or [{name}(false)].");
                }
                break;
            }

            case IStructDeclaration
            {
                IsByRefLike: true, PrimaryConstructorDeclaration: not { }, ConstructorDeclarations: [], DeclaredElement: { } type,
            }:
            {
                if (type.HasDisposeMethods())
                {
                    var name = nameof(MustDisposeResourceAttribute).WithoutSuffix();

                    HighlightMissing(
                        PurityOrDisposabilityKind.MustDisposeResource,
                        $"Struct is disposable, but it doesn't have any constructor to be annotated with [{name}] or [{name}(false)].");
                }
                break;
            }

            case ICSharpTypeMemberDeclaration { DeclaredElement: { } constructor } constructorDeclaration
                and (IConstructorDeclaration or IPrimaryConstructorDeclaration):
            {
                var typeDeclaration = constructorDeclaration.GetContainingTypeDeclaration();

                var typeDescription = typeDeclaration switch
                {
                    IClassDeclaration => "class",
                    IStructDeclaration => "struct",
                    IRecordDeclaration => "record",

                    _ => throw new NotSupportedException(),
                };

                switch (typeDeclaration)
                {
                    case IClassLikeDeclaration { DeclaredElement: { } type }
                        and (IClassDeclaration or IStructDeclaration { IsByRefLike: false } or IRecordDeclaration)
                        when type.IsDisposable(element.GetPsiModule()):
                    {
                        if (!IsAnnotatedWithAnyOf(
                                constructor,
                                PurityOrDisposabilityKind.MustDisposeResource,
                                PurityOrDisposabilityKind.MustDisposeResourceFalse)
                            && IsAnnotated(type, PurityOrDisposabilityKind.MustDisposeResourceFalse)
                            && typeDeclaration is IClassDeclaration or IRecordDeclaration { IsStruct: false })
                        {
                            var name = nameof(MustDisposeResourceAttribute).WithoutSuffix();

                            HighlightMissing(
                                PurityOrDisposabilityKind.MustDisposeResource,
                                $"Constructor of the disposable {typeDescription} (with the [{name}(false)] annotation) is not annotated with [{name}] or [{name}(false)].");
                        }

                        if (TryGetAnnotation(constructor, PurityOrDisposabilityKind.MustDisposeResource) is { } annotation)
                        {
                            if ((IsAnnotated(type, PurityOrDisposabilityKind.MustDisposeResource)
                                    || IsAnyBaseTypeAnnotated(type, PurityOrDisposabilityKind.MustDisposeResource))
                                && typeDeclaration is IClassDeclaration or IRecordDeclaration { IsStruct: false })
                            {
                                var name = nameof(MustDisposeResourceAttribute).WithoutSuffix();

                                HighlightRedundant(
                                    PurityOrDisposabilityKind.MustDisposeResource,
                                    $"Annotation is redundant because the {typeDescription} or a base {typeDescription} is already annotated with [{name}].");
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
                                constructor,
                                PurityOrDisposabilityKind.MustDisposeResource,
                                PurityOrDisposabilityKind.MustDisposeResourceFalse)
                            && typeDeclaration is IStructDeclaration or IRecordDeclaration { IsStruct: true })
                        {
                            var name = nameof(MustDisposeResourceAttribute).WithoutSuffix();

                            HighlightMissing(
                                PurityOrDisposabilityKind.MustDisposeResource,
                                $"Constructor of the disposable {typeDescription} is not annotated with [{name}] or [{name}(false)].");
                        }
                        break;
                    }

                    case IClassLikeDeclaration { DeclaredElement: { } }
                        and (IClassDeclaration or IStructDeclaration { IsByRefLike: false } or IRecordDeclaration):
                    {
                        if (IsAnnotatedWithAnyOf(
                            constructor,
                            PurityOrDisposabilityKind.MustDisposeResource,
                            PurityOrDisposabilityKind.MustDisposeResourceFalse))
                        {
                            HighlightNotAllowed(
                                PurityOrDisposabilityKind.MustDisposeResource,
                                $"Annotation is not valid because the {typeDescription} is not disposable.");
                        }
                        break;
                    }

                    case IStructDeclaration { IsByRefLike: true, DeclaredElement: { } type }:
                    {
                        if (!IsAnnotatedWithAnyOf(
                                constructor,
                                PurityOrDisposabilityKind.MustDisposeResource,
                                PurityOrDisposabilityKind.MustDisposeResourceFalse)
                            && type.HasDisposeMethods())
                        {
                            var name = nameof(MustDisposeResourceAttribute).WithoutSuffix();

                            HighlightMissing(
                                PurityOrDisposabilityKind.MustDisposeResource,
                                $"Constructor of the disposable struct is not annotated with [{name}] or [{name}(false)].");
                        }

                        if (TryGetAnnotationAnyOf(
                                constructor,
                                PurityOrDisposabilityKind.MustDisposeResource,
                                PurityOrDisposabilityKind.MustDisposeResourceFalse) is { } annotation)
                        {
                            if (type.HasDisposeMethods())
                            {
                                if (IsMustDisposeResourceTrueAttribute(annotation))
                                {
                                    HighlightRedundantMustDisposeResourceTrueArgument(
                                        $"Passing 'true' to the [{nameof(MustDisposeResourceAttribute).WithoutSuffix()}] annotation is redundant.");
                                }
                            }
                            else
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

                if (returnType.IsDisposable(element))
                {
                    if (IsAnnotated(methodOrLocalFunction, PurityOrDisposabilityKind.Pure))
                    {
                        HighlightNotAllowed(
                            PurityOrDisposabilityKind.Pure,
                            $"Annotation is not valid because the {functionDescription} return type is disposable.");
                    }

                    if (IsAnnotated(methodOrLocalFunction, PurityOrDisposabilityKind.MustUseReturnValue))
                    {
#if MUST_DISPOSE_RESOURCE_NO_TASK_LIKE
                        if (returnType.IsTasklike(element))
                        {
                            if (methodOrLocalFunction is IMethod method
                                && IsAnyBaseMethodAnnotated(method, PurityOrDisposabilityKind.MustUseReturnValue))
                            {
                                var name = nameof(MustUseReturnValueAttribute).WithoutSuffix();

                                HighlightRedundant(
                                    PurityOrDisposabilityKind.MustUseReturnValue,
                                    $"Annotation is redundant because a base method is already annotated with [{name}].");
                            }
                        }
                        else
                        {
                            HighlightNotAllowed(
                                PurityOrDisposabilityKind.MustUseReturnValue,
                                $"Annotation is not valid because the {functionDescription} return type is disposable.");
                        }
#else
                        HighlightNotAllowed(
                            PurityOrDisposabilityKind.MustUseReturnValue,
                            $"Annotation is not valid because the {functionDescription} return type is disposable.");
#endif
                    }

                    if (TryGetAnnotation(methodOrLocalFunction, PurityOrDisposabilityKind.MustDisposeResource) is { } annotation)
                    {
#if MUST_DISPOSE_RESOURCE_NO_TASK_LIKE
                        if (returnType.IsTasklike(element))
                        {
                            HighlightNotAllowed(
                                PurityOrDisposabilityKind.MustDisposeResource,
                                $"Annotation is not valid because task-like disposable {functionDescription} return types are not yet supported.");
                        }
                        else
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
#else
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
#endif
                    }

#if MUST_DISPOSE_RESOURCE_NO_TASK_LIKE
                    if (IsAnnotated(methodOrLocalFunction, PurityOrDisposabilityKind.MustDisposeResourceFalse) && returnType.IsTasklike(element))
                    {
                        HighlightNotAllowed(
                            PurityOrDisposabilityKind.MustDisposeResourceFalse,
                            $"Annotation is not valid because task-like disposable {functionDescription} return types are not yet supported.");
                    }
#endif

                    if (!IsAnnotatedWithAnyOf(
                        methodOrLocalFunction,
                        PurityOrDisposabilityKind.MustDisposeResource,
                        PurityOrDisposabilityKind.MustDisposeResourceFalse))
                    {
#if MUST_DISPOSE_RESOURCE_NO_TASK_LIKE
                        if (!returnType.IsTasklike(element))
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
#else
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
#endif
                    }

#if MUST_DISPOSE_RESOURCE_NO_TASK_LIKE
                    if (!IsAnnotated(methodOrLocalFunction, PurityOrDisposabilityKind.MustUseReturnValue)
                        && returnType.IsTasklike(element)
                        && (methodOrLocalFunction is IMethod m && !IsAnyBaseMethodAnnotated(m, PurityOrDisposabilityKind.MustUseReturnValue)
                            || methodOrLocalFunction is ILocalFunction))
                    {
                        var name = nameof(MustUseReturnValueAttribute).WithoutSuffix();
                        var name2 = nameof(MustDisposeResourceAttribute).WithoutSuffix();

                        HighlightMissing(
                            PurityOrDisposabilityKind.MustUseReturnValue,
                            $"{functionDescription.WithFirstCharacterUpperCased()} with the task-like disposable return type is not annotated with [{name}] (annotating with [{name2}] is not yet supported).");
                    }
#endif
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

            case IParameterDeclaration { DeclaredElement: { Kind: ParameterKind.READONLY_REFERENCE } parameter }:
            {
                if (IsAnnotatedWithAnyOf(
                    parameter,
                    PurityOrDisposabilityKind.MustDisposeResource,
                    PurityOrDisposabilityKind.MustDisposeResourceFalse))
                {
                    // this is a workaround for the missing warning for the [MustDisposeResource] annotation on 'ref readonly' parameters
                    HighlightNotAllowed(PurityOrDisposabilityKind.MustDisposeResource, "Annotation is not valid for input parameters.");
                }

                break;
            }

            case IParameterDeclaration { DeclaredElement: { Kind: ParameterKind.REFERENCE or ParameterKind.OUTPUT } parameter }:
            {
                if (parameter.Type.IsDisposable(element))
                {
                    if (TryGetAnnotation(parameter, PurityOrDisposabilityKind.MustDisposeResource) is { } annotation)
                    {
#if MUST_DISPOSE_RESOURCE_NO_TASK_LIKE
                        if (parameter.Type.IsTasklike(element))
                        {
                            HighlightNotAllowed(
                                PurityOrDisposabilityKind.MustDisposeResource,
                                "Annotation is not valid because task-like disposable parameters are not yet supported.");
                        }
                        else
                        {
                            if (IsMustDisposeResourceTrueAttribute(annotation))
                            {
                                HighlightRedundantMustDisposeResourceTrueArgument(
                                    $"Passing 'true' to the [{nameof(MustDisposeResourceAttribute).WithoutSuffix()}] annotation is redundant.");
                            }
                        }
#else
                        if (IsParameterOfAnyBaseMethodAnnotated(parameter, PurityOrDisposabilityKind.MustDisposeResource))
                        {
                            var name = nameof(IsMustDisposeResourceAttribute).WithoutSuffix();

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
#endif
                    }

#if MUST_DISPOSE_RESOURCE_NO_TASK_LIKE
                    if (IsAnnotated(parameter, PurityOrDisposabilityKind.MustDisposeResourceFalse) && parameter.Type.IsTasklike(element))
                    {
                        HighlightNotAllowed(
                            PurityOrDisposabilityKind.MustDisposeResourceFalse,
                            "Annotation is not valid because task-like disposable parameters are not yet supported.");
                    }
#endif
                    if (!IsAnnotatedWithAnyOf(
                        parameter,
                        PurityOrDisposabilityKind.MustDisposeResource,
                        PurityOrDisposabilityKind.MustDisposeResourceFalse))
                    {
#if MUST_DISPOSE_RESOURCE_NO_TASK_LIKE
                        if (!IsParameterOfAnyBaseMethodAnnotated(parameter, PurityOrDisposabilityKind.MustDisposeResource)
                            && !parameter.Type.IsTasklike(element))
                        {
                            var name = nameof(IsMustDisposeResourceAttribute).WithoutSuffix();

                            HighlightMissing(
                                PurityOrDisposabilityKind.MustDisposeResource,
                                $"Parameter is disposable, but not annotated with [{name}] or [{name}(false)].");
                        }
#else
                        if (!IsParameterOfAnyBaseMethodAnnotated(parameter, PurityOrDisposabilityKind.MustDisposeResource))
                        {
                            var name = nameof(IsMustDisposeResourceAttribute).WithoutSuffix();

                            HighlightMissing(
                                PurityOrDisposabilityKind.MustDisposeResource,
                                $"Parameter is disposable, but not annotated with [{name}] or [{name}(false)].");
                        }
#endif
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

    static void AnalyzeNumericRangeAnnotations(IHighlightingConsumer consumer, IAttributesOwnerDeclaration attributesOwnerDeclaration)
    {
        if (!attributesOwnerDeclaration.IsAnnotationProvided(nameof(NonNegativeValueAttribute))
            && !attributesOwnerDeclaration.IsAnnotationProvided(nameof(ValueRangeAttribute)))
        {
            return;
        }

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
                new MissingAnnotationWarning(attributesOwnerDeclaration, $"Annotate the attribute with [{nameof(AttributeUsageAttribute).WithoutSuffix()}]."));
        }
    }

    static void AnalyzeMissingNotNullWhenAnnotations(IHighlightingConsumer consumer, IAttributesOwnerDeclaration attributesOwnerDeclaration)
    {
        if (attributesOwnerDeclaration.IsNullableAnnotationsContextEnabled()
            && attributesOwnerDeclaration is IParameterDeclaration { DeclaredElement.Kind: ParameterKind.VALUE } parameterDeclaration
            && !parameterDeclaration.IsInsideClosure()
            && parameterDeclaration.GetContainingTypeMemberDeclarationIgnoringClosures() is IMethodDeclaration
            {
                DeclaredName: nameof(IEquatable<int>.Equals), TypeParameterDeclarations: [], ParameterDeclarations: [var p], DeclaredElement: { },
            } methodDeclaration
            && p == parameterDeclaration
            && methodDeclaration.DeclaredElement.ReturnType.IsBool()
            && methodDeclaration.GetContainingTypeDeclaration() is IClassDeclaration { DeclaredElement: { } } classDeclaration
            && parameterDeclaration.Type.Equals(TypeFactory.CreateType(classDeclaration.DeclaredElement))
            && attributesOwnerDeclaration.Attributes.All(a => !a.GetAttributeType().IsClrType(ClrTypeNames.NotNullWhenAttribute))
            && ClrTypeNames.NotNullWhenAttribute.TryGetTypeElement(attributesOwnerDeclaration.GetPsiModule()) is { })
        {
            // we do not verify if the type implements the IEquatable<T> interface: in any case, the parameter should not be null when such a method
            // returns true

            consumer.AddHighlighting(
                new MissingAnnotationWarning(attributesOwnerDeclaration, $"Annotate the parameter with [{nameof(NotNullWhenAttribute).WithoutSuffix()}(true)]."));
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
    IEnumerable<NullabilityAttributeMark?> GetAttributeMarks(IAttributesOwnerDeclaration declaration)
    {
        var markFound = false;

        foreach (var attribute in declaration.Attributes)
        {
            if (nullnessProvider.GetNullableAttributeMark(attribute.GetAttributeInstance()) is { } mark)
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
                        new RedundantAnnotationSuggestion(
                            attributesOwnerDeclaration,
                            attributeMark.Attribute,
                            "Annotation is redundant because the declared element can never be null by default."));
                    break;

                case { AnnotationNullableValue: CodeAnnotationNullableValue.CAN_BE_NULL }
                    when attributesOwnerDeclaration.IsAnnotationProvided(nameof(CanBeNullAttribute)):

                    consumer.AddHighlighting(
                        new NotAllowedAnnotationWarning(
                            attributesOwnerDeclaration,
                            attributeMark.Attribute,
                            "Annotation is not valid because the declared element can never be null by default."));
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
                        new NotAllowedAnnotationWarning(
                            attributesOwnerDeclaration,
                            attributeMark.Attribute,
                            "Annotation is not valid because the declared element can never be null by default."));
                }
            }
            else
            {
                if (attributesOwnerDeclaration.IsAnnotationProvided(nameof(NotNullAttribute)))
                {
                    consumer.AddHighlighting(
                        new MissingAnnotationWarning(
                            attributesOwnerDeclaration,
                            $"Declared element can never be null by default, but is not annotated with '{nameof(NotNullAttribute)}'."));
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
                    if (attributeMark is not { })
                    {
                        if (attributesOwnerDeclaration.IsAnnotationProvided(nameof(NotNullAttribute))
                            && attributesOwnerDeclaration.IsAnnotationProvided(nameof(CanBeNullAttribute)))
                        {
                            consumer.AddHighlighting(
                                new MissingAnnotationWarning(
                                    attributesOwnerDeclaration,
                                    $"Declared element is nullable, but is not annotated with '{nameof(NotNullAttribute)}' or '{nameof(CanBeNullAttribute)}'."));
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
                            new RedundantAnnotationSuggestion(
                                attributesOwnerDeclaration,
                                attributeMark.Attribute,
                                "Annotation is redundant because the declared element can be null by default."));
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
                        attributesOwnerDeclaration,
                        attributeMark.Attribute,
                        "Annotation is not allowed because the declared element overrides or implements the inherited member."));
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