using JetBrains.ReSharper.Feature.Services.Daemon;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.CSharp.Tree;
using ReCommendedExtension.Analyzers.BaseTypes.Collections;
using ReCommendedExtension.Extensions;
using ReCommendedExtension.Extensions.MethodFinding;

namespace ReCommendedExtension.Analyzers.BaseTypes.Analyzers;

/// <remarks>
/// C# language version checks are only done when a quick fix would require it.
/// </remarks>
[ElementProblemAnalyzer(
    typeof(IInvocationExpression),
    HighlightingTypes =
    [
        typeof(RedundantMethodInvocationHint),
        typeof(UseBinaryOperatorSuggestion),
        typeof(UseExpressionResultSuggestion),
        typeof(UseOtherArgumentSuggestion),
        typeof(RedundantElementHint),
    ])]
public sealed class DateOnlyAnalyzer : ElementProblemAnalyzer<IInvocationExpression>
{
    [SuppressMessage("ReSharper", "InconsistentNaming", Justification = "Underscore character used intentionally as a separator.")]
    static class Parameters
    {
        public static IReadOnlyList<Parameter> String_String { get; } = [Parameter.String, Parameter.String];

        public static IReadOnlyList<Parameter> String_String_outDateOnly { get; } =
        [
            Parameter.String, Parameter.String, Parameter.DateOnly with { Kind = ParameterKind.OUTPUT },
        ];

        public static IReadOnlyList<Parameter> String_String_IFormatProvider_DateTimeStyles { get; } =
        [
            Parameter.String, Parameter.String, Parameter.IFormatProvider, Parameter.DateTimeStyles,
        ];

        public static IReadOnlyList<Parameter> String_String_IFormatProvider_DateTimeStyles_outDateOnly { get; } =
        [
            Parameter.String,
            Parameter.String,
            Parameter.IFormatProvider,
            Parameter.DateTimeStyles,
            Parameter.DateOnly with { Kind = ParameterKind.OUTPUT },
        ];
    }

    /// <remarks>
    /// <c>dateOnly.AddDays(0)</c> → <c>dateOnly</c>
    /// </remarks>
    static void AnalyzeAddDays_Int32(
        IHighlightingConsumer consumer,
        IInvocationExpression invocationExpression,
        IReferenceExpression invokedExpression,
        ICSharpArgument valueArgument)
    {
        if (!invocationExpression.IsUsedAsStatement() && valueArgument.Value.TryGetInt32Constant() == 0)
        {
            consumer.AddHighlighting(
                new RedundantMethodInvocationHint("Calling 'AddDays' with 0 is redundant.", invocationExpression, invokedExpression)); // todo: nameof(DateOnly.AddDays) when available
        }
    }

    /// <remarks>
    /// <c>dateOnly.Equals(value)</c> → <c>dateOnly == value</c>
    /// </remarks>
    static void AnalyzeEquals_DateOnly(
        IHighlightingConsumer consumer,
        IInvocationExpression invocationExpression,
        IReferenceExpression invokedExpression,
        ICSharpArgument valueArgument)
    {
        Debug.Assert(invokedExpression.QualifierExpression is { });

        if (!invocationExpression.IsUsedAsStatement() && valueArgument.Value is { })
        {
            consumer.AddHighlighting(
                new UseBinaryOperatorSuggestion(
                    "Use the '==' operator.",
                    invocationExpression,
                    "==",
                    invokedExpression.QualifierExpression.GetText(),
                    valueArgument.Value.GetText()));
        }
    }

    /// <remarks>
    /// <c>dateOnly.Equals(null)</c> → <c>false</c>
    /// </remarks>
    static void AnalyzeEquals_Object(IHighlightingConsumer consumer, IInvocationExpression invocationExpression, ICSharpArgument valueArgument)
    {
        if (!invocationExpression.IsUsedAsStatement() && valueArgument.Value.IsDefaultValue())
        {
            consumer.AddHighlighting(new UseExpressionResultSuggestion("The expression is always false.", invocationExpression, "false"));
        }
    }

