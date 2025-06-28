using JetBrains.ReSharper.Feature.Services.Daemon;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.CSharp.Tree;
using JetBrains.ReSharper.Psi.Util;
using ReCommendedExtension.Extensions;
using ReCommendedExtension.Extensions.MethodFinding;

namespace ReCommendedExtension.Analyzers.BaseTypes.Analyzers;

/// <remarks>
/// C# language version checks are only done when a quick fix would require it.
/// </remarks>
[ElementProblemAnalyzer(typeof(IInvocationExpression), HighlightingTypes = [typeof(RedundantArgumentHint)])]
public sealed class EnumAnalyzer : ElementProblemAnalyzer<IInvocationExpression>
{
    [SuppressMessage("ReSharper", "InconsistentNaming", Justification = "Underscore character used intentionally as a separator.")]
    static class ParameterTypes
    {
        public static IReadOnlyList<ParameterType> String { get; } = [new() { ClrTypeName = PredefinedType.STRING_FQN }];

        public static IReadOnlyList<ParameterType> ReadOnlySpanOfT { get; } =
        [
            new GenericParameterType { ClrTypeName = PredefinedType.SYSTEM_READ_ONLY_SPAN_FQN },
        ];

        public static IReadOnlyList<ParameterType> Type_String { get; } =
        [
            new() { ClrTypeName = PredefinedType.TYPE_FQN }, new() { ClrTypeName = PredefinedType.STRING_FQN },
        ];

        public static IReadOnlyList<ParameterType> Type_ReadOnlySpanOfT { get; } =
        [
            new() { ClrTypeName = PredefinedType.TYPE_FQN }, new GenericParameterType { ClrTypeName = PredefinedType.SYSTEM_READ_ONLY_SPAN_FQN },
        ];

        public static IReadOnlyList<ParameterType> String_E { get; } = [new() { ClrTypeName = PredefinedType.STRING_FQN }, new()];

        public static IReadOnlyList<ParameterType> ReadOnlySpanOfT_E { get; } =
        [
            new GenericParameterType { ClrTypeName = PredefinedType.SYSTEM_READ_ONLY_SPAN_FQN }, new(),
        ];

        public static IReadOnlyList<ParameterType> Type_String_Object { get; } =
        [
            new() { ClrTypeName = PredefinedType.TYPE_FQN },
            new() { ClrTypeName = PredefinedType.STRING_FQN },
            new() { ClrTypeName = PredefinedType.OBJECT_FQN },
        ];

        public static IReadOnlyList<ParameterType> Type_ReadOnlySpanOfT_Object { get; } =
        [
            new() { ClrTypeName = PredefinedType.TYPE_FQN },
            new GenericParameterType { ClrTypeName = PredefinedType.SYSTEM_READ_ONLY_SPAN_FQN },
            new() { ClrTypeName = PredefinedType.OBJECT_FQN },
        ];
    }

    /// <remarks>
    /// <c>Enum.Parse&lt;E>(value, false)</c> → <c>Enum.Parse&lt;E>(value)</c> (.NET Core 2.0)
    /// </remarks>
    static void AnalyzeParse_String_Boolean(
        IHighlightingConsumer consumer,
        IInvocationExpression invocationExpression,
        ICSharpArgument ignoreCaseArgument)
    {
        if (ignoreCaseArgument.Value.TryGetBooleanConstant() is false
            && PredefinedType.ENUM_FQN.HasMethod(
                new MethodSignature
                {
                    Name = nameof(Enum.Parse), ParameterTypes = ParameterTypes.String, GenericParametersCount = 1, IsStatic = true,
                },
                invocationExpression.PsiModule))
        {
            consumer.AddHighlighting(new RedundantArgumentHint("Passing false is redundant.", ignoreCaseArgument));
        }
    }

