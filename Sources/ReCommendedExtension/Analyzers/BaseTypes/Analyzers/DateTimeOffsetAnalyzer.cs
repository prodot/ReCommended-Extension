using JetBrains.ReSharper.Feature.Services.Daemon;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.CSharp;
using JetBrains.ReSharper.Psi.CSharp.Tree;
using ReCommendedExtension.Analyzers.BaseTypes.Collections;
using ReCommendedExtension.Extensions;
using ReCommendedExtension.Extensions.MethodFinding;

namespace ReCommendedExtension.Analyzers.BaseTypes.Analyzers;

/// <remarks>
/// C# language version checks are only done when a quick fix would require it.
/// </remarks>
[ElementProblemAnalyzer(
    typeof(ICSharpInvocationInfo),
    HighlightingTypes =
    [
        typeof(RedundantMethodInvocationHint),
        typeof(UseBinaryOperatorSuggestion),
        typeof(UseExpressionResultSuggestion),
        typeof(UseOtherArgumentSuggestion),
        typeof(RedundantElementHint),
    ])]
public sealed class DateTimeOffsetAnalyzer : ElementProblemAnalyzer<ICSharpInvocationInfo>
{
    [SuppressMessage("ReSharper", "InconsistentNaming", Justification = "Underscore character used intentionally as a separator.")]
    static class Parameters
    {
        public static IReadOnlyList<Parameter> String_String_IFormatProvider_DateTimeStyles { get; } =
        [
            Parameter.String, Parameter.String, Parameter.IFormatProvider, Parameter.DateTimeStyles,
        ];

        public static IReadOnlyList<Parameter> String_String_IFormatProvider_DateTimeStyles_outDateTimeOffset { get; } =
        [
            Parameter.String,
            Parameter.String,
            Parameter.IFormatProvider,
            Parameter.DateTimeStyles,
            Parameter.DateTimeOffset with { Kind = ParameterKind.OUTPUT },
        ];
    }

    /// <remarks>
    /// <c>dateTimeOffset.Add(timeSpan)</c> → <c>dateTimeOffset + timeSpan</c>
    /// </remarks>
    static void AnalyzeAdd_TimeSpan(
        IHighlightingConsumer consumer,
        IInvocationExpression invocationExpression,
        IReferenceExpression invokedExpression,
        ICSharpArgument timeSpanArgument)
    {
        Debug.Assert(invokedExpression.QualifierExpression is { });

        if (!invocationExpression.IsUsedAsStatement() && timeSpanArgument.Value is { })
        {
            consumer.AddHighlighting(
                new UseBinaryOperatorSuggestion(
                    "Use the '+' operator.",
                    invocationExpression,
                    "+",
                    invokedExpression.QualifierExpression.GetText(),
                    timeSpanArgument.Value.GetText()));
        }
    }

    /// <remarks>
    /// <c>dateTimeOffset.AddTicks(0)</c> → <c>dateTimeOffset</c>
    /// </remarks>
    static void AnalyzeAddTicks_Int64(
        IHighlightingConsumer consumer,
        IInvocationExpression invocationExpression,
        IReferenceExpression invokedExpression,
        ICSharpArgument valueArgument)
    {
        if (!invocationExpression.IsUsedAsStatement() && valueArgument.Value.TryGetInt64Constant() == 0)
        {
            consumer.AddHighlighting(
                new RedundantMethodInvocationHint(
                    $"Calling '{nameof(DateTimeOffset.AddTicks)}' with 0 is redundant.",
                    invocationExpression,
                    invokedExpression));
        }
    }

    /// <remarks>
    /// <c>dateTimeOffset.Equals(other)</c> → <c>dateTimeOffset == other</c>
    /// </remarks>
    static void AnalyzeEquals_DateTimeOffset(
        IHighlightingConsumer consumer,
        IInvocationExpression invocationExpression,
        IReferenceExpression invokedExpression,
        ICSharpArgument otherArgument)
    {
        Debug.Assert(invokedExpression.QualifierExpression is { });

        if (!invocationExpression.IsUsedAsStatement() && otherArgument.Value is { })
        {
            consumer.AddHighlighting(
                new UseBinaryOperatorSuggestion(
                    "Use the '==' operator.",
                    invocationExpression,
                    "==",
                    invokedExpression.QualifierExpression.GetText(),
                    otherArgument.Value.GetText()));
        }
    }