    /// <remarks>
    /// <c>DateOnly.ParseExact(s, "R", provider, style)</c> → <c>DateOnly.ParseExact(s, "R", null, style)</c>
    /// </remarks>
    static void AnalyzeParseExact_String_String_IFormatProvider_DateTimeStyles(
        IHighlightingConsumer consumer,
        ICSharpArgument formatArgument,
        ICSharpArgument providerArgument)
    {
        if (formatArgument.Value.TryGetStringConstant() is "o" or "O" or "r" or "R"
            && providerArgument.Value is { }
            && !providerArgument.Value.IsDefaultValue())
        {
            consumer.AddHighlighting(
                new UseOtherArgumentSuggestion(
                    "The format provider is ignored (pass null instead).",
                    providerArgument,
                    providerArgument.NameIdentifier?.Name,
                    "null"));
        }
    }

    /// <remarks>
    /// <c>DateOnly.ParseExact(s, [format])</c> → <c>DateOnly.ParseExact(s, format)</c><para/>
    /// <c>DateOnly.ParseExact(s, ["m", "M"])</c> → <c>DateOnly.ParseExact(s, ["m"])</c>
    /// </remarks>
    static void AnalyzeParseExact_String_StringArray(
        IHighlightingConsumer consumer,
        IInvocationExpression invocationExpression,
        ICSharpArgument formatsArgument)
    {
        switch (CollectionCreation.TryFrom(formatsArgument.Value))
        {
            case { Count: 1 } collectionCreation when PredefinedType.DATE_ONLY_FQN.HasMethod(
                new MethodSignature { Name = "ParseExact", Parameters = Parameters.String_String, IsStatic = true }, // todo: nameof(DateOnly.ParseExact) when available
                formatsArgument.NameIdentifier is { },
                out var parameterNames,
                invocationExpression.PsiModule):

                consumer.AddHighlighting(
                    new UseOtherArgumentSuggestion(
                        "The only collection element should be passed directly.",
                        formatsArgument,
                        parameterNames is [_, var formatParameterName] ? formatParameterName : null,
                        collectionCreation.SingleElement.GetText()));
                break;

            case { Count: > 1 } collectionCreation:
                var set = new HashSet<string>(collectionCreation.Count, StringComparer.Ordinal);

                foreach (var (element, s) in collectionCreation.ElementsWithStringConstants)
                {
                    if (s is "o" or "O" && (set.Contains("o") || set.Contains("O"))
                        || s is "r" or "R" && (set.Contains("r") || set.Contains("R"))
                        || s is "m" or "M" && (set.Contains("m") || set.Contains("M"))
                        || s is "y" or "Y" && (set.Contains("y") || set.Contains("Y")))
                    {
                        consumer.AddHighlighting(new RedundantElementHint("The equivalent string is already passed.", element));
                        continue;
                    }

                    if (s != "" && !set.Add(s))
                    {
                        consumer.AddHighlighting(new RedundantElementHint("The string is already passed.", element));
                    }
                }

                break;
        }
    }

    /// <remarks>
    /// <c>DateOnly.ParseExact(s, [format], provider, styles)</c> → <c>DateOnly.ParseExact(s, format, provider, styles)</c><para/>
    /// <c>DateOnly.ParseExact(s, ["m", "M"], provider, styles)</c> → <c>DateOnly.ParseExact(s, ["m"], provider, styles)</c><para/>
    /// <c>DateOnly.ParseExact(s, ["o", "R"], provider, styles)</c> → <c>DateOnly.ParseExact(s, ["o", "R"], null, styles)</c>
    /// </remarks>
    static void AnalyzeParseExact_String_StringArray_IFormatProvider_DateTimeStyles(
        IHighlightingConsumer consumer,
        IInvocationExpression invocationExpression,
        ICSharpArgument formatsArgument,
        ICSharpArgument providerArgument)
    {
        switch (CollectionCreation.TryFrom(formatsArgument.Value))
        {
            case { Count: 1 } collectionCreation when PredefinedType.DATE_ONLY_FQN.HasMethod(
                new MethodSignature { Name = "ParseExact", Parameters = Parameters.String_String_IFormatProvider_DateTimeStyles, IsStatic = true }, // todo: nameof(DateOnly.ParseExact) when available
                formatsArgument.NameIdentifier is { },
                out var parameterNames,
                invocationExpression.PsiModule):

                consumer.AddHighlighting(
                    new UseOtherArgumentSuggestion(
                        "The only collection element should be passed directly.",
                        formatsArgument,
                        parameterNames is [_, var formatParameterName, _, _] ? formatParameterName : null,
                        collectionCreation.SingleElement.GetText()));
                break;

            case { Count: > 1 } collectionCreation:
                var set = new HashSet<string>(collectionCreation.Count, StringComparer.Ordinal);

                foreach (var (element, s) in collectionCreation.ElementsWithStringConstants)
                {
                    if (s is "o" or "O" && (set.Contains("o") || set.Contains("O"))
                        || s is "r" or "R" && (set.Contains("r") || set.Contains("R"))
                        || s is "m" or "M" && (set.Contains("m") || set.Contains("M"))
                        || s is "y" or "Y" && (set.Contains("y") || set.Contains("Y")))
                    {
                        consumer.AddHighlighting(new RedundantElementHint("The equivalent string is already passed.", element));
                        continue;
                    }

                    if (s != "" && !set.Add(s))
                    {
                        consumer.AddHighlighting(new RedundantElementHint("The string is already passed.", element));
                    }
                }

                if (collectionCreation.AllElementsAreStringConstants)
                {
                    set.Remove("o");
                    set.Remove("O");
                    set.Remove("r");
                    set.Remove("R");

                    if (set.Count == 0)
                    {
                        consumer.AddHighlighting(
                            new UseOtherArgumentSuggestion(
                                "The format provider is ignored (pass null instead).",
                                providerArgument,
                                providerArgument.NameIdentifier?.Name,
                                "null"));
                    }
                }

                break;
        }
    }

