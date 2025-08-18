using System.Globalization;
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
        typeof(RedundantArgumentRangeHint),
        typeof(RedundantArgumentHint),
        typeof(UseOtherArgumentSuggestion),
        typeof(RedundantElementHint),
    ])]
public sealed class DateOnlyAnalyzer : ElementProblemAnalyzer<IInvocationExpression>
{
    [SuppressMessage("ReSharper", "InconsistentNaming", Justification = "Underscore character used intentionally as a separator.")]
    static class ParameterTypes
    {
        public static IReadOnlyList<ParameterType> String { get; } = [new() { ClrTypeName = PredefinedType.STRING_FQN }];

        public static IReadOnlyList<ParameterType> String_String { get; } =
        [
            new() { ClrTypeName = PredefinedType.STRING_FQN }, new() { ClrTypeName = PredefinedType.STRING_FQN },
        ];

        public static IReadOnlyList<ParameterType> String_StringArray { get; } =
        [
            new() { ClrTypeName = PredefinedType.STRING_FQN }, new ArrayParameterType { ClrTypeName = PredefinedType.STRING_FQN },
        ];

        public static IReadOnlyList<ParameterType> ReadOnlySpanOfT_StringArray { get; } =
        [
            new GenericParameterType { ClrTypeName = PredefinedType.SYSTEM_READ_ONLY_SPAN_FQN },
            new ArrayParameterType { ClrTypeName = PredefinedType.STRING_FQN },
        ];

        public static IReadOnlyList<ParameterType> ReadOnlySpanOfT_IFormatProvider_DateTimeStyles { get; } =
        [
            new GenericParameterType { ClrTypeName = PredefinedType.SYSTEM_READ_ONLY_SPAN_FQN },
            new() { ClrTypeName = PredefinedType.IFORMATPROVIDER_FQN },
            new() { ClrTypeName = ClrTypeNames.DateTimeStyles },
        ];

        public static IReadOnlyList<ParameterType> String_String_IFormatProvider_DateTimeStyles { get; } =
        [
            new() { ClrTypeName = PredefinedType.STRING_FQN },
            new() { ClrTypeName = PredefinedType.STRING_FQN },
            new() { ClrTypeName = PredefinedType.IFORMATPROVIDER_FQN },
            new() { ClrTypeName = ClrTypeNames.DateTimeStyles },
        ];
    }

    [Pure]
    static bool IsDateOnly(IType type) => type.IsClrType(PredefinedType.DATE_ONLY_FQN);

