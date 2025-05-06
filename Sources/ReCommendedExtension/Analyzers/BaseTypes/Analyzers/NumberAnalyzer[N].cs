using System.Globalization;
using JetBrains.ReSharper.Feature.Services.Daemon;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.CSharp.Tree;
using ReCommendedExtension.Analyzers.BaseTypes.Analyzers.NumberInfos;
using ReCommendedExtension.Extensions;
using ReCommendedExtension.Extensions.MethodFinding;
using MethodSignature = ReCommendedExtension.Extensions.MethodFinding.MethodSignature;

namespace ReCommendedExtension.Analyzers.BaseTypes.Analyzers;

public abstract class NumberAnalyzer<N>(NumberInfo<N> numberInfo) : NumberAnalyzer where N : struct
{
    [Pure]
    static bool IsNumberStyles(IType type) => type.IsClrType(ClrTypeNames.NumberStyles);

    /// <remarks>
    /// <c>T.Clamp(value, n, n)</c> → <c>n</c><para/>
    /// <c>T.Clamp(value, 0, 255)</c> → <c>value</c>
    /// </remarks>
    void AnalyzeClamp(
        IHighlightingConsumer consumer,
        IInvocationExpression invocationExpression,
        ICSharpArgument valueArgument,
        ICSharpArgument minArgument,
        ICSharpArgument maxArgument)
    {
        if (!invocationExpression.IsUsedAsStatement()
            && numberInfo.TryGetConstant(minArgument.Value, out _) is { } min
            && numberInfo.TryGetConstant(maxArgument.Value, out _) is { } max)
        {
            if (numberInfo.AreEqual(min, max))
            {
                Debug.Assert(minArgument.Value is { });
                Debug.Assert(maxArgument.Value is { });

                var replacementMin = GetReplacementFromArgument(invocationExpression, minArgument.Value);
                var replacementMax = GetReplacementFromArgument(invocationExpression, maxArgument.Value);

                consumer.AddHighlighting(
                    new UseExpressionResultSuggestion(
                        $"The expression is always {min}.",
                        invocationExpression,
                        replacementMin,
                        replacementMax != replacementMin ? replacementMax : null));
            }

            if (numberInfo.AreMinMaxValues(min, max) && valueArgument.Value is { } value)
            {
                consumer.AddHighlighting(
                    new UseExpressionResultSuggestion(
                        "The expression is always the same as the first argument.",
                        invocationExpression,
                        GetReplacementFromArgument(invocationExpression, value)));
            }
        }
    }

    /// <remarks>
    /// <c>number.Equals(obj)</c> → <c>number == obj</c>
    /// </remarks>
    void AnalyzeEquals_N(
        IHighlightingConsumer consumer,
        IInvocationExpression invocationExpression,
        IReferenceExpression invokedExpression,
        ICSharpArgument objArgument)
    {
        Debug.Assert(invokedExpression.QualifierExpression is { });

        if (numberInfo.CanUseEqualityOperator && !invocationExpression.IsUsedAsStatement() && objArgument.Value is { })
        {
            consumer.AddHighlighting(
                new UseBinaryOperatorSuggestion(
                    "Use the '==' operator.",
                    invocationExpression,
                    "==",
                    invokedExpression.QualifierExpression.GetText(),
                    objArgument.Value.GetText()));
        }
    }

    /// <remarks>
    /// <c>number.Equals(null)</c> → <c>false</c>
    /// </remarks>
    static void AnalyzeEquals_Object(IHighlightingConsumer consumer, IInvocationExpression invocationExpression, ICSharpArgument objArgument)
    {
        if (!invocationExpression.IsUsedAsStatement() && objArgument.Value.IsDefaultValue())
        {
            consumer.AddHighlighting(new UseExpressionResultSuggestion("The expression is always false.", invocationExpression, "false"));
        }
    }

    /// <remarks>
    /// <c>number.GetTypeCode()</c> → <c>TypeCode...</c>
    /// </remarks>
    void AnalyzeGetTypeCode(IHighlightingConsumer consumer, IInvocationExpression invocationExpression)
    {
        if (!invocationExpression.IsUsedAsStatement() && numberInfo.TypeCode is { } typeCode)
        {
            var replacement = $"{nameof(TypeCode)}.{typeCode:G}";

            consumer.AddHighlighting(
                new UseExpressionResultSuggestion($"The expression is always {replacement}.", invocationExpression, replacement));
        }
    }

    /// <remarks>
    /// <c>T.Max(n, n)</c> → <c>n</c>
    /// </remarks>
    void AnalyzeMax(IHighlightingConsumer consumer, IInvocationExpression invocationExpression, ICSharpArgument xArgument, ICSharpArgument yArgument)
    {
        if (!invocationExpression.IsUsedAsStatement()
            && numberInfo.TryGetConstant(xArgument.Value, out _) is { } x
            && numberInfo.TryGetConstant(yArgument.Value, out _) is { } y
            && numberInfo.AreEqual(x, y))
        {
            Debug.Assert(xArgument.Value is { });
            Debug.Assert(yArgument.Value is { });

            var replacementX = GetReplacementFromArgument(invocationExpression, xArgument.Value);
            var replacementY = GetReplacementFromArgument(invocationExpression, yArgument.Value);

            consumer.AddHighlighting(
                new UseExpressionResultSuggestion(
                    $"The expression is always {x}.",
                    invocationExpression,
                    replacementX,
                    replacementY != replacementX ? replacementY : null));
        }
    }