    /// <remarks>
    /// <c>DateOnly.ParseExact(s, ["m", "M"])</c> → <c>DateOnly.ParseExact(s, ["m"])</c>
    /// </remarks>
    static void AnalyzeParseExact_ReadOnlySpanOfChar_StringArray(IHighlightingConsumer consumer, ICSharpArgument formatsArgument)
    {
        if (CollectionCreation.TryFrom(formatsArgument.Value) is { Count: > 1 } collectionCreation)
        {
            var set = new HashSet<string>(collectionCreation.Count, StringComparer.Ordinal);

            foreach (var (element, s) in collectionCreation.ElementsWithStringConstants)
            {
                if (s is "o" or "O" && (set.Contains("o") || set.Contains("O"))
                    || s is "r" or "R" && (set.Contains("r") || set.Contains("R"))
                    || s is "m" or "M" && (set.Contains("m") || set.Contains("M"))
                    || s is "y" or "Y" && (set.Contains("y") || set.Contains("Y")))
                {
                    consumer.AddHighlighting(new RedundantElementHint("The equivalent string is already passed.", element));
                    continue;
                }

                if (s != "" && !set.Add(s))
                {
                    consumer.AddHighlighting(new RedundantElementHint("The string is already passed.", element));
                }
            }
        }
    }

    /// <remarks>
    /// <c>DateOnly.ParseExact(s, ["m", "M"], provider, styles)</c> → <c>DateOnly.ParseExact(s, ["m"], provider, styles)</c><para/>
    /// <c>DateOnly.ParseExact(s, ["o", "R"], provider, styles)</c> → <c>DateOnly.ParseExact(s, ["o", "R"], null, styles)</c>
    /// </remarks>
    static void AnalyzeParseExact_ReadOnlySpanOfChar_StringArray_IFormatProvider_DateTimeStyles(
        IHighlightingConsumer consumer,
        ICSharpArgument formatsArgument,
        ICSharpArgument providerArgument)
    {
        if (CollectionCreation.TryFrom(formatsArgument.Value) is { Count: > 1 } collectionCreation)
        {
            var set = new HashSet<string>(collectionCreation.Count, StringComparer.Ordinal);

            foreach (var (element, s) in collectionCreation.ElementsWithStringConstants)
            {
                if (s is "o" or "O" && (set.Contains("o") || set.Contains("O"))
                    || s is "r" or "R" && (set.Contains("r") || set.Contains("R"))
                    || s is "m" or "M" && (set.Contains("m") || set.Contains("M"))
                    || s is "y" or "Y" && (set.Contains("y") || set.Contains("Y")))
                {
                    consumer.AddHighlighting(new RedundantElementHint("The equivalent string is already passed.", element));
                    continue;
                }

                if (s != "" && !set.Add(s))
                {
                    consumer.AddHighlighting(new RedundantElementHint("The string is already passed.", element));
                }
            }

            if (collectionCreation.AllElementsAreStringConstants)
            {
                set.Remove("o");
                set.Remove("O");
                set.Remove("r");
                set.Remove("R");

                if (set.Count == 0)
                {
                    consumer.AddHighlighting(
                        new UseOtherArgumentSuggestion(
                            "The format provider is ignored (pass null instead).",
                            providerArgument,
                            providerArgument.NameIdentifier?.Name,
                            "null"));
                }
            }
        }
    }

