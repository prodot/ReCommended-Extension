using JetBrains.ReSharper.Feature.Services.Daemon;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.CSharp.Tree;
using ReCommendedExtension.Extensions;
using ReCommendedExtension.Extensions.MethodFinding;

namespace ReCommendedExtension.Analyzers.BaseTypes.Analyzers;

/// <remarks>
/// C# language version checks are only done when a quick fix would require it.
/// </remarks>
[ElementProblemAnalyzer(
    typeof(IInvocationExpression),
    HighlightingTypes = [typeof(UseBinaryOperatorSuggestion), typeof(UseExpressionResultSuggestion), typeof(RedundantArgumentHint)])]
public sealed class GuidAnalyzer : ElementProblemAnalyzer<IInvocationExpression>
{
    [SuppressMessage("ReSharper", "InconsistentNaming", Justification = "Underscore character used intentionally as a separator.")]
    static class ParameterTypes
    {
        public static IReadOnlyList<ParameterType> String { get; } = [new() { ClrTypeName = PredefinedType.STRING_FQN }];

        public static IReadOnlyList<ParameterType> ReadOnlySpanOfT { get; } =
        [
            new GenericParameterType { ClrTypeName = PredefinedType.SYSTEM_READ_ONLY_SPAN_FQN },
        ];

        public static IReadOnlyList<ParameterType> String_Guid { get; } =
        [
            new() { ClrTypeName = PredefinedType.STRING_FQN }, new() { ClrTypeName = PredefinedType.GUID_FQN },
        ];

        public static IReadOnlyList<ParameterType> ReadOnlySpanOfT_Guid { get; } =
        [
            new GenericParameterType { ClrTypeName = PredefinedType.SYSTEM_READ_ONLY_SPAN_FQN }, new() { ClrTypeName = PredefinedType.GUID_FQN },
        ];
    }

    /// <remarks>
    /// <c>guid.Equals(g)</c> → <c>guid == g</c>
    /// </remarks>
    static void AnalyzeEquals_Guid(
        IHighlightingConsumer consumer,
        IInvocationExpression invocationExpression,
        IReferenceExpression invokedExpression,
        ICSharpArgument gArgument)
    {
        Debug.Assert(invokedExpression.QualifierExpression is { });

        if (!invocationExpression.IsUsedAsStatement() && gArgument.Value is { })
        {
            consumer.AddHighlighting(
                new UseBinaryOperatorSuggestion(
                    "Use the '==' operator.",
                    invocationExpression,
                    "==",
                    invokedExpression.QualifierExpression.GetText(),
                    gArgument.Value.GetText()));
        }
    }

    /// <remarks>
    /// <c>guid.Equals(null)</c> → <c>false</c>
    /// </remarks>
    static void AnalyzeEquals_Object(IHighlightingConsumer consumer, IInvocationExpression invocationExpression, ICSharpArgument objArgument)
    {
        if (!invocationExpression.IsUsedAsStatement() && objArgument.Value.IsDefaultValue())
        {
            consumer.AddHighlighting(new UseExpressionResultSuggestion("The expression is always false.", invocationExpression, "false"));
        }
    }

    /// <remarks>
    /// <c>Guid.Parse(s, provider)</c> → <c>Guid.Parse(s)</c>
    /// </remarks>
    static void AnalyzeParse_String_IFormatProvider(
        IHighlightingConsumer consumer,
        IInvocationExpression invocationExpression,
        ICSharpArgument providerArgument)
    {
        if (PredefinedType.GUID_FQN.HasMethod(
            new MethodSignature { Name = nameof(Guid.Parse), ParameterTypes = ParameterTypes.String, IsStatic = true },
            invocationExpression.PsiModule))
        {
            consumer.AddHighlighting(new RedundantArgumentHint("Passing a format provider is redundant.", providerArgument));
        }
    }

    /// <remarks>
    /// <c>Guid.Parse(s, provider)</c> → <c>Guid.Parse(s)</c> (.NET Core 2.1)
    /// </remarks>
    static void AnalyzeParse_ReadOnlySpanOfChar_IFormatProvider(
        IHighlightingConsumer consumer,
        IInvocationExpression invocationExpression,
        ICSharpArgument providerArgument)
    {
        if (PredefinedType.GUID_FQN.HasMethod(
            new MethodSignature { Name = nameof(Guid.Parse), ParameterTypes = ParameterTypes.ReadOnlySpanOfT, IsStatic = true },
            invocationExpression.PsiModule))
        {
            consumer.AddHighlighting(new RedundantArgumentHint("Passing a format provider is redundant.", providerArgument));
        }
    }