    /// <remarks>
    /// <c>T.Min(n, n)</c> → <c>n</c>
    /// </remarks>
    void AnalyzeMin(IHighlightingConsumer consumer, IInvocationExpression invocationExpression, ICSharpArgument xArgument, ICSharpArgument yArgument)
    {
        if (!invocationExpression.IsUsedAsStatement()
            && numberInfo.TryGetConstant(xArgument.Value, out _) is { } x
            && numberInfo.TryGetConstant(yArgument.Value, out _) is { } y
            && numberInfo.AreEqual(x, y))
        {
            Debug.Assert(xArgument.Value is { });
            Debug.Assert(yArgument.Value is { });

            var replacementX = GetReplacementFromArgument(invocationExpression, xArgument.Value);
            var replacementY = GetReplacementFromArgument(invocationExpression, yArgument.Value);

            consumer.AddHighlighting(
                new UseExpressionResultSuggestion(
                    $"The expression is always {x}.",
                    invocationExpression,
                    replacementX,
                    replacementY != replacementX ? replacementY : null));
        }
    }

    /// <remarks>
    /// <c>T.Parse(s, defaultStyle)</c> → <c>T.Parse(s)</c>
    /// </remarks>
    void AnalyzeParse_String_NumberStyles(IHighlightingConsumer consumer, IInvocationExpression invocationExpression, ICSharpArgument styleArgument)
    {
        var defaultNumberStyles = numberInfo.DefaultNumberStyles;

        if (styleArgument.Value.TryGetNumberStylesConstant() == defaultNumberStyles
            && numberInfo.ClrTypeName.HasMethod(
                new MethodSignature { Name = nameof(int.Parse), ParameterTypes = ParameterTypes.String, IsStatic = true },
                invocationExpression.PsiModule))
        {
            var styles = string.Join(" | ", from t in $"{defaultNumberStyles:G}".Split(',') select $"{nameof(NumberStyles)}.{t.Trim()}");

            consumer.AddHighlighting(new RedundantArgumentHint($"Passing {styles} is redundant.", styleArgument));
        }
    }

    /// <remarks>
    /// <c>T.Parse(s, null)</c> → <c>T.Parse(s)</c>
    /// </remarks>
    void AnalyzeParse_String_IFormatProvider(
        IHighlightingConsumer consumer,
        IInvocationExpression invocationExpression,
        ICSharpArgument providerArgument)
    {
        if (providerArgument.Value.IsDefaultValue()
            && numberInfo.ClrTypeName.HasMethod(
                new MethodSignature { Name = nameof(int.Parse), ParameterTypes = ParameterTypes.String, IsStatic = true },
                invocationExpression.PsiModule))
        {
            consumer.AddHighlighting(new RedundantArgumentHint("Passing null is redundant.", providerArgument));
        }
    }

    /// <remarks>
    /// <c>T.Parse(s, defaultStyle, provider)</c> → <c>T.Parse(s, provider)</c><para/>
    /// <c>T.Parse(s, style, null)</c> → <c>T.Parse(s, style)</c>
    /// </remarks>
    void AnalyzeParse_String_NumberStyles_IFormatProvider(
        IHighlightingConsumer consumer,
        IInvocationExpression invocationExpression,
        ICSharpArgument styleArgument,
        ICSharpArgument providerArgument)
    {
        var defaultNumberStyles = numberInfo.DefaultNumberStyles;

        if (styleArgument.Value.TryGetNumberStylesConstant() == defaultNumberStyles
            && numberInfo.ClrTypeName.HasMethod(
                new MethodSignature { Name = "Parse", ParameterTypes = ParameterTypes.String_IFormatProvider, IsStatic = true }, // todo: nameof(IParsable<T>.Parse) when available
                invocationExpression.PsiModule))
        {
            var styles = string.Join(" | ", from t in $"{defaultNumberStyles:G}".Split(',') select $"{nameof(NumberStyles)}.{t.Trim()}");

            consumer.AddHighlighting(new RedundantArgumentHint($"Passing {styles} is redundant.", styleArgument));
        }

        if (providerArgument.Value.IsDefaultValue()
            && numberInfo.ClrTypeName.HasMethod(
                new MethodSignature { Name = nameof(int.Parse), ParameterTypes = ParameterTypes.String_NumberStyles, IsStatic = true },
                invocationExpression.PsiModule))
        {
            consumer.AddHighlighting(new RedundantArgumentHint("Passing null is redundant.", providerArgument));
        }
    }

    /// <remarks>
    /// <c>T.Parse(s, null)</c> → <c>T.Parse(s)</c> (.NET Core 2.1)
    /// </remarks>
    void AnalyzeParse_ReadOnlySpanOfChar_IFormatProvider(
        IHighlightingConsumer consumer,
        IInvocationExpression invocationExpression,
        ICSharpArgument providerArgument)
    {
        if (providerArgument.Value.IsDefaultValue()
            && numberInfo.ClrTypeName.HasMethod(
                new MethodSignature
                {
                    Name = "Parse", // todo: nameof(INumberBase<T>.Parse) when available
                    ParameterTypes = ParameterTypes.ReadOnlySpanOfT_NumberStyles_IFormatProvider,
                    IsStatic = true,
                },
                invocationExpression.PsiModule))
        {
            consumer.AddHighlighting(new RedundantArgumentHint("Passing null is redundant.", providerArgument));
        }
    }