    /// <remarks>
    /// <c>DateTimeOffset.Equals(first, second)</c> → <c>first == second</c>
    /// </remarks>
    static void AnalyzeEquals_DateTimeOffset_DateTimeOffset(
        IHighlightingConsumer consumer,
        IInvocationExpression invocationExpression,
        ICSharpArgument firstArgument,
        ICSharpArgument secondArgument)
    {
        if (!invocationExpression.IsUsedAsStatement() && firstArgument.Value is { } && secondArgument.Value is { })
        {
            consumer.AddHighlighting(
                new UseBinaryOperatorSuggestion(
                    "Use the '==' operator.",
                    invocationExpression,
                    "==",
                    firstArgument.Value.GetText(),
                    secondArgument.Value.GetText()));
        }
    }

    /// <remarks>
    /// <c>dateTimeOffset.Equals(null)</c> → <c>false</c>
    /// </remarks>
    static void AnalyzeEquals_Object(IHighlightingConsumer consumer, IInvocationExpression invocationExpression, ICSharpArgument objArgument)
    {
        if (!invocationExpression.IsUsedAsStatement() && objArgument.Value.IsDefaultValue())
        {
            consumer.AddHighlighting(new UseExpressionResultSuggestion("The expression is always false.", invocationExpression, "false"));
        }
    }

    /// <remarks>
    /// <c>DateTimeOffset.ParseExact(input, "R", provider)</c> → <c>DateTimeOffset.ParseExact(input, "R", null)</c>
    /// </remarks>
    static void AnalyzeParseExact_String_String_IFormatProvider(
        IHighlightingConsumer consumer,
        ICSharpArgument formatArgument,
        ICSharpArgument formatProviderArgument)
    {
        if (formatArgument.Value.TryGetStringConstant() is "o" or "O" or "r" or "R" or "s" or "u"
            && !formatProviderArgument.Value.IsDefaultValue()
            && formatProviderArgument.Value is { })
        {
            consumer.AddHighlighting(
                new UseOtherArgumentSuggestion(
                    "The format provider is ignored (pass null instead).",
                    formatProviderArgument,
                    formatProviderArgument.NameIdentifier?.Name,
                    "null"));
        }
    }

    /// <remarks>
    /// <c>DateTimeOffset.ParseExact(input, "R", formatProvider, styles)</c> → <c>DateTimeOffset.ParseExact(input, "R", null, styles)</c>
    /// </remarks>
    static void AnalyzeParseExact_String_String_IFormatProvider_DateTimeStyles(
        IHighlightingConsumer consumer,
        ICSharpArgument formatArgument,
        ICSharpArgument formatProviderArgument)
    {
        if (formatArgument.Value.TryGetStringConstant() is "o" or "O" or "r" or "R" or "s" or "u"
            && !formatProviderArgument.Value.IsDefaultValue()
            && formatProviderArgument.Value is { })
        {
            consumer.AddHighlighting(
                new UseOtherArgumentSuggestion(
                    "The format provider is ignored (pass null instead).",
                    formatProviderArgument,
                    formatProviderArgument.NameIdentifier?.Name,
                    "null"));
        }
    }