    /// <remarks>
    /// <c>Guid.TryParse(s, provider, out result)</c> → <c>Guid.TryParse(s, out result)</c>
    /// </remarks>
    static void AnalyzeTryParse_String_IFormatProvider_Guid(
        IHighlightingConsumer consumer,
        IInvocationExpression invocationExpression,
        ICSharpArgument providerArgument)
    {
        if (PredefinedType.GUID_FQN.HasMethod(
            new MethodSignature { Name = nameof(Guid.TryParse), ParameterTypes = ParameterTypes.String_Guid, IsStatic = true },
            invocationExpression.PsiModule))
        {
            consumer.AddHighlighting(new RedundantArgumentHint("Passing a format provider is redundant.", providerArgument));
        }
    }

    /// <remarks>
    /// <c>Guid.TryParse(s, provider, out result)</c> → <c>Guid.TryParse(s, out result)</c> (.NET 7)
    /// </remarks>
    static void AnalyzeTryParse_ReadOnlySpanOfChar_IFormatProvider_Guid(
        IHighlightingConsumer consumer,
        IInvocationExpression invocationExpression,
        ICSharpArgument providerArgument)
    {
        if (PredefinedType.GUID_FQN.HasMethod(
            new MethodSignature { Name = nameof(Guid.TryParse), ParameterTypes = ParameterTypes.ReadOnlySpanOfT_Guid, IsStatic = true },
            invocationExpression.PsiModule))
        {
            consumer.AddHighlighting(new RedundantArgumentHint("Passing a format provider is redundant.", providerArgument));
        }
    }

    protected override void Run(IInvocationExpression element, ElementProblemAnalyzerData data, IHighlightingConsumer consumer)
    {
        if (element is { InvokedExpression: IReferenceExpression { Reference: var reference } invokedExpression }
            && reference.Resolve().DeclaredElement is IMethod
            {
                AccessibilityDomain.DomainType: AccessibilityDomain.AccessibilityDomainType.PUBLIC, TypeParameters: [],
            } method
            && method.ContainingType.IsClrType(PredefinedType.GUID_FQN))
        {
            switch (invokedExpression, method)
            {
                case ({ QualifierExpression: { } }, { IsStatic: false }):
                    switch (method.ShortName)
                    {
                        case nameof(Guid.Equals):
                            switch (method.Parameters, element.TryGetArgumentsInDeclarationOrder())
                            {
                                case ([{ Type: var gType }], [{ } gArgument]) when gType.IsGuid():
                                    AnalyzeEquals_Guid(consumer, element, invokedExpression, gArgument);
                                    break;

                                case ([{ Type: var objType }], [{ } objArgument]) when objType.IsObject():
                                    AnalyzeEquals_Object(consumer, element, objArgument);
                                    break;
                            }
                            break;
                    }
                    break;

                case (_, { IsStatic: true }):
                    switch (method.ShortName)
                    {
                        case nameof(Guid.Parse):
                            switch (method.Parameters, element.TryGetArgumentsInDeclarationOrder())
                            {
                                case ([{ Type: var sType }, { Type: var providerType }], [_, { } providerArgument])
                                    when sType.IsString() && providerType.IsIFormatProvider():

                                    AnalyzeParse_String_IFormatProvider(consumer, element, providerArgument);
                                    break;

                                case ([{ Type: var sType }, { Type: var providerType }], [_, { } providerArgument])
                                    when sType.IsReadOnlySpanOfChar() && providerType.IsIFormatProvider():

                                    AnalyzeParse_ReadOnlySpanOfChar_IFormatProvider(consumer, element, providerArgument);
                                    break;
                            }
                            break;

                        case nameof(Guid.TryParse):
                            switch (method.Parameters, element.TryGetArgumentsInDeclarationOrder())
                            {
                                case ([{ Type: var sType }, { Type: var providerType }, { Type: var resultType }], [_, { } providerArgument, _])
                                    when sType.IsString() && providerType.IsIFormatProvider() && resultType.IsGuid():

                                    AnalyzeTryParse_String_IFormatProvider_Guid(consumer, element, providerArgument);
                                    break;

                                case ([{ Type: var sType }, { Type: var providerType }, { Type: var resultType }], [_, { } providerArgument, _])
                                    when sType.IsReadOnlySpanOfChar() && providerType.IsIFormatProvider() && resultType.IsGuid():

                                    AnalyzeTryParse_ReadOnlySpanOfChar_IFormatProvider_Guid(consumer, element, providerArgument);
                                    break;
                            }
                            break;
                    }
                    break;
            }
        }
    }
}