    /// <remarks>
    /// <c>T.Parse(utf8Text, null)</c> → <c>T.Parse(utf8Text)</c> (.NET 8)
    /// </remarks>
    void AnalyzeParse_ReadOnlySpanOfByte_IFormatProvider(
        IHighlightingConsumer consumer,
        IInvocationExpression invocationExpression,
        ICSharpArgument providerArgument)
    {
        if (providerArgument.Value.IsDefaultValue()
            && numberInfo.ClrTypeName.HasMethod(
                new MethodSignature
                {
                    Name = "Parse", // todo: nameof(INumberBase<T>.Parse) when available
                    ParameterTypes = ParameterTypes.ReadOnlySpanOfT_NumberStyles_IFormatProvider,
                    IsStatic = true,
                },
                invocationExpression.PsiModule))
        {
            consumer.AddHighlighting(new RedundantArgumentHint("Passing null is redundant.", providerArgument));
        }
    }

    /// <remarks>
    /// <c>number.ToString(null)</c> → <c>number.ToString()</c><para/>
    /// <c>number.ToString("")</c> → <c>number.ToString()</c><para/>
    /// <c>number.ToString("G...")</c> → <c>number.ToString()</c><para/>
    /// <c>number.ToString("E6")</c> → <c>number.ToString("E")</c><para/>
    /// <c>number.ToString("R...")</c> → <c>number.ToString("G17")</c> (for <see cref="double"/> values)<para/>
    /// <c>number.ToString("R...")</c> → <c>number.ToString("G9")</c> (for <see cref="float"/> values)<para/>
    /// <c>number.ToString("R...")</c> → ⚠️ (for integer values)<para/>
    /// <c>number.ToString("B0")</c> → <c>number.ToString("B")</c> (for integer values)<para/>
    /// <c>number.ToString("B1")</c> → <c>number.ToString("B")</c> (for integer values)<para/>
    /// <c>number.ToString("X0")</c> → <c>number.ToString("X")</c> (for integer values)<para/>
    /// <c>number.ToString("X1")</c> → <c>number.ToString("X")</c> (for integer values)<para/>
    /// <c>number.ToString("D0")</c> → <c>number.ToString("D")</c> (for integer values)<para/>
    /// <c>number.ToString("D1")</c> → <c>number.ToString("D")</c> (for integer values)<para/>
    /// </remarks>
    void AnalyzeToString_String(IHighlightingConsumer consumer, IInvocationExpression invocationExpression, ICSharpArgument formatArgument)
    {
        var format = formatArgument.Value.TryGetStringConstant();

        if ((formatArgument.Value.IsDefaultValue() || format == "")
            && numberInfo.ClrTypeName.HasMethod(new MethodSignature { Name = nameof(ToString), ParameterTypes = [] }, invocationExpression.PsiModule))
        {
            consumer.AddHighlighting(new RedundantArgumentHint("Passing null or an empty string is redundant.", formatArgument));
        }

        switch (format)
        {
            case ['G', .. var precisionSpecifier] when (precisionSpecifier == ""
                    || int.TryParse(precisionSpecifier, out var precision)
                    && (precision == 0 || numberInfo.MaxValueStringLength is { } maxValueStringLength && precision >= maxValueStringLength))
                && numberInfo.ClrTypeName.HasMethod(
                    new MethodSignature { Name = nameof(ToString), ParameterTypes = [] },
                    invocationExpression.PsiModule):
            {
                consumer.AddHighlighting(new RedundantArgumentHint($"Passing \"{format}\" is redundant.", formatArgument));
                break;
            }

            case ['g', .. var precisionSpecifier] when numberInfo.SupportsCaseInsensitiveGeneralFormatSpecifierWithoutPrecision
                && (precisionSpecifier == ""
                    || int.TryParse(precisionSpecifier, out var precision)
                    && (precision == 0 || numberInfo.MaxValueStringLength is { } maxValueStringLength && precision >= maxValueStringLength))
                && numberInfo.ClrTypeName.HasMethod(
                    new MethodSignature { Name = nameof(ToString), ParameterTypes = [] },
                    invocationExpression.PsiModule):
            {
                consumer.AddHighlighting(new RedundantArgumentHint($"Passing \"{format}\" is redundant.", formatArgument));
                break;
            }

            case ['R' or 'r', .. var precisionSpecifier]:
            {
                switch (numberInfo.GetRoundTripFormatSpecifier(precisionSpecifier, out var replacement))
                {
                    case RoundTripFormatSpecifierSupport.Unsupported:
                        consumer.AddHighlighting(new SuspiciousFormatSpecifierWarning("The format specifier might be unsupported.", formatArgument));
                        break;

                    case RoundTripFormatSpecifierSupport.ToBeReplaced:
                        Debug.Assert(replacement is { });
                        consumer.AddHighlighting(
                            new PassOtherFormatSpecifierSuggestion(
                                $"Pass the '{replacement}' format specifier (string length may vary).",
                                formatArgument,
                                replacement));
                        break;

                    case RoundTripFormatSpecifierSupport.RedundantPrecisionSpecifier:
                        consumer.AddHighlighting(
                            new RedundantFormatPrecisionSpecifierHint(
                                $"The format precision specifier is redundant, '{format[0].ToString()}' has the same effect.",
                                formatArgument));
                        break;

                    case RoundTripFormatSpecifierSupport.Ignore:
                        // do nothing
                        break;
                }
                break;
            }

            case ['E' or 'e', .. var precisionSpecifier] when precisionSpecifier != ""
                && int.TryParse(precisionSpecifier, out var precision)
                && precision == 6:
            {
                consumer.AddHighlighting(
                    new RedundantFormatPrecisionSpecifierHint(
                        $"The format precision specifier is redundant, '{format[0].ToString()}' has the same effect.",
                        formatArgument));
                break;
            }

            case ['D' or 'd', .. var precisionSpecifier] when numberInfo.SupportsDecimalFormatSpecifier
                && precisionSpecifier != ""
                && int.TryParse(precisionSpecifier, out var precision)
                && precision is 0 or 1:
            {
                consumer.AddHighlighting(
                    new RedundantFormatPrecisionSpecifierHint(
                        $"The format precision specifier is redundant, '{format[0].ToString()}' has the same effect.",
                        formatArgument));
                break;
            }

            case ['B' or 'b' or 'X' or 'x', .. var precisionSpecifier] when numberInfo.SupportsBinaryOrHexFormatSpecifier
                && precisionSpecifier != ""
                && int.TryParse(precisionSpecifier, out var precision)
                && precision is 0 or 1:
            {
                consumer.AddHighlighting(
                    new RedundantFormatPrecisionSpecifierHint(
                        $"The format precision specifier is redundant, '{format[0].ToString()}' has the same effect.",
                        formatArgument));
                break;
            }
        }
    }

