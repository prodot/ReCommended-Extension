using System.Runtime.CompilerServices;
using System.Text;
using JetBrains.ReSharper.Feature.Services.Daemon;
using JetBrains.ReSharper.Feature.Services.Util.ClrLanguages;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.CSharp.Conversions;
using JetBrains.ReSharper.Psi.CSharp.Tree;
using JetBrains.ReSharper.Psi.Util;
using JetBrains.Util;
using ReCommendedExtension.Extensions;
using ReCommendedExtension.Extensions.MemberFinding;
using ReCommendedExtension.Extensions.NumberInfos;

namespace ReCommendedExtension.Analyzers.Formatter;

[ElementProblemAnalyzer(
    typeof(ICSharpTreeNode),
    HighlightingTypes =
    [
        typeof(RedundantFormatSpecifierHint),
        typeof(RedundantFormatProviderHint),
        typeof(RedundantFormatPrecisionSpecifierHint),
        typeof(PassOtherFormatSpecifierSuggestion),
        typeof(SuspiciousFormatSpecifierWarning),
        typeof(ReplaceTypeCastWithFormatSpecifierSuggestion),
    ])]
public sealed class FormatterAnalyzer(FormattingFunctionInvocationInfoProvider formattingFunctionInvocationInfoProvider)
    : ElementProblemAnalyzer<ICSharpTreeNode>
{
    [SuppressMessage("ReSharper", "InconsistentNaming", Justification = "Underscore character used intentionally as a separator.")]
    static class Parameters
    {
        public static IReadOnlyList<Parameter> String { get; } = [Parameter.String];

        public static IReadOnlyList<Parameter> IFormatProvider { get; } = [Parameter.IFormatProvider];
    }

    record struct ToStringInvocation
    {
        public required IType QualifierType { get; init; }

        public ToStringInvocationArgument? FormatArgument { get; init; }

        public ToStringInvocationArgument? ProviderArgument { get; init; }
    }

    record struct ToStringInvocationArgument
    {
        public required ICSharpArgument Argument { get; init; }

        public required bool CanBeRemoved { get; init; }
    }

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

    [Pure]
    static ToStringInvocation? TryGetToStringArguments(IInvocationExpression invocationExpression)
    {
        if (invocationExpression is
            {
                InvokedExpression: IReferenceExpression { QualifierExpression: { } qualifierExpression, Reference: var reference },
            }
            && reference.Resolve().DeclaredElement is IMethod
            {
                AccessibilityDomain.DomainType: AccessibilityDomain.AccessibilityDomainType.PUBLIC,
                TypeParameters: [],
                IsStatic: false,
                ShortName: nameof(ToString),
                ContainingType: { } containingType,
            } method)
        {
            var invocation = new ToStringInvocation { QualifierType = qualifierExpression.Type() };

            if (!invocation.QualifierType.IsNullable())
            {
                switch (method.Parameters, invocationExpression.TryGetArgumentsInDeclarationOrder())
                {
                    case ([{ Type: var formatType }], [{ } formatArgument]) when formatType.IsString():
                        return invocation with
                        {
                            FormatArgument = new ToStringInvocationArgument
                            {
                                Argument = formatArgument,
                                CanBeRemoved = containingType.HasMethod(new MethodSignature { Name = nameof(ToString), Parameters = [] }),
                            },
                        };

                    case ([{ Type: var providerType }], [{ } providerArgument]) when providerType.IsIFormatProvider():
                    {
                        return invocation with
                        {
                            ProviderArgument = new ToStringInvocationArgument
                            {
                                Argument = providerArgument,
                                CanBeRemoved = containingType.HasMethod(new MethodSignature { Name = nameof(ToString), Parameters = [] }),
                            },
                        };
                    }

                    case ([{ Type: var formatType }, { Type: var providerType }], [{ } formatArgument, { } providerArgument])
                        when formatType.IsString() && providerType.IsIFormatProvider():
                    {
                        return invocation with
                        {
                            FormatArgument = new ToStringInvocationArgument
                            {
                                Argument = formatArgument,
                                CanBeRemoved = containingType.HasMethod(
                                    new MethodSignature { Name = nameof(ToString), Parameters = Parameters.IFormatProvider }),
                            },
                            ProviderArgument = new ToStringInvocationArgument
                            {
                                Argument = providerArgument,
                                CanBeRemoved = containingType.HasMethod(
                                    new MethodSignature { Name = nameof(ToString), Parameters = Parameters.String }),
                            },
                        };
                    }
                }
            }
        }

        return null;
    }

    [Pure]
    IEnumerable<FormatElement> GetFormatElements(ICSharpTreeNode treeNode)
    {
        switch (treeNode)
        {
            case IInterpolatedStringExpression interpolatedStringExpression when IsWellKnownImplementation(interpolatedStringExpression):
            {
                foreach (var insert in interpolatedStringExpression.Inserts)
                {
                    var expressionType = insert.Expression.Type();

                    if (insert.FormatSpecifier is { })
                    {
                        if (insert.FormatSpecifier.GetText() is [':', .. not "" and var format])
                        {
                            yield return new FormatElement(format, expressionType.IsNullable() ? expressionType.Unlift() : expressionType, insert);
                        }
                    }
                    else
                    {
                        yield return new FormatElement(null, expressionType.IsNullable() ? expressionType.Unlift() : expressionType, insert);
                    }
                }
                break;
            }

            case ICSharpArgumentsOwner argumentsOwner when formattingFunctionInvocationInfoProvider.TryGetByArgumentsOwner(argumentsOwner) is
                {
                    FormatString.Expression: ICSharpLiteralExpression formatStringExpression,
                } formattingFunctionInvocationInfo
                && IsWellKnownImplementation(formattingFunctionInvocationInfo):
            {
                var formatString = formatStringExpression.GetText();

                foreach (var formatItem in FormatStringParser.Parse(formatString))
                {
                    if (formatItem is { FormatStringRange: { IsValid: true, IsEmpty: false }, IndexValue: { } index and >= 0 }
                        && index < formattingFunctionInvocationInfo.FormattingExpressions.Count
                        && formattingFunctionInvocationInfo.FormattingExpressions[index] is { } formattingExpression)
                    {
                        var expressionType = formattingExpression.Type();

                        if (formatString.Substring(formatItem.FormatStringRange) is [_, ..] format)
                        {
                            yield return new FormatElement(
                                format,
                                expressionType.IsNullable() ? expressionType.Unlift() : expressionType,
                                formatStringExpression,
                                formatItem);
                        }
                    }
                }
                break;
            }

            case IInvocationExpression invocationExpression when TryGetToStringArguments(invocationExpression) is
            {
                QualifierType: var expressionType, FormatArgument: { Argument: var argument, CanBeRemoved: var canBeRemoved },
            }:
            {
                switch (argument.Value)
                {
                    case { IsDefaultValueOrNull: true }: yield return new FormatElement(null, expressionType, argument, canBeRemoved); break;
                    case { AsStringConstant: { } format }: yield return new FormatElement(format, expressionType, argument, canBeRemoved); break;
                }

                break;
            }
        }
    }

    protected override void Run(ICSharpTreeNode element, ElementProblemAnalyzerData data, IHighlightingConsumer consumer)
    {
        foreach (var formatElement in GetFormatElements(element))
        {
            switch (formatElement.Format)
            {
                case null or "" when (NumberInfo.TryGet(formatElement.ExpressionType) is { }
                        || formatElement.ExpressionType.IsEnumType()
                        || formatElement.ExpressionType.IsGuid()
                        || formatElement.ExpressionType.IsDateOnly()
                        || formatElement.ExpressionType.IsTimeOnly()
                        || formatElement.ExpressionType.IsTimeSpan()
                        || formatElement.ExpressionType.IsDateTime()
                        || formatElement.ExpressionType.IsDateTimeOffset())
                    && formatElement is { Argument: { }, CanBeRemoved: true }:
                {
                    consumer.AddHighlighting(
                        new RedundantFormatSpecifierHint("Specifying null or an empty string is redundant.") { FormatElement = formatElement });
                    break;
                }

                case ['G', .. var precisionSpecifier] when NumberInfo.TryGet(formatElement.ExpressionType) is { } numberInfo
                    && (precisionSpecifier == ""
                        || int.TryParse(precisionSpecifier, out var precision)
                        && (precision == 0 && (numberInfo.FormatSpecifiers & FormatSpecifiers.GeneralZeroPrecisionRedundant) != 0
                            || numberInfo.MaxValueStringLength is { } maxValueStringLength && precision >= maxValueStringLength))
                    && formatElement.CanBeRemoved:
                {
                    consumer.AddHighlighting(
                        new RedundantFormatSpecifierHint($"Specifying 'G{precisionSpecifier}' is redundant.") { FormatElement = formatElement });
                    break;
                }

                case ['g', .. var precisionSpecifier] when NumberInfo.TryGet(formatElement.ExpressionType) is { } numberInfo
                    && (numberInfo.FormatSpecifiers & FormatSpecifiers.GeneralCaseInsensitiveWithoutPrecision) != 0
                    && (precisionSpecifier == ""
                        || int.TryParse(precisionSpecifier, out var precision)
                        && (precision == 0 && (numberInfo.FormatSpecifiers & FormatSpecifiers.GeneralZeroPrecisionRedundant) != 0
                            || numberInfo.MaxValueStringLength is { } maxValueStringLength && precision >= maxValueStringLength))
                    && formatElement.CanBeRemoved:
                {
                    consumer.AddHighlighting(
                        new RedundantFormatSpecifierHint($"Specifying 'g{precisionSpecifier}' is redundant.") { FormatElement = formatElement });
                    break;
                }

                case [('G' or 'g') and var replacement] when formatElement.ExpressionType.IsEnumType() && formatElement.CanBeRemoved:
                {
                    consumer.AddHighlighting(
                        new RedundantFormatSpecifierHint($"Specifying '{replacement.ToString()}' is redundant.") { FormatElement = formatElement });
                    break;
                }

                case [('D' or 'd') and var replacement] when formatElement.ExpressionType.IsGuid() && formatElement.CanBeRemoved:
                {
                    consumer.AddHighlighting(
                        new RedundantFormatSpecifierHint($"Specifying '{replacement.ToString()}' is redundant.") { FormatElement = formatElement });
                    break;
                }

                case ['d' and var replacement] when formatElement.ExpressionType.IsDateOnly() && formatElement.CanBeRemoved:
                {
                    consumer.AddHighlighting(
                        new RedundantFormatSpecifierHint($"Specifying '{replacement.ToString()}' is redundant.") { FormatElement = formatElement });
                    break;
                }

                case ['t' and var replacement] when formatElement.ExpressionType.IsTimeOnly() && formatElement.CanBeRemoved:
                {
                    consumer.AddHighlighting(
                        new RedundantFormatSpecifierHint($"Specifying '{replacement.ToString()}' is redundant.") { FormatElement = formatElement });
                    break;
                }

                case [('c' or 't' or 'T') and var replacement] when formatElement.ExpressionType.IsTimeSpan() && formatElement.CanBeRemoved:
                {
                    consumer.AddHighlighting(
                        new RedundantFormatSpecifierHint($"Specifying '{replacement.ToString()}' is redundant.") { FormatElement = formatElement });
                    break;
                }

                case [('E' or 'e') and var replacement, .. var precisionSpecifier] when NumberInfo.TryGet(formatElement.ExpressionType) is { }
                    && precisionSpecifier != ""
                    && int.TryParse(precisionSpecifier, out var precision)
                    && precision == 6:
                {
                    consumer.AddHighlighting(
                        new RedundantFormatPrecisionSpecifierHint(
                            $"The format precision specifier is redundant, '{replacement.ToString()}' has the same effect.")
                        {
                            FormatElement = formatElement,
                        });
                    break;
                }

                case [('D' or 'd') and var replacement, .. var precisionSpecifier]
                    when NumberInfo.TryGet(formatElement.ExpressionType) is { } numberInfo
                    && (numberInfo.FormatSpecifiers & FormatSpecifiers.Decimal) != 0
                    && precisionSpecifier != ""
                    && int.TryParse(precisionSpecifier, out var precision)
                    && precision is 0 or 1:
                {
                    consumer.AddHighlighting(
                        new RedundantFormatPrecisionSpecifierHint(
                            $"The format precision specifier is redundant, '{replacement.ToString()}' has the same effect.")
                        {
                            FormatElement = formatElement,
                        });
                    break;
                }

                case [('B' or 'b') and var replacement, .. var precisionSpecifier]
                    when NumberInfo.TryGet(formatElement.ExpressionType) is { } numberInfo
                    && (numberInfo.FormatSpecifiers & FormatSpecifiers.Binary) != 0
                    && precisionSpecifier != ""
                    && int.TryParse(precisionSpecifier, out var precision)
                    && precision is 0 or 1:
                {
                    consumer.AddHighlighting(
                        new RedundantFormatPrecisionSpecifierHint(
                            $"The format precision specifier is redundant, '{replacement.ToString()}' has the same effect.")
                        {
                            FormatElement = formatElement,
                        });
                    break;
                }

                case [('X' or 'x') and var replacement, .. var precisionSpecifier]
                    when NumberInfo.TryGet(formatElement.ExpressionType) is { } numberInfo
                    && (numberInfo.FormatSpecifiers & FormatSpecifiers.Hexadecimal) != 0
                    && precisionSpecifier != ""
                    && int.TryParse(precisionSpecifier, out var precision)
                    && precision is 0 or 1:
                {
                    consumer.AddHighlighting(
                        new RedundantFormatPrecisionSpecifierHint(
                            $"The format precision specifier is redundant, '{replacement.ToString()}' has the same effect.")
                        {
                            FormatElement = formatElement,
                        });
                    break;
                }

                case [('R' or 'r') and var replacement, .. var precisionSpecifier]
                    when NumberInfo.TryGet(formatElement.ExpressionType) is { } numberInfo:
                {
                    Debug.Assert(
                        (numberInfo.FormatSpecifiers & (FormatSpecifiers.RoundtripToBeReplaced | FormatSpecifiers.RoundtripPrecisionRedundant))
                        != (FormatSpecifiers.RoundtripToBeReplaced | FormatSpecifiers.RoundtripPrecisionRedundant));

                    if ((numberInfo.FormatSpecifiers & FormatSpecifiers.RoundtripToBeReplaced) != 0)
                    {
                        Debug.Assert(numberInfo.RoundTripFormatSpecifierReplacement is { });

                        consumer.AddHighlighting(
                            new PassOtherFormatSpecifierSuggestion(
                                $"Pass the '{numberInfo.RoundTripFormatSpecifierReplacement}' format specifier (string length may vary).")
                            {
                                FormatElement = formatElement, Replacement = numberInfo.RoundTripFormatSpecifierReplacement,
                            });
                    }

                    if ((numberInfo.FormatSpecifiers & FormatSpecifiers.RoundtripPrecisionRedundant) != 0 && precisionSpecifier != "")
                    {
                        consumer.AddHighlighting(
                            new RedundantFormatPrecisionSpecifierHint(
                                $"The format precision specifier is redundant, '{replacement.ToString()}' has the same effect.")
                            {
                                FormatElement = formatElement,
                            });
                    }

                    if ((numberInfo.FormatSpecifiers & (FormatSpecifiers.RoundtripToBeReplaced | FormatSpecifiers.RoundtripPrecisionRedundant)) == 0)
                    {
                        consumer.AddHighlighting(new SuspiciousFormatSpecifierWarning("The format specifier might be unsupported.", formatElement));
                    }

                    break;
                }

                case null when formatElement is { Insert: { Expression: ICastExpression castExpression } insert }:
                {
                    var type = castExpression.Op.Type();

                    const string formatSpecifier = "D";

                    if (type.IsEnumType(out var enumBaseType)
                        && (castExpression.TargetType is IPredefinedTypeUsage { ScalarPredefinedTypeName.PredefinedType: var castType }
                            && enumBaseType.IsImplicitlyConvertibleTo(castType, insert.GetPsiModule().GetTypeConversionRule())
                            || castExpression.TargetType is INullableTypeUsage
                            {
                                UnderlyingType: IPredefinedTypeUsage { ScalarPredefinedTypeName.PredefinedType: var castType2 },
                            }
                            && enumBaseType.IsImplicitlyConvertibleTo(castType2, insert.GetPsiModule().GetTypeConversionRule())))
                    {
                        consumer.AddHighlighting(
                            new ReplaceTypeCastWithFormatSpecifierSuggestion(
                                $"Use the '{formatSpecifier}' format specifier instead of the type cast.",
                                insert) { Expression = castExpression.Op, FormatSpecifier = formatSpecifier });
                    }

                    if (type.IsNullable()
                        && type.Unlift().IsEnumType(out enumBaseType)
                        && castExpression.TargetType is INullableTypeUsage
                        {
                            UnderlyingType: IPredefinedTypeUsage { ScalarPredefinedTypeName.PredefinedType: var castType3 },
                        }
                        && enumBaseType.IsImplicitlyConvertibleTo(castType3, insert.GetPsiModule().GetTypeConversionRule()))
                    {
                        consumer.AddHighlighting(
                            new ReplaceTypeCastWithFormatSpecifierSuggestion(
                                $"Use the '{formatSpecifier}' format specifier instead of the type cast.",
                                insert) { Expression = castExpression.Op, FormatSpecifier = formatSpecifier });
                    }
                    break;
                }
            }
        }

        if (element is IInvocationExpression invocationExpression && TryGetToStringArguments(invocationExpression) is { } toStringInvocation)
        {
            switch (toStringInvocation.QualifierType)
            {
                case var t
                    when (NumberInfo.TryGet(t) is { } || t.IsDateOnly() || t.IsTimeOnly() || t.IsTimeSpan() || t.IsDateTime() || t.IsDateTimeOffset())
                    && toStringInvocation.ProviderArgument is { CanBeRemoved: true, Argument: { Value.IsDefaultValueOrNull: true } argument }:
                {
                    consumer.AddHighlighting(new RedundantFormatProviderHint("Passing null is redundant.") { ProviderArgument = argument });
                    break;
                }

                case var t when (t.IsBool() || t.IsGuid() || t.IsChar() || t.IsString())
                    && toStringInvocation.ProviderArgument is { CanBeRemoved: true, Argument: var argument }:
                {
                    consumer.AddHighlighting(
                        new RedundantFormatProviderHint("Passing a format provider is redundant.") { ProviderArgument = argument });
                    break;
                }

                case var t when NumberInfo.TryGet(t) is { } numberInfo
                    && (numberInfo.FormatSpecifiers & FormatSpecifiers.Binary) != 0
                    && toStringInvocation is
                    {
                        FormatArgument.Argument.Value.AsStringConstant: ['B' or 'b', ..],
                        ProviderArgument: { CanBeRemoved: true, Argument: var providerArgument },
                    }:
                {
                    consumer.AddHighlighting(
                        new RedundantFormatProviderHint("Passing a provider with a binary format specifier is redundant.")
                        {
                            ProviderArgument = providerArgument,
                        });
                    break;
                }

                case var t when NumberInfo.TryGet(t) is { } numberInfo
                    && (numberInfo.FormatSpecifiers & FormatSpecifiers.Hexadecimal) != 0
                    && toStringInvocation is
                    {
                        FormatArgument.Argument.Value.AsStringConstant: ['X' or 'x', ..],
                        ProviderArgument: { CanBeRemoved: true, Argument: var providerArgument },
                    }:
                {
                    consumer.AddHighlighting(
                        new RedundantFormatProviderHint("Passing a provider with a hexadecimal format specifier is redundant.")
                        {
                            ProviderArgument = providerArgument,
                        });
                    break;
                }

                case var t when (t.IsDateOnly() || t.IsTimeOnly())
                    && toStringInvocation is
                    {
                        FormatArgument.Argument.Value.AsStringConstant: "o" or "O" or "r" or "R",
                        ProviderArgument: { CanBeRemoved: true, Argument: var providerArgument },
                    }:
                {
                    consumer.AddHighlighting(
                        new RedundantFormatProviderHint("Passing a format provider is redundant.") { ProviderArgument = providerArgument });
                    break;
                }

                case var t when t.IsTimeSpan()
                    && toStringInvocation is
                    {
                        FormatArgument.Argument.Value: { IsDefaultValueOrNull: true } or { AsStringConstant : "" or "c" or "t" or "T" },
                        ProviderArgument: { CanBeRemoved: true, Argument: var providerArgument },
                    }:
                {
                    consumer.AddHighlighting(
                        new RedundantFormatProviderHint("Passing a format provider is redundant.") { ProviderArgument = providerArgument });
                    break;
                }

                case var t when (t.IsDateTime() || t.IsDateTimeOffset())
                    && toStringInvocation is
                    {
                        FormatArgument.Argument.Value.AsStringConstant: "o" or "O" or "r" or "R" or "s" or "u",
                        ProviderArgument: { CanBeRemoved: true, Argument: var providerArgument },
                    }:
                {
                    consumer.AddHighlighting(
                        new RedundantFormatProviderHint("Passing a format provider is redundant.") { ProviderArgument = providerArgument });
                    break;
                }
            }
        }
    }
}