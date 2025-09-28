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
        typeof(UseExpressionResultSuggestion),
        typeof(RedundantArgumentHint),
        typeof(UseBinaryOperatorSuggestion),
        typeof(RedundantArgumentRangeHint),
        typeof(UseOtherArgumentSuggestion),
        typeof(RedundantElementHint),
    ])]
public sealed class TimeOnlyAnalyzer : ElementProblemAnalyzer<ICSharpInvocationInfo>
{
    [SuppressMessage("ReSharper", "InconsistentNaming", Justification = "Underscore character used intentionally as a separator.")]
    static class ParameterTypes
    {
        public static IReadOnlyList<ParameterType> TimeSpan { get; } = [new() { ClrTypeName = PredefinedType.TIMESPAN_FQN }];

        public static IReadOnlyList<ParameterType> Double { get; } = [new() { ClrTypeName = PredefinedType.DOUBLE_FQN }];

        public static IReadOnlyList<ParameterType> String { get; } = [new() { ClrTypeName = PredefinedType.STRING_FQN }];

        public static IReadOnlyList<ParameterType> String_String { get; } =
        [
            new() { ClrTypeName = PredefinedType.STRING_FQN }, new() { ClrTypeName = PredefinedType.STRING_FQN },
        ];

        public static IReadOnlyList<ParameterType> String_StringArray { get; } =
        [
            new() { ClrTypeName = PredefinedType.STRING_FQN }, new ArrayParameterType { ClrTypeName = PredefinedType.STRING_FQN },
        ];

        public static IReadOnlyList<ParameterType> String_TimeOnly { get; } =
        [
            new() { ClrTypeName = PredefinedType.STRING_FQN }, new() { ClrTypeName = PredefinedType.TIME_ONLY_FQN },
        ];

        public static IReadOnlyList<ParameterType> ReadOnlySpanOfT_TimeOnly { get; } =
        [
            new GenericParameterType { ClrTypeName = PredefinedType.SYSTEM_READ_ONLY_SPAN_FQN },
            new() { ClrTypeName = PredefinedType.TIME_ONLY_FQN },
        ];

        public static IReadOnlyList<ParameterType> ReadOnlySpanOfT_StringArray { get; } =
        [
            new GenericParameterType { ClrTypeName = PredefinedType.SYSTEM_READ_ONLY_SPAN_FQN },
            new ArrayParameterType { ClrTypeName = PredefinedType.STRING_FQN },
        ];

        public static IReadOnlyList<ParameterType> String_String_TimeOnly { get; } =
        [
            new() { ClrTypeName = PredefinedType.STRING_FQN },
            new() { ClrTypeName = PredefinedType.STRING_FQN },
            new() { ClrTypeName = PredefinedType.TIME_ONLY_FQN },
        ];

        public static IReadOnlyList<ParameterType> String_IFormatProvider_TimeOnly { get; } =
        [
            new() { ClrTypeName = PredefinedType.STRING_FQN },
            new() { ClrTypeName = PredefinedType.IFORMATPROVIDER_FQN },
            new() { ClrTypeName = PredefinedType.TIME_ONLY_FQN },
        ];

        public static IReadOnlyList<ParameterType> String_StringArray_TimeOnly { get; } =
        [
            new() { ClrTypeName = PredefinedType.STRING_FQN },
            new ArrayParameterType { ClrTypeName = PredefinedType.STRING_FQN },
            new() { ClrTypeName = PredefinedType.TIME_ONLY_FQN },
        ];

        public static IReadOnlyList<ParameterType> ReadOnlySpanOfT_IFormatProvider_DateTimeStyles { get; } =
        [
            new GenericParameterType { ClrTypeName = PredefinedType.SYSTEM_READ_ONLY_SPAN_FQN },
            new() { ClrTypeName = PredefinedType.IFORMATPROVIDER_FQN },
            new() { ClrTypeName = ClrTypeNames.DateTimeStyles },
        ];

        public static IReadOnlyList<ParameterType> ReadOnlySpanOfT_IFormatProvider_TimeOnly { get; } =
        [
            new GenericParameterType { ClrTypeName = PredefinedType.SYSTEM_READ_ONLY_SPAN_FQN },
            new() { ClrTypeName = PredefinedType.IFORMATPROVIDER_FQN },
            new() { ClrTypeName = PredefinedType.TIME_ONLY_FQN },
        ];

        public static IReadOnlyList<ParameterType> ReadOnlySpanOfT_ReadOnlySpanOfT_TimeOnly { get; } =
        [
            new GenericParameterType { ClrTypeName = PredefinedType.SYSTEM_READ_ONLY_SPAN_FQN },
            new GenericParameterType { ClrTypeName = PredefinedType.SYSTEM_READ_ONLY_SPAN_FQN },
            new() { ClrTypeName = PredefinedType.TIME_ONLY_FQN },
        ];

        public static IReadOnlyList<ParameterType> ReadOnlySpanOfT_StringArray_TimeOnly { get; } =
        [
            new GenericParameterType { ClrTypeName = PredefinedType.SYSTEM_READ_ONLY_SPAN_FQN },
            new ArrayParameterType { ClrTypeName = PredefinedType.STRING_FQN },
            new() { ClrTypeName = PredefinedType.TIME_ONLY_FQN },
        ];

        public static IReadOnlyList<ParameterType> String_String_IFormatProvider_DateTimeStyles { get; } =
        [
            new() { ClrTypeName = PredefinedType.STRING_FQN },
            new() { ClrTypeName = PredefinedType.STRING_FQN },
            new() { ClrTypeName = PredefinedType.IFORMATPROVIDER_FQN },
            new() { ClrTypeName = ClrTypeNames.DateTimeStyles },
        ];

        public static IReadOnlyList<ParameterType> String_String_IFormatProvider_DateTimeStyles_TimeOnly { get; } =
        [
            new() { ClrTypeName = PredefinedType.STRING_FQN },
            new() { ClrTypeName = PredefinedType.STRING_FQN },
            new() { ClrTypeName = PredefinedType.IFORMATPROVIDER_FQN },
            new() { ClrTypeName = ClrTypeNames.DateTimeStyles },
            new() { ClrTypeName = PredefinedType.TIME_ONLY_FQN },
        ];
    }

    /// <remarks>
    /// <c>new TimeOnly(0)</c> → <c>TimeOnly.MinValue</c>
    /// </remarks>
    static void Analyze_Ctor_Int64(IHighlightingConsumer consumer, IObjectCreationExpression objectCreationExpression, ICSharpArgument ticksArgument)
    {
        if (!objectCreationExpression.IsUsedAsStatement() && ticksArgument.Value.TryGetInt64Constant() == 0)
        {
            consumer.AddHighlighting(
                new UseExpressionResultSuggestion("The expression is always TimeOnly.MinValue.", objectCreationExpression, "TimeOnly.MinValue")); // todo: 2x nameof(TimeOnly), nameof(TimeOnly.MinValue) when available
        }
    }