    [Pure]
    static bool IsDateTimeStyles(IType type) => type.IsClrType(ClrTypeNames.DateTimeStyles);

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
    /// <c>DateOnly.Parse(s, null, DateTimeStyles.None)</c> → <c>DateOnly.Parse(s)</c>
    /// </remarks>
    static void AnalyzeParse_String_IFormatProvider_DateTimeStyles(
        IHighlightingConsumer consumer,
        IInvocationExpression invocationExpression,
        ICSharpArgument providerArgument,
        ICSharpArgument? styleArgument)
    {
        if (providerArgument.Value.IsDefaultValue()
            && (styleArgument == null || styleArgument.Value.TryGetDateTimeStylesConstant() == DateTimeStyles.None)
            && PredefinedType.DATE_ONLY_FQN.HasMethod(
                new MethodSignature { Name = "Parse", ParameterTypes = ParameterTypes.String, IsStatic = true }, // todo: nameof(DateOnly.Parse) when available
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
    /// <c>DateOnly.Parse(s, null)</c> → <c>DateOnly.Parse(s)</c>
    /// </remarks>
    static void AnalyzeParse_String_IFormatProvider(
        IHighlightingConsumer consumer,
        IInvocationExpression invocationExpression,
        ICSharpArgument providerArgument)
    {
        if (providerArgument.Value.IsDefaultValue()
            && PredefinedType.DATE_ONLY_FQN.HasMethod(
                new MethodSignature { Name = "Parse", ParameterTypes = ParameterTypes.String, IsStatic = true }, // todo: nameof(DateOnly.Parse) when available
                invocationExpression.PsiModule))
        {
            consumer.AddHighlighting(new RedundantArgumentHint("Passing null is redundant.", providerArgument));
        }
    }

    /// <remarks>
    /// <c>DateOnly.Parse(s, null)</c> → <c>DateOnly.Parse(s)</c>
    /// </remarks>
    static void AnalyzeParse_ReadOnlySpanOfChar_IFormatProvider(
        IHighlightingConsumer consumer,
        IInvocationExpression invocationExpression,
        ICSharpArgument providerArgument)
    {
        if (providerArgument.Value.IsDefaultValue()
            && PredefinedType.DATE_ONLY_FQN.HasMethod(
                new MethodSignature
                {
                    Name = "Parse", ParameterTypes = ParameterTypes.ReadOnlySpanOfT_IFormatProvider_DateTimeStyles, IsStatic = true, // todo: nameof(DateOnly.Parse) when available
                },
                invocationExpression.PsiModule))
        {
            consumer.AddHighlighting(new RedundantArgumentHint("Passing null is redundant.", providerArgument));
        }
    }

    /// <remarks>
    /// <c>DateOnly.ParseExact(s, format, null, DateTimeStyles.None)</c> → <c>DateOnly.ParseExact(s, format)</c>
    /// <c>DateOnly.ParseExact(s, "R", provider, style)</c> → <c>DateOnly.ParseExact(s, "R", null, style)</c>
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
                && PredefinedType.DATE_ONLY_FQN.HasMethod(
                    new MethodSignature { Name = "ParseExact", ParameterTypes = ParameterTypes.String_String, IsStatic = true }, // todo: nameof(DateOnly.ParseExact) when available
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
                new MethodSignature { Name = "ParseExact", ParameterTypes = ParameterTypes.String_String, IsStatic = true }, // todo: nameof(DateOnly.ParseExact) when available
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
    /// <c>DateOnly.ParseExact(s, formats, null, DateTimeStyles.None)</c> → <c>DateOnly.ParseExact(s, formats)</c><para/>
    /// <c>DateOnly.ParseExact(s, [format], provider, styles)</c> → <c>DateOnly.ParseExact(s, format, provider, styles)</c><para/>
    /// <c>DateOnly.ParseExact(s, ["m", "M"], provider, styles)</c> → <c>DateOnly.ParseExact(s, ["m"], provider, styles)</c><para/>
    /// <c>DateOnly.ParseExact(s, ["o", "R"], provider, styles)</c> → <c>DateOnly.ParseExact(s, ["o", "R"], null, styles)</c>
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
            && PredefinedType.DATE_ONLY_FQN.HasMethod(
                new MethodSignature { Name = "ParseExact", ParameterTypes = ParameterTypes.String_StringArray, IsStatic = true }, // todo: nameof(DateOnly.ParseExact) when available
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
            case { Count: 1 } collectionCreation when PredefinedType.DATE_ONLY_FQN.HasMethod(
                new MethodSignature
                {
                    Name = "ParseExact", ParameterTypes = ParameterTypes.String_String_IFormatProvider_DateTimeStyles, IsStatic = true, // todo: nameof(DateOnly.ParseExact) when available
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
    /// <c>DateOnly.ParseExact(s, formats, null, DateTimeStyles.None)</c> → <c>DateOnly.ParseExact(s, formats)</c><para/>
    /// <c>DateOnly.ParseExact(s, ["m", "M"], provider, styles)</c> → <c>DateOnly.ParseExact(s, ["m"], provider, styles)</c><para/>
    /// <c>DateOnly.ParseExact(s, ["o", "R"], provider, styles)</c> → <c>DateOnly.ParseExact(s, ["o", "R"], null, styles)</c>
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
            && PredefinedType.DATE_ONLY_FQN.HasMethod(
                new MethodSignature { Name = "ParseExact", ParameterTypes = ParameterTypes.ReadOnlySpanOfT_StringArray, IsStatic = true }, // todo: nameof(DateOnly.ParseExact) when available
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
                                case ([{ Type: var valueType }], [{ } valueArgument]) when IsDateOnly(valueType):
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
                        case "Parse": // todo: nameof(DateOnly.Parse) when available
                            switch (method.Parameters, element.TryGetArgumentsInDeclarationOrder())
                            {
                                case ([{ Type: var sType }, { Type: var providerType }, { Type: var styleType }], [
                                    _, { } providerArgument, var styleArgument,
                                ]) when sType.IsString() && providerType.IsIFormatProvider() && IsDateTimeStyles(styleType):
                                    AnalyzeParse_String_IFormatProvider_DateTimeStyles(consumer, element, providerArgument, styleArgument);
                                    break;

                                case ([{ Type: var sType }, { Type: var providerType }], [_, { } providerArgument])
                                    when sType.IsString() && providerType.IsIFormatProvider():

                                    AnalyzeParse_String_IFormatProvider(consumer, element, providerArgument);
                                    break;

                                case ([{ Type: var sType }, { Type: var providerType }], [_, { } providerArgument])
                                    when sType.IsReadOnlySpan(out var spanTypeArgument)
                                    && spanTypeArgument.IsChar()
                                    && providerType.IsIFormatProvider():

                                    AnalyzeParse_ReadOnlySpanOfChar_IFormatProvider(consumer, element, providerArgument);
                                    break;
                            }
                            break;

                        case "ParseExact": // todo: nameof(DateOnly.ParseExact) when available
                            switch (method.Parameters, element.TryGetArgumentsInDeclarationOrder())
                            {
                                case ([{ Type: var sType }, { Type: var formatType }, { Type: var providerType }, { Type: var styleType }], [
                                    _, { } formatArgument, { } providerArgument, var styleArgument,
                                ]) when sType.IsString() && formatType.IsString() && providerType.IsIFormatProvider() && IsDateTimeStyles(styleType):
                                    AnalyzeParseExact_String_String_IFormatProvider_DateTimeStyles(
                                        consumer,
                                        element,
                                        formatArgument,
                                        providerArgument,
                                        styleArgument);
                                    break;

                                case ([{ Type: var sType }, { Type: var formatsType }], [_, { } formatsArgument])
                                    when sType.IsString() && formatsType.IsGenericArrayOf(PredefinedType.STRING_FQN, element):

                                    AnalyzeParseExact_String_StringArray(consumer, element, formatsArgument);
                                    break;

                                case ([{ Type: var sType }, { Type: var formatsType }, { Type: var providerType }, { Type: var styleType }], [
                                        _, { } formatsArgument, { } providerArgument, var styleArgument,
                                    ]) when sType.IsString()
                                    && formatsType.IsGenericArrayOf(PredefinedType.STRING_FQN, element)
                                    && providerType.IsIFormatProvider()
                                    && IsDateTimeStyles(styleType):

                                    AnalyzeParseExact_String_StringArray_IFormatProvider_DateTimeStyles(
                                        consumer,
                                        element,
                                        formatsArgument,
                                        providerArgument,
                                        styleArgument);
                                    break;

                                case ([{ Type: var sType }, { Type: var formatsType }], [_, { } formatsArgument])
                                    when sType.IsReadOnlySpan(out var spanTypeArgument)
                                    && spanTypeArgument.IsChar()
                                    && formatsType.IsGenericArrayOf(PredefinedType.STRING_FQN, element):

                                    AnalyzeParseExact_ReadOnlySpanOfChar_StringArray(consumer, formatsArgument);
                                    break;

                                case ([{ Type: var sType }, { Type: var formatsType }, { Type: var providerType }, { Type: var styleType }], [
                                        _, { } formatsArgument, { } providerArgument, var styleArgument,
                                    ]) when sType.IsReadOnlySpan(out var spanTypeArgument)
                                    && spanTypeArgument.IsChar()
                                    && formatsType.IsGenericArrayOf(PredefinedType.STRING_FQN, element)
                                    && providerType.IsIFormatProvider()
                                    && IsDateTimeStyles(styleType):

                                    AnalyzeParseExact_ReadOnlySpanOfChar_StringArray_IFormatProvider_DateTimeStyles(
                                        consumer,
                                        element,
                                        formatsArgument,
                                        providerArgument,
                                        styleArgument);
                                    break;
                            }
                            break;
                    }
                    break;
            }
        }
    }
}