    /// <remarks>
    /// <c>DateTimeOffset.ParseExact(input, [format], formatProvider, styles)</c> → <c>DateTimeOffset.ParseExact(input, format, formatProvider, styles)</c><para/>
    /// <c>DateTimeOffset.ParseExact(input, ["R", "r"], formatProvider, styles)</c> → <c>DateTimeOffset.ParseExact(input, ["R"], formatProvider, styles)</c><para/>
    /// <c>DateTimeOffset.ParseExact(input, ["R", "s"], formatProvider, styles)</c> → <c>DateTimeOffset.ParseExact(input, ["R", "s"], null, styles)</c>
    /// </remarks>
    static void AnalyzeParseExact_String_StringArray_IFormatProvider_DateTimeStyles(
        IHighlightingConsumer consumer,
        IInvocationExpression invocationExpression,
        ICSharpArgument formatsArgument,
        ICSharpArgument formatProviderArgument)
    {
        switch (CollectionCreation.TryFrom(formatsArgument.Value))
        {
            case { Count: 1 } collectionCreation when PredefinedType.DATETIMEOFFSET_FQN.HasMethod(
                new MethodSignature
                {
                    Name = nameof(DateTimeOffset.ParseExact),
                    Parameters = Parameters.String_String_IFormatProvider_DateTimeStyles,
                    IsStatic = true,
                },
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
                    set.Remove("s");
                    set.Remove("u");

                    if (set.Count == 0)
                    {
                        consumer.AddHighlighting(
                            new UseOtherArgumentSuggestion(
                                "The format provider is ignored (pass null instead).",
                                formatProviderArgument,
                                formatProviderArgument.NameIdentifier?.Name,
                                "null"));
                    }
                }

                break;
        }
    }

    /// <remarks>
    /// <c>DateTimeOffset.ParseExact(input, ["R", "r"], formatProvider, styles)</c> → <c>DateTimeOffset.ParseExact(input, ["R"], formatProvider, styles)</c> (.NET Core 2.1)<para/>
    /// <c>DateTimeOffset.ParseExact(input, ["R", "s"], formatProvider, styles)</c> → <c>DateTimeOffset.ParseExact(input, ["R", "s"], null, style)</c> (.NET Core 2.1)
    /// </remarks>
    static void AnalyzeParseExact_ReadOnlyCSpanOfChar_StringArray_IFormatProvider_DateTimeStyles(
        IHighlightingConsumer consumer,
        ICSharpArgument formatsArgument,
        ICSharpArgument formatProviderArgument)
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
                set.Remove("s");
                set.Remove("u");