    /// <remarks>
    /// <c>DateOnly.TryParseExact(s, "R", provider, style, out result)</c> → <c>DateOnly.TryParseExact(s, "R", null, style, out result)</c>
    /// </remarks>
    static void AnalyzeTryParseExact_String_String_IFormatProvider_DateTimeStyles_DateOnly(
        IHighlightingConsumer consumer,
        ICSharpArgument formatArgument,
        ICSharpArgument providerArgument)
    {
        if (formatArgument.Value.TryGetStringConstant() is "o" or "O" or "r" or "R"
            && providerArgument.Value is { }
            && !providerArgument.Value.IsDefaultValue())
        {
            consumer.AddHighlighting(
                new UseOtherArgumentSuggestion(
                    "The format provider is ignored (pass null instead).",
                    providerArgument,
                    providerArgument.NameIdentifier?.Name,
                    "null"));
        }
    }

    /// <remarks>
    /// <c>DateOnly.TryParseExact(s, [format], out result)</c> → <c>DateOnly.TryParseExact(s, format, out result)</c><para/>
    /// <c>DateOnly.TryParseExact(s, ["m", "M"], out result)</c> → <c>DateOnly.TryParseExact(s, ["m"], out result)</c>
    /// </remarks>
    static void AnalyzeTryParseExact_String_StringArray_DateOnly(
        IHighlightingConsumer consumer,
        IInvocationExpression invocationExpression,
        ICSharpArgument formatsArgument)
    {
        switch (CollectionCreation.TryFrom(formatsArgument.Value))
        {
            case { Count: 1 } collectionCreation when PredefinedType.DATE_ONLY_FQN.HasMethod(
                new MethodSignature { Name = "TryParseExact", Parameters = Parameters.String_String_outDateOnly, IsStatic = true }, // todo: nameof(DateOnly.TryParseExact) when available
                formatsArgument.NameIdentifier is { },
                out var parameterNames,
                invocationExpression.PsiModule):

                consumer.AddHighlighting(
                    new UseOtherArgumentSuggestion(
                        "The only collection element should be passed directly.",
                        formatsArgument,
                        parameterNames is [_, var formatParameterName, _] ? formatParameterName : null,
                        collectionCreation.SingleElement.GetText()));
                break;

            case { Count: > 1 } collectionCreation:
                var set = new HashSet<string>(collectionCreation.Count, StringComparer.Ordinal);

                foreach (var (element, s) in collectionCreation.ElementsWithStringConstants)
                {
                    if (s is "o" or "O" && (set.Contains("o") || set.Contains("O"))
                        || s is "r" or "R" && (set.Contains("r") || set.Contains("R"))
                        || s is "m" or "M" && (set.Contains("m") || set.Contains("M"))
                        || s is "y" or "Y" && (set.Contains("y") || set.Contains("Y")))
                    {
                        consumer.AddHighlighting(new RedundantElementHint("The equivalent string is already passed.", element));
                        continue;
                    }

                    if (s != "" && !set.Add(s))
                    {
                        consumer.AddHighlighting(new RedundantElementHint("The string is already passed.", element));
                    }
                }

                break;
        }
    }

    /// <remarks>
    /// <c>DateOnly.TryParseExact(s, ["m", "M"], out result)</c> → <c>DateOnly.TryParseExact(s, ["m"], out result)</c>
    /// </remarks>
    static void AnalyzeTryParseExact_ReadOnlySpanOfChar_StringArray_DateOnly(IHighlightingConsumer consumer, ICSharpArgument formatsArgument)
    {
        if (CollectionCreation.TryFrom(formatsArgument.Value) is { Count: > 1 } collectionCreation)
        {
            var set = new HashSet<string>(collectionCreation.Count, StringComparer.Ordinal);

            foreach (var (element, s) in collectionCreation.ElementsWithStringConstants)
            {
                if (s is "o" or "O" && (set.Contains("o") || set.Contains("O"))
                    || s is "r" or "R" && (set.Contains("r") || set.Contains("R"))
                    || s is "m" or "M" && (set.Contains("m") || set.Contains("M"))
                    || s is "y" or "Y" && (set.Contains("y") || set.Contains("Y")))
                {
                    consumer.AddHighlighting(new RedundantElementHint("The equivalent string is already passed.", element));
                    continue;
                }

                if (s != "" && !set.Add(s))
                {
                    consumer.AddHighlighting(new RedundantElementHint("The string is already passed.", element));
                }
            }
        }
    }