    /// <remarks>
    /// <c>number.ToString(null)</c> → <c>number.ToString()</c>
    /// </remarks>
    void AnalyzeToString_IFormatProvider(IHighlightingConsumer consumer, IInvocationExpression invocationExpression, ICSharpArgument providerArgument)
    {
        if (providerArgument.Value.IsDefaultValue()
            && numberInfo.ClrTypeName.HasMethod(new MethodSignature { Name = nameof(ToString), ParameterTypes = [] }, invocationExpression.PsiModule))
        {
            consumer.AddHighlighting(new RedundantArgumentHint("Passing null is redundant.", providerArgument));
        }
    }

    /// <remarks>
    /// <c>number.ToString(null, provider)</c> → <c>number.ToString(provider)</c><para/>
    /// <c>number.ToString("", provider)</c> → <c>number.ToString(provider)</c><para/>
    /// <c>number.ToString("G...", provider)</c> → <c>number.ToString(provider)</c><para/>
    /// <c>number.ToString("E6", provider)</c> → <c>number.ToString("E", provider)</c><para/>
    /// <c>number.ToString("R...", provider)</c> → <c>number.ToString("G17")</c> (for <see cref="double"/> values)<para/>
    /// <c>number.ToString("R...", provider)</c> → <c>number.ToString("G9")</c> (for <see cref="float"/> values)<para/>
    /// <c>number.ToString("R...", provider)</c> → ⚠️ (for integer values)<para/>
    /// <c>number.ToString("B0", provider)</c> → <c>number.ToString("B")</c> (for integer values)<para/>
    /// <c>number.ToString("B1", provider)</c> → <c>number.ToString("B")</c> (for integer values)<para/>
    /// <c>number.ToString("B...", provider)</c> → <c>number.ToString("B...")</c> (for integer values)<para/>
    /// <c>number.ToString("X0", provider)</c> → <c>number.ToString("X")</c> (for integer values)<para/>
    /// <c>number.ToString("X1", provider)</c> → <c>number.ToString("X")</c> (for integer values)<para/>
    /// <c>number.ToString("X...", provider)</c> → <c>number.ToString("X...")</c> (for integer values)<para/>
    /// <c>number.ToString("D0", provider)</c> → <c>number.ToString("D", provider)</c> (for integer values)<para/>
    /// <c>number.ToString("D1", provider)</c> → <c>number.ToString("D", provider)</c> (for integer values)<para/>
    /// <c>number.ToString(format, null)</c> → <c>number.ToString(format)</c>
    /// </remarks>
    void AnalyzeToString_String_IFormatProvider(
        IHighlightingConsumer consumer,
        IInvocationExpression invocationExpression,
        ICSharpArgument formatArgument,
        ICSharpArgument providerArgument)
    {
        var format = formatArgument.Value.TryGetStringConstant();

        if ((formatArgument.Value.IsDefaultValue() || format == "")
            && numberInfo.ClrTypeName.HasMethod(
                new MethodSignature { Name = nameof(ToString), ParameterTypes = ParameterTypes.IFormatProvider },
                invocationExpression.PsiModule))
        {
            consumer.AddHighlighting(new RedundantArgumentHint("Passing null or an empty string is redundant.", formatArgument));
        }

        switch (format)
        {
            case ['G', .. var precisionSpecifier]
                when (precisionSpecifier == ""
                    || int.TryParse(precisionSpecifier, out var precision)
                    && (precision == 0 || numberInfo.MaxValueStringLength is { } maxValueStringLength && precision >= maxValueStringLength))
                && numberInfo.ClrTypeName.HasMethod(
                    new MethodSignature { Name = nameof(ToString), ParameterTypes = ParameterTypes.IFormatProvider },
                    invocationExpression.PsiModule):
            {
                consumer.AddHighlighting(new RedundantArgumentHint($"Passing \"{format}\" is redundant.", formatArgument));
                break;
            }

            case ['g', .. var precisionSpecifier] when numberInfo.SupportsCaseInsensitiveGeneralFormatSpecifierWithoutPrecision
                && (precisionSpecifier == ""
                    || int.TryParse(precisionSpecifier, out var precision)
                    && (precision == 0 || numberInfo.MaxValueStringLength is { } maxValueStringLength && precision >= maxValueStringLength))
                && numberInfo.ClrTypeName.HasMethod(
                    new MethodSignature { Name = nameof(ToString), ParameterTypes = ParameterTypes.IFormatProvider },
                    invocationExpression.PsiModule):
            {
                consumer.AddHighlighting(new RedundantArgumentHint($"Passing \"{format}\" is redundant.", formatArgument));
                break;
            }

            case ['R' or 'r', .. var precisionSpecifier]:
            {
                switch (numberInfo.GetRoundTripFormatSpecifier(precisionSpecifier, out var replacement))
                {
                    case RoundTripFormatSpecifierSupport.Unsupported:
                        consumer.AddHighlighting(new SuspiciousFormatSpecifierWarning("The format specifier might be unsupported.", formatArgument));
                        break;

                    case RoundTripFormatSpecifierSupport.ToBeReplaced:
                        Debug.Assert(replacement is { });
                        consumer.AddHighlighting(
                            new PassOtherFormatSpecifierSuggestion(
                                $"Pass the '{replacement}' format specifier (string length may vary).",
                                formatArgument,
                                replacement));
                        break;

                    case RoundTripFormatSpecifierSupport.RedundantPrecisionSpecifier:
                        consumer.AddHighlighting(
                            new RedundantFormatPrecisionSpecifierHint(
                                $"The format precision specifier is redundant, '{format[0].ToString()}' has the same effect.",
                                formatArgument));
                        break;

                    case RoundTripFormatSpecifierSupport.Ignore:
                        // do nothing
                        break;
                }
                break;
            }

            case ['E' or 'e', .. var precisionSpecifier] when precisionSpecifier != ""
                && int.TryParse(precisionSpecifier, out var precision)
                && precision == 6:
            {
                consumer.AddHighlighting(
                    new RedundantFormatPrecisionSpecifierHint(
                        $"The format precision specifier is redundant, '{format[0].ToString()}' has the same effect.",
                        formatArgument));
                break;
            }

            case ['D' or 'd', .. var precisionSpecifier] when numberInfo.SupportsDecimalFormatSpecifier
                && precisionSpecifier != ""
                && int.TryParse(precisionSpecifier, out var precision)
                && precision is 0 or 1:
            {
                consumer.AddHighlighting(
                    new RedundantFormatPrecisionSpecifierHint(
                        $"The format precision specifier is redundant, '{format[0].ToString()}' has the same effect.",
                        formatArgument));
                break;
            }

            case ['B' or 'b' or 'X' or 'x', .. var precisionSpecifier] when numberInfo.SupportsBinaryOrHexFormatSpecifier:
            {
                if (precisionSpecifier != "" && int.TryParse(precisionSpecifier, out var precision) && precision is 0 or 1)
                {
                    consumer.AddHighlighting(
                        new RedundantFormatPrecisionSpecifierHint(
                            $"The format precision specifier is redundant, '{format[0].ToString()}' has the same effect.",
                            formatArgument));
                }

                if (!providerArgument.Value.IsDefaultValue()
                    && numberInfo.ClrTypeName.HasMethod(
                        new MethodSignature { Name = nameof(ToString), ParameterTypes = ParameterTypes.String },
                        invocationExpression.PsiModule))
                {
                    consumer.AddHighlighting(
                        new RedundantArgumentHint(
                            "Passing a provider with a binary or hexadecimal format specifier is redundant.",
                            providerArgument));
                }

                break;
            }
        }

        if (providerArgument.Value.IsDefaultValue())
        {
            if (numberInfo.ClrTypeName.HasMethod(
                new MethodSignature { Name = nameof(ToString), ParameterTypes = ParameterTypes.String },
                invocationExpression.PsiModule))
            {
                consumer.AddHighlighting(new RedundantArgumentHint("Passing null is redundant.", providerArgument));
            }
        }
    }