                if (set.Count == 0)
                {
                    consumer.AddHighlighting(
                        new UseOtherArgumentSuggestion(
                            "The format provider is ignored (pass null instead).",
                            formatProviderArgument,
                            formatProviderArgument.NameIdentifier?.Name,
                            "null"));
                }
            }
        }
    }

    /// <remarks>
    /// <c>dateTimeOffset.Subtract(value)</c> → <c>dateTimeOffset - value</c>
    /// </remarks>
    static void AnalyzeSubtract_DateTimeOffset(
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
                    "Use the '-' operator.",
                    invocationExpression,
                    "-",
                    invokedExpression.QualifierExpression.GetText(),
                    valueArgument.Value.GetText()));
        }
    }

    /// <remarks>
    /// <c>dateTimeOffset.Subtract(value)</c> → <c>dateTimeOffset - value</c>
    /// </remarks>
    static void AnalyzeSubtract_TimeSpan(
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
                    "Use the '-' operator.",
                    invocationExpression,
                    "-",
                    invokedExpression.QualifierExpression.GetText(),
                    valueArgument.Value.GetText()));
        }
    }

    /// <remarks>
    /// <c>DateTimeOffset.TryParseExact(input, "R", formatProvider, styles, out result)</c> → <c>DateTimeOffset.TryParseExact(input, "R", null, styles, out result)</c>
    /// </remarks>
    static void AnalyzeTryParseExact_String_String_IFormatProvider_DateTimeStyles_DateTimeOffset(
        IHighlightingConsumer consumer,
        ICSharpArgument formatArgument,
        ICSharpArgument formatProviderArgument)
    {
        if (formatArgument.Value.TryGetStringConstant() is "o" or "O" or "r" or "R" or "s" or "u"
            && !formatProviderArgument.Value.IsDefaultValue()
            && formatProviderArgument.Value is { })
        {
            consumer.AddHighlighting(
                new UseOtherArgumentSuggestion(
                    "The format provider is ignored (pass null instead).",
                    formatProviderArgument,
                    formatProviderArgument.NameIdentifier?.Name,
                    "null"));
        }
    }

    /// <remarks>
    /// <c>DateTimeOffset.TryParseExact(input, [format], formatProvider, styles, out result)</c> → <c>DateTimeOffset.TryParseExact(input, format, formatProvider, styles, out result)</c><para/>
    /// <c>DateTimeOffset.TryParseExact(input, ["R", "r"], formatProvider, styles, out result)</c> → <c>DateTimeOffset.TryParseExact(input, ["R"], formatProvider, styles, out result)</c><para/>
    /// <c>DateTimeOffset.TryParseExact(input, ["R", "s"], formatProvider, styles, out result)</c> → <c>DateTimeOffset.TryParseExact(input, ["R", "s"], null, styles, out result)</c>
    /// </remarks>
    static void AnalyzeTryParseExact_String_StringArray_IFormatProvider_DateTimeStyles_DateTimeOffset(
        IHighlightingConsumer consumer,
        IInvocationExpression invocationExpression,
        ICSharpArgument formatsArgument,
        ICSharpArgument formatProviderArgument)
    {
        switch (CollectionCreation.TryFrom(formatsArgument.Value))
        {
            case { Count: 1 } collectionCreation when PredefinedType.DATETIMEOFFSET_FQN.HasMethod(
                new MethodSignature
                {
                    Name = nameof(DateTime.TryParseExact),
                    Parameters = Parameters.String_String_IFormatProvider_DateTimeStyles_outDateTimeOffset,
                    IsStatic = true,
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
                    set.Remove("s");
                    set.Remove("u");

                    if (set.Count == 0)
                    {
                        consumer.AddHighlighting(
                            new UseOtherArgumentSuggestion(
                                "The format provider is ignored (pass null instead).",
                                formatProviderArgument,
                                formatProviderArgument.NameIdentifier?.Name,
                                "null"));
                    }
                }

                break;
        }
    }

    /// <remarks>
    /// <c>DateTimeOffset.TryParseExact(input, ["R", "r"], formatProvider, styles, out result)</c> → <c>DateTimeOffset.TryParseExact(input, ["R"], formatProvider, styles, out result)</c> (.NET Core 2.1)<para/>
    /// <c>DateTimeOffset.TryParseExact(input, ["R", "s"], formatProvider, styles, out result)</c> → <c>DateTimeOffset.TryParseExact(input, ["R", "s"], null, styles, out result)</c> (.NET Core 2.1)
    /// </remarks>
    static void AnalyzeTryParseExact_ReadOnlyCSpanOfChar_StringArray_IFormatProvider_DateTimeStyles_DateTimeOffset(
        IHighlightingConsumer consumer,
        ICSharpArgument formatsArgument,
        ICSharpArgument formatProviderArgument)
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
                set.Remove("s");
                set.Remove("u");

                if (set.Count == 0)
                {
                    consumer.AddHighlighting(
                        new UseOtherArgumentSuggestion(
                            "The format provider is ignored (pass null instead).",
                            formatProviderArgument,
                            formatProviderArgument.NameIdentifier?.Name,
                            "null"));
                }
            }
        }
    }

    protected override void Run(ICSharpInvocationInfo element, ElementProblemAnalyzerData data, IHighlightingConsumer consumer)
    {
        switch (element)
        {
            case IInvocationExpression
                {
                    InvokedExpression: IReferenceExpression { Reference: var reference } invokedExpression,
                } invocationExpression
                when reference.Resolve().DeclaredElement is IMethod
                {
                    AccessibilityDomain.DomainType: AccessibilityDomain.AccessibilityDomainType.PUBLIC, TypeParameters: [],
                } method
                && method.ContainingType.IsClrType(PredefinedType.DATETIMEOFFSET_FQN):

                switch (invokedExpression, method)
                {
                    case ({ QualifierExpression: { } }, { IsStatic: false }):
                        switch (method.ShortName)
                        {
                            case nameof(DateTimeOffset.Add):
                                switch (method.Parameters, invocationExpression.TryGetArgumentsInDeclarationOrder())
                                {
                                    case ([{ Type: var timeSpanType }], [{ } timeSpanArgument]) when timeSpanType.IsTimeSpan():
                                        AnalyzeAdd_TimeSpan(consumer, invocationExpression, invokedExpression, timeSpanArgument);
                                        break;
                                }
                                break;

                            case nameof(DateTimeOffset.AddTicks):
                                switch (method.Parameters, invocationExpression.TryGetArgumentsInDeclarationOrder())
                                {
                                    case ([{ Type: var valueType }], [{ } valueArgument]) when valueType.IsLong():
                                        AnalyzeAddTicks_Int64(consumer, invocationExpression, invokedExpression, valueArgument);
                                        break;
                                }
                                break;

                            case nameof(DateTimeOffset.Equals):
                                switch (method.Parameters, invocationExpression.TryGetArgumentsInDeclarationOrder())
                                {
                                    case ([{ Type: var otherType }], [{ } otherArgument]) when otherType.IsDateTimeOffset():
                                        AnalyzeEquals_DateTimeOffset(consumer, invocationExpression, invokedExpression, otherArgument);
                                        break;

                                    case ([{ Type: var objType }], [{ } objArgument]) when objType.IsObject():
                                        AnalyzeEquals_Object(consumer, invocationExpression, objArgument);
                                        break;
                                }
                                break;

                            case nameof(DateTimeOffset.Subtract):
                                switch (method.Parameters, invocationExpression.TryGetArgumentsInDeclarationOrder())
                                {
                                    case ([{ Type: var valueType }], [{ } valueArgument]) when valueType.IsDateTimeOffset():
                                        AnalyzeSubtract_DateTimeOffset(consumer, invocationExpression, invokedExpression, valueArgument);
                                        break;

                                    case ([{ Type: var valueType }], [{ } valueArgument]) when valueType.IsTimeSpan():
                                        AnalyzeSubtract_TimeSpan(consumer, invocationExpression, invokedExpression, valueArgument);
                                        break;
                                }
                                break;
                        }
                        break;

                    case (_, { IsStatic: true }):
                        switch (method.ShortName)
                        {
                            case nameof(DateTimeOffset.Equals):
                                switch (method.Parameters, invocationExpression.TryGetArgumentsInDeclarationOrder())
                                {
                                    case ([{ Type: var firstType }, { Type: var secondType }], [{ } firstArgument, { } secondArgument])
                                        when firstType.IsDateTimeOffset() && secondType.IsDateTimeOffset():

                                        AnalyzeEquals_DateTimeOffset_DateTimeOffset(consumer, invocationExpression, firstArgument, secondArgument);
                                        break;
                                }
                                break;

                            case nameof(DateTimeOffset.ParseExact):
                                switch (method.Parameters, invocationExpression.TryGetArgumentsInDeclarationOrder())
                                {
                                    case ([{ Type: var inputType }, { Type: var formatType }, { Type: var formatProviderType }], [
                                        _, { } formatArgument, { } formatProviderArgument,
                                    ]) when inputType.IsString() && formatType.IsString() && formatProviderType.IsIFormatProvider():
                                        AnalyzeParseExact_String_String_IFormatProvider(consumer, formatArgument, formatProviderArgument);
                                        break;

                                    case ([
                                            { Type: var inputType },
                                            { Type: var formatType },
                                            { Type: var formatProviderType },
                                            { Type: var stylesType },
                                        ], [_, { } formatArgument, { } formatProviderArgument, _]) when inputType.IsString()
                                        && formatType.IsString()
                                        && formatProviderType.IsIFormatProvider()
                                        && stylesType.IsDateTimeStyles():

                                        AnalyzeParseExact_String_String_IFormatProvider_DateTimeStyles(
                                            consumer,
                                            formatArgument,
                                            formatProviderArgument);
                                        break;

                                    case ([
                                            { Type: var inputType },
                                            { Type: var formatsType },
                                            { Type: var formatProviderType },
                                            { Type: var stylesType },
                                        ], [_, { } formatsArgument, { } formatProviderArgument, _]) when inputType.IsString()
                                        && formatsType.IsGenericArrayOfString()
                                        && formatProviderType.IsIFormatProvider()
                                        && stylesType.IsDateTimeStyles():

                                        AnalyzeParseExact_String_StringArray_IFormatProvider_DateTimeStyles(
                                            consumer,
                                            invocationExpression,
                                            formatsArgument,
                                            formatProviderArgument);
                                        break;

                                    case ([
                                            { Type: var inputType },
                                            { Type: var formatsType },
                                            { Type: var formatProviderType },
                                            { Type: var stylesType },
                                        ], [_, { } formatsArgument, { } formatProviderArgument, _]) when inputType.IsReadOnlySpanOfChar()
                                        && formatsType.IsGenericArrayOfString()
                                        && formatProviderType.IsIFormatProvider()
                                        && stylesType.IsDateTimeStyles():

                                        AnalyzeParseExact_ReadOnlyCSpanOfChar_StringArray_IFormatProvider_DateTimeStyles(
                                            consumer,
                                            formatsArgument,
                                            formatProviderArgument);
                                        break;
                                }
                                break;

                            case nameof(DateTimeOffset.TryParseExact):
                                switch (method.Parameters, invocationExpression.TryGetArgumentsInDeclarationOrder())
                                {
                                    case ([
                                            { Type: var inputType },
                                            { Type: var formatType },
                                            { Type: var formatProviderType },
                                            { Type: var stylesType },
                                            { Type: var resultType },
                                        ], [_, { } formatArgument, { } formatProviderArgument, _, _])
                                        when inputType.IsString()
                                        && formatType.IsString()
                                        && formatProviderType.IsIFormatProvider()
                                        && stylesType.IsDateTimeStyles()
                                        && resultType.IsDateTimeOffset():

                                        AnalyzeTryParseExact_String_String_IFormatProvider_DateTimeStyles_DateTimeOffset(
                                            consumer,
                                            formatArgument,
                                            formatProviderArgument);
                                        break;

                                    case ([
                                            { Type: var inputType },
                                            { Type: var formatsType },
                                            { Type: var formatProviderType },
                                            { Type: var stylesType },
                                            { Type: var resultType },
                                        ], [_, { } formatsArgument, { } formatProviderArgument, _, _]) when inputType.IsString()
                                        && formatsType.IsGenericArrayOfString()
                                        && formatProviderType.IsIFormatProvider()
                                        && stylesType.IsDateTimeStyles()
                                        && resultType.IsDateTimeOffset():

                                        AnalyzeTryParseExact_String_StringArray_IFormatProvider_DateTimeStyles_DateTimeOffset(
                                            consumer,
                                            invocationExpression,
                                            formatsArgument,
                                            formatProviderArgument);
                                        break;

                                    case ([
                                            { Type: var inputType },
                                            { Type: var formatsType },
                                            { Type: var formatProviderType },
                                            { Type: var stylesType },
                                            { Type: var resultType },
                                        ], [_, { } formatsArgument, { } formatProviderArgument, _, _]) when inputType.IsReadOnlySpanOfChar()
                                        && formatsType.IsGenericArrayOfString()
                                        && formatProviderType.IsIFormatProvider()
                                        && stylesType.IsDateTimeStyles()
                                        && resultType.IsDateTimeOffset():

                                        AnalyzeTryParseExact_ReadOnlyCSpanOfChar_StringArray_IFormatProvider_DateTimeStyles_DateTimeOffset(
                                            consumer,
                                            formatsArgument,
                                            formatProviderArgument);
                                        break;
                                }
                                break;
                        }
                        break;
                }
                break;
        }
    }
}