using JetBrains.ReSharper.Feature.Services.Daemon;
using JetBrains.ReSharper.Psi.CSharp.Tree;
using ReCommendedExtension.Analyzers.BaseTypes.NumberInfos;

namespace ReCommendedExtension.Analyzers.BaseTypes.Analyzers;

[ElementProblemAnalyzer(
    typeof(IInterpolatedStringInsert),
    HighlightingTypes =
    [
        typeof(RedundantFormatSpecifierHint),
        typeof(RedundantFormatPrecisionSpecifierHint),
        typeof(PassOtherFormatSpecifierSuggestion),
        typeof(SuspiciousFormatSpecifierWarning),
    ])]
public sealed class InterpolatedStringItemAnalyzer : ElementProblemAnalyzer<IInterpolatedStringInsert>
{
    protected override void Run(IInterpolatedStringInsert element, ElementProblemAnalyzerData data, IHighlightingConsumer consumer)
    {
        var format = element.FormatSpecifier?.GetText();

        switch (format)
        {
            case [':', 'G', .. var precisionSpecifier] when NumberInfo.TryGet(element.Expression.Type()) is { } numberInfo
                && (precisionSpecifier == ""
                    || int.TryParse(precisionSpecifier, out var precision)
                    && (precision == 0 && (numberInfo.FormatSpecifiers & FormatSpecifiers.GeneralZeroPrecisionRedundant) != 0
                        || numberInfo.MaxValueStringLength is { } maxValueStringLength && precision >= maxValueStringLength)):
            {
                consumer.AddHighlighting(new RedundantFormatSpecifierHint($"Specifying 'G{precisionSpecifier}' is redundant.", element));
                break;
            }

            case [':', 'g', .. var precisionSpecifier] when NumberInfo.TryGet(element.Expression.Type()) is { } numberInfo
                && (numberInfo.FormatSpecifiers & FormatSpecifiers.GeneralCaseInsensitiveWithoutPrecision) != 0
                && (precisionSpecifier == ""
                    || int.TryParse(precisionSpecifier, out var precision)
                    && (precision == 0 && (numberInfo.FormatSpecifiers & FormatSpecifiers.GeneralZeroPrecisionRedundant) != 0
                        || numberInfo.MaxValueStringLength is { } maxValueStringLength && precision >= maxValueStringLength)):
            {
                consumer.AddHighlighting(new RedundantFormatSpecifierHint($"Specifying 'g{precisionSpecifier}' is redundant.", element));
                break;
            }

            case [':', 'E' or 'e', .. var precisionSpecifier] when precisionSpecifier != ""
                && int.TryParse(precisionSpecifier, out var precision)
                && precision == 6:
            {
                consumer.AddHighlighting(
                    new RedundantFormatPrecisionSpecifierHint(
                        $"The format precision specifier is redundant, '{format[1].ToString()}' has the same effect.",
                        element));
                break;
            }

            case [':', 'D' or 'd', .. var precisionSpecifier] when NumberInfo.TryGet(element.Expression.Type()) is { } numberInfo
                && (numberInfo.FormatSpecifiers & FormatSpecifiers.Decimal) != 0
                && precisionSpecifier != ""
                && int.TryParse(precisionSpecifier, out var precision)
                && precision is 0 or 1:
            {
                consumer.AddHighlighting(
                    new RedundantFormatPrecisionSpecifierHint(
                        $"The format precision specifier is redundant, '{format[1].ToString()}' has the same effect.",
                        element));
                break;
            }

            case [':', 'B' or 'b', .. var precisionSpecifier] when NumberInfo.TryGet(element.Expression.Type()) is { } numberInfo
                && (numberInfo.FormatSpecifiers & FormatSpecifiers.Binary) != 0
                && precisionSpecifier != ""
                && int.TryParse(precisionSpecifier, out var precision)
                && precision is 0 or 1:
            {
                consumer.AddHighlighting(
                    new RedundantFormatPrecisionSpecifierHint(
                        $"The format precision specifier is redundant, '{format[1].ToString()}' has the same effect.",
                        element));
                break;
            }

            case [':', 'X' or 'x', .. var precisionSpecifier] when NumberInfo.TryGet(element.Expression.Type()) is { } numberInfo
                && (numberInfo.FormatSpecifiers & FormatSpecifiers.Hexadecimal) != 0
                && precisionSpecifier != ""
                && int.TryParse(precisionSpecifier, out var precision)
                && precision is 0 or 1:
            {
                consumer.AddHighlighting(
                    new RedundantFormatPrecisionSpecifierHint(
                        $"The format precision specifier is redundant, '{format[1].ToString()}' has the same effect.",
                        element));
                break;
            }

            case [':', 'R' or 'r', .. var precisionSpecifier] when NumberInfo.TryGet(element.Expression.Type()) is { } numberInfo:
            {
                Debug.Assert(
                    (numberInfo.FormatSpecifiers & (FormatSpecifiers.RoundtripToBeReplaced | FormatSpecifiers.RoundtripPrecisionRedundant))
                    != (FormatSpecifiers.RoundtripToBeReplaced | FormatSpecifiers.RoundtripPrecisionRedundant));

                if ((numberInfo.FormatSpecifiers & FormatSpecifiers.RoundtripToBeReplaced) != 0)
                {
                    Debug.Assert(numberInfo.RoundTripFormatSpecifierReplacement is { });

                    consumer.AddHighlighting(
                        new PassOtherFormatSpecifierSuggestion(
                            $"Pass the '{numberInfo.RoundTripFormatSpecifierReplacement}' format specifier (string length may vary).",
                            element,
                            numberInfo.RoundTripFormatSpecifierReplacement));
                }

                if ((numberInfo.FormatSpecifiers & FormatSpecifiers.RoundtripPrecisionRedundant) != 0 && precisionSpecifier != "")
                {
                    consumer.AddHighlighting(
                        new RedundantFormatPrecisionSpecifierHint(
                            $"The format precision specifier is redundant, '{format[1].ToString()}' has the same effect.",
                            element));
                }

                if ((numberInfo.FormatSpecifiers & (FormatSpecifiers.RoundtripToBeReplaced | FormatSpecifiers.RoundtripPrecisionRedundant)) == 0)
                {
                    consumer.AddHighlighting(new SuspiciousFormatSpecifierWarning("The format specifier might be unsupported.", element));
                }

                break;
            }
        }
    }
}