    /// <remarks>
    /// <c>T.TryParse(s, defaultStyle, provider, out result)</c> → <c>T.TryParse(s, provider, out result)</c> (.NET 7)
    /// </remarks>
    void AnalyzeTryParse_String_NumberStyles_IFormatProvider_N(
        IHighlightingConsumer consumer,
        IInvocationExpression invocationExpression,
        ICSharpArgument styleArgument)
    {
        var defaultNumberStyles = numberInfo.DefaultNumberStyles;

        if (styleArgument.Value.TryGetNumberStylesConstant() == defaultNumberStyles
            && numberInfo.ClrTypeName.HasMethod(
                new MethodSignature
                {
                    Name = "TryParse", // todo: nameof(IParsable<T>.TryParse) when available
                    ParameterTypes = [..ParameterTypes.String_IFormatProvider, new ParameterType { ClrTypeName = numberInfo.ClrTypeName }],
                    IsStatic = true,
                },
                invocationExpression.PsiModule))
        {
            var styles = string.Join(" | ", from t in $"{defaultNumberStyles:G}".Split(',') select $"{nameof(NumberStyles)}.{t.Trim()}");

            consumer.AddHighlighting(new RedundantArgumentHint($"Passing {styles} is redundant.", styleArgument));
        }
    }

    /// <remarks>
    /// <c>T.TryParse(s, null, out result)</c> → <c>T.TryParse(s, out result)</c>
    /// </remarks>
    void AnalyzeTryParse_String_IFormatProvider_N(
        IHighlightingConsumer consumer,
        IInvocationExpression invocationExpression,
        ICSharpArgument providerArgument)
    {
        if (providerArgument.Value.IsDefaultValue()
            && numberInfo.ClrTypeName.HasMethod(
                new MethodSignature
                {
                    Name = nameof(int.TryParse),
                    ParameterTypes = [..ParameterTypes.String, new ParameterType { ClrTypeName = numberInfo.ClrTypeName }],
                    IsStatic = true,
                },
                invocationExpression.PsiModule))
        {
            consumer.AddHighlighting(new RedundantArgumentHint("Passing null is redundant.", providerArgument));
        }
    }