    /// <remarks>
    /// <c>new TimeOnly(0, 0)</c> → <c>TimeOnly.MinValue</c>
    /// </remarks>
    static void Analyze_Ctor_Int32_Int32(
        IHighlightingConsumer consumer,
        IObjectCreationExpression objectCreationExpression,
        ICSharpArgument hourArgument,
        ICSharpArgument minuteArgument)
    {
        if (!objectCreationExpression.IsUsedAsStatement()
            && hourArgument.Value.TryGetInt32Constant() == 0
            && minuteArgument.Value.TryGetInt32Constant() == 0)
        {
            consumer.AddHighlighting(
                new UseExpressionResultSuggestion("The expression is always TimeOnly.MinValue.", objectCreationExpression, "TimeOnly.MinValue")); // todo: 2x nameof(TimeOnly), nameof(TimeOnly.MinValue) when available
        }
    }

    /// <remarks>
    /// <c>new TimeOnly(0, 0, 0)</c> → <c>TimeOnly.MinValue</c>
    /// </remarks>
    static void Analyze_Ctor_Int32_Int32_Int32(
        IHighlightingConsumer consumer,
        IObjectCreationExpression objectCreationExpression,
        ICSharpArgument hourArgument,
        ICSharpArgument minuteArgument,
        ICSharpArgument secondArgument)
    {
        if (!objectCreationExpression.IsUsedAsStatement()
            && hourArgument.Value.TryGetInt32Constant() == 0
            && minuteArgument.Value.TryGetInt32Constant() == 0
            && secondArgument.Value.TryGetInt32Constant() == 0)
        {
            consumer.AddHighlighting(
                new UseExpressionResultSuggestion("The expression is always TimeOnly.MinValue.", objectCreationExpression, "TimeOnly.MinValue")); // todo: 2x nameof(TimeOnly), nameof(TimeOnly.MinValue) when available
        }
    }

    /// <remarks>
    /// <c>new TimeOnly(0, 0, 0, 0)</c> → <c>TimeOnly.MinValue</c>
    /// </remarks>
    static void Analyze_Ctor_Int32_Int32_Int32_Int32(
        IHighlightingConsumer consumer,
        IObjectCreationExpression objectCreationExpression,
        ICSharpArgument hourArgument,
        ICSharpArgument minuteArgument,
        ICSharpArgument secondArgument,
        ICSharpArgument millisecondArgument)
    {
        if (!objectCreationExpression.IsUsedAsStatement()
            && hourArgument.Value.TryGetInt32Constant() == 0
            && minuteArgument.Value.TryGetInt32Constant() == 0
            && secondArgument.Value.TryGetInt32Constant() == 0
            && millisecondArgument.Value.TryGetInt32Constant() == 0)
        {
            consumer.AddHighlighting(
                new UseExpressionResultSuggestion("The expression is always TimeOnly.MinValue.", objectCreationExpression, "TimeOnly.MinValue")); // todo: 2x nameof(TimeOnly), nameof(TimeOnly.MinValue) when available
        }
    }

    /// <remarks>
    /// <c>new TimeOnly(0, 0, 0, 0, 0)</c> → <c>TimeOnly.MinValue</c>
    /// </remarks>
    static void Analyze_Ctor_Int32_Int32_Int32_Int32_Int32(
        IHighlightingConsumer consumer,
        IObjectCreationExpression objectCreationExpression,
        ICSharpArgument hourArgument,
        ICSharpArgument minuteArgument,
        ICSharpArgument secondArgument,
        ICSharpArgument millisecondArgument,
        ICSharpArgument microsecondArgument)
    {
        if (!objectCreationExpression.IsUsedAsStatement()
            && hourArgument.Value.TryGetInt32Constant() == 0
            && minuteArgument.Value.TryGetInt32Constant() == 0
            && secondArgument.Value.TryGetInt32Constant() == 0
            && millisecondArgument.Value.TryGetInt32Constant() == 0
            && microsecondArgument.Value.TryGetInt32Constant() == 0)
        {
            consumer.AddHighlighting(
                new UseExpressionResultSuggestion("The expression is always TimeOnly.MinValue.", objectCreationExpression, "TimeOnly.MinValue")); // todo: 2x nameof(TimeOnly), nameof(TimeOnly.MinValue) when available
        }
    }

    /// <remarks>
    /// <c>timeOnly.Add(value, out _)</c> → <c>timeOnly.Add(value)</c>
    /// </remarks>
    static void AnalyzeAdd_TimeSpan_Int32(
        IHighlightingConsumer consumer,
        IInvocationExpression invocationExpression,
        ICSharpArgument wrappedDaysArgument)
    {
        if (wrappedDaysArgument.IsDiscard()
            && PredefinedType.TIME_ONLY_FQN.HasMethod(
                new MethodSignature { Name = "Add", ParameterTypes = ParameterTypes.TimeSpan }, // todo: nameof(TimeOnly.Add) when available
                invocationExpression.PsiModule))
        {
            consumer.AddHighlighting(new RedundantArgumentHint("Discarding is redundant.", wrappedDaysArgument));
        }
    }

    /// <remarks>
    /// <c>timeOnly.AddHours(value, out _)</c> → <c>timeOnly.AddHours(value)</c>
    /// </remarks>
    static void AnalyzeAddHours_Double_Int32(
        IHighlightingConsumer consumer,
        IInvocationExpression invocationExpression,
        ICSharpArgument wrappedDaysArgument)
    {
        if (wrappedDaysArgument.IsDiscard()
            && PredefinedType.TIME_ONLY_FQN.HasMethod(
                new MethodSignature { Name = "AddHours", ParameterTypes = ParameterTypes.Double }, // todo: nameof(TimeOnly.AddHours) when available
                invocationExpression.PsiModule))
        {
            consumer.AddHighlighting(new RedundantArgumentHint("Discarding is redundant.", wrappedDaysArgument));
        }
    }

    /// <remarks>
    /// <c>timeOnly.AddMinutes(value, out _)</c> → <c>timeOnly.AddMinutes(value)</c>
    /// </remarks>
    static void AnalyzeAddMinutes_Double_Int32(
        IHighlightingConsumer consumer,
        IInvocationExpression invocationExpression,
        ICSharpArgument wrappedDaysArgument)
    {
        if (wrappedDaysArgument.IsDiscard()
            && PredefinedType.TIME_ONLY_FQN.HasMethod(
                new MethodSignature { Name = "AddMinutes", ParameterTypes = ParameterTypes.Double }, // todo: nameof(TimeOnly.AddMinutes) when available
                invocationExpression.PsiModule))
        {
            consumer.AddHighlighting(new RedundantArgumentHint("Discarding is redundant.", wrappedDaysArgument));
        }
    }