    /// <remarks>
    /// <c>DateOnly.TryParseExact(s, [format], provider, style, out result)</c> → <c>DateOnly.TryParseExact(s, format, provider, style, out result)</c><para/>
    /// <c>DateOnly.TryParseExact(s, ["m", "M"], provider, style, out result)</c> → <c>DateOnly.TryParseExact(s, ["m"], provider, style, out result)</c><para/>
    /// <c>DateOnly.TryParseExact(s, ["o", "R"], provider, style, out result)</c> → <c>DateOnly.TryParseExact(s, ["o", "R"], null, style, out result)</c>
    /// </remarks>
    static void AnalyzeTryParseExact_String_StringArray_IFormatProvider_DateTimeStyles_DateOnly(
        IHighlightingConsumer consumer,
        IInvocationExpression invocationExpression,
        ICSharpArgument formatsArgument,
        ICSharpArgument providerArgument)
    {
        switch (CollectionCreation.TryFrom(formatsArgument.Value))
        {
            case { Count: 1 } collectionCreation when PredefinedType.DATE_ONLY_FQN.HasMethod(
                new MethodSignature
                {
                    Name = "TryParseExact", Parameters = Parameters.String_String_IFormatProvider_DateTimeStyles_outDateOnly, IsStatic = true, // todo: nameof(DateOnly.TryParseExact) when available
                },
                formatsArgument.NameIdentifier is { },
                out var parameterNames,
                invocationExpression.PsiModule):

                consumer.AddHighlighting(
                    new UseOtherArgumentSuggestion(
                        "The only collection element should be passed directly.",
                        formatsArgument,
                        parameterNames is [_, var formatParameterName, _, _, _] ? formatParameterName : null,
                        collectionCreation.SingleElement.GetText()));
                break;

            case { Count: > 1 } collectionCreation:
                var set = new HashSet<string>(collectionCreation.Count, StringComparer.Ordinal);

                foreach (var (element, s) in collectionCreation.ElementsWithStringConstants)
                {
                    if (s is "o" or "O" && (set.Contains("o") || set.Contains("O"))
                        || s is "r" or "R" && (set.Contains("r") || set.Contains("R"))
                        || s is "m" or "M" && (set.Contains("m") || set.Contains("M"))
                        || s is "y" or "Y" && (set.Contains("y") || set.Contains("Y")))
                    {
                        consumer.AddHighlighting(new RedundantElementHint("The equivalent string is already passed.", element));
                        continue;
                    }

                    if (s != "" && !set.Add(s))
                    {
                        consumer.AddHighlighting(new RedundantElementHint("The string is already passed.", element));
                    }
                }

                if (collectionCreation.AllElementsAreStringConstants)
                {
                    set.Remove("o");
                    set.Remove("O");
                    set.Remove("r");
                    set.Remove("R");

                    if (set.Count == 0)
                    {
                        consumer.AddHighlighting(
                            new UseOtherArgumentSuggestion(
                                "The format provider is ignored (pass null instead).",
                                providerArgument,
                                providerArgument.NameIdentifier?.Name,
                                "null"));
                    }
                }

                break;
        }
    }