    /// <remarks>
    /// <c>T.TryParse(s, defaultStyle, provider, out result)</c> → <c>T.TryParse(s, provider, out result)</c> (.NET 7)
    /// </remarks>
    void AnalyzeTryParse_ReadOnlySpanOfChar_NumberStyles_IFormatProvider_N(
        IHighlightingConsumer consumer,
        IInvocationExpression invocationExpression,
        ICSharpArgument styleArgument)
    {
        var defaultNumberStyles = numberInfo.DefaultNumberStyles;

        if (styleArgument.Value.TryGetNumberStylesConstant() == defaultNumberStyles
            && numberInfo.ClrTypeName.HasMethod(
                new MethodSignature
                {
                    Name = "TryParse", // todo: nameof(IParsable<T>.TryParse) when available
                    ParameterTypes = [..ParameterTypes.ReadOnlySpanOfT_IFormatProvider, new ParameterType { ClrTypeName = numberInfo.ClrTypeName }],
                    IsStatic = true,
                },
                invocationExpression.PsiModule))
        {
            var styles = string.Join(" | ", from t in $"{defaultNumberStyles:G}".Split(',') select $"{nameof(NumberStyles)}.{t.Trim()}");

            consumer.AddHighlighting(new RedundantArgumentHint($"Passing {styles} is redundant.", styleArgument));
        }
    }

    /// <remarks>
    /// <c>T.TryParse(s, null, out result)</c> → <c>T.TryParse(s, out result)</c> (.NET Core 2.1)
    /// </remarks>
    void AnalyzeTryParse_ReadOnlySpanOfChar_IFormatProvider_N(
        IHighlightingConsumer consumer,
        IInvocationExpression invocationExpression,
        ICSharpArgument providerArgument)
    {
        if (providerArgument.Value.IsDefaultValue()
            && numberInfo.ClrTypeName.HasMethod(
                new MethodSignature
                {
                    Name = "TryParse", // todo: nameof(ISpanParsable<T>.TryParse) when available
                    ParameterTypes = [..ParameterTypes.ReadOnlySpanOfT, new ParameterType { ClrTypeName = numberInfo.ClrTypeName }],
                    IsStatic = true,
                },
                invocationExpression.PsiModule))
        {
            consumer.AddHighlighting(new RedundantArgumentHint("Passing null is redundant.", providerArgument));
        }
    }

    /// <remarks>
    /// <c>T.TryParse(utf8Text, defaultStyle, provider, out result)</c> → <c>T.TryParse(utf8Text, provider, out result)</c> (.NET 8)
    /// </remarks>
    void AnalyzeTryParse_ReadOnlySpanOfByte_NumberStyles_IFormatProvider_N(
        IHighlightingConsumer consumer,
        IInvocationExpression invocationExpression,
        ICSharpArgument styleArgument)
    {
        var defaultNumberStyles = numberInfo.DefaultNumberStyles;

        if (styleArgument.Value.TryGetNumberStylesConstant() == defaultNumberStyles
            && numberInfo.ClrTypeName.HasMethod(
                new MethodSignature
                {
                    Name = "TryParse", // todo: nameof(IUtf8SpanParsable<T>.TryParse) when available
                    ParameterTypes = [..ParameterTypes.ReadOnlySpanOfT_IFormatProvider, new ParameterType { ClrTypeName = numberInfo.ClrTypeName }],
                    IsStatic = true,
                },
                invocationExpression.PsiModule))
        {
            var styles = string.Join(" | ", from t in $"{defaultNumberStyles:G}".Split(',') select $"{nameof(NumberStyles)}.{t.Trim()}");

            consumer.AddHighlighting(new RedundantArgumentHint($"Passing {styles} is redundant.", styleArgument));
        }
    }

    /// <remarks>
    /// <c>T.TryParse(utf8Text, null, out result)</c> → <c>T.TryParse(utf8Text, out result)</c> (.NET 8)
    /// </remarks>
    void AnalyzeTryParse_ReadOnlySpanOfByte_IFormatProvider_N(
        IHighlightingConsumer consumer,
        IInvocationExpression invocationExpression,
        ICSharpArgument providerArgument)
    {
        if (providerArgument.Value.IsDefaultValue()
            && numberInfo.ClrTypeName.HasMethod(
                new MethodSignature
                {
                    Name = nameof(int.TryParse),
                    ParameterTypes = [..ParameterTypes.ReadOnlySpanOfT, new ParameterType { ClrTypeName = numberInfo.ClrTypeName }],
                    IsStatic = true,
                },
                invocationExpression.PsiModule))
        {
            consumer.AddHighlighting(new RedundantArgumentHint("Passing null is redundant.", providerArgument));
        }
    }

    [Pure]
    private protected string GetReplacementFromArgument(IInvocationExpression invocationExpression, ICSharpExpression argumentValue)
        => invocationExpression.TryGetTargetType().IsClrType(numberInfo.ClrTypeName) || argumentValue.Type().IsClrType(numberInfo.ClrTypeName)
            ? argumentValue.GetText()
            : numberInfo.TryGetConstant(argumentValue, out var valueImplicitlyConverted) is { } && valueImplicitlyConverted
                ? numberInfo.CastConstant(argumentValue, valueImplicitlyConverted)
                : numberInfo.Cast(argumentValue);

