using System.Runtime.CompilerServices;
using System.Text;
using JetBrains.ReSharper.Feature.Services.Daemon;
using JetBrains.ReSharper.Feature.Services.Util.ClrLanguages;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.CSharp.Tree;
using JetBrains.ReSharper.Psi.Util;
using JetBrains.Util;
using ReCommendedExtension.Analyzers.BaseTypes.NumberInfos;

namespace ReCommendedExtension.Analyzers.BaseTypes.Analyzers;

[ElementProblemAnalyzer(
    typeof(ICSharpArgumentsOwner),
    HighlightingTypes =
    [
        typeof(RedundantFormatSpecifierHint),
        typeof(RedundantFormatPrecisionSpecifierHint),
        typeof(PassOtherFormatSpecifierSuggestion),
        typeof(SuspiciousFormatSpecifierWarning),
    ])]
public sealed class FormatStringAnalyzer(FormattingFunctionInvocationInfoProvider formattingFunctionInvocationInfoProvider)
    : ElementProblemAnalyzer<ICSharpArgumentsOwner>
{
    [Pure]
    static bool IsWellKnownImplementation(FormattingFunctionInvocationInfo formattingFunctionInvocationInfo)
    {
        if (formattingFunctionInvocationInfo.Invocation.InvokedFunction is IMethod method)
        {
            return method is { ShortName: nameof(string.Format), IsStatic: true } && method.ContainingType.IsSystemString()
                || method is { ShortName: nameof(FormattableStringFactory.Create), IsStatic: true }
                && method.ContainingType.IsClrType(PredefinedType.FORMATTABLE_STRING_FACTORY_FQN)
                || method is { ShortName: nameof(StringBuilder.AppendFormat), IsSealed: false }
                && method.ContainingType.IsClrType(PredefinedType.STRING_BUILDER_FQN)
                || method is { ShortName: nameof(TextWriter.Write), IsStatic: false } && method.ContainingType.IsClrType(ClrTypeNames.TextWriter)
                || method is { ShortName: nameof(TextWriter.WriteLine), IsStatic: false } && method.ContainingType.IsClrType(ClrTypeNames.TextWriter)
                || method is { ShortName: nameof(Console.Write), IsStatic: true } && method.ContainingType.IsClrType(PredefinedType.CONSOLE_FQN)
                || method is { ShortName: nameof(Console.WriteLine), IsStatic: true } && method.ContainingType.IsClrType(PredefinedType.CONSOLE_FQN);
        }

        return false;
    }

    protected override void Run(ICSharpArgumentsOwner element, ElementProblemAnalyzerData data, IHighlightingConsumer consumer)
    {
        if (formattingFunctionInvocationInfoProvider.TryGetByArgumentsOwner(element, out _) is
            {
                FormatString.Expression: ICSharpLiteralExpression formatStringExpression,
            } formattingFunctionInvocationInfo
            && IsWellKnownImplementation(formattingFunctionInvocationInfo))
        {
            var formatString = formatStringExpression.GetText();
            var formatItems = FormatStringParser.Parse(formatString);

            foreach (var formatItem in formatItems)
            {
                if (formatItem is { FormatStringRange : { IsValid: true, IsEmpty: false }, IndexValue: { } index and >= 0 }
                    && index < formattingFunctionInvocationInfo.FormattingExpressions.Count
                    && formattingFunctionInvocationInfo.FormattingExpressions[index] is { } formattingExpression)
                {
                    var format = formatString.Substring(formatItem.FormatStringRange);
                    var expressionType = formattingExpression.Type();

                    switch (format)
                    {
                        case ['G', .. var precisionSpecifier] when NumberInfo.TryGet(expressionType) is { } numberInfo
                            && (precisionSpecifier == ""
                                || int.TryParse(precisionSpecifier, out var precision)
                                && (precision == 0 && (numberInfo.FormatSpecifiers & FormatSpecifiers.GeneralZeroPrecisionRedundant) != 0
                                    || numberInfo.MaxValueStringLength is { } maxValueStringLength && precision >= maxValueStringLength)):
                        {
                            consumer.AddHighlighting(
                                new RedundantFormatSpecifierHint(
                                    $"Specifying 'G{precisionSpecifier}' is redundant.",
                                    formatStringExpression,
                                    formatItem));
                            break;
                        }

                        case ['g', .. var precisionSpecifier] when NumberInfo.TryGet(expressionType) is { } numberInfo
                            && (numberInfo.FormatSpecifiers & FormatSpecifiers.GeneralCaseInsensitiveWithoutPrecision) != 0
                            && (precisionSpecifier == ""
                                || int.TryParse(precisionSpecifier, out var precision)
                                && (precision == 0 && (numberInfo.FormatSpecifiers & FormatSpecifiers.GeneralZeroPrecisionRedundant) != 0
                                    || numberInfo.MaxValueStringLength is { } maxValueStringLength && precision >= maxValueStringLength)):
                        {
                            consumer.AddHighlighting(
                                new RedundantFormatSpecifierHint(
                                    $"Specifying 'g{precisionSpecifier}' is redundant.",
                                    formatStringExpression,
                                    formatItem));
                            break;
                        }

                        case ['G' or 'g'] when expressionType.IsEnumType() || expressionType.IsNullable() && expressionType.Unlift().IsEnumType():
                        {
                            consumer.AddHighlighting(
                                new RedundantFormatSpecifierHint(
                                    $"Specifying '{format[0].ToString()}' is redundant.",
                                    formatStringExpression,
                                    formatItem));
                            break;
                        }

                        case ['D' or 'd'] when expressionType.IsGuid() || expressionType.IsNullable() && expressionType.Unlift().IsGuid():
                        {
                            consumer.AddHighlighting(
                                new RedundantFormatSpecifierHint(
                                    $"Specifying '{format[0].ToString()}' is redundant.",
                                    formatStringExpression,
                                    formatItem));
                            break;
                        }

                        case ['E' or 'e', .. var precisionSpecifier] when NumberInfo.TryGet(expressionType) is { }
                            && precisionSpecifier != ""
                            && int.TryParse(precisionSpecifier, out var precision)
                            && precision == 6:
                        {
                            consumer.AddHighlighting(
                                new RedundantFormatPrecisionSpecifierHint(
                                    $"The format precision specifier is redundant, '{format[0].ToString()}' has the same effect.",
                                    formatStringExpression,
                                    formatItem));
                            break;
                        }

                        case ['D' or 'd', .. var precisionSpecifier] when NumberInfo.TryGet(expressionType) is { } numberInfo
                            && (numberInfo.FormatSpecifiers & FormatSpecifiers.Decimal) != 0
                            && precisionSpecifier != ""
                            && int.TryParse(precisionSpecifier, out var precision)
                            && precision is 0 or 1:
                        {
                            consumer.AddHighlighting(
                                new RedundantFormatPrecisionSpecifierHint(
                                    $"The format precision specifier is redundant, '{format[0].ToString()}' has the same effect.",
                                    formatStringExpression,
                                    formatItem));
                            break;
                        }

                        case ['B' or 'b', .. var precisionSpecifier] when NumberInfo.TryGet(expressionType) is { } numberInfo
                            && (numberInfo.FormatSpecifiers & FormatSpecifiers.Binary) != 0
                            && precisionSpecifier != ""
                            && int.TryParse(precisionSpecifier, out var precision)
                            && precision is 0 or 1:
                        {
                            consumer.AddHighlighting(
                                new RedundantFormatPrecisionSpecifierHint(
                                    $"The format precision specifier is redundant, '{format[0].ToString()}' has the same effect.",
                                    formatStringExpression,
                                    formatItem));
                            break;
                        }

                        case ['X' or 'x', .. var precisionSpecifier] when NumberInfo.TryGet(expressionType) is { } numberInfo
                            && (numberInfo.FormatSpecifiers & FormatSpecifiers.Hexadecimal) != 0
                            && precisionSpecifier != ""
                            && int.TryParse(precisionSpecifier, out var precision)
                            && precision is 0 or 1:
                        {
                            consumer.AddHighlighting(
                                new RedundantFormatPrecisionSpecifierHint(
                                    $"The format precision specifier is redundant, '{format[0].ToString()}' has the same effect.",
                                    formatStringExpression,
                                    formatItem));
                            break;
                        }

                        case ['R' or 'r', .. var precisionSpecifier] when NumberInfo.TryGet(expressionType) is { } numberInfo:
                        {
                            Debug.Assert(
                                (numberInfo.FormatSpecifiers
                                    & (FormatSpecifiers.RoundtripToBeReplaced | FormatSpecifiers.RoundtripPrecisionRedundant))
                                != (FormatSpecifiers.RoundtripToBeReplaced | FormatSpecifiers.RoundtripPrecisionRedundant));

                            if ((numberInfo.FormatSpecifiers & FormatSpecifiers.RoundtripToBeReplaced) != 0)
                            {
                                Debug.Assert(numberInfo.RoundTripFormatSpecifierReplacement is { });

                                consumer.AddHighlighting(
                                    new PassOtherFormatSpecifierSuggestion(
                                        $"Pass the '{numberInfo.RoundTripFormatSpecifierReplacement}' format specifier (string length may vary).",
                                        formatStringExpression,
                                        formatItem,
                                        numberInfo.RoundTripFormatSpecifierReplacement));
                            }

                            if ((numberInfo.FormatSpecifiers & FormatSpecifiers.RoundtripPrecisionRedundant) != 0 && precisionSpecifier != "")
                            {
                                consumer.AddHighlighting(
                                    new RedundantFormatPrecisionSpecifierHint(
                                        $"The format precision specifier is redundant, '{format[0].ToString()}' has the same effect.",
                                        formatStringExpression,
                                        formatItem));
                            }

                            if ((numberInfo.FormatSpecifiers
                                    & (FormatSpecifiers.RoundtripToBeReplaced | FormatSpecifiers.RoundtripPrecisionRedundant))
                                == 0)
                            {
                                consumer.AddHighlighting(
                                    new SuspiciousFormatSpecifierWarning(
                                        "The format specifier might be unsupported.",
                                        formatStringExpression,
                                        formatItem));
                            }

                            break;
                        }
                    }
                }
            }
        }
    }
}