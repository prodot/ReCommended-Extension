using System.Runtime.CompilerServices;
using JetBrains.ReSharper.Feature.Services.Daemon;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.CSharp.Tree;
using JetBrains.ReSharper.Psi.Util;
using ReCommendedExtension.Analyzers.BaseTypes.NumberInfos;

namespace ReCommendedExtension.Analyzers.BaseTypes.Analyzers;

[ElementProblemAnalyzer(
    typeof(IInterpolatedStringExpression),
    HighlightingTypes =
    [
        typeof(RedundantFormatSpecifierHint),
        typeof(RedundantFormatPrecisionSpecifierHint),
        typeof(PassOtherFormatSpecifierSuggestion),
        typeof(SuspiciousFormatSpecifierWarning),
    ])]
public sealed class InterpolatedStringExpressionAnalyzer : ElementProblemAnalyzer<IInterpolatedStringExpression>
{
    [Pure]
    static bool IsWellKnownImplementation(IInterpolatedStringExpression interpolatedStringExpression)
    {
        if (interpolatedStringExpression.HandlerConstructorReference.Resolve().DeclaredElement is IConstructor { ContainingType: var type })
        {
            return type.IsClrType(PredefinedType.DEFAULT_INTERPOLATED_STRING_HANDLER_FQN)
                || type.IsClrType(ClrTypeNames.AppendInterpolatedStringHandler)
                || type.IsClrType(ClrTypeNames.MemoryExtensions_TryWriteInterpolatedStringHandler)
                || type.IsClrType(ClrTypeNames.Utf8_TryWriteInterpolatedStringHandler)
                || type.IsClrType(ClrTypeNames.AssertInterpolatedStringHandler)
                || type.IsClrType(ClrTypeNames.WriteIfInterpolatedStringHandler)
                || type.IsClrType(ClrTypeNames.TraceVerboseInterpolatedStringHandler);
        }

        if (interpolatedStringExpression.FormatReference.Resolve().DeclaredElement is IMethod { IsStatic: true } method)
        {
            return method.ShortName is nameof(string.Format) or nameof(string.Concat) && method.ContainingType.IsSystemString()
                || method.ShortName == nameof(FormattableStringFactory.Create)
                && method.ContainingType.IsClrType(PredefinedType.FORMATTABLE_STRING_FACTORY_FQN);
        }

        return false;
    }