    private protected virtual void Analyze(
        IInvocationExpression element,
        IReferenceExpression invokedExpression,
        IMethod method,
        IHighlightingConsumer consumer)
    {
        if (method.ContainingType.IsClrType(numberInfo.ClrTypeName))
        {
            switch (invokedExpression, method)
            {
                case ({ QualifierExpression: { } }, { IsStatic: false }):
                    switch (method.ShortName)
                    {
                        case nameof(Equals):
                            switch (method.Parameters, element.Arguments)
                            {
                                case ([{ Type: var objType }], [var objArgument]) when objType.IsClrType(numberInfo.ClrTypeName):
                                    AnalyzeEquals_N(consumer, element, invokedExpression, objArgument);
                                    break;

                                case ([{ Type: var objType }], [var objArgument]) when objType.IsObject():
                                    AnalyzeEquals_Object(consumer, element, objArgument);
                                    break;
                            }
                            break;

                        case nameof(IConvertible.GetTypeCode):
                            switch (method.Parameters, element.Arguments)
                            {
                                case ([], []): AnalyzeGetTypeCode(consumer, element); break;
                            }
                            break;

                        case nameof(ToString):
                            switch (method.Parameters, element.Arguments)
                            {
                                case ([{ Type: var formatType }], [var formatArgument]) when formatType.IsString():
                                    AnalyzeToString_String(consumer, element, formatArgument);
                                    break;

                                case ([{ Type: var providerType }], [var providerArgument]) when providerType.IsIFormatProvider():
                                    AnalyzeToString_IFormatProvider(consumer, element, providerArgument);
                                    break;

                                case ([{ Type: var formatType }, { Type: var providerType }], [var formatArgument, var providerArgument])
                                    when formatType.IsString() && providerType.IsIFormatProvider():

                                    AnalyzeToString_String_IFormatProvider(consumer, element, formatArgument, providerArgument);
                                    break;
                            }
                            break;
                    }
                    break;

                case (_, { IsStatic: true }):
                    switch (method.ShortName)
                    {
                        case "Clamp": // todo: nameof(INumber<T>.Clamp) when available
                            switch (method.Parameters, element.Arguments)
                            {
                                case ([{ Type: var valueType }, { Type: var minType }, { Type: var maxType }], [
                                        var valueArgument, var minArgument, var maxArgument,
                                    ]) when valueType.IsClrType(numberInfo.ClrTypeName)
                                    && minType.IsClrType(numberInfo.ClrTypeName)
                                    && maxType.IsClrType(numberInfo.ClrTypeName):

                                    AnalyzeClamp(consumer, element, valueArgument, minArgument, maxArgument);
                                    break;
                            }
                            break;

                        case "Max": // todo: nameof(INumber<T>.Max) when available
                            switch (method.Parameters, element.Arguments)
                            {
                                case ([{ Type: var xType }, { Type: var yType }], [var xArgument, var yArgument])
                                    when xType.IsClrType(numberInfo.ClrTypeName) && yType.IsClrType(numberInfo.ClrTypeName):

                                    AnalyzeMax(consumer, element, xArgument, yArgument);
                                    break;
                            }
                            break;

                        case "Min": // todo: nameof(INumber<T>.Min) when available
                            switch (method.Parameters, element.Arguments)
                            {
                                case ([{ Type: var xType }, { Type: var yType }], [var xArgument, var yArgument])
                                    when xType.IsClrType(numberInfo.ClrTypeName) && yType.IsClrType(numberInfo.ClrTypeName):

                                    AnalyzeMin(consumer, element, xArgument, yArgument);
                                    break;
                            }
                            break;

                        case nameof(int.Parse):
                            switch (method.Parameters, element.Arguments)
                            {
                                case ([{ Type: var sType }, { Type: var styleType }], [_, var styleArgument])
                                    when sType.IsString() && IsNumberStyles(styleType):

                                    AnalyzeParse_String_NumberStyles(consumer, element, styleArgument);
                                    break;

                                case ([{ Type: var sType }, { Type: var providerType }], [_, var providerArgument])
                                    when sType.IsString() && providerType.IsIFormatProvider():

                                    AnalyzeParse_String_IFormatProvider(consumer, element, providerArgument);
                                    break;

                                case ([{ Type: var sType }, { Type: var styleType }, { Type: var providerType }], [
                                    _, var styleArgument, var providerArgument,
                                ]) when sType.IsString() && IsNumberStyles(styleType) && providerType.IsIFormatProvider():
                                    AnalyzeParse_String_NumberStyles_IFormatProvider(consumer, element, styleArgument, providerArgument);
                                    break;

                                case ([{ Type: var sType }, { Type: var providerType }], [_, var providerArgument])
                                    when sType.IsReadOnlySpan(out var spanTypeArgument)
                                    && spanTypeArgument.IsChar()
                                    && providerType.IsIFormatProvider():

                                    AnalyzeParse_ReadOnlySpanOfChar_IFormatProvider(consumer, element, providerArgument);
                                    break;

                                case ([{ Type: var utf8TextType }, { Type: var providerType }], [_, var providerArgument])
                                    when utf8TextType.IsReadOnlySpan(out var spanTypeArgument)
                                    && spanTypeArgument.IsByte()
                                    && providerType.IsIFormatProvider():

                                    AnalyzeParse_ReadOnlySpanOfByte_IFormatProvider(consumer, element, providerArgument);
                                    break;
                            }
                            break;

                        case nameof(int.TryParse):
                            switch (method.Parameters, element.Arguments)
                            {
                                case ([{ Type: var sType }, { Type: var styleType }, { Type: var providerType }, { Type: var resultType }], [
                                        _, var styleArgument, _, _,
                                    ]) when sType.IsString()
                                    && IsNumberStyles(styleType)
                                    && providerType.IsIFormatProvider()
                                    && resultType.IsClrType(numberInfo.ClrTypeName):

                                    AnalyzeTryParse_String_NumberStyles_IFormatProvider_N(consumer, element, styleArgument);
                                    break;

                                case ([{ Type: var sType }, { Type: var providerType }, { Type: var resultType }], [_, var providerArgument, _])
                                    when sType.IsString() && providerType.IsIFormatProvider() && resultType.IsClrType(numberInfo.ClrTypeName):

                                    AnalyzeTryParse_String_IFormatProvider_N(consumer, element, providerArgument);
                                    break;

                                case ([{ Type: var sType }, { Type: var styleType }, { Type: var providerType }, { Type: var resultType }], [
                                        _, var styleArgument, _, _,
                                    ]) when sType.IsReadOnlySpan(out var spanTypeArgument)
                                    && spanTypeArgument.IsChar()
                                    && IsNumberStyles(styleType)
                                    && providerType.IsIFormatProvider()
                                    && resultType.IsClrType(numberInfo.ClrTypeName):

                                    AnalyzeTryParse_ReadOnlySpanOfChar_NumberStyles_IFormatProvider_N(consumer, element, styleArgument);
                                    break;

                                case ([{ Type: var sType }, { Type: var providerType }, { Type: var resultType }], [_, var providerArgument, _])
                                    when sType.IsReadOnlySpan(out var spanTypeArgument)
                                    && spanTypeArgument.IsChar()
                                    && providerType.IsIFormatProvider()
                                    && resultType.IsClrType(numberInfo.ClrTypeName):

                                    AnalyzeTryParse_ReadOnlySpanOfChar_IFormatProvider_N(consumer, element, providerArgument);
                                    break;

                                case ([{ Type: var utf8TextType }, { Type: var styleType }, { Type: var providerType }, { Type: var resultType }], [
                                        _, var styleArgument, _, _,
                                    ]) when utf8TextType.IsReadOnlySpan(out var spanTypeArgument)
                                    && spanTypeArgument.IsByte()
                                    && IsNumberStyles(styleType)
                                    && providerType.IsIFormatProvider()
                                    && resultType.IsClrType(numberInfo.ClrTypeName):

                                    AnalyzeTryParse_ReadOnlySpanOfByte_NumberStyles_IFormatProvider_N(consumer, element, styleArgument);
                                    break;

                                case ([{ Type: var sType }, { Type: var providerType }, { Type: var resultType }], [_, var providerArgument, _])
                                    when sType.IsReadOnlySpan(out var spanTypeArgument)
                                    && spanTypeArgument.IsByte()
                                    && providerType.IsIFormatProvider()
                                    && resultType.IsClrType(numberInfo.ClrTypeName):

                                    AnalyzeTryParse_ReadOnlySpanOfByte_IFormatProvider_N(consumer, element, providerArgument);
                                    break;
                            }
                            break;
                    }
                    break;
            }
        }

        if (method.ContainingType.IsClrType(ClrTypeNames.Math) && method.IsStatic)
        {
            switch (method.ShortName)
            {
                case "Clamp": // todo: nameof(Math.Clamp) when available
                    switch (method.Parameters, element.Arguments)
                    {
                        case ([{ Type: var valueType }, { Type: var minType }, { Type: var maxType }], [
                                var valueArgument, var minArgument, var maxArgument,
                            ]) when valueType.IsClrType(numberInfo.ClrTypeName)
                            && minType.IsClrType(numberInfo.ClrTypeName)
                            && maxType.IsClrType(numberInfo.ClrTypeName):

                            AnalyzeClamp(consumer, element, valueArgument, minArgument, maxArgument);
                            break;
                    }
                    break;

                case nameof(Math.Max):
                    switch (method.Parameters, element.Arguments)
                    {
                        case ([{ Type: var val1Type }, { Type: var val2Type }], [var val1Argument, var val2Argument])
                            when val1Type.IsClrType(numberInfo.ClrTypeName) && val2Type.IsClrType(numberInfo.ClrTypeName):

                            AnalyzeMax(consumer, element, val1Argument, val2Argument);
                            break;
                    }
                    break;

                case nameof(Math.Min):
                    switch (method.Parameters, element.Arguments)
                    {
                        case ([{ Type: var val1Type }, { Type: var val2Type }], [var val1Argument, var val2Argument])
                            when val1Type.IsClrType(numberInfo.ClrTypeName) && val2Type.IsClrType(numberInfo.ClrTypeName):

                            AnalyzeMin(consumer, element, val1Argument, val2Argument);
                            break;
                    }
                    break;
            }
        }
    }

    protected sealed override void Run(IInvocationExpression element, ElementProblemAnalyzerData data, IHighlightingConsumer consumer)
    {
        if (element is { InvokedExpression: IReferenceExpression { Reference: var reference } invokedExpression }
            && reference.Resolve().DeclaredElement is IMethod
            {
                AccessibilityDomain.DomainType: AccessibilityDomain.AccessibilityDomainType.PUBLIC, TypeParameters: [],
            } method)
        {
            Analyze(element, invokedExpression, method, consumer);
        }
    }
}