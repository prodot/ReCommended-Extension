using System.Globalization;
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
        typeof(RedundantArgumentHint),
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
        public static IReadOnlyList<Parameter> String { get; } = [Parameter.String];

        public static IReadOnlyList<Parameter> String_IFormatProvider { get; } = [Parameter.String, Parameter.IFormatProvider];

        public static IReadOnlyList<Parameter> String_outDateTimeOffset { get; } =
        [
            Parameter.String, Parameter.DateTimeOffset with { Kind = ParameterKind.OUTPUT },
        ];

        public static IReadOnlyList<Parameter> ReadOnlySpanOfChar_outDateTimeOffset { get; } =
        [
            Parameter.ReadOnlySpanOfChar, Parameter.DateTimeOffset with { Kind = ParameterKind.OUTPUT },
        ];

        public static IReadOnlyList<Parameter> String_IFormatProvider_outDateTimeOffset { get; } =
        [
            Parameter.String, Parameter.IFormatProvider, Parameter.DateTimeOffset with { Kind = ParameterKind.OUTPUT },
        ];

        public static IReadOnlyList<Parameter> ReadOnlySpanOfChar_IFormatProvider_outDateTimeOffset { get; } =
        [
            Parameter.ReadOnlySpanOfChar, Parameter.IFormatProvider, Parameter.DateTimeOffset with { Kind = ParameterKind.OUTPUT },
        ];

        public static IReadOnlyList<Parameter> String_String_IFormatProvider { get; } =
        [
            Parameter.String, Parameter.String, Parameter.IFormatProvider,
        ];

        public static IReadOnlyList<Parameter> ReadOnlySpanOfChar_IFormatProvider_DateTimeStyles { get; } =
        [
            Parameter.ReadOnlySpanOfChar, Parameter.IFormatProvider, Parameter.DateTimeStyles,
        ];

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

        public static IReadOnlyList<Parameter> Int32_Int32_Int32_Int32_Int32_Int32_TimeSpan { get; } =
        [
            Parameter.Int32, Parameter.Int32, Parameter.Int32, Parameter.Int32, Parameter.Int32, Parameter.Int32, Parameter.TimeSpan,
        ];

        public static IReadOnlyList<Parameter> Int32_Int32_Int32_Int32_Int32_Int32_Int32_TimeSpan { get; } =
        [
            Parameter.Int32,
            Parameter.Int32,
            Parameter.Int32,
            Parameter.Int32,
            Parameter.Int32,
            Parameter.Int32,
            Parameter.Int32,
            Parameter.TimeSpan,
        ];

        public static IReadOnlyList<Parameter> Int32_Int32_Int32_Int32_Int32_Int32_Int32_Calendar_TimeSpan { get; } =
        [
            Parameter.Int32,
            Parameter.Int32,
            Parameter.Int32,
            Parameter.Int32,
            Parameter.Int32,
            Parameter.Int32,
            Parameter.Int32,
            Parameter.Calendar,
            Parameter.TimeSpan,
        ];
    }

    /// <remarks>
    /// <c>new DateTimeOffset(year, month, day, hour, minute, second, 0, offset)</c> → <c>new DateTimeOffset(year, month, day, hour, minute, second, offset)</c>
    /// </remarks>
    static void Analyze_Ctor_Int32_Int32_Int32_Int32_Int32_Int32_Int32_TimeSpan(
        IHighlightingConsumer consumer,
        IObjectCreationExpression objectCreationExpression,
        ICSharpArgument millisecondArgument)
    {
        if (millisecondArgument.Value.TryGetInt32Constant() == 0
            && PredefinedType.DATETIMEOFFSET_FQN.HasConstructor(
                new ConstructorSignature { Parameters = Parameters.Int32_Int32_Int32_Int32_Int32_Int32_TimeSpan },
                objectCreationExpression.PsiModule))
        {
            consumer.AddHighlighting(new RedundantArgumentHint("Passing 0 is redundant.", millisecondArgument));
        }
    }

    /// <remarks>
    /// <c>new DateTimeOffset(year, month, day, hour, minute, second, millisecond, 0, offset)</c> → <c>new DateTimeOffset(year, month, day, hour, minute, second, millisecond, offset)</c>
    /// </remarks>
    static void Analyze_Ctor_Int32_Int32_Int32_Int32_Int32_Int32_Int32_Int32_TimeSpan(
        IHighlightingConsumer consumer,
        IObjectCreationExpression objectCreationExpression,
        ICSharpArgument microsecondArgument)
    {
        if (microsecondArgument.Value.TryGetInt32Constant() == 0
            && PredefinedType.DATETIMEOFFSET_FQN.HasConstructor(
                new ConstructorSignature { Parameters = Parameters.Int32_Int32_Int32_Int32_Int32_Int32_Int32_TimeSpan },
                objectCreationExpression.PsiModule))
        {
            consumer.AddHighlighting(new RedundantArgumentHint("Passing 0 is redundant.", microsecondArgument));
        }
    }

    /// <remarks>
    /// <c>new DateTimeOffset(year, month, day, hour, minute, second, millisecond, 0, calendar, offset)</c> → <c>new DateTimeOffset(year, month, day, hour, minute, second, millisecond, calendar, offset)</c>
    /// </remarks>
    static void Analyze_Ctor_Int32_Int32_Int32_Int32_Int32_Int32_Int32_Int32_Calendar_TimeSpan(
        IHighlightingConsumer consumer,
        IObjectCreationExpression objectCreationExpression,
        ICSharpArgument microsecondArgument)
    {
        if (microsecondArgument.Value.TryGetInt32Constant() == 0
            && PredefinedType.DATETIMEOFFSET_FQN.HasConstructor(
                new ConstructorSignature { Parameters = Parameters.Int32_Int32_Int32_Int32_Int32_Int32_Int32_Calendar_TimeSpan },
                objectCreationExpression.PsiModule))
        {
            consumer.AddHighlighting(new RedundantArgumentHint("Passing 0 is redundant.", microsecondArgument));
        }
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
    /// <c>DateTimeOffset.Parse(input, null)</c> → <c>DateTimeOffset.Parse(input)</c>
    /// </remarks>
    static void AnalyzeParse_String_IFormatProvider(
        IHighlightingConsumer consumer,
        IInvocationExpression invocationExpression,
        ICSharpArgument formatProviderArgument)
    {
        if (formatProviderArgument.Value.IsDefaultValue()
            && PredefinedType.DATETIMEOFFSET_FQN.HasMethod(
                new MethodSignature { Name = nameof(DateTimeOffset.Parse), Parameters = Parameters.String, IsStatic = true },
                invocationExpression.PsiModule))
        {
            consumer.AddHighlighting(new RedundantArgumentHint("Passing null is redundant.", formatProviderArgument));
        }
    }

    /// <remarks>
    /// <c>DateTimeOffset.Parse(input, provider, DateTimeStyles.None)</c> → <c>DateTimeOffset.Parse(input, provider)</c>
    /// </remarks>
    static void AnalyzeParse_String_IFormatProvider_DateTimeStyles(
        IHighlightingConsumer consumer,
        IInvocationExpression invocationExpression,
        ICSharpArgument stylesArgument)
    {
        if (stylesArgument.Value.TryGetDateTimeStylesConstant() == DateTimeStyles.None
            && PredefinedType.DATETIMEOFFSET_FQN.HasMethod(
                new MethodSignature { Name = nameof(DateTimeOffset.Parse), Parameters = Parameters.String_IFormatProvider, IsStatic = true },
                invocationExpression.PsiModule))
        {
            consumer.AddHighlighting(
                new RedundantArgumentHint($"Passing {nameof(DateTimeStyles)}.{nameof(DateTimeStyles.None)} is redundant.", stylesArgument));
        }
    }

    /// <remarks>
    /// <c>DateTimeOffset.Parse(s, null)</c> → <c>DateTimeOffset.Parse(s)</c> (.NET Core 2.1)
    /// </remarks>
    static void AnalyzeParse_ReadOnlySpanOfChar_IFormatProvider(
        IHighlightingConsumer consumer,
        IInvocationExpression invocationExpression,
        ICSharpArgument providerArgument)
    {
        if (providerArgument.Value.IsDefaultValue()
            && PredefinedType.DATETIMEOFFSET_FQN.HasMethod(
                new MethodSignature
                {
                    Name = nameof(DateTimeOffset.Parse),
                    Parameters = Parameters.ReadOnlySpanOfChar_IFormatProvider_DateTimeStyles,
                    IsStatic = true,
                },
                invocationExpression.PsiModule))
        {
            consumer.AddHighlighting(new RedundantArgumentHint("Passing null is redundant.", providerArgument));
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
    /// <c>DateTimeOffset.ParseExact(input, "R", formatProvider, styles)</c> → <c>DateTimeOffset.ParseExact(input, "R", null, styles)</c><para/>
    /// <c>DateTimeOffset.ParseExact(input, format, formatProvider, DateTimeStyles.None)</c> → <c>DateTimeOffset.ParseExact(input, format, formatProvider)</c>
    /// </remarks>
    static void AnalyzeParseExact_String_String_IFormatProvider_DateTimeStyles(
        IHighlightingConsumer consumer,
        IInvocationExpression invocationExpression,
        ICSharpArgument formatArgument,
        ICSharpArgument formatProviderArgument,
        ICSharpArgument stylesArgument)
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

        if (stylesArgument.Value.TryGetDateTimeStylesConstant() == DateTimeStyles.None
            && PredefinedType.DATETIMEOFFSET_FQN.HasMethod(
                new MethodSignature
                {
                    Name = nameof(DateTimeOffset.ParseExact), Parameters = Parameters.String_String_IFormatProvider, IsStatic = true,
                },
                invocationExpression.PsiModule))
        {
            consumer.AddHighlighting(
                new RedundantArgumentHint($"Passing {nameof(DateTimeStyles)}.{nameof(DateTimeStyles.None)} is redundant.", stylesArgument));
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
    /// <c>DateTimeOffset.TryParse(s, null, out result)</c> → <c>DateTimeOffset.TryParse(s, out result)</c>
    /// </remarks>
    static void AnalyzeTryParse_String_IFormatProvider_DateTimeOffset(
        IHighlightingConsumer consumer,
        IInvocationExpression invocationExpression,
        ICSharpArgument providerArgument)
    {
        if (providerArgument.Value.IsDefaultValue()
            && PredefinedType.DATETIMEOFFSET_FQN.HasMethod(
                new MethodSignature { Name = nameof(DateTimeOffset.TryParse), Parameters = Parameters.String_outDateTimeOffset, IsStatic = true },
                invocationExpression.PsiModule))
        {
            consumer.AddHighlighting(new RedundantArgumentHint("Passing null is redundant.", providerArgument));
        }
    }

    /// <remarks>
    /// <c>DateTimeOffset.TryParse(s, null, out result)</c> → <c>DateTimeOffset.TryParse(s, out result)</c> (.NET Core 2.1)
    /// </remarks>
    static void AnalyzeTryParse_ReadOnlySpanOfChar_IFormatProvider_DateTimeOffset(
        IHighlightingConsumer consumer,
        IInvocationExpression invocationExpression,
        ICSharpArgument providerArgument)
    {
        if (providerArgument.Value.IsDefaultValue()
            && PredefinedType.DATETIMEOFFSET_FQN.HasMethod(
                new MethodSignature
                {
                    Name = nameof(DateTimeOffset.TryParse), Parameters = Parameters.ReadOnlySpanOfChar_outDateTimeOffset, IsStatic = true,
                },
                invocationExpression.PsiModule))
        {
            consumer.AddHighlighting(new RedundantArgumentHint("Passing null is redundant.", providerArgument));
        }
    }

    /// <remarks>
    /// <c>DateTimeOffset.TryParse(input, formatProvider, DateTimeStyles.None, out result)</c> → <c>DateTimeOffset.TryParse(input, formatProvider, out result)</c> (.NET 7)
    /// </remarks>
    static void AnalyzeTryParse_String_IFormatProvider_DateTimeStyles_DateTimeOffset(
        IHighlightingConsumer consumer,
        IInvocationExpression invocationExpression,
        ICSharpArgument stylesArgument)
    {
        if (stylesArgument.Value.TryGetDateTimeStylesConstant() == DateTimeStyles.None
            && PredefinedType.DATETIMEOFFSET_FQN.HasMethod(
                new MethodSignature
                {
                    Name = nameof(DateTimeOffset.TryParse), Parameters = Parameters.String_IFormatProvider_outDateTimeOffset, IsStatic = true,
                },
                invocationExpression.PsiModule))
        {
            consumer.AddHighlighting(
                new RedundantArgumentHint($"Passing {nameof(DateTimeStyles)}.{nameof(DateTimeStyles.None)} is redundant.", stylesArgument));
        }
    }

    /// <remarks>
    /// <c>DateTimeOffset.TryParse(input, formatProvider, DateTimeStyles.None, out result)</c> → <c>DateTimeOffset.TryParse(input, formatProvider, out result)</c> (.NET 7)
    /// </remarks>
    static void AnalyzeTryParse_ReadOnlySpanOfChar_IFormatProvider_DateTimeStyles_DateTimeOffset(
        IHighlightingConsumer consumer,
        IInvocationExpression invocationExpression,
        ICSharpArgument stylesArgument)
    {
        if (stylesArgument.Value.TryGetDateTimeStylesConstant() == DateTimeStyles.None
            && PredefinedType.DATETIMEOFFSET_FQN.HasMethod(
                new MethodSignature
                {
                    Name = nameof(DateTimeOffset.TryParse),
                    Parameters = Parameters.ReadOnlySpanOfChar_IFormatProvider_outDateTimeOffset,
                    IsStatic = true,
                },
                invocationExpression.PsiModule))
        {
            consumer.AddHighlighting(
                new RedundantArgumentHint($"Passing {nameof(DateTimeStyles)}.{nameof(DateTimeStyles.None)} is redundant.", stylesArgument));
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
            case IObjectCreationExpression { ConstructorReference: var reference } objectCreationExpression
                when reference.Resolve().DeclaredElement is IConstructor
                {
                    AccessibilityDomain.DomainType: AccessibilityDomain.AccessibilityDomainType.PUBLIC, IsStatic: false,
                } constructor
                && constructor.ContainingType.IsClrType(PredefinedType.DATETIMEOFFSET_FQN):

                switch (constructor.Parameters, objectCreationExpression.TryGetArgumentsInDeclarationOrder())
                {
                    case ([
                            { Type: var yearType },
                            { Type: var monthType },
                            { Type: var dayType },
                            { Type: var hourType },
                            { Type: var minuteType },
                            { Type: var secondType },
                            { Type: var millisecondType },
                            { Type: var offsetType },
                        ], [_, _, _, _, _, _, { } millisecondArgument, _])
                        when yearType.IsInt()
                        && monthType.IsInt()
                        && dayType.IsInt()
                        && hourType.IsInt()
                        && minuteType.IsInt()
                        && secondType.IsInt()
                        && millisecondType.IsInt()
                        && offsetType.IsTimeSpan():

                        Analyze_Ctor_Int32_Int32_Int32_Int32_Int32_Int32_Int32_TimeSpan(consumer, objectCreationExpression, millisecondArgument);
                        break;

                    case ([
                            { Type: var yearType },
                            { Type: var monthType },
                            { Type: var dayType },
                            { Type: var hourType },
                            { Type: var minuteType },
                            { Type: var secondType },
                            { Type: var millisecondType },
                            { Type: var microsecondType },
                            { Type: var offsetType },
                        ], [_, _, _, _, _, _, _, { } microsecondArgument, _])
                        when yearType.IsInt()
                        && monthType.IsInt()
                        && dayType.IsInt()
                        && hourType.IsInt()
                        && minuteType.IsInt()
                        && secondType.IsInt()
                        && millisecondType.IsInt()
                        && microsecondType.IsInt()
                        && offsetType.IsTimeSpan():

                        Analyze_Ctor_Int32_Int32_Int32_Int32_Int32_Int32_Int32_Int32_TimeSpan(
                            consumer,
                            objectCreationExpression,
                            microsecondArgument);
                        break;

                    case ([
                            { Type: var yearType },
                            { Type: var monthType },
                            { Type: var dayType },
                            { Type: var hourType },
                            { Type: var minuteType },
                            { Type: var secondType },
                            { Type: var millisecondType },
                            { Type: var microsecondType },
                            { Type: var calendarType },
                            { Type: var offsetType },
                        ], [_, _, _, _, _, _, _, { } microsecondArgument, _, _])
                        when yearType.IsInt()
                        && monthType.IsInt()
                        && dayType.IsInt()
                        && hourType.IsInt()
                        && minuteType.IsInt()
                        && secondType.IsInt()
                        && millisecondType.IsInt()
                        && microsecondType.IsInt()
                        && calendarType.IsCalendar()
                        && offsetType.IsTimeSpan():

                        Analyze_Ctor_Int32_Int32_Int32_Int32_Int32_Int32_Int32_Int32_Calendar_TimeSpan(
                            consumer,
                            objectCreationExpression,
                            microsecondArgument);
                        break;
                }
                break;

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

                            case nameof(DateTimeOffset.Parse):
                                switch (method.Parameters, invocationExpression.TryGetArgumentsInDeclarationOrder())
                                {
                                    case ([{ Type: var inputType }, { Type: var formatProviderType }], [_, { } formatProviderArgument])
                                        when inputType.IsString() && formatProviderType.IsIFormatProvider():

                                        AnalyzeParse_String_IFormatProvider(consumer, invocationExpression, formatProviderArgument);
                                        break;

                                    case ([{ Type: var inputType }, { Type: var formatProviderType }, { Type: var stylesType }], [
                                        _, _, { } stylesArgument,
                                    ]) when inputType.IsString() && formatProviderType.IsIFormatProvider() && stylesType.IsDateTimeStyles():
                                        AnalyzeParse_String_IFormatProvider_DateTimeStyles(consumer, invocationExpression, stylesArgument);
                                        break;

                                    case ([{ Type: var sType }, { Type: var providerType }], [_, { } providerArgument])
                                        when sType.IsReadOnlySpanOfChar() && providerType.IsIFormatProvider():

                                        AnalyzeParse_ReadOnlySpanOfChar_IFormatProvider(consumer, invocationExpression, providerArgument);
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
                                        ], [_, { } formatArgument, { } formatProviderArgument, { } stylesArgument])
                                        when inputType.IsString()
                                        && formatType.IsString()
                                        && formatProviderType.IsIFormatProvider()
                                        && stylesType.IsDateTimeStyles():

                                        AnalyzeParseExact_String_String_IFormatProvider_DateTimeStyles(
                                            consumer,
                                            invocationExpression,
                                            formatArgument,
                                            formatProviderArgument,
                                            stylesArgument);
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

                            case nameof(DateTimeOffset.TryParse):
                                switch (method.Parameters, invocationExpression.TryGetArgumentsInDeclarationOrder())
                                {
                                    case ([{ Type: var sType }, { Type: var providerType }, { Type: var resultType }], [_, { } providerArgument, _])
                                        when sType.IsString() && providerType.IsIFormatProvider() && resultType.IsDateTimeOffset():

                                        AnalyzeTryParse_String_IFormatProvider_DateTimeOffset(consumer, invocationExpression, providerArgument);
                                        break;

                                    case ([{ Type: var sType }, { Type: var providerType }, { Type: var resultType }], [_, { } providerArgument, _])
                                        when sType.IsReadOnlySpanOfChar() && providerType.IsIFormatProvider() && resultType.IsDateTimeOffset():

                                        AnalyzeTryParse_ReadOnlySpanOfChar_IFormatProvider_DateTimeOffset(
                                            consumer,
                                            invocationExpression,
                                            providerArgument);
                                        break;

                                    case ([
                                            { Type: var inputType },
                                            { Type: var formatProviderType },
                                            { Type: var stylesType },
                                            { Type: var resultType },
                                        ], [_, _, { } stylesArgument, _])
                                        when inputType.IsString()
                                        && formatProviderType.IsIFormatProvider()
                                        && stylesType.IsDateTimeStyles()
                                        && resultType.IsDateTimeOffset():

                                        AnalyzeTryParse_String_IFormatProvider_DateTimeStyles_DateTimeOffset(
                                            consumer,
                                            invocationExpression,
                                            stylesArgument);
                                        break;

                                    case ([
                                            { Type: var inputType },
                                            { Type: var formatProviderType },
                                            { Type: var stylesType },
                                            { Type: var resultType },
                                        ], [_, _, { } stylesArgument, _]) when inputType.IsReadOnlySpanOfChar()
                                        && formatProviderType.IsIFormatProvider()
                                        && stylesType.IsDateTimeStyles()
                                        && resultType.IsDateTimeOffset():

                                        AnalyzeTryParse_ReadOnlySpanOfChar_IFormatProvider_DateTimeStyles_DateTimeOffset(
                                            consumer,
                                            invocationExpression,
                                            stylesArgument);
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