    /// <remarks>
    /// <c>Enum.Parse&lt;E>(value, false)</c> → <c>Enum.Parse&lt;E>(value)</c> (.NET 6)
    /// </remarks>
    static void AnalyzeParse_ReadOnlySpanOfChar_Boolean(
        IHighlightingConsumer consumer,
        IInvocationExpression invocationExpression,
        ICSharpArgument ignoreCaseArgument)
    {
        if (ignoreCaseArgument.Value.TryGetBooleanConstant() is false
            && PredefinedType.ENUM_FQN.HasMethod(
                new MethodSignature
                {
                    Name = nameof(Enum.Parse),
                    ParameterTypes = ParameterTypes.ReadOnlySpanOfT,
                    GenericParametersCount = 1,
                    IsStatic = true,
                },
                invocationExpression.PsiModule))
        {
            consumer.AddHighlighting(new RedundantArgumentHint("Passing false is redundant.", ignoreCaseArgument));
        }
    }

    /// <remarks>
    /// <c>Enum.Parse(enumType, value, false)</c> → <c>Enum.Parse(enumType, value)</c>
    /// </remarks>
    static void AnalyzeParse_Type_String_Boolean(
        IHighlightingConsumer consumer,
        IInvocationExpression invocationExpression,
        ICSharpArgument ignoreCaseArgument)
    {
        if (ignoreCaseArgument.Value.TryGetBooleanConstant() is false
            && PredefinedType.ENUM_FQN.HasMethod(
                new MethodSignature { Name = nameof(Enum.Parse), ParameterTypes = ParameterTypes.Type_String, IsStatic = true },
                invocationExpression.PsiModule))
        {
            consumer.AddHighlighting(new RedundantArgumentHint("Passing false is redundant.", ignoreCaseArgument));
        }
    }

    /// <remarks>
    /// <c>Enum.Parse(enumType, value, false)</c> → <c>Enum.Parse(enumType, value)</c> (.NET 6)
    /// </remarks>
    static void AnalyzeParse_Type_ReadOnlySpanOfChar_Boolean(
        IHighlightingConsumer consumer,
        IInvocationExpression invocationExpression,
        ICSharpArgument ignoreCaseArgument)
    {
        if (ignoreCaseArgument.Value.TryGetBooleanConstant() is false
            && PredefinedType.ENUM_FQN.HasMethod(
                new MethodSignature { Name = nameof(Enum.Parse), ParameterTypes = ParameterTypes.Type_ReadOnlySpanOfT, IsStatic = true },
                invocationExpression.PsiModule))
        {
            consumer.AddHighlighting(new RedundantArgumentHint("Passing false is redundant.", ignoreCaseArgument));
        }
    }

    /// <remarks>
    /// <c>enum.ToString(null)</c> → <c>enum.ToString()</c><para/>
    /// <c>enum.ToString("")</c> → <c>enum.ToString()</c><para/>
    /// <c>enum.ToString("G")</c> → <c>enum.ToString()</c>
    /// </remarks>
    static void AnalyzeToString_String(IHighlightingConsumer consumer, IInvocationExpression invocationExpression, ICSharpArgument formatArgument)
    {
        [Pure]
        bool MethodExists()
            => PredefinedType.ENUM_FQN.HasMethod(
                new MethodSignature { Name = nameof(ToString), ParameterTypes = [] },
                invocationExpression.PsiModule);

        var format = formatArgument.Value.TryGetStringConstant();

        if ((formatArgument.Value.IsDefaultValue() || format == "") && MethodExists())
        {
            consumer.AddHighlighting(new RedundantArgumentHint("Passing null or an empty string is redundant.", formatArgument));
        }

        if (format is "G" or "g" && MethodExists())
        {
            consumer.AddHighlighting(new RedundantArgumentHint($"Passing \"{format}\" is redundant.", formatArgument));
        }
    }

    /// <remarks>
    /// <c>Enum.TryParse&lt;E>(value, false, out result)</c> → <c>Enum.TryParse&lt;E>(value, out result)</c>
    /// </remarks>
    static void AnalyzeTryParse_String_Boolean_E(
        IHighlightingConsumer consumer,
        IInvocationExpression invocationExpression,
        ICSharpArgument ignoreCaseArgument)
    {
        if (ignoreCaseArgument.Value.TryGetBooleanConstant() is false
            && PredefinedType.ENUM_FQN.HasMethod(
                new MethodSignature
                {
                    Name = nameof(Enum.TryParse), ParameterTypes = ParameterTypes.String_E, GenericParametersCount = 1, IsStatic = true,
                },
                invocationExpression.PsiModule))
        {
            consumer.AddHighlighting(new RedundantArgumentHint("Passing false is redundant.", ignoreCaseArgument));
        }
    }

