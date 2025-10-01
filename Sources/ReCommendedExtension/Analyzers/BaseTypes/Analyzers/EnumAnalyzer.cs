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
    static class Parameters
    {
        public static IReadOnlyList<Parameter> String { get; } = [Parameter.String];

        public static IReadOnlyList<Parameter> ReadOnlySpanOfChar { get; } = [Parameter.ReadOnlySpanOfChar];

        public static IReadOnlyList<Parameter> Type_String { get; } = [Parameter.Type, Parameter.String];

        public static IReadOnlyList<Parameter> Type_ReadOnlySpanOfChar { get; } = [Parameter.Type, Parameter.ReadOnlySpanOfChar];

        public static IReadOnlyList<Parameter> String_outE { get; } = [Parameter.String, Parameter.T with { Kind = ParameterKind.OUTPUT }];

        public static IReadOnlyList<Parameter> ReadOnlySpanOfChar_outE { get; } =
        [
            Parameter.ReadOnlySpanOfChar, Parameter.T with { Kind = ParameterKind.OUTPUT },
        ];

        public static IReadOnlyList<Parameter> Type_String_outObject { get; } =
        [
            Parameter.Type, Parameter.String, Parameter.Object with { Kind = ParameterKind.OUTPUT },
        ];

        public static IReadOnlyList<Parameter> Type_ReadOnlySpanOfChar_outObject { get; } =
        [
            Parameter.Type, Parameter.ReadOnlySpanOfChar, Parameter.Object with { Kind = ParameterKind.OUTPUT },
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
                    Name = nameof(Enum.Parse), Parameters = Parameters.String, GenericParametersCount = 1, IsStatic = true,
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
                    Name = nameof(Enum.Parse), Parameters = Parameters.ReadOnlySpanOfChar, GenericParametersCount = 1, IsStatic = true,
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
                new MethodSignature { Name = nameof(Enum.Parse), Parameters = Parameters.Type_String, IsStatic = true },
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
                new MethodSignature { Name = nameof(Enum.Parse), Parameters = Parameters.Type_ReadOnlySpanOfChar, IsStatic = true },
                invocationExpression.PsiModule))
        {
            consumer.AddHighlighting(new RedundantArgumentHint("Passing false is redundant.", ignoreCaseArgument));
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
                    Name = nameof(Enum.TryParse), Parameters = Parameters.String_outE, GenericParametersCount = 1, IsStatic = true,
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
                    Name = nameof(Enum.TryParse), Parameters = Parameters.ReadOnlySpanOfChar_outE, GenericParametersCount = 1, IsStatic = true,
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
                new MethodSignature { Name = nameof(Enum.TryParse), Parameters = Parameters.Type_String_outObject, IsStatic = true },
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
                new MethodSignature { Name = nameof(Enum.TryParse), Parameters = Parameters.Type_ReadOnlySpanOfChar_outObject, IsStatic = true },
                invocationExpression.PsiModule))
        {
            consumer.AddHighlighting(new RedundantArgumentHint("Passing false is redundant.", ignoreCaseArgument));
        }
    }

    protected override void Run(IInvocationExpression element, ElementProblemAnalyzerData data, IHighlightingConsumer consumer)
    {
        if (element is { InvokedExpression: IReferenceExpression { Reference: var reference } }
            && reference.Resolve().DeclaredElement is IMethod
            {
                AccessibilityDomain.DomainType: AccessibilityDomain.AccessibilityDomainType.PUBLIC, IsStatic: true,
            } method
            && method.ContainingType.IsSystemEnumClass())
        {
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
                            when valueType.IsReadOnlySpanOfChar() && ignoreCaseType.IsBool():

                            AnalyzeParse_ReadOnlySpanOfChar_Boolean(consumer, element, ignoreCaseArgument);
                            break;

                        case ([], [{ Type: var enumTypeType }, { Type: var valueType }, { Type: var ignoreCaseType }], [_, _, { } ignoreCaseArgument])
                            when enumTypeType.IsSystemType() && valueType.IsString() && ignoreCaseType.IsBool():

                            AnalyzeParse_Type_String_Boolean(consumer, element, ignoreCaseArgument);
                            break;

                        case ([], [{ Type: var enumTypeType }, { Type: var valueType }, { Type: var ignoreCaseType }], [_, _, { } ignoreCaseArgument])
                            when enumTypeType.IsSystemType() && valueType.IsReadOnlySpanOfChar() && ignoreCaseType.IsBool():

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
                            ]) when valueType.IsReadOnlySpanOfChar()
                            && ignoreCaseType.IsBool()
                            && TypeEqualityComparer.Default.Equals(TypeFactory.CreateType(typeParameter), resultType):

                            AnalyzeTryParse_ReadOnlySpanOfChar_Boolean_E(consumer, element, ignoreCaseArgument);
                            break;

                        case ([], [{ Type: var enumTypeType }, { Type: var valueType }, { Type: var ignoreCaseType }, { Type: var resultType }], [
                            _, _, { } ignoreCaseArgument, _,
                        ]) when enumTypeType.IsSystemType() && valueType.IsString() && ignoreCaseType.IsBool() && resultType.IsObject():
                            AnalyzeTryParse_Type_String_Boolean_Object(consumer, element, ignoreCaseArgument);
                            break;

                        case ([], [{ Type: var enumTypeType }, { Type: var valueType }, { Type: var ignoreCaseType }, { Type: var resultType }], [
                            _, _, { } ignoreCaseArgument, _,
                        ]) when enumTypeType.IsSystemType() && valueType.IsReadOnlySpanOfChar() && ignoreCaseType.IsBool() && resultType.IsObject():
                            AnalyzeTryParse_Type_ReadOnlySpanOfChar_Boolean_Object(consumer, element, ignoreCaseArgument);
                            break;
                    }
                    break;
            }
        }
    }
}