    protected override void Run(IInterpolatedStringExpression element, ElementProblemAnalyzerData data, IHighlightingConsumer consumer)
    {
        if (IsWellKnownImplementation(element))
        {
            foreach (var insert in element.Inserts)
            {
                var format = insert.FormatSpecifier?.GetText();
                var expressionType = insert.Expression.Type();

                switch (format)
                {
                    case [':', 'G', .. var precisionSpecifier] when NumberInfo.TryGet(expressionType) is { } numberInfo
                        && (precisionSpecifier == ""
                            || int.TryParse(precisionSpecifier, out var precision)
                            && (precision == 0 && (numberInfo.FormatSpecifiers & FormatSpecifiers.GeneralZeroPrecisionRedundant) != 0
                                || numberInfo.MaxValueStringLength is { } maxValueStringLength && precision >= maxValueStringLength)):
                    {
                        consumer.AddHighlighting(new RedundantFormatSpecifierHint($"Specifying 'G{precisionSpecifier}' is redundant.", insert));
                        break;
                    }

                    case [':', 'g', .. var precisionSpecifier] when NumberInfo.TryGet(expressionType) is { } numberInfo
                        && (numberInfo.FormatSpecifiers & FormatSpecifiers.GeneralCaseInsensitiveWithoutPrecision) != 0
                        && (precisionSpecifier == ""
                            || int.TryParse(precisionSpecifier, out var precision)
                            && (precision == 0 && (numberInfo.FormatSpecifiers & FormatSpecifiers.GeneralZeroPrecisionRedundant) != 0
                                || numberInfo.MaxValueStringLength is { } maxValueStringLength && precision >= maxValueStringLength)):
                    {
                        consumer.AddHighlighting(new RedundantFormatSpecifierHint($"Specifying 'g{precisionSpecifier}' is redundant.", insert));
                        break;
                    }

                    case [':', 'G' or 'g'] when expressionType.IsEnumType() || expressionType.IsNullable() && expressionType.Unlift().IsEnumType():
                    {
                        consumer.AddHighlighting(new RedundantFormatSpecifierHint($"Specifying '{format[1].ToString()}' is redundant.", insert));
                        break;
                    }

                    case [':', 'E' or 'e', .. var precisionSpecifier] when NumberInfo.TryGet(expressionType) is { }
                        && precisionSpecifier != ""
                        && int.TryParse(precisionSpecifier, out var precision)
                        && precision == 6:
                    {
                        consumer.AddHighlighting(
                            new RedundantFormatPrecisionSpecifierHint(
                                $"The format precision specifier is redundant, '{format[1].ToString()}' has the same effect.",
                                insert));
                        break;
                    }

                    case [':', 'D' or 'd', .. var precisionSpecifier] when NumberInfo.TryGet(expressionType) is { } numberInfo
                        && (numberInfo.FormatSpecifiers & FormatSpecifiers.Decimal) != 0
                        && precisionSpecifier != ""
                        && int.TryParse(precisionSpecifier, out var precision)
                        && precision is 0 or 1:
                    {
                        consumer.AddHighlighting(
                            new RedundantFormatPrecisionSpecifierHint(
                                $"The format precision specifier is redundant, '{format[1].ToString()}' has the same effect.",
                                insert));
                        break;
                    }

                    case [':', 'B' or 'b', .. var precisionSpecifier] when NumberInfo.TryGet(expressionType) is { } numberInfo
                        && (numberInfo.FormatSpecifiers & FormatSpecifiers.Binary) != 0
                        && precisionSpecifier != ""
                        && int.TryParse(precisionSpecifier, out var precision)
                        && precision is 0 or 1:
                    {
                        consumer.AddHighlighting(
                            new RedundantFormatPrecisionSpecifierHint(
                                $"The format precision specifier is redundant, '{format[1].ToString()}' has the same effect.",
                                insert));
                        break;
                    }

                    case [':', 'X' or 'x', .. var precisionSpecifier] when NumberInfo.TryGet(expressionType) is { } numberInfo
                        && (numberInfo.FormatSpecifiers & FormatSpecifiers.Hexadecimal) != 0
                        && precisionSpecifier != ""
                        && int.TryParse(precisionSpecifier, out var precision)
                        && precision is 0 or 1:
                    {
                        consumer.AddHighlighting(
                            new RedundantFormatPrecisionSpecifierHint(
                                $"The format precision specifier is redundant, '{format[1].ToString()}' has the same effect.",
                                insert));
                        break;
                    }

                    case [':', 'R' or 'r', .. var precisionSpecifier] when NumberInfo.TryGet(expressionType) is { } numberInfo:
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
                                    insert,
                                    numberInfo.RoundTripFormatSpecifierReplacement));
                        }

                        if ((numberInfo.FormatSpecifiers & FormatSpecifiers.RoundtripPrecisionRedundant) != 0 && precisionSpecifier != "")
                        {
                            consumer.AddHighlighting(
                                new RedundantFormatPrecisionSpecifierHint(
                                    $"The format precision specifier is redundant, '{format[1].ToString()}' has the same effect.",
                                    insert));
                        }

                        if ((numberInfo.FormatSpecifiers & (FormatSpecifiers.RoundtripToBeReplaced | FormatSpecifiers.RoundtripPrecisionRedundant))
                            == 0)
                        {
                            consumer.AddHighlighting(new SuspiciousFormatSpecifierWarning("The format specifier might be unsupported.", insert));
                        }

                        break;
                    }
                }
            }
        }
    }
}