    /// <remarks>
    /// <c>Enum.TryParse&lt;E>(value, false, out result)</c> → <c>Enum.TryParse&lt;E>(value, out result)</c> (.NET 6)
    /// </remarks>
    static void AnalyzeTryParse_ReadOnlySpanOfChar_Boolean_E(
        IHighlightingConsumer consumer,
        IInvocationExpression invocationExpression,
        ICSharpArgument ignoreCaseArgument)
    {
        if (ignoreCaseArgument.Value.TryGetBooleanConstant() is false
            && PredefinedType.ENUM_FQN.HasMethod(
                new MethodSignature
                {
                    Name = nameof(Enum.TryParse), ParameterTypes = ParameterTypes.ReadOnlySpanOfT_E, GenericParametersCount = 1, IsStatic = true,
                },
                invocationExpression.PsiModule))
        {
            consumer.AddHighlighting(new RedundantArgumentHint("Passing false is redundant.", ignoreCaseArgument));
        }
    }

    /// <remarks>
    /// <c>Enum.TryParse(enumType, value, false, out result)</c> → <c>Enum.TryParse(enumType, value, out result)</c> (.NET Core 2.0)
    /// </remarks>
    static void AnalyzeTryParse_Type_String_Boolean_Object(
        IHighlightingConsumer consumer,
        IInvocationExpression invocationExpression,
        ICSharpArgument ignoreCaseArgument)
    {
        if (ignoreCaseArgument.Value.TryGetBooleanConstant() is false
            && PredefinedType.ENUM_FQN.HasMethod(
                new MethodSignature { Name = nameof(Enum.TryParse), ParameterTypes = ParameterTypes.Type_String_Object, IsStatic = true },
                invocationExpression.PsiModule))
        {
            consumer.AddHighlighting(new RedundantArgumentHint("Passing false is redundant.", ignoreCaseArgument));
        }
    }

    /// <remarks>
    /// <c>Enum.TryParse(enumType, value, false, out result)</c> → <c>Enum.TryParse(enumType, value, out result)</c> (.NET 6)
    /// </remarks>
    static void AnalyzeTryParse_Type_ReadOnlySpanOfChar_Boolean_Object(
        IHighlightingConsumer consumer,
        IInvocationExpression invocationExpression,
        ICSharpArgument ignoreCaseArgument)
    {
        if (ignoreCaseArgument.Value.TryGetBooleanConstant() is false
            && PredefinedType.ENUM_FQN.HasMethod(
                new MethodSignature { Name = nameof(Enum.TryParse), ParameterTypes = ParameterTypes.Type_ReadOnlySpanOfT_Object, IsStatic = true },
                invocationExpression.PsiModule))
        {
            consumer.AddHighlighting(new RedundantArgumentHint("Passing false is redundant.", ignoreCaseArgument));
        }
    }