    /// <remarks>
    /// <c>DateOnly.TryParseExact(s, ["m", "M"], provider, style, out result)</c> → <c>DateOnly.TryParseExact(s, ["m"], provider, style, out result)</c><para/>
    /// <c>DateOnly.TryParseExact(s, ["o", "R"], provider, style, out result)</c> → <c>DateOnly.TryParseExact(s, ["o", "R"], null, style, out result)</c>
    /// </remarks>
    static void AnalyzeTryParseExact_ReadOnlySpanOfChar_StringArray_IFormatProvider_DateTimeStyles_DateOnly(
        IHighlightingConsumer consumer,
        ICSharpArgument formatsArgument,
        ICSharpArgument providerArgument)
    {
        if (CollectionCreation.TryFrom(formatsArgument.Value) is { Count: > 1 } collectionCreation)
        {
            var set = new HashSet<string>(collectionCreation.Count, StringComparer.Ordinal);

            foreach (var (element, s) in collectionCreation.ElementsWithStringConstants)
            {
                if (s is "o" or "O" && (set.Contains("o") || set.Contains("O"))
                    || s is "r" or "R" && (set.Contains("r") || set.Contains("R"))
                    || s is "m" or "M" && (set.Contains("m") || set.Contains("M"))
                    || s is "y" or "Y" && (set.Contains("y") || set.Contains("Y")))
                {
                    consumer.AddHighlighting(new RedundantElementHint("The equivalent string is already passed.", element));
                    continue;
                }

                if (s != "" && !set.Add(s))
                {
                    consumer.AddHighlighting(new RedundantElementHint("The string is already passed.", element));
                }
            }

            if (collectionCreation.AllElementsAreStringConstants)
            {
                set.Remove("o");
                set.Remove("O");
                set.Remove("r");
                set.Remove("R");

                if (set.Count == 0)
                {
                    consumer.AddHighlighting(
                        new UseOtherArgumentSuggestion(
                            "The format provider is ignored (pass null instead).",
                            providerArgument,
                            providerArgument.NameIdentifier?.Name,
                            "null"));
                }
            }
        }
    }