    /// <remarks>
    /// <c>timeOnly.Equals(value)</c> → <c>timeOnly == value</c>
    /// </remarks>
    static void AnalyzeEquals_TimeOnly(
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
    /// <c>timeOnly.Equals(null)</c> → <c>false</c>
    /// </remarks>
    static void AnalyzeEquals_Object(IHighlightingConsumer consumer, IInvocationExpression invocationExpression, ICSharpArgument valueArgument)
    {
        if (!invocationExpression.IsUsedAsStatement() && valueArgument.Value.IsDefaultValue())
        {
            consumer.AddHighlighting(new UseExpressionResultSuggestion("The expression is always false.", invocationExpression, "false"));
        }
    }

    /// <remarks>
    /// <c>TimeOnly.Parse(s, null, DateTimeStyles.None)</c> → <c>TimeOnly.Parse(s)</c>
    /// </remarks>
    static void AnalyzeParse_String_IFormatProvider_DateTimeStyles(
        IHighlightingConsumer consumer,
        IInvocationExpression invocationExpression,
        ICSharpArgument providerArgument,
        ICSharpArgument? styleArgument)
    {
        if (providerArgument.Value.IsDefaultValue()
            && (styleArgument == null || styleArgument.Value.TryGetDateTimeStylesConstant() == DateTimeStyles.None)
            && PredefinedType.TIME_ONLY_FQN.HasMethod(
                new MethodSignature { Name = "Parse", ParameterTypes = ParameterTypes.String, IsStatic = true }, // todo: nameof(TimeOnly.Parse) when available
                invocationExpression.PsiModule))
        {
            consumer.AddHighlighting(
                styleArgument is { }
                    ? new RedundantArgumentRangeHint(
                        $"Passing 'null, {nameof(DateTimeStyles)}.{nameof(DateTimeStyles.None)}' is redundant.",
                        providerArgument,
                        styleArgument)
                    : new RedundantArgumentHint("Passing null is redundant.", providerArgument));
        }
    }

    /// <remarks>
    /// <c>TimeOnly.Parse(s, null)</c> → <c>TimeOnly.Parse(s)</c>
    /// </remarks>
    static void AnalyzeParse_String_IFormatProvider(
        IHighlightingConsumer consumer,
        IInvocationExpression invocationExpression,
        ICSharpArgument providerArgument)
    {
        if (providerArgument.Value.IsDefaultValue()
            && PredefinedType.TIME_ONLY_FQN.HasMethod(
                new MethodSignature { Name = "Parse", ParameterTypes = ParameterTypes.String, IsStatic = true }, // todo: nameof(TimeOnly.Parse) when available
                invocationExpression.PsiModule))
        {
            consumer.AddHighlighting(new RedundantArgumentHint("Passing null is redundant.", providerArgument));
        }
    }

    /// <remarks>
    /// <c>TimeOnly.Parse(s, null)</c> → <c>TimeOnly.Parse(s)</c>
    /// </remarks>
    static void AnalyzeParse_ReadOnlySpanOfChar_IFormatProvider(
        IHighlightingConsumer consumer,
        IInvocationExpression invocationExpression,
        ICSharpArgument providerArgument)
    {
        if (providerArgument.Value.IsDefaultValue()
            && PredefinedType.TIME_ONLY_FQN.HasMethod(
                new MethodSignature
                {
                    Name = "Parse", ParameterTypes = ParameterTypes.ReadOnlySpanOfT_IFormatProvider_DateTimeStyles, IsStatic = true, // todo: nameof(TimeOnly.Parse) when available
                },
                invocationExpression.PsiModule))
        {
            consumer.AddHighlighting(new RedundantArgumentHint("Passing null is redundant.", providerArgument));
        }
    }

    /// <remarks>
    /// <c>TimeOnly.ParseExact(s, format, null, DateTimeStyles.None)</c> → <c>TimeOnly.ParseExact(s, format)</c>
    /// <c>TimeOnly.ParseExact(s, "R", provider, style)</c> → <c>TimeOnly.ParseExact(s, "R", null, style)</c>
    /// </remarks>
    static void AnalyzeParseExact_String_String_IFormatProvider_DateTimeStyles(
        IHighlightingConsumer consumer,
        IInvocationExpression invocationExpression,
        ICSharpArgument formatArgument,
        ICSharpArgument providerArgument,
        ICSharpArgument? styleArgument)
    {
        if (providerArgument.Value.IsDefaultValue())
        {
            if ((styleArgument == null || styleArgument.Value.TryGetDateTimeStylesConstant() == DateTimeStyles.None)
                && PredefinedType.TIME_ONLY_FQN.HasMethod(
                    new MethodSignature { Name = "ParseExact", ParameterTypes = ParameterTypes.String_String, IsStatic = true }, // todo: nameof(TimeOnly.ParseExact) when available
                    invocationExpression.PsiModule))
            {
                consumer.AddHighlighting(
                    styleArgument is { }
                        ? new RedundantArgumentRangeHint(
                            $"Passing 'null, {nameof(DateTimeStyles)}.{nameof(DateTimeStyles.None)}' is redundant.",
                            providerArgument,
                            styleArgument)
                        : new RedundantArgumentHint("Passing null is redundant.", providerArgument));
            }
        }
        else
        {
            if (formatArgument.Value.TryGetStringConstant() is "o" or "O" or "r" or "R" && providerArgument.Value is { })
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

    /// <remarks>
    /// <c>TimeOnly.ParseExact(s, [format])</c> → <c>TimeOnly.ParseExact(s, format)</c><para/>
    /// <c>TimeOnly.ParseExact(s, ["r", "R"])</c> → <c>TimeOnly.ParseExact(s, ["r"])</c>
    /// </remarks>
    static void AnalyzeParseExact_String_StringArray(
        IHighlightingConsumer consumer,
        IInvocationExpression invocationExpression,
        ICSharpArgument formatsArgument)
    {
        switch (CollectionCreation.TryFrom(formatsArgument.Value))
        {
            case { Count: 1 } collectionCreation when PredefinedType.TIME_ONLY_FQN.HasMethod(
                new MethodSignature { Name = "ParseExact", ParameterTypes = ParameterTypes.String_String, IsStatic = true }, // todo: nameof(TimeOnly.ParseExact) when available
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
                    if (s is "o" or "O" && (set.Contains("o") || set.Contains("O")) || s is "r" or "R" && (set.Contains("r") || set.Contains("R")))
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
    /// <c>TimeOnly.ParseExact(s, formats, null, DateTimeStyles.None)</c> → <c>TimeOnly.ParseExact(s, formats)</c><para/>
    /// <c>TimeOnly.ParseExact(s, [format], provider, styles)</c> → <c>TimeOnly.ParseExact(s, format, provider, styles)</c><para/>
    /// <c>TimeOnly.ParseExact(s, ["r", "R"], provider, styles)</c> → <c>TimeOnly.ParseExact(s, ["r"], provider, styles)</c><para/>
    /// <c>TimeOnly.ParseExact(s, ["o", "R"], provider, styles)</c> → <c>TimeOnly.ParseExact(s, ["o", "R"], null, styles)</c>
    /// </remarks>
    static void AnalyzeParseExact_String_StringArray_IFormatProvider_DateTimeStyles(
        IHighlightingConsumer consumer,
        IInvocationExpression invocationExpression,
        ICSharpArgument formatsArgument,
        ICSharpArgument providerArgument,
        ICSharpArgument? styleArgument)
    {
        if (providerArgument.Value.IsDefaultValue()
            && (styleArgument == null || styleArgument.Value.TryGetDateTimeStylesConstant() == DateTimeStyles.None)
            && PredefinedType.TIME_ONLY_FQN.HasMethod(
                new MethodSignature { Name = "ParseExact", ParameterTypes = ParameterTypes.String_StringArray, IsStatic = true }, // todo: nameof(TimeOnly.ParseExact) when available
                invocationExpression.PsiModule))
        {
            consumer.AddHighlighting(
                styleArgument is { }
                    ? new RedundantArgumentRangeHint(
                        $"Passing 'null, {nameof(DateTimeStyles)}.{nameof(DateTimeStyles.None)}' is redundant.",
                        providerArgument,
                        styleArgument)
                    : new RedundantArgumentHint("Passing null is redundant.", providerArgument));
        }

        switch (CollectionCreation.TryFrom(formatsArgument.Value))
        {
            case { Count: 1 } collectionCreation when PredefinedType.TIME_ONLY_FQN.HasMethod(
                new MethodSignature
                {
                    Name = "ParseExact", ParameterTypes = ParameterTypes.String_String_IFormatProvider_DateTimeStyles, IsStatic = true, // todo: nameof(TimeOnly.ParseExact) when available
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
                    if (s is "o" or "O" && (set.Contains("o") || set.Contains("O")) || s is "r" or "R" && (set.Contains("r") || set.Contains("R")))
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
    /// <c>TimeOnly.ParseExact(s, ["r", "R"])</c> → <c>TimeOnly.ParseExact(s, ["r"])</c>
    /// </remarks>
    static void AnalyzeParseExact_ReadOnlySpanOfChar_StringArray(IHighlightingConsumer consumer, ICSharpArgument formatsArgument)
    {
        if (CollectionCreation.TryFrom(formatsArgument.Value) is { Count: > 1 } collectionCreation)
        {
            var set = new HashSet<string>(collectionCreation.Count, StringComparer.Ordinal);

            foreach (var (element, s) in collectionCreation.ElementsWithStringConstants)
            {
                if (s is "o" or "O" && (set.Contains("o") || set.Contains("O")) || s is "r" or "R" && (set.Contains("r") || set.Contains("R")))
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
    /// <c>TimeOnly.ParseExact(s, formats, null, DateTimeStyles.None)</c> → <c>TimeOnly.ParseExact(s, formats)</c><para/>
    /// <c>TimeOnly.ParseExact(s, ["r", "R"], provider, styles)</c> → <c>TimeOnly.ParseExact(s, ["r"], provider, styles)</c><para/>
    /// <c>TimeOnly.ParseExact(s, ["o", "R"], provider, styles)</c> → <c>TimeOnly.ParseExact(s, ["o", "R"], null, styles)</c>
    /// </remarks>
    static void AnalyzeParseExact_ReadOnlySpanOfChar_StringArray_IFormatProvider_DateTimeStyles(
        IHighlightingConsumer consumer,
        IInvocationExpression invocationExpression,
        ICSharpArgument formatsArgument,
        ICSharpArgument providerArgument,
        ICSharpArgument? styleArgument)
    {
        if (providerArgument.Value.IsDefaultValue()
            && (styleArgument == null || styleArgument.Value.TryGetDateTimeStylesConstant() == DateTimeStyles.None)
            && PredefinedType.TIME_ONLY_FQN.HasMethod(
                new MethodSignature { Name = "ParseExact", ParameterTypes = ParameterTypes.ReadOnlySpanOfT_StringArray, IsStatic = true }, // todo: nameof(TimeOnly.ParseExact) when available
                invocationExpression.PsiModule))
        {
            consumer.AddHighlighting(
                styleArgument is { }
                    ? new RedundantArgumentRangeHint(
                        $"Passing 'null, {nameof(DateTimeStyles)}.{nameof(DateTimeStyles.None)}' is redundant.",
                        providerArgument,
                        styleArgument)
                    : new RedundantArgumentHint("Passing null is redundant.", providerArgument));
        }

        if (CollectionCreation.TryFrom(formatsArgument.Value) is { Count: > 1 } collectionCreation)
        {
            var set = new HashSet<string>(collectionCreation.Count, StringComparer.Ordinal);

            foreach (var (element, s) in collectionCreation.ElementsWithStringConstants)
            {
                if (s is "o" or "O" && (set.Contains("o") || set.Contains("O")) || s is "r" or "R" && (set.Contains("r") || set.Contains("R")))
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
    /// <c>TimeOnly.TryParse(s, provider, DateTimeStyles.None, out result)</c> → <c>TimeOnly.TryParse(s, provider, out result)</c> (.NET 7)
    /// </remarks>
    static void AnalyzeTryParse_String_IFormatProvider_DateTimeStyles_TimeOnly(
        IHighlightingConsumer consumer,
        IInvocationExpression invocationExpression,
        ICSharpArgument styleArgument)
    {
        if (styleArgument.Value.TryGetDateTimeStylesConstant() == DateTimeStyles.None
            && PredefinedType.TIME_ONLY_FQN.HasMethod(
                new MethodSignature { Name = "TryParse", ParameterTypes = ParameterTypes.String_IFormatProvider_TimeOnly, IsStatic = true }, // todo: nameof(TimeOnly.TryParse) when available
                invocationExpression.PsiModule))
        {
            consumer.AddHighlighting(
                new RedundantArgumentHint($"Passing {nameof(DateTimeStyles)}.{nameof(DateTimeStyles.None)} is redundant.", styleArgument));
        }
    }

    /// <remarks>
    /// <c>TimeOnly.TryParse(s, null, out result)</c> → <c>TimeOnly.TryParse(s, out result)</c>
    /// </remarks>
    static void AnalyzeTryParse_String_IFormatProvider_TimeOnly(
        IHighlightingConsumer consumer,
        IInvocationExpression invocationExpression,
        ICSharpArgument providerArgument)
    {
        if (providerArgument.Value.IsDefaultValue()
            && PredefinedType.TIME_ONLY_FQN.HasMethod(
                new MethodSignature { Name = "TryParse", ParameterTypes = ParameterTypes.String_TimeOnly, IsStatic = true }, // todo: nameof(TimeOnly.TryParse) when available
                invocationExpression.PsiModule))
        {
            consumer.AddHighlighting(new RedundantArgumentHint("Passing null is redundant.", providerArgument));
        }
    }

    /// <remarks>
    /// <c>TimeOnly.TryParse(s, provider, DateTimeStyles.None, out result)</c> → <c>TimeOnly.TryParse(s, provider, out result)</c> (.NET 7)
    /// </remarks>
    static void AnalyzeTryParse_ReadOnlySpanOfChar_IFormatProvider_DateTimeStyles_TimeOnly(
        IHighlightingConsumer consumer,
        IInvocationExpression invocationExpression,
        ICSharpArgument styleArgument)
    {
        if (styleArgument.Value.TryGetDateTimeStylesConstant() == DateTimeStyles.None
            && PredefinedType.TIME_ONLY_FQN.HasMethod(
                new MethodSignature { Name = "TryParse", ParameterTypes = ParameterTypes.ReadOnlySpanOfT_IFormatProvider_TimeOnly, IsStatic = true }, // todo: nameof(TimeOnly.TryParse) when available
                invocationExpression.PsiModule))
        {
            consumer.AddHighlighting(
                new RedundantArgumentHint($"Passing {nameof(DateTimeStyles)}.{nameof(DateTimeStyles.None)} is redundant.", styleArgument));
        }
    }

    /// <remarks>
    /// <c>TimeOnly.TryParse(s, null, out result)</c> → <c>TimeOnly.TryParse(s, out result)</c>
    /// </remarks>
    static void AnalyzeTryParse_ReadOnlySpanOfChar_IFormatProvider_TimeOnly(
        IHighlightingConsumer consumer,
        IInvocationExpression invocationExpression,
        ICSharpArgument providerArgument)
    {
        if (providerArgument.Value.IsDefaultValue()
            && PredefinedType.TIME_ONLY_FQN.HasMethod(
                new MethodSignature { Name = "TryParse", ParameterTypes = ParameterTypes.ReadOnlySpanOfT_TimeOnly, IsStatic = true }, // todo: nameof(TimeOnly.TryParse) when available
                invocationExpression.PsiModule))
        {
            consumer.AddHighlighting(new RedundantArgumentHint("Passing null is redundant.", providerArgument));
        }
    }

    /// <remarks>
    /// <c>TimeOnly.TryParseExact(s, format, null, DateTimeStyles.None, out result)</c> → <c>TimeOnly.TryParseExact(s, format, out result)</c><para/>
    /// <c>TimeOnly.TryParseExact(s, "R", provider, style, out result)</c> → <c>TimeOnly.TryParseExact(s, "R", null, style, out result)</c>
    /// </remarks>
    static void AnalyzeTryParseExact_String_String_IFormatProvider_DateTimeStyles_TimeOnly(
        IHighlightingConsumer consumer,
        IInvocationExpression invocationExpression,
        ICSharpArgument formatArgument,
        ICSharpArgument providerArgument,
        ICSharpArgument styleArgument)
    {
        if (providerArgument.Value.IsDefaultValue())
        {
            if (styleArgument.Value.TryGetDateTimeStylesConstant() == DateTimeStyles.None
                && PredefinedType.TIME_ONLY_FQN.HasMethod(
                    new MethodSignature { Name = "TryParseExact", ParameterTypes = ParameterTypes.String_String_TimeOnly, IsStatic = true }, // todo: nameof(TimeOnly.TryParseExact) when available
                    invocationExpression.PsiModule))
            {
                consumer.AddHighlighting(
                    new RedundantArgumentRangeHint(
                        $"Passing 'null, {nameof(DateTimeStyles)}.{nameof(DateTimeStyles.None)}' is redundant.",
                        providerArgument,
                        styleArgument));
            }
        }
        else
        {
            if (formatArgument.Value.TryGetStringConstant() is "o" or "O" or "r" or "R" && providerArgument.Value is { })
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

    /// <remarks>
    /// <c>TimeOnly.TryParseExact(s, format, null, DateTimeStyles.None, out result)</c> → <c>TimeOnly.TryParseExact(s, format, out result)</c>
    /// </remarks>
    static void AnalyzeTryParseExact_ReadOnlySpanOfChar_ReadOnlySpanOfChar_IFormatProvider_DateTimeStyles_TimeOnly(
        IHighlightingConsumer consumer,
        IInvocationExpression invocationExpression,
        ICSharpArgument providerArgument,
        ICSharpArgument styleArgument)
    {
        if (providerArgument.Value.IsDefaultValue()
            && styleArgument.Value.TryGetDateTimeStylesConstant() == DateTimeStyles.None
            && PredefinedType.TIME_ONLY_FQN.HasMethod(
                new MethodSignature
                {
                    Name = "TryParseExact", ParameterTypes = ParameterTypes.ReadOnlySpanOfT_ReadOnlySpanOfT_TimeOnly, IsStatic = true, // todo: nameof(TimeOnly.TryParseExact) when available
                },
                invocationExpression.PsiModule))
        {
            consumer.AddHighlighting(
                new RedundantArgumentRangeHint(
                    $"Passing 'null, {nameof(DateTimeStyles)}.{nameof(DateTimeStyles.None)}' is redundant.",
                    providerArgument,
                    styleArgument));
        }
    }

    /// <remarks>
    /// <c>TimeOnly.TryParseExact(s, [format], out result)</c> → <c>TimeOnly.TryParseExact(s, format, out result)</c><para/>
    /// <c>TimeOnly.TryParseExact(s, ["r", "R"], out result)</c> → <c>TimeOnly.TryParseExact(s, ["r"], out result)</c>
    /// </remarks>
    static void AnalyzeTryParseExact_String_StringArray_TimeOnly(
        IHighlightingConsumer consumer,
        IInvocationExpression invocationExpression,
        ICSharpArgument formatsArgument)
    {
        switch (CollectionCreation.TryFrom(formatsArgument.Value))
        {
            case { Count: 1 } collectionCreation when PredefinedType.TIME_ONLY_FQN.HasMethod(
                new MethodSignature { Name = "TryParseExact", ParameterTypes = ParameterTypes.String_String_TimeOnly, IsStatic = true }, // todo: nameof(TimeOnly.TryParseExact) when available
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
                    if (s is "o" or "O" && (set.Contains("o") || set.Contains("O")) || s is "r" or "R" && (set.Contains("r") || set.Contains("R")))
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
    /// <c>TimeOnly.TryParseExact(s, ["r", "R"], out result)</c> → <c>TimeOnly.TryParseExact(s, ["r"], out result)</c>
    /// </remarks>
    static void AnalyzeTryParseExact_ReadOnlySpanOfChar_StringArray_TimeOnly(IHighlightingConsumer consumer, ICSharpArgument formatsArgument)
    {
        if (CollectionCreation.TryFrom(formatsArgument.Value) is { Count: > 1 } collectionCreation)
        {
            var set = new HashSet<string>(collectionCreation.Count, StringComparer.Ordinal);

            foreach (var (element, s) in collectionCreation.ElementsWithStringConstants)
            {
                if (s is "o" or "O" && (set.Contains("o") || set.Contains("O")) || s is "r" or "R" && (set.Contains("r") || set.Contains("R")))
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
    /// <c>TimeOnly.TryParseExact(s, [format], provider, style, out result)</c> → <c>TimeOnly.TryParseExact(s, format, provider, style, out result)</c><para/>
    /// <c>TimeOnly.TryParseExact(s, ["r", "R"], provider, style, out result)</c> → <c>TimeOnly.TryParseExact(s, ["r"], provider, style, out result)</c><para/>
    /// <c>TimeOnly.TryParseExact(s, ["o", "R"], provider, style, out result)</c> → <c>TimeOnly.TryParseExact(s, ["o", "R"], null, style, out result)</c><para/>
    /// <c>TimeOnly.TryParseExact(s, formats, null, DateTimeStyles.None, out result)</c> → <c>TimeOnly.TryParseExact(s, formats, out result)</c>
    /// </remarks>
    static void AnalyzeTryParseExact_String_StringArray_IFormatProvider_DateTimeStyles_TimeOnly(
        IHighlightingConsumer consumer,
        IInvocationExpression invocationExpression,
        ICSharpArgument formatsArgument,
        ICSharpArgument providerArgument,
        ICSharpArgument styleArgument)
    {
        if (providerArgument.Value.IsDefaultValue()
            && styleArgument.Value.TryGetDateTimeStylesConstant() == DateTimeStyles.None
            && PredefinedType.TIME_ONLY_FQN.HasMethod(
                new MethodSignature { Name = "TryParseExact", ParameterTypes = ParameterTypes.String_StringArray_TimeOnly, IsStatic = true }, // todo: nameof(TimeOnly.TryParseExact) when available
                invocationExpression.PsiModule))
        {
            consumer.AddHighlighting(
                new RedundantArgumentRangeHint(
                    $"Passing 'null, {nameof(DateTimeStyles)}.{nameof(DateTimeStyles.None)}' is redundant.",
                    providerArgument,
                    styleArgument));
        }

        switch (CollectionCreation.TryFrom(formatsArgument.Value))
        {
            case { Count: 1 } collectionCreation when PredefinedType.TIME_ONLY_FQN.HasMethod(
                new MethodSignature
                {
                    Name = "TryParseExact",
                    ParameterTypes = ParameterTypes.String_String_IFormatProvider_DateTimeStyles_TimeOnly, // todo: nameof(TimeOnly.TryParseExact) when available
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
                    if (s is "o" or "O" && (set.Contains("o") || set.Contains("O")) || s is "r" or "R" && (set.Contains("r") || set.Contains("R")))
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
    /// <c>TimeOnly.TryParseExact(s, ["r", "R"], provider, style, out result)</c> → <c>TimeOnly.TryParseExact(s, ["r"], provider, style, out result)</c><para/>
    /// <c>TimeOnly.TryParseExact(s, ["o", "R"], provider, style, out result)</c> → <c>TimeOnly.TryParseExact(s, ["o", "R"], null, style, out result)</c><para/>
    /// <c>TimeOnly.TryParseExact(s, formats, null, DateTimeStyles.None, out result)</c> → <c>TimeOnly.TryParseExact(s, formats, out result)</c>
    /// </remarks>
    static void AnalyzeTryParseExact_ReadOnlySpanOfChar_StringArray_IFormatProvider_DateTimeStyles_TimeOnly(
        IHighlightingConsumer consumer,
        IInvocationExpression invocationExpression,
        ICSharpArgument formatsArgument,
        ICSharpArgument providerArgument,
        ICSharpArgument styleArgument)
    {
        if (providerArgument.Value.IsDefaultValue()
            && styleArgument.Value.TryGetDateTimeStylesConstant() == DateTimeStyles.None
            && PredefinedType.TIME_ONLY_FQN.HasMethod(
                new MethodSignature { Name = "TryParseExact", ParameterTypes = ParameterTypes.ReadOnlySpanOfT_StringArray_TimeOnly, IsStatic = true }, // todo: nameof(TimeOnly.TryParseExact) when available
                invocationExpression.PsiModule))
        {
            consumer.AddHighlighting(
                new RedundantArgumentRangeHint(
                    $"Passing 'null, {nameof(DateTimeStyles)}.{nameof(DateTimeStyles.None)}' is redundant.",
                    providerArgument,
                    styleArgument));
        }

        if (CollectionCreation.TryFrom(formatsArgument.Value) is { Count: > 1 } collectionCreation)
        {
            var set = new HashSet<string>(collectionCreation.Count, StringComparer.Ordinal);

            foreach (var (element, s) in collectionCreation.ElementsWithStringConstants)
            {
                if (s is "o" or "O" && (set.Contains("o") || set.Contains("O")) || s is "r" or "R" && (set.Contains("r") || set.Contains("R")))
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

    protected override void Run(ICSharpInvocationInfo element, ElementProblemAnalyzerData data, IHighlightingConsumer consumer)
    {
        switch (element)
        {
            case IObjectCreationExpression { ConstructorReference: var reference } objectCreationExpression
                when reference.Resolve().DeclaredElement is IConstructor
                {
                    AccessibilityDomain.DomainType: AccessibilityDomain.AccessibilityDomainType.PUBLIC, IsStatic: false,
                } constructor
                && constructor.ContainingType.IsClrType(PredefinedType.TIME_ONLY_FQN):

                switch (constructor.Parameters, objectCreationExpression.TryGetArgumentsInDeclarationOrder())
                {
                    case ([{ Type: var ticksType }], [{ } ticksArgument]) when ticksType.IsLong():
                        Analyze_Ctor_Int64(consumer, objectCreationExpression, ticksArgument);
                        break;

                    case ([{ Type: var hourType }, { Type: var minuteType }], [{ } hourArgument, { } minuteArgument])
                        when hourType.IsInt() && minuteType.IsInt():

                        Analyze_Ctor_Int32_Int32(consumer, objectCreationExpression, hourArgument, minuteArgument);
                        break;

                    case ([{ Type: var hourType }, { Type: var minuteType }, { Type: var secondType }], [
                        { } hourArgument, { } minuteArgument, { } secondArgument,
                    ]) when hourType.IsInt() && minuteType.IsInt() && secondType.IsInt():
                        Analyze_Ctor_Int32_Int32_Int32(consumer, objectCreationExpression, hourArgument, minuteArgument, secondArgument);
                        break;

                    case ([{ Type: var hourType }, { Type: var minuteType }, { Type: var secondType }, { Type: var millisecondType }], [
                        { } hourArgument, { } minuteArgument, { } secondArgument, { } millisecondArgument,
                    ]) when hourType.IsInt() && minuteType.IsInt() && secondType.IsInt() && millisecondType.IsInt():
                        Analyze_Ctor_Int32_Int32_Int32_Int32(
                            consumer,
                            objectCreationExpression,
                            hourArgument,
                            minuteArgument,
                            secondArgument,
                            millisecondArgument);
                        break;

                    case ([
                            { Type: var hourType },
                            { Type: var minuteType },
                            { Type: var secondType },
                            { Type: var millisecondType },
                            { Type: var microsecondType },
                        ], [{ } hourArgument, { } minuteArgument, { } secondArgument, { } millisecondArgument, { } microsecondArgument])
                        when hourType.IsInt() && minuteType.IsInt() && secondType.IsInt() && millisecondType.IsInt() && microsecondType.IsInt():

                        Analyze_Ctor_Int32_Int32_Int32_Int32_Int32(
                            consumer,
                            objectCreationExpression,
                            hourArgument,
                            minuteArgument,
                            secondArgument,
                            millisecondArgument,
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
                && method.ContainingType.IsClrType(PredefinedType.TIME_ONLY_FQN):

                switch (invokedExpression, method)
                {
                    case ({ QualifierExpression: { } }, { IsStatic: false }):
                        switch (method.ShortName)
                        {
                            case "Add": // todo: nameof(TimeOnly.Add) when available
                                switch (method.Parameters, invocationExpression.TryGetArgumentsInDeclarationOrder())
                                {
                                    case ([{ Type: var valueType}, { Type: var wrappedDaysType}], [_, { } wrappedDaysArgument])
                                        when valueType.IsTimeSpan() && wrappedDaysType.IsInt():

                                        AnalyzeAdd_TimeSpan_Int32(consumer, invocationExpression, wrappedDaysArgument);
                                        break;
                                }
                                break;

                            case "AddHours": // todo: nameof(TimeOnly.AddHours) when available
                                switch (method.Parameters, invocationExpression.TryGetArgumentsInDeclarationOrder())
                                {
                                    case ([{ Type: var valueType }, { Type: var wrappedDaysType }], [_, { } wrappedDaysArgument])
                                        when valueType.IsDouble() && wrappedDaysType.IsInt():

                                        AnalyzeAddHours_Double_Int32(consumer, invocationExpression,wrappedDaysArgument);
                                        break;
                                }
                                break;

                            case "AddMinutes": // todo: nameof(TimeOnly.AddMinutes) when available
                                switch (method.Parameters, invocationExpression.TryGetArgumentsInDeclarationOrder())
                                {
                                    case ([{ Type: var valueType }, { Type: var wrappedDaysType }], [_, { } wrappedDaysArgument])
                                        when valueType.IsDouble() && wrappedDaysType.IsInt():

                                        AnalyzeAddMinutes_Double_Int32(consumer, invocationExpression, wrappedDaysArgument);
                                        break;
                                }
                                break;

                            case "Equals": // todo: nameof(TimeOnly.Equals) when available
                                switch (method.Parameters, invocationExpression.TryGetArgumentsInDeclarationOrder())
                                {
                                    case ([{ Type: var valueType }], [{ } valueArgument]) when valueType.IsTimeOnly():
                                        AnalyzeEquals_TimeOnly(consumer, invocationExpression, invokedExpression, valueArgument);
                                        break;

                                    case ([{ Type: var valueType }], [{ } valueArgument]) when valueType.IsObject():
                                        AnalyzeEquals_Object(consumer, invocationExpression, valueArgument);
                                        break;
                                }
                                break;
                        }
                        break;

                    case (_, { IsStatic: true }):
                        switch (method.ShortName)
                        {
                            case "Parse": // todo: nameof(TimeOnly.Parse) when available
                                switch (method.Parameters, invocationExpression.TryGetArgumentsInDeclarationOrder())
                                {
                                    case ([{ Type: var sType }, { Type: var providerType }, { Type: var styleType }], [
                                        _, { } providerArgument, var styleArgument,
                                    ]) when sType.IsString() && providerType.IsIFormatProvider() && styleType.IsDateTimeStyles():
                                        AnalyzeParse_String_IFormatProvider_DateTimeStyles(
                                            consumer,
                                            invocationExpression,
                                            providerArgument,
                                            styleArgument);
                                        break;

                                    case ([{ Type: var sType }, { Type: var providerType }], [_, { } providerArgument])
                                        when sType.IsString() && providerType.IsIFormatProvider():

                                        AnalyzeParse_String_IFormatProvider(consumer, invocationExpression, providerArgument);
                                        break;

                                    case ([{ Type: var sType }, { Type: var providerType }], [_, { } providerArgument])
                                        when sType.IsReadOnlySpanOfChar() && providerType.IsIFormatProvider():

                                        AnalyzeParse_ReadOnlySpanOfChar_IFormatProvider(consumer, invocationExpression, providerArgument);
                                        break;
                                }
                                break;

                            case "ParseExact": // todo: nameof(TimeOnly.ParseExact) when available
                                switch (method.Parameters, invocationExpression.TryGetArgumentsInDeclarationOrder())
                                {
                                    case ([{ Type: var sType }, { Type: var formatType }, { Type: var providerType }, { Type: var styleType }], [
                                            _, { } formatArgument, { } providerArgument, var styleArgument,
                                        ]) when sType.IsString()
                                        && formatType.IsString()
                                        && providerType.IsIFormatProvider()
                                        && styleType.IsDateTimeStyles():

                                        AnalyzeParseExact_String_String_IFormatProvider_DateTimeStyles(
                                            consumer,
                                            invocationExpression,
                                            formatArgument,
                                            providerArgument,
                                            styleArgument);
                                        break;

                                    case ([{ Type: var sType }, { Type: var formatsType }], [_, { } formatsArgument])
                                        when sType.IsString() && formatsType.IsGenericArrayOfString():

                                        AnalyzeParseExact_String_StringArray(consumer, invocationExpression, formatsArgument);
                                        break;

                                    case ([{ Type: var sType }, { Type: var formatsType }, { Type: var providerType }, { Type: var styleType }], [
                                            _, { } formatsArgument, { } providerArgument, var styleArgument,
                                        ]) when sType.IsString()
                                        && formatsType.IsGenericArrayOfString()
                                        && providerType.IsIFormatProvider()
                                        && styleType.IsDateTimeStyles():

                                        AnalyzeParseExact_String_StringArray_IFormatProvider_DateTimeStyles(
                                            consumer,
                                            invocationExpression,
                                            formatsArgument,
                                            providerArgument,
                                            styleArgument);
                                        break;

                                    case ([{ Type: var sType }, { Type: var formatsType }], [_, { } formatsArgument])
                                        when sType.IsReadOnlySpanOfChar() && formatsType.IsGenericArrayOfString():

                                        AnalyzeParseExact_ReadOnlySpanOfChar_StringArray(consumer, formatsArgument);
                                        break;

                                    case ([{ Type: var sType }, { Type: var formatsType }, { Type: var providerType }, { Type: var styleType }], [
                                            _, { } formatsArgument, { } providerArgument, var styleArgument,
                                        ]) when sType.IsReadOnlySpanOfChar()
                                        && formatsType.IsGenericArrayOfString()
                                        && providerType.IsIFormatProvider()
                                        && styleType.IsDateTimeStyles():

                                        AnalyzeParseExact_ReadOnlySpanOfChar_StringArray_IFormatProvider_DateTimeStyles(
                                            consumer,
                                            invocationExpression,
                                            formatsArgument,
                                            providerArgument,
                                            styleArgument);
                                        break;
                                }
                                break;

                            case "TryParse": // todo: nameof(TimeOnly.TryParse) when available
                                switch (method.Parameters, invocationExpression.TryGetArgumentsInDeclarationOrder())
                                {
                                    case ([{ Type: var sType }, { Type: var providerType }, { Type: var styleType }, { Type: var resultType }], [
                                            _, _, { } styleArgument, _,
                                        ]) when sType.IsString()
                                        && providerType.IsIFormatProvider()
                                        && styleType.IsDateTimeStyles()
                                        && resultType.IsTimeOnly():

                                        AnalyzeTryParse_String_IFormatProvider_DateTimeStyles_TimeOnly(consumer, invocationExpression, styleArgument);
                                        break;

                                    case ([{ Type: var sType }, { Type: var providerType }, { Type: var resultType }], [_, { } providerArgument, _])
                                        when sType.IsString() && providerType.IsIFormatProvider() && resultType.IsTimeOnly():

                                        AnalyzeTryParse_String_IFormatProvider_TimeOnly(consumer, invocationExpression, providerArgument);
                                        break;

                                    case ([{ Type: var sType }, { Type: var providerType }, { Type: var styleType }, { Type: var resultType }], [
                                            _, _, { } styleArgument, _,
                                        ]) when sType.IsReadOnlySpanOfChar()
                                        && providerType.IsIFormatProvider()
                                        && styleType.IsDateTimeStyles()
                                        && resultType.IsTimeOnly():

                                        AnalyzeTryParse_ReadOnlySpanOfChar_IFormatProvider_DateTimeStyles_TimeOnly(
                                            consumer,
                                            invocationExpression,
                                            styleArgument);
                                        break;

                                    case ([{ Type: var sType }, { Type: var providerType }, { Type: var resultType }], [_, { } providerArgument, _])
                                        when sType.IsReadOnlySpanOfChar() && providerType.IsIFormatProvider() && resultType.IsTimeOnly():

                                        AnalyzeTryParse_ReadOnlySpanOfChar_IFormatProvider_TimeOnly(consumer, invocationExpression, providerArgument);
                                        break;
                                }
                                break;

                            case "TryParseExact": // todo: nameof(TimeOnly.TryParseExact) when available
                                switch (method.Parameters, invocationExpression.TryGetArgumentsInDeclarationOrder())
                                {
                                    case ([
                                            { Type: var sType },
                                            { Type: var formatType },
                                            { Type: var providerType },
                                            { Type: var styleType },
                                            { Type: var resultType },
                                        ], [_, { } formatArgument, { } providerArgument, { } styleArgument, _]) when sType.IsString()
                                        && formatType.IsString()
                                        && providerType.IsIFormatProvider()
                                        && styleType.IsDateTimeStyles()
                                        && resultType.IsTimeOnly():

                                        AnalyzeTryParseExact_String_String_IFormatProvider_DateTimeStyles_TimeOnly(
                                            consumer,
                                            invocationExpression,
                                            formatArgument,
                                            providerArgument,
                                            styleArgument);
                                        break;

                                    case ([
                                            { Type: var sType },
                                            { Type: var formatType },
                                            { Type: var providerType },
                                            { Type: var styleType },
                                            { Type: var resultType },
                                        ], [_, _, { } providerArgument, { } styleArgument, _]) when sType.IsReadOnlySpanOfChar()
                                        && formatType.IsReadOnlySpanOfChar()
                                        && providerType.IsIFormatProvider()
                                        && styleType.IsDateTimeStyles()
                                        && resultType.IsTimeOnly():

                                        AnalyzeTryParseExact_ReadOnlySpanOfChar_ReadOnlySpanOfChar_IFormatProvider_DateTimeStyles_TimeOnly(
                                            consumer,
                                            invocationExpression,
                                            providerArgument,
                                            styleArgument);
                                        break;

                                    case ([{ Type: var sType }, { Type: var formatsType }, { Type: var resultType }], [_, { } formatsArgument, _])
                                        when sType.IsString() && formatsType.IsGenericArrayOfString() && resultType.IsTimeOnly():

                                        AnalyzeTryParseExact_String_StringArray_TimeOnly(consumer, invocationExpression, formatsArgument);
                                        break;

                                    case ([{ Type: var sType }, { Type: var formatsType }, { Type: var resultType }], [_, { } formatsArgument, _])
                                        when sType.IsReadOnlySpanOfChar() && formatsType.IsGenericArrayOfString() && resultType.IsTimeOnly():

                                        AnalyzeTryParseExact_ReadOnlySpanOfChar_StringArray_TimeOnly(consumer, formatsArgument);
                                        break;

                                    case ([
                                            { Type: var sType },
                                            { Type: var formatsType },
                                            { Type: var providerType },
                                            { Type: var styleType },
                                            { Type: var resultType },
                                        ], [_, { } formatsArgument, { } providerArgument, { } styleArgument, _]) when sType.IsString()
                                        && formatsType.IsGenericArrayOfString()
                                        && providerType.IsIFormatProvider()
                                        && styleType.IsDateTimeStyles()
                                        && resultType.IsTimeOnly():

                                        AnalyzeTryParseExact_String_StringArray_IFormatProvider_DateTimeStyles_TimeOnly(
                                            consumer,
                                            invocationExpression,
                                            formatsArgument,
                                            providerArgument,
                                            styleArgument);
                                        break;

                                    case ([
                                            { Type: var sType },
                                            { Type: var formatsType },
                                            { Type: var providerType },
                                            { Type: var styleType },
                                            { Type: var resultType },
                                        ], [_, { } formatsArgument, { } providerArgument, { } styleArgument, _]) when sType.IsReadOnlySpanOfChar()
                                        && formatsType.IsGenericArrayOfString()
                                        && providerType.IsIFormatProvider()
                                        && styleType.IsDateTimeStyles()
                                        && resultType.IsTimeOnly():

                                        AnalyzeTryParseExact_ReadOnlySpanOfChar_StringArray_IFormatProvider_DateTimeStyles_TimeOnly(
                                            consumer,
                                            invocationExpression,
                                            formatsArgument,
                                            providerArgument,
                                            styleArgument);
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