    protected override void Run(IInvocationExpression element, ElementProblemAnalyzerData data, IHighlightingConsumer consumer)
    {
        if (element is { InvokedExpression: IReferenceExpression { Reference: var reference } invokedExpression }
            && reference.Resolve().DeclaredElement is IMethod
            {
                AccessibilityDomain.DomainType: AccessibilityDomain.AccessibilityDomainType.PUBLIC,
            } method
            && method.ContainingType.IsSystemEnumClass())
        {
            switch (invokedExpression, method)
            {
                case ({ QualifierExpression: { } }, { IsStatic: false, TypeParameters: [] }):
                    switch (method.ShortName)
                    {
                        case nameof(Enum.ToString):
                            switch (method.Parameters, element.TryGetArgumentsInDeclarationOrder())
                            {
                                case ([{ Type: var formatType }], [{ } formatArgument]) when formatType.IsString():
                                    AnalyzeToString_String(consumer, element, formatArgument);
                                    break;
                            }
                            break;
                    }
                    break;

                case (_, { IsStatic: true }):
                    switch (method.ShortName)
                    {
                        case nameof(Enum.Parse):
                            switch (method.TypeParameters, method.Parameters, element.TryGetArgumentsInDeclarationOrder())
                            {
                                case ([_], [{ Type: var valueType }, { Type: var ignoreCaseType }], [_, { } ignoreCaseArgument])
                                    when valueType.IsString() && ignoreCaseType.IsBool():

                                    AnalyzeParse_String_Boolean(consumer, element, ignoreCaseArgument);
                                    break;

                                case ([_], [{ Type: var valueType }, { Type: var ignoreCaseType }], [_, { } ignoreCaseArgument])
                                    when valueType.IsReadOnlySpan(out var spanTypeArgument) && spanTypeArgument.IsChar() && ignoreCaseType.IsBool():

                                    AnalyzeParse_ReadOnlySpanOfChar_Boolean(consumer, element, ignoreCaseArgument);
                                    break;

                                case ([], [{ Type: var enumTypeType }, { Type: var valueType }, { Type: var ignoreCaseType }], [
                                    _, _, { } ignoreCaseArgument,
                                ]) when enumTypeType.IsSystemType() && valueType.IsString() && ignoreCaseType.IsBool():

                                    AnalyzeParse_Type_String_Boolean(consumer, element, ignoreCaseArgument);
                                    break;

                                case ([], [{ Type: var enumTypeType }, { Type: var valueType }, { Type: var ignoreCaseType }], [
                                        _, _, { } ignoreCaseArgument,
                                    ]) when enumTypeType.IsSystemType()
                                    && valueType.IsReadOnlySpan(out var spanTypeArgument)
                                    && spanTypeArgument.IsChar()
                                    && ignoreCaseType.IsBool():

                                    AnalyzeParse_Type_ReadOnlySpanOfChar_Boolean(consumer, element, ignoreCaseArgument);
                                    break;
                            }
                            break;

                        case nameof(Enum.TryParse):
                            switch (method.TypeParameters, method.Parameters, element.TryGetArgumentsInDeclarationOrder())
                            {
                                case ([var typeParameter], [{ Type: var valueType }, { Type: var ignoreCaseType }, { Type: var resultType }], [
                                        _, { } ignoreCaseArgument, _,
                                    ]) when valueType.IsString()
                                    && ignoreCaseType.IsBool()
                                    && TypeEqualityComparer.Default.Equals(TypeFactory.CreateType(typeParameter), resultType):

                                    AnalyzeTryParse_String_Boolean_E(consumer, element, ignoreCaseArgument);
                                    break;

                                case ([var typeParameter], [{ Type: var valueType }, { Type: var ignoreCaseType }, { Type: var resultType }], [
                                        _, { } ignoreCaseArgument, _,
                                    ]) when valueType.IsReadOnlySpan(out var spanTypeArgument)
                                    && spanTypeArgument.IsChar()
                                    && ignoreCaseType.IsBool()
                                    && TypeEqualityComparer.Default.Equals(TypeFactory.CreateType(typeParameter), resultType):

                                    AnalyzeTryParse_ReadOnlySpanOfChar_Boolean_E(consumer, element, ignoreCaseArgument);
                                    break;

                                case ([], [
                                        { Type: var enumTypeType }, { Type: var valueType }, { Type: var ignoreCaseType }, { Type: var resultType },
                                    ], [_, _, { } ignoreCaseArgument, _]) when enumTypeType.IsSystemType()
                                    && valueType.IsString()
                                    && ignoreCaseType.IsBool()
                                    && resultType.IsObject():

                                    AnalyzeTryParse_Type_String_Boolean_Object(consumer, element, ignoreCaseArgument);
                                    break;

                                case ([], [
                                        { Type: var enumTypeType }, { Type: var valueType }, { Type: var ignoreCaseType }, { Type: var resultType },
                                    ], [_, _, { } ignoreCaseArgument, _]) when enumTypeType.IsSystemType()
                                    && valueType.IsReadOnlySpan(out var spanTypeArgument)
                                    && spanTypeArgument.IsChar()
                                    && ignoreCaseType.IsBool()
                                    && resultType.IsObject():

                                    AnalyzeTryParse_Type_ReadOnlySpanOfChar_Boolean_Object(consumer, element, ignoreCaseArgument);
                                    break;
                            }
                            break;
                    }
                    break;
            }
        }
    }
}