    protected override void Run(IInvocationExpression element, ElementProblemAnalyzerData data, IHighlightingConsumer consumer)
    {
        if (element is { InvokedExpression: IReferenceExpression { Reference: var reference } invokedExpression }
            && reference.Resolve().DeclaredElement is IMethod
            {
                AccessibilityDomain.DomainType: AccessibilityDomain.AccessibilityDomainType.PUBLIC, TypeParameters: [],
            } method
            && method.ContainingType.IsClrType(PredefinedType.DATE_ONLY_FQN))
        {
            switch (invokedExpression, method)
            {
                case ({ QualifierExpression: { } }, { IsStatic: false }):
                    switch (method.ShortName)
                    {
                        case "AddDays": // todo: nameof(DateOnly.AddDays) when available
                            switch (method.Parameters, element.TryGetArgumentsInDeclarationOrder())
                            {
                                case ([{ Type: var valueType}], [{ } valueArgument]) when valueType.IsInt():
                                    AnalyzeAddDays_Int32(consumer, element, invokedExpression, valueArgument);
                                    break;
                            }
                            break;

                        case "Equals": // todo: nameof(DateOnly.Equals) when available
                            switch (method.Parameters, element.TryGetArgumentsInDeclarationOrder())
                            {
                                case ([{ Type: var valueType }], [{ } valueArgument]) when valueType.IsDateOnly():
                                    AnalyzeEquals_DateOnly(consumer, element, invokedExpression, valueArgument);
                                    break;

                                case ([{ Type: var valueType }], [{ } valueArgument]) when valueType.IsObject():
                                    AnalyzeEquals_Object(consumer, element, valueArgument);
                                    break;
                            }
                            break;
                    }
                    break;

                case (_, { IsStatic: true }):
                    switch (method.ShortName)
                    {
                        case "ParseExact": // todo: nameof(DateOnly.ParseExact) when available
                            switch (method.Parameters, element.TryGetArgumentsInDeclarationOrder())
                            {
                                case ([{ Type: var sType }, { Type: var formatType }, { Type: var providerType }, { Type: var styleType }], [
                                    _, { } formatArgument, { } providerArgument, _,
                                ]) when sType.IsString() && formatType.IsString() && providerType.IsIFormatProvider() && styleType.IsDateTimeStyles():
                                    AnalyzeParseExact_String_String_IFormatProvider_DateTimeStyles(consumer, formatArgument, providerArgument);
                                    break;

                                case ([{ Type: var sType }, { Type: var formatsType }], [_, { } formatsArgument])
                                    when sType.IsString() && formatsType.IsGenericArrayOfString():

                                    AnalyzeParseExact_String_StringArray(consumer, element, formatsArgument);
                                    break;

                                case ([{ Type: var sType }, { Type: var formatsType }, { Type: var providerType }, { Type: var styleType }], [
                                        _, { } formatsArgument, { } providerArgument, _,
                                    ]) when sType.IsString()
                                    && formatsType.IsGenericArrayOfString()
                                    && providerType.IsIFormatProvider()
                                    && styleType.IsDateTimeStyles():

                                    AnalyzeParseExact_String_StringArray_IFormatProvider_DateTimeStyles(
                                        consumer,
                                        element,
                                        formatsArgument,
                                        providerArgument);
                                    break;

                                case ([{ Type: var sType }, { Type: var formatsType }], [_, { } formatsArgument]) when sType.IsReadOnlySpanOfChar()
                                    && formatsType.IsGenericArrayOfString():

                                    AnalyzeParseExact_ReadOnlySpanOfChar_StringArray(consumer, formatsArgument);
                                    break;

                                case ([{ Type: var sType }, { Type: var formatsType }, { Type: var providerType }, { Type: var styleType }], [
                                        _, { } formatsArgument, { } providerArgument, _,
                                    ]) when sType.IsReadOnlySpanOfChar()
                                    && formatsType.IsGenericArrayOfString()
                                    && providerType.IsIFormatProvider()
                                    && styleType.IsDateTimeStyles():

                                    AnalyzeParseExact_ReadOnlySpanOfChar_StringArray_IFormatProvider_DateTimeStyles(
                                        consumer,
                                        formatsArgument,
                                        providerArgument);
                                    break;
                            }
                            break;

                        case "TryParseExact": // todo: nameof(DateOnly.TryParseExact) when available
                            switch (method.Parameters, element.TryGetArgumentsInDeclarationOrder())
                            {
                                case ([
                                        { Type: var sType },
                                        { Type: var formatType },
                                        { Type: var providerType },
                                        { Type: var styleType },
                                        { Type: var resultType },
                                    ], [_, { } formatArgument, { } providerArgument, _, _]) when sType.IsString()
                                    && formatType.IsString()
                                    && providerType.IsIFormatProvider()
                                    && styleType.IsDateTimeStyles()
                                    && resultType.IsDateOnly():

                                    AnalyzeTryParseExact_String_String_IFormatProvider_DateTimeStyles_DateOnly(
                                        consumer,
                                        formatArgument,
                                        providerArgument);
                                    break;

                                case ([{ Type: var sType }, { Type: var formatsType }, { Type: var resultType }], [_, { } formatsArgument, _])
                                    when sType.IsString() && formatsType.IsGenericArrayOfString() && resultType.IsDateOnly():

                                    AnalyzeTryParseExact_String_StringArray_DateOnly(consumer, element, formatsArgument);
                                    break;

                                case ([{ Type: var sType }, { Type: var formatsType }, { Type: var resultType }], [_, { } formatsArgument, _])
                                    when sType.IsReadOnlySpanOfChar() && formatsType.IsGenericArrayOfString() && resultType.IsDateOnly():

                                    AnalyzeTryParseExact_ReadOnlySpanOfChar_StringArray_DateOnly(consumer, formatsArgument);
                                    break;

                                case ([
                                        { Type: var sType },
                                        { Type: var formatsType },
                                        { Type: var providerType },
                                        { Type: var styleType },
                                        { Type: var resultType },
                                    ], [_, { } formatsArgument, { } providerArgument, _, _]) when sType.IsString()
                                    && formatsType.IsGenericArrayOfString()
                                    && providerType.IsIFormatProvider()
                                    && styleType.IsDateTimeStyles()
                                    && resultType.IsDateOnly():

                                    AnalyzeTryParseExact_String_StringArray_IFormatProvider_DateTimeStyles_DateOnly(
                                        consumer,
                                        element,
                                        formatsArgument,
                                        providerArgument);
                                    break;

                                case ([
                                        { Type: var sType },
                                        { Type: var formatsType },
                                        { Type: var providerType },
                                        { Type: var styleType },
                                        { Type: var resultType },
                                    ], [_, { } formatsArgument, { } providerArgument, _, _]) when sType.IsReadOnlySpanOfChar()
                                    && formatsType.IsGenericArrayOfString()
                                    && providerType.IsIFormatProvider()
                                    && styleType.IsDateTimeStyles()
                                    && resultType.IsDateOnly():

                                    AnalyzeTryParseExact_ReadOnlySpanOfChar_StringArray_IFormatProvider_DateTimeStyles_DateOnly(
                                        consumer,
                                        formatsArgument,
                                        providerArgument);
                                    break;
                            }
                            break;
                    }
                    break;
            }
        }
    }
}