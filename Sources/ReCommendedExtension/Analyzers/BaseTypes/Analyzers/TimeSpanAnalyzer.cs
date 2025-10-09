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
        typeof(UseBinaryOperatorSuggestion),
        typeof(UseUnaryOperatorSuggestion),
        typeof(UseOtherArgumentSuggestion),
    ])]
public sealed class TimeSpanAnalyzer : ElementProblemAnalyzer<ICSharpInvocationInfo>
{
    [SuppressMessage("ReSharper", "InconsistentNaming", Justification = "Underscore character used intentionally as a separator.")]
    static class Parameters
    {
        public static IReadOnlyList<Parameter> String_String_IFormatProvider { get; } =
        [
            Parameter.String, Parameter.String, Parameter.IFormatProvider,
        ];

        public static IReadOnlyList<Parameter> String_String_IFormatProvider_outTimeSpan { get; } =
        [
            Parameter.String, Parameter.String, Parameter.IFormatProvider, Parameter.TimeSpan with { Kind = ParameterKind.OUTPUT },
        ];

        public static IReadOnlyList<Parameter> String_String_IFormatProvider_TimeSpanStyles { get; } =
        [
            Parameter.String, Parameter.String, Parameter.IFormatProvider, Parameter.TimeSpanStyles,
        ];

        public static IReadOnlyList<Parameter> String_String_IFormatProvider_TimeSpanStyles_outTimeSpan { get; } =
        [
            Parameter.String,
            Parameter.String,
            Parameter.IFormatProvider,
            Parameter.TimeSpanStyles,
            Parameter.TimeSpan with { Kind = ParameterKind.OUTPUT },
        ];
    }

    /// <remarks>
    /// <c>new TimeSpan(0)</c> → <c>TimeSpan.Zero</c><para/>
    /// <c>new TimeSpan(long.MinValue)</c> → <c>TimeSpan.MinValue</c><para/>
    /// <c>new TimeSpan(long.MaxValue)</c> → <c>TimeSpan.MaxValue</c>
    /// </remarks>
    static void Analyze_Ctor_Int64(IHighlightingConsumer consumer, IObjectCreationExpression objectCreationExpression, ICSharpArgument ticksArgument)
    {
        if (!objectCreationExpression.IsUsedAsStatement())
        {
            switch (ticksArgument.Value.TryGetInt64Constant())
            {
                case 0:
                    consumer.AddHighlighting(
                        new UseExpressionResultSuggestion(
                            $"The expression is always {nameof(TimeSpan)}.{nameof(TimeSpan.Zero)}.",
                            objectCreationExpression,
                            $"{nameof(TimeSpan)}.{nameof(TimeSpan.Zero)}"));
                    break;

                case long.MinValue:
                    consumer.AddHighlighting(
                        new UseExpressionResultSuggestion(
                            $"The expression is always {nameof(TimeSpan)}.{nameof(TimeSpan.MinValue)}.",
                            objectCreationExpression,
                            $"{nameof(TimeSpan)}.{nameof(TimeSpan.MinValue)}"));
                    break;

                case long.MaxValue:
                    consumer.AddHighlighting(
                        new UseExpressionResultSuggestion(
                            $"The expression is always {nameof(TimeSpan)}.{nameof(TimeSpan.MaxValue)}.",
                            objectCreationExpression,
                            $"{nameof(TimeSpan)}.{nameof(TimeSpan.MaxValue)}"));
                    break;
            }
        }
    }

    /// <remarks>
    /// <c>new TimeSpan(0, 0, 0)</c> → <c>TimeSpan.Zero</c>
    /// </remarks>
    static void Analyze_Ctor_Int32_Int32_Int32(
        IHighlightingConsumer consumer,
        IObjectCreationExpression objectCreationExpression,
        ICSharpArgument hoursArgument,
        ICSharpArgument minutesArgument,
        ICSharpArgument secondsArgument)
    {
        if (!objectCreationExpression.IsUsedAsStatement()
            && (hoursArgument.Value.TryGetInt32Constant(), minutesArgument.Value.TryGetInt32Constant(), secondsArgument.Value.TryGetInt32Constant())
            == (0, 0, 0))
        {
            consumer.AddHighlighting(
                new UseExpressionResultSuggestion(
                    $"The expression is always {nameof(TimeSpan)}.{nameof(TimeSpan.Zero)}.",
                    objectCreationExpression,
                    $"{nameof(TimeSpan)}.{nameof(TimeSpan.Zero)}"));
        }
    }

    /// <remarks>
    /// <c>new TimeSpan(0, 0, 0, 0)</c> → <c>TimeSpan.Zero</c><para/>
    /// </remarks>
    static void Analyze_Ctor_Int32_Int32_Int32_Int32(
        IHighlightingConsumer consumer,
        IObjectCreationExpression objectCreationExpression,
        ICSharpArgument daysArgument,
        ICSharpArgument hoursArgument,
        ICSharpArgument minutesArgument,
        ICSharpArgument secondsArgument)
    {
        if (daysArgument.Value.TryGetInt32Constant() == 0
            && (hoursArgument.Value.TryGetInt32Constant(), minutesArgument.Value.TryGetInt32Constant(), secondsArgument.Value.TryGetInt32Constant())
            == (0, 0, 0)
            && !objectCreationExpression.IsUsedAsStatement())
        {
            consumer.AddHighlighting(
                new UseExpressionResultSuggestion(
                    $"The expression is always {nameof(TimeSpan)}.{nameof(TimeSpan.Zero)}.",
                    objectCreationExpression,
                    $"{nameof(TimeSpan)}.{nameof(TimeSpan.Zero)}"));
        }
    }

    /// <remarks>
    /// <c>new TimeSpan(0, 0, 0, 0, 0)</c> → <c>TimeSpan.Zero</c>
    /// </remarks>
    static void Analyze_Ctor_Int32_Int32_Int32_Int32_Int32(
        IHighlightingConsumer consumer,
        IObjectCreationExpression objectCreationExpression,
        ICSharpArgument daysArgument,
        ICSharpArgument hoursArgument,
        ICSharpArgument minutesArgument,
        ICSharpArgument secondsArgument,
        ICSharpArgument millisecondsArgument)
    {
        if (millisecondsArgument.Value.TryGetInt32Constant() == 0
            && (daysArgument.Value.TryGetInt32Constant(), hoursArgument.Value.TryGetInt32Constant(), minutesArgument.Value.TryGetInt32Constant(),
                secondsArgument.Value.TryGetInt32Constant())
            == (0, 0, 0, 0)
            && !objectCreationExpression.IsUsedAsStatement())
        {
            consumer.AddHighlighting(
                new UseExpressionResultSuggestion(
                    $"The expression is always {nameof(TimeSpan)}.{nameof(TimeSpan.Zero)}.",
                    objectCreationExpression,
                    $"{nameof(TimeSpan)}.{nameof(TimeSpan.Zero)}"));
        }
    }

    /// <remarks>
    /// <c>new TimeSpan(0, 0, 0, 0, 0, 0)</c> → <c>TimeSpan.Zero</c>
    /// </remarks>
    static void Analyze_Ctor_Int32_Int32_Int32_Int32_Int32_Int32(
        IHighlightingConsumer consumer,
        IObjectCreationExpression objectCreationExpression,
        ICSharpArgument daysArgument,
        ICSharpArgument hoursArgument,
        ICSharpArgument minutesArgument,
        ICSharpArgument secondsArgument,
        ICSharpArgument millisecondsArgument,
        ICSharpArgument microsecondsArgument)
    {
        if (microsecondsArgument.Value.TryGetInt32Constant() == 0
            && (daysArgument.Value.TryGetInt32Constant(), hoursArgument.Value.TryGetInt32Constant(), minutesArgument.Value.TryGetInt32Constant(),
                secondsArgument.Value.TryGetInt32Constant(), millisecondsArgument.Value.TryGetInt32Constant())
            == (0, 0, 0, 0, 0)
            && !objectCreationExpression.IsUsedAsStatement())
        {
            consumer.AddHighlighting(
                new UseExpressionResultSuggestion(
                    $"The expression is always {nameof(TimeSpan)}.{nameof(TimeSpan.Zero)}.",
                    objectCreationExpression,
                    $"{nameof(TimeSpan)}.{nameof(TimeSpan.Zero)}"));
        }
    }

    /// <remarks>
    /// <c>timeSpan.Add(ts)</c> → <c>timeSpan + ts</c>
    /// </remarks>
    static void AnalyzeAdd_TimeSpan(
        IHighlightingConsumer consumer,
        IInvocationExpression invocationExpression,
        IReferenceExpression invokedExpression,
        ICSharpArgument tsArgument)
    {
        Debug.Assert(invokedExpression.QualifierExpression is { });

        if (!invocationExpression.IsUsedAsStatement() && tsArgument.Value is { })
        {
            consumer.AddHighlighting(
                new UseBinaryOperatorSuggestion(
                    "Use the '+' operator.",
                    invocationExpression,
                    "+",
                    invokedExpression.QualifierExpression.GetText(),
                    tsArgument.Value.GetText()));
        }
    }

    /// <remarks>
    /// <c>timeSpan.Divide(divisor)</c> → <c>timeSpan / divisor</c> (.NET Core 2.0)
    /// </remarks>
    static void AnalyzeDivide_Double(
        IHighlightingConsumer consumer,
        IInvocationExpression invocationExpression,
        IReferenceExpression invokedExpression,
        ICSharpArgument divisorArgument)
    {
        Debug.Assert(invokedExpression.QualifierExpression is { });

        if (!invocationExpression.IsUsedAsStatement() && divisorArgument.Value is { })
        {
            consumer.AddHighlighting(
                new UseBinaryOperatorSuggestion(
                    "Use the '/' operator.",
                    invocationExpression,
                    "/",
                    invokedExpression.QualifierExpression.GetText(),
                    divisorArgument.Value.GetText()));
        }
    }

    /// <remarks>
    /// <c>timeSpan.Divide(ts)</c> → <c>timeSpan / ts</c> (.NET Core 2.0)
    /// </remarks>
    static void AnalyzeDivide_TimeSpan(
        IHighlightingConsumer consumer,
        IInvocationExpression invocationExpression,
        IReferenceExpression invokedExpression,
        ICSharpArgument tsArgument)
    {
        Debug.Assert(invokedExpression.QualifierExpression is { });

        if (!invocationExpression.IsUsedAsStatement() && tsArgument.Value is { })
        {
            consumer.AddHighlighting(
                new UseBinaryOperatorSuggestion(
                    "Use the '/' operator.",
                    invocationExpression,
                    "/",
                    invokedExpression.QualifierExpression.GetText(),
                    tsArgument.Value.GetText()));
        }
    }

    /// <remarks>
    /// <c>timeSpan.Equals(obj)</c> → <c>timeSpan == obj</c>
    /// </remarks>
    static void AnalyzeEquals_TimeSpan(
        IHighlightingConsumer consumer,
        IInvocationExpression invocationExpression,
        IReferenceExpression invokedExpression,
        ICSharpArgument objArgument)
    {
        Debug.Assert(invokedExpression.QualifierExpression is { });

        if (!invocationExpression.IsUsedAsStatement() && objArgument.Value is { })
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
    /// <c>timeSpan.Equals(null)</c> → <c>false</c>
    /// </remarks>
    static void AnalyzeEquals_Object(IHighlightingConsumer consumer, IInvocationExpression invocationExpression, ICSharpArgument objArgument)
    {
        if (!invocationExpression.IsUsedAsStatement() && objArgument.Value.IsDefaultValue())
        {
            consumer.AddHighlighting(new UseExpressionResultSuggestion("The expression is always false.", invocationExpression, "false"));
        }
    }

    /// <remarks>
    /// <c>TimeSpan.Equals(t1, t2)</c> → <c>t1 == t2</c>
    /// </remarks>
    static void AnalyzeEquals_TimeSpan_TimeSpan(
        IHighlightingConsumer consumer,
        IInvocationExpression invocationExpression,
        ICSharpArgument t1Argument,
        ICSharpArgument t2Argument)
    {
        if (!invocationExpression.IsUsedAsStatement() && t1Argument.Value is { } && t2Argument.Value is { })
        {
            consumer.AddHighlighting(
                new UseBinaryOperatorSuggestion(
                    "Use the '==' operator.",
                    invocationExpression,
                    "==",
                    t1Argument.Value.GetText(),
                    t2Argument.Value.GetText()));
        }
    }

    /// <remarks>
    /// <c>TimeSpan.FromDays(0)</c> → <c>TimeSpan.Zero</c>
    /// </remarks>
    static void AnalyzeFromDays_Int32(IHighlightingConsumer consumer, IInvocationExpression invocationExpression, ICSharpArgument daysArgument)
    {
        if (!invocationExpression.IsUsedAsStatement() && daysArgument.Value.TryGetInt32Constant() == 0)
        {
            consumer.AddHighlighting(
                new UseExpressionResultSuggestion(
                    $"The expression is always {nameof(TimeSpan)}.{nameof(TimeSpan.Zero)}.",
                    invocationExpression,
                    $"{nameof(TimeSpan)}.{nameof(TimeSpan.Zero)}"));
        }
    }

    /// <remarks>
    /// <c>TimeSpan.FromDays(0)</c> → <c>TimeSpan.Zero</c>
    /// </remarks>
    static void AnalyzeFromDays_Int32_Int32_Int64_Int64_Int64_Int64(
        IHighlightingConsumer consumer,
        IInvocationExpression invocationExpression,
        ICSharpArgument daysArgument,
        ICSharpArgument? hoursArgument,
        ICSharpArgument? minutesArgument,
        ICSharpArgument? secondsArgument,
        ICSharpArgument? millisecondsArgument,
        ICSharpArgument? microsecondsArgument)
    {
        if (!invocationExpression.IsUsedAsStatement()
            && daysArgument.Value.TryGetInt32Constant() == 0
            && (hoursArgument == null || hoursArgument.Value.TryGetInt32Constant() == 0)
            && (minutesArgument == null || minutesArgument.Value.TryGetInt64Constant() == 0)
            && (secondsArgument == null || secondsArgument.Value.TryGetInt64Constant() == 0)
            && (millisecondsArgument == null || millisecondsArgument.Value.TryGetInt64Constant() == 0)
            && (microsecondsArgument == null || microsecondsArgument.Value.TryGetInt64Constant() == 0))
        {
            consumer.AddHighlighting(
                new UseExpressionResultSuggestion(
                    $"The expression is always {nameof(TimeSpan)}.{nameof(TimeSpan.Zero)}.",
                    invocationExpression,
                    $"{nameof(TimeSpan)}.{nameof(TimeSpan.Zero)}"));
        }
    }

    /// <remarks>
    /// <c>TimeSpan.FromFromHours(0)</c> → <c>TimeSpan.Zero</c>
    /// </remarks>
    static void AnalyzeFromHours_Int32(IHighlightingConsumer consumer, IInvocationExpression invocationExpression, ICSharpArgument hoursArgument)
    {
        if (!invocationExpression.IsUsedAsStatement() && hoursArgument.Value.TryGetInt32Constant() == 0)
        {
            consumer.AddHighlighting(
                new UseExpressionResultSuggestion(
                    $"The expression is always {nameof(TimeSpan)}.{nameof(TimeSpan.Zero)}.",
                    invocationExpression,
                    $"{nameof(TimeSpan)}.{nameof(TimeSpan.Zero)}"));
        }
    }

    /// <remarks>
    /// <c>TimeSpan.FromFromHours(0)</c> → <c>TimeSpan.Zero</c>
    /// </remarks>
    static void AnalyzeFromHours_Int32_Int64_Int64_Int64_Int64(
        IHighlightingConsumer consumer,
        IInvocationExpression invocationExpression,
        ICSharpArgument hoursArgument,
        ICSharpArgument? minutesArgument,
        ICSharpArgument? secondsArgument,
        ICSharpArgument? millisecondsArgument,
        ICSharpArgument? microsecondsArgument)
    {
        if (!invocationExpression.IsUsedAsStatement()
            && hoursArgument.Value.TryGetInt32Constant() == 0
            && (minutesArgument == null || minutesArgument.Value.TryGetInt64Constant() == 0)
            && (secondsArgument == null || secondsArgument.Value.TryGetInt64Constant() == 0)
            && (millisecondsArgument == null || millisecondsArgument.Value.TryGetInt64Constant() == 0)
            && (microsecondsArgument == null || microsecondsArgument.Value.TryGetInt64Constant() == 0))
        {
            consumer.AddHighlighting(
                new UseExpressionResultSuggestion(
                    $"The expression is always {nameof(TimeSpan)}.{nameof(TimeSpan.Zero)}.",
                    invocationExpression,
                    $"{nameof(TimeSpan)}.{nameof(TimeSpan.Zero)}"));
        }
    }

    /// <remarks>
    /// <c>TimeSpan.FromMicroseconds(0)</c> → <c>TimeSpan.Zero</c>
    /// </remarks>
    static void AnalyzeFromMicroseconds_Int64(
        IHighlightingConsumer consumer,
        IInvocationExpression invocationExpression,
        ICSharpArgument microsecondsArgument)
    {
        if (!invocationExpression.IsUsedAsStatement() && microsecondsArgument.Value.TryGetInt64Constant() == 0)
        {
            consumer.AddHighlighting(
                new UseExpressionResultSuggestion(
                    $"The expression is always {nameof(TimeSpan)}.{nameof(TimeSpan.Zero)}.",
                    invocationExpression,
                    $"{nameof(TimeSpan)}.{nameof(TimeSpan.Zero)}"));
        }
    }

    /// <remarks>
    /// <c>TimeSpan.FromMilliseconds(0, 0)</c> → <c>TimeSpan.Zero</c>
    /// </remarks>
    static void AnalyzeFromMilliseconds_Int64_Int64(
        IHighlightingConsumer consumer,
        IInvocationExpression invocationExpression,
        ICSharpArgument millisecondsArgument,
        ICSharpArgument? microsecondsArgument)
    {
        if (!invocationExpression.IsUsedAsStatement()
            && millisecondsArgument.Value.TryGetInt64Constant() == 0
            && (microsecondsArgument == null || microsecondsArgument.Value.TryGetInt64Constant() == 0))
        {
            consumer.AddHighlighting(
                new UseExpressionResultSuggestion(
                    $"The expression is always {nameof(TimeSpan)}.{nameof(TimeSpan.Zero)}.",
                    invocationExpression,
                    $"{nameof(TimeSpan)}.{nameof(TimeSpan.Zero)}"));
        }
    }

    /// <remarks>
    /// <c>TimeSpan.FromMinutes(0)</c> → <c>TimeSpan.Zero</c>
    /// </remarks>
    static void AnalyzeFromMinutes_Int64(IHighlightingConsumer consumer, IInvocationExpression invocationExpression, ICSharpArgument minutesArgument)
    {
        if (!invocationExpression.IsUsedAsStatement() && minutesArgument.Value.TryGetInt64Constant() == 0)
        {
            consumer.AddHighlighting(
                new UseExpressionResultSuggestion(
                    $"The expression is always {nameof(TimeSpan)}.{nameof(TimeSpan.Zero)}.",
                    invocationExpression,
                    $"{nameof(TimeSpan)}.{nameof(TimeSpan.Zero)}"));
        }
    }

    /// <remarks>
    /// <c>TimeSpan.FromMinutes(0)</c> → <c>TimeSpan.Zero</c>
    /// </remarks>
    static void AnalyzeFromMinutes_Int64_Int64_Int64_Int64(
        IHighlightingConsumer consumer,
        IInvocationExpression invocationExpression,
        ICSharpArgument minutesArgument,
        ICSharpArgument? secondsArgument,
        ICSharpArgument? millisecondsArgument,
        ICSharpArgument? microsecondsArgument)
    {
        if (!invocationExpression.IsUsedAsStatement()
            && minutesArgument.Value.TryGetInt64Constant() == 0
            && (secondsArgument == null || secondsArgument.Value.TryGetInt64Constant() == 0)
            && (millisecondsArgument == null || millisecondsArgument.Value.TryGetInt64Constant() == 0)
            && (microsecondsArgument == null || microsecondsArgument.Value.TryGetInt64Constant() == 0))
        {
            consumer.AddHighlighting(
                new UseExpressionResultSuggestion(
                    $"The expression is always {nameof(TimeSpan)}.{nameof(TimeSpan.Zero)}.",
                    invocationExpression,
                    $"{nameof(TimeSpan)}.{nameof(TimeSpan.Zero)}"));
        }
    }

    /// <remarks>
    /// <c>TimeSpan.FromSeconds(0)</c> → <c>TimeSpan.Zero</c>
    /// </remarks>
    static void AnalyzeFromSeconds_Int64(IHighlightingConsumer consumer, IInvocationExpression invocationExpression, ICSharpArgument secondsArgument)
    {
        if (!invocationExpression.IsUsedAsStatement() && secondsArgument.Value.TryGetInt64Constant() == 0)
        {
            consumer.AddHighlighting(
                new UseExpressionResultSuggestion(
                    $"The expression is always {nameof(TimeSpan)}.{nameof(TimeSpan.Zero)}.",
                    invocationExpression,
                    $"{nameof(TimeSpan)}.{nameof(TimeSpan.Zero)}"));
        }
    }

    /// <remarks>
    /// <c>TimeSpan.FromSeconds(0)</c> → <c>TimeSpan.Zero</c>
    /// </remarks>
    static void AnalyzeFromSeconds_Int64_Int64_Int64(
        IHighlightingConsumer consumer,
        IInvocationExpression invocationExpression,
        ICSharpArgument secondsArgument,
        ICSharpArgument? millisecondsArgument,
        ICSharpArgument? microsecondsArgument)
    {
        if (!invocationExpression.IsUsedAsStatement()
            && secondsArgument.Value.TryGetInt64Constant() == 0
            && (millisecondsArgument == null || millisecondsArgument.Value.TryGetInt64Constant() == 0)
            && (microsecondsArgument == null || microsecondsArgument.Value.TryGetInt64Constant() == 0))
        {
            consumer.AddHighlighting(
                new UseExpressionResultSuggestion(
                    $"The expression is always {nameof(TimeSpan)}.{nameof(TimeSpan.Zero)}.",
                    invocationExpression,
                    $"{nameof(TimeSpan)}.{nameof(TimeSpan.Zero)}"));
        }
    }

    /// <remarks>
    /// <c>TimeSpan.FromTicks(0)</c> → <c>TimeSpan.Zero</c><para/>
    /// <c>TimeSpan.FromTicks(long.MinValue)</c> → <c>TimeSpan.MinValue</c><para/>
    /// <c>TimeSpan.FromTicks(long.MaxValue)</c> → <c>TimeSpan.MaxValue</c>
    /// </remarks>
    static void AnalyzeFromTicks_Int64(IHighlightingConsumer consumer, IInvocationExpression invocationExpression, ICSharpArgument valueArgument)
    {
        if (!invocationExpression.IsUsedAsStatement())
        {
            switch (valueArgument.Value.TryGetInt64Constant())
            {
                case 0:
                    consumer.AddHighlighting(
                        new UseExpressionResultSuggestion(
                            $"The expression is always {nameof(TimeSpan)}.{nameof(TimeSpan.Zero)}.",
                            invocationExpression,
                            $"{nameof(TimeSpan)}.{nameof(TimeSpan.Zero)}"));
                    break;

                case long.MinValue:
                    consumer.AddHighlighting(
                        new UseExpressionResultSuggestion(
                            $"The expression is always {nameof(TimeSpan)}.{nameof(TimeSpan.MinValue)}.",
                            invocationExpression,
                            $"{nameof(TimeSpan)}.{nameof(TimeSpan.MinValue)}"));
                    break;

                case long.MaxValue:
                    consumer.AddHighlighting(
                        new UseExpressionResultSuggestion(
                            $"The expression is always {nameof(TimeSpan)}.{nameof(TimeSpan.MaxValue)}.",
                            invocationExpression,
                            $"{nameof(TimeSpan)}.{nameof(TimeSpan.MaxValue)}"));
                    break;
            }
        }
    }

    /// <remarks>
    /// <c>timeSpan.Multiply(factor)</c> → <c>timeSpan * factor</c> (.NET Core 2.0)
    /// </remarks>
    static void AnalyzeMultiply_Double(
        IHighlightingConsumer consumer,
        IInvocationExpression invocationExpression,
        IReferenceExpression invokedExpression,
        ICSharpArgument factorArgument)
    {
        Debug.Assert(invokedExpression.QualifierExpression is { });

        if (!invocationExpression.IsUsedAsStatement() && factorArgument.Value is { })
        {
            consumer.AddHighlighting(
                new UseBinaryOperatorSuggestion(
                    "Use the '*' operator.",
                    invocationExpression,
                    "*",
                    invokedExpression.QualifierExpression.GetText(),
                    factorArgument.Value.GetText()));
        }
    }

    /// <remarks>
    /// <c>timeSpan.Negate()</c> → <c>-timeSpan</c>
    /// </remarks>
    static void AnalyzeNegate(IHighlightingConsumer consumer, IInvocationExpression invocationExpression, IReferenceExpression invokedExpression)
    {
        Debug.Assert(invokedExpression.QualifierExpression is { });

        if (!invocationExpression.IsUsedAsStatement())
        {
            consumer.AddHighlighting(
                new UseUnaryOperatorSuggestion("Use the '-' operator.", invocationExpression, "-", invokedExpression.QualifierExpression.GetText()));
        }
    }

    /// <remarks>
    /// <c>TimeSpan.ParseExact(input, "c", formatProvider)</c> → <c>TimeSpan.ParseExact(input, "c", null)</c><para/>
    /// <c>TimeSpan.ParseExact(input, "t", formatProvider)</c> → <c>TimeSpan.ParseExact(input, "t", null)</c><para/>
    /// <c>TimeSpan.ParseExact(input, "T", formatProvider)</c> → <c>TimeSpan.ParseExact(input, "T", null)</c>
    /// </remarks>
    static void AnalyzeParseExact_String_String_IFormatProvider(
        IHighlightingConsumer consumer,
        ICSharpArgument formatArgument,
        ICSharpArgument formatProviderArgument)
    {
        if (formatArgument.Value.TryGetStringConstant() is "c" or "t" or "T"
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
    /// <c>TimeSpan.ParseExact(input, "c", formatProvider, styles)</c> → <c>TimeSpan.ParseExact(input, "c", null, styles)</c><para/>
    /// <c>TimeSpan.ParseExact(input, "t", formatProvider, styles)</c> → <c>TimeSpan.ParseExact(input, "t", null, styles)</c><para/>
    /// <c>TimeSpan.ParseExact(input, "T", formatProvider, styles)</c> → <c>TimeSpan.ParseExact(input, "T", null, styles)</c>
    /// </remarks>
    static void AnalyzeParseExact_String_String_IFormatProvider_TimeSpanStyles(
        IHighlightingConsumer consumer,
        ICSharpArgument formatArgument,
        ICSharpArgument formatProviderArgument)
    {
        if (formatArgument.Value.TryGetStringConstant() is "c" or "t" or "T"
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
    /// <c>TimeSpan.ParseExact(input, [format], formatProvider)</c> → <c>TimeSpan.ParseExact(input, format, formatProvider)</c>
    /// </remarks>
    static void AnalyzeParseExact_String_StringArray_IFormatProvider(
        IHighlightingConsumer consumer,
        IInvocationExpression invocationExpression,
        ICSharpArgument formatsArgument)
    {
        if (CollectionCreation.TryFrom(formatsArgument.Value) is { Count: 1 } collectionCreation
            && PredefinedType.TIMESPAN_FQN.HasMethod(
                new MethodSignature { Name = nameof(TimeSpan.ParseExact), Parameters = Parameters.String_String_IFormatProvider, IsStatic = true },
                formatsArgument.NameIdentifier is { },
                out var parameterNames,
                invocationExpression.PsiModule))
        {
            consumer.AddHighlighting(
                new UseOtherArgumentSuggestion(
                    "The only collection element should be passed directly.",
                    formatsArgument,
                    parameterNames is [_, var formatParameterName, _] ? formatParameterName : null,
                    collectionCreation.SingleElement.GetText()));
        }
    }

    /// <remarks>
    /// <c>TimeSpan.ParseExact(input, [format], formatProvider, styles)</c> → <c>TimeSpan.ParseExact(input, format, formatProvider, styles)</c>
    /// </remarks>
    static void AnalyzeParseExact_String_StringArray_IFormatProvider_TimeSpanStyles(
        IHighlightingConsumer consumer,
        IInvocationExpression invocationExpression,
        ICSharpArgument formatsArgument)
    {
        if (CollectionCreation.TryFrom(formatsArgument.Value) is { Count: 1 } collectionCreation
            && PredefinedType.TIMESPAN_FQN.HasMethod(
                new MethodSignature
                {
                    Name = nameof(TimeSpan.ParseExact), Parameters = Parameters.String_String_IFormatProvider_TimeSpanStyles, IsStatic = true,
                },
                formatsArgument.NameIdentifier is { },
                out var parameterNames,
                invocationExpression.PsiModule))
        {
            consumer.AddHighlighting(
                new UseOtherArgumentSuggestion(
                    "The only collection element should be passed directly.",
                    formatsArgument,
                    parameterNames is [_, var formatParameterName, _, _] ? formatParameterName : null,
                    collectionCreation.SingleElement.GetText()));
        }
    }

    /// <remarks>
    /// <c>timeSpan.Subtract(ts)</c> → <c>timeSpan - ts</c>
    /// </remarks>
    static void AnalyzeSubtract_TimeSpan(
        IHighlightingConsumer consumer,
        IInvocationExpression invocationExpression,
        IReferenceExpression invokedExpression,
        ICSharpArgument tsArgument)
    {
        Debug.Assert(invokedExpression.QualifierExpression is { });

        if (!invocationExpression.IsUsedAsStatement() && tsArgument.Value is { })
        {
            consumer.AddHighlighting(
                new UseBinaryOperatorSuggestion(
                    "Use the '-' operator.",
                    invocationExpression,
                    "-",
                    invokedExpression.QualifierExpression.GetText(),
                    tsArgument.Value.GetText()));
        }
    }

    /// <remarks>
    /// <c>TimeSpan.TryParseExact(input, "c", formatProvider, out result)</c> → <c>TimeSpan.TryParseExact(input, "c", null, out result)</c><para/>
    /// <c>TimeSpan.TryParseExact(input, "t", formatProvider, out result)</c> → <c>TimeSpan.TryParseExact(input, "t", null, out result)</c><para/>
    /// <c>TimeSpan.TryParseExact(input, "T", formatProvider, out result)</c> → <c>TimeSpan.TryParseExact(input, "T", null, out result)</c>
    /// </remarks>
    static void AnalyzeTryParseExact_String_String_IFormatProvider_TimeSpan(
        IHighlightingConsumer consumer,
        ICSharpArgument formatArgument,
        ICSharpArgument formatProviderArgument)
    {
        if (formatArgument.Value.TryGetStringConstant() is "c" or "t" or "T"
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
    /// <c>TimeSpan.TryParseExact(input, "c", formatProvider, styles, out result)</c> → <c>TimeSpan.TryParseExact(input, "c", null, styles, out result)</c><para/>
    /// <c>TimeSpan.TryParseExact(input, "t", formatProvider, styles, out result)</c> → <c>TimeSpan.TryParseExact(input, "t", null, styles, out result)</c><para/>
    /// <c>TimeSpan.TryParseExact(input, "t", formatProvider, styles, out result)</c> → <c>TimeSpan.TryParseExact(input, "T", null, styles, out result)</c>
    /// </remarks>
    static void AnalyzeTryParseExact_String_String_IFormatProvider_TimeSpanStyles_TimeSpan(
        IHighlightingConsumer consumer,
        ICSharpArgument formatArgument,
        ICSharpArgument formatProviderArgument)
    {
        if (formatArgument.Value.TryGetStringConstant() is "c" or "t" or "T"
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
    /// <c>TimeSpan.TryParseExact(input, [format], formatProvider, out result)</c> → <c>TimeSpan.TryParseExact(input, format, formatProvider, out result)</c>
    /// </remarks>
    static void AnalyzeTryParseExact_String_StringArray_IFormatProvider_TimeSpan(
        IHighlightingConsumer consumer,
        IInvocationExpression invocationExpression,
        ICSharpArgument formatsArgument)
    {
        if (CollectionCreation.TryFrom(formatsArgument.Value) is { Count: 1 } collectionCreation
            && PredefinedType.TIMESPAN_FQN.HasMethod(
                new MethodSignature
                {
                    Name = nameof(TimeSpan.TryParseExact), Parameters = Parameters.String_String_IFormatProvider_outTimeSpan, IsStatic = true,
                },
                formatsArgument.NameIdentifier is { },
                out var parameterNames,
                invocationExpression.PsiModule))
        {
            consumer.AddHighlighting(
                new UseOtherArgumentSuggestion(
                    "The only collection element should be passed directly.",
                    formatsArgument,
                    parameterNames is [_, var formatParameterName, _] ? formatParameterName : null,
                    collectionCreation.SingleElement.GetText()));
        }
    }

    /// <remarks>
    /// <c>TimeSpan.TryParseExact(input, [format], formatProvider, styles, out result)</c> → <c>TimeSpan.TryParseExact(input, format, formatProvider, styles, out result)</c>
    /// </remarks>
    static void AnalyzeTryParseExact_String_StringArray_IFormatProvider_TimeSpanStyles_TimeSpan(
        IHighlightingConsumer consumer,
        IInvocationExpression invocationExpression,
        ICSharpArgument formatsArgument)
    {
        if (CollectionCreation.TryFrom(formatsArgument.Value) is { Count: 1 } collectionCreation
            && PredefinedType.TIMESPAN_FQN.HasMethod(
                new MethodSignature
                {
                    Name = nameof(TimeSpan.TryParseExact),
                    Parameters = Parameters.String_String_IFormatProvider_TimeSpanStyles_outTimeSpan,
                    IsStatic = true,
                },
                formatsArgument.NameIdentifier is { },
                out var parameterNames,
                invocationExpression.PsiModule))
        {
            consumer.AddHighlighting(
                new UseOtherArgumentSuggestion(
                    "The only collection element should be passed directly.",
                    formatsArgument,
                    parameterNames is [_, var formatParameterName, _, _] ? formatParameterName : null,
                    collectionCreation.SingleElement.GetText()));
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
                && constructor.ContainingType.IsClrType(PredefinedType.TIMESPAN_FQN):

                switch (constructor.Parameters, objectCreationExpression.TryGetArgumentsInDeclarationOrder())
                {
                    case ([{ Type: var ticksType }], [{ } ticksArgument]) when ticksType.IsLong():
                        Analyze_Ctor_Int64(consumer, objectCreationExpression, ticksArgument);
                        break;

                    case ([{ Type: var hoursType }, { Type: var minutesType }, { Type: var secondsType }], [
                        { } hoursArgument, { } minutesArgument, { } secondsArgument,
                    ]) when hoursType.IsInt() && minutesType.IsInt() && secondsType.IsInt():
                        Analyze_Ctor_Int32_Int32_Int32(consumer, objectCreationExpression, hoursArgument, minutesArgument, secondsArgument);
                        break;

                    case ([{ Type: var daysType }, { Type: var hoursType }, { Type: var minutesType }, { Type: var secondsType }], [
                        { } daysArgument, { } hoursArgument, { } minutesArgument, { } secondsArgument,
                    ]) when daysType.IsInt() && hoursType.IsInt() && minutesType.IsInt() && secondsType.IsInt():
                        Analyze_Ctor_Int32_Int32_Int32_Int32(
                            consumer,
                            objectCreationExpression,
                            daysArgument,
                            hoursArgument,
                            minutesArgument,
                            secondsArgument);
                        break;

                    case ([
                            { Type: var daysType },
                            { Type: var hoursType },
                            { Type: var minutesType },
                            { Type: var secondsType },
                            { Type: var millisecondsType },
                        ], [{ } daysArgument, { } hoursArgument, { } minutesArgument, { } secondsArgument, { } millisecondsArgument])
                        when daysType.IsInt() && hoursType.IsInt() && minutesType.IsInt() && secondsType.IsInt() && millisecondsType.IsInt():
                        Analyze_Ctor_Int32_Int32_Int32_Int32_Int32(
                            consumer,
                            objectCreationExpression,
                            daysArgument,
                            hoursArgument,
                            minutesArgument,
                            secondsArgument,
                            millisecondsArgument);
                        break;

                    case ([
                            { Type: var daysType },
                            { Type: var hoursType },
                            { Type: var minutesType },
                            { Type: var secondsType },
                            { Type: var millisecondsType },
                            { Type: var microsecondsType },
                        ], [
                            { } daysArgument,
                            { } hoursArgument,
                            { } minutesArgument,
                            { } secondsArgument,
                            { } millisecondsArgument,
                            { } microsecondsArgument,
                        ]) when daysType.IsInt()
                        && hoursType.IsInt()
                        && minutesType.IsInt()
                        && secondsType.IsInt()
                        && millisecondsType.IsInt()
                        && microsecondsType.IsInt():
                        Analyze_Ctor_Int32_Int32_Int32_Int32_Int32_Int32(
                            consumer,
                            objectCreationExpression,
                            daysArgument,
                            hoursArgument,
                            minutesArgument,
                            secondsArgument,
                            millisecondsArgument,
                            microsecondsArgument);
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
                && method.ContainingType.IsClrType(PredefinedType.TIMESPAN_FQN):

                switch (invokedExpression, method)
                {
                    case ({ QualifierExpression: { } }, { IsStatic: false }):
                        switch (method.ShortName)
                        {
                            case nameof(TimeSpan.Add):
                                switch (method.Parameters, invocationExpression.TryGetArgumentsInDeclarationOrder())
                                {
                                    case ([{ Type: var tsType }], [{ } tsArgument]) when tsType.IsTimeSpan():
                                        AnalyzeAdd_TimeSpan(consumer, invocationExpression, invokedExpression, tsArgument);
                                        break;
                                }
                                break;

                            case "Divide": // todo: nameof(TimeSpan.Divide) when available
                                switch (method.Parameters, invocationExpression.TryGetArgumentsInDeclarationOrder())
                                {
                                    case ([{ Type: var divisorType }], [{ } divisorArgument]) when divisorType.IsDouble():
                                        AnalyzeDivide_Double(consumer, invocationExpression, invokedExpression, divisorArgument);
                                        break;

                                    case ([{ Type: var tsType }], [{ } tsArgument]) when tsType.IsTimeSpan():
                                        AnalyzeDivide_TimeSpan(consumer, invocationExpression, invokedExpression, tsArgument);
                                        break;
                                }
                                break;

                            case nameof(TimeSpan.Equals):
                                switch (method.Parameters, invocationExpression.TryGetArgumentsInDeclarationOrder())
                                {
                                    case ([{ Type: var objType }], [{ } objArgument]) when objType.IsTimeSpan():
                                        AnalyzeEquals_TimeSpan(consumer, invocationExpression, invokedExpression, objArgument);
                                        break;

                                    case ([{ Type: var objType }], [{ } objArgument]) when objType.IsObject():
                                        AnalyzeEquals_Object(consumer, invocationExpression, objArgument);
                                        break;
                                }
                                break;

                            case "Multiply": // todo: nameof(TimeSpan.Multiply) when available
                                switch (method.Parameters, invocationExpression.TryGetArgumentsInDeclarationOrder())
                                {
                                    case ([{ Type: var factorType }], [{ } factorArgument]) when factorType.IsDouble():
                                        AnalyzeMultiply_Double(consumer, invocationExpression, invokedExpression, factorArgument);
                                        break;
                                }
                                break;

                            case nameof(TimeSpan.Negate):
                                switch (method.Parameters, invocationExpression.TryGetArgumentsInDeclarationOrder())
                                {
                                    case ([], []): AnalyzeNegate(consumer, invocationExpression, invokedExpression); break;
                                }
                                break;

                            case nameof(TimeSpan.Subtract):
                                switch (method.Parameters, invocationExpression.TryGetArgumentsInDeclarationOrder())
                                {
                                    case ([{ Type: var tsType }], [{ } tsArgument]) when tsType.IsTimeSpan():
                                        AnalyzeSubtract_TimeSpan(consumer, invocationExpression, invokedExpression, tsArgument);
                                        break;
                                }
                                break;
                        }
                        break;

                    case (_, { IsStatic: true }):
                        switch (method.ShortName)
                        {
                            case nameof(TimeSpan.Equals):
                                switch (method.Parameters, invocationExpression.TryGetArgumentsInDeclarationOrder())
                                {
                                    case ([{ Type: var t1Type }, { Type: var t2Type }], [{ } t1Argument, { } t2Argument])
                                        when t1Type.IsTimeSpan() && t2Type.IsTimeSpan():

                                        AnalyzeEquals_TimeSpan_TimeSpan(consumer, invocationExpression, t1Argument, t2Argument);
                                        break;
                                }
                                break;

                            case nameof(TimeSpan.FromDays):
                                switch (method.Parameters, invocationExpression.TryGetArgumentsInDeclarationOrder())
                                {
                                    case ([{ Type: var daysType }], [{ } daysArgument]) when daysType.IsInt():
                                        AnalyzeFromDays_Int32(consumer, invocationExpression, daysArgument);
                                        break;

                                    case ([
                                            { Type: var daysType },
                                            { Type: var hoursType },
                                            { Type: var minutesType },
                                            { Type: var secondsType },
                                            { Type: var millisecondsType },
                                            { Type: var microsecondsType },
                                        ], [
                                            { } daysArgument,
                                            var hoursArgument,
                                            var minutesArgument,
                                            var secondsArgument,
                                            var millisecondsArgument,
                                            var microsecondsArgument,
                                        ]) when daysType.IsInt()
                                        && hoursType.IsInt()
                                        && minutesType.IsLong()
                                        && secondsType.IsLong()
                                        && millisecondsType.IsLong()
                                        && microsecondsType.IsLong():

                                        AnalyzeFromDays_Int32_Int32_Int64_Int64_Int64_Int64(
                                            consumer,
                                            invocationExpression,
                                            daysArgument,
                                            hoursArgument,
                                            minutesArgument,
                                            secondsArgument,
                                            millisecondsArgument,
                                            microsecondsArgument);
                                        break;
                                }
                                break;

                            case nameof(TimeSpan.FromHours):
                                switch (method.Parameters, invocationExpression.TryGetArgumentsInDeclarationOrder())
                                {
                                    case ([{ Type: var hoursType }], [{ } hoursArgument]) when hoursType.IsInt():
                                        AnalyzeFromHours_Int32(consumer, invocationExpression, hoursArgument);
                                        break;

                                    case ([
                                            { Type: var hoursType },
                                            { Type: var minutesType },
                                            { Type: var secondsType },
                                            { Type: var millisecondsType },
                                            { Type: var microsecondsType },
                                        ], [
                                            { } hoursArgument,
                                            var minutesArgument,
                                            var secondsArgument,
                                            var millisecondsArgument,
                                            var microsecondsArgument,
                                        ]) when hoursType.IsInt()
                                        && minutesType.IsLong()
                                        && secondsType.IsLong()
                                        && millisecondsType.IsLong()
                                        && microsecondsType.IsLong():

                                        AnalyzeFromHours_Int32_Int64_Int64_Int64_Int64(
                                            consumer,
                                            invocationExpression,
                                            hoursArgument,
                                            minutesArgument,
                                            secondsArgument,
                                            millisecondsArgument,
                                            microsecondsArgument);
                                        break;
                                }
                                break;

                            case "FromMicroseconds": // todo: nameof(TimeSpan.FromMicroseconds) when available
                                switch (method.Parameters, invocationExpression.TryGetArgumentsInDeclarationOrder())
                                {
                                    case ([{ Type: var microsecondsType }], [{ } microsecondsArgument]) when microsecondsType.IsLong():
                                        AnalyzeFromMicroseconds_Int64(consumer, invocationExpression, microsecondsArgument);
                                        break;
                                }
                                break;

                            case nameof(TimeSpan.FromMilliseconds):
                                switch (method.Parameters, invocationExpression.TryGetArgumentsInDeclarationOrder())
                                {
                                    case ([{ Type: var millisecondsType }, { Type: var microsecondsType }], [
                                        { } millisecondsArgument, var microsecondsArgument,
                                    ]) when millisecondsType.IsLong() && microsecondsType.IsLong():
                                        AnalyzeFromMilliseconds_Int64_Int64(
                                            consumer,
                                            invocationExpression,
                                            millisecondsArgument,
                                            microsecondsArgument);
                                        break;
                                }
                                break;

                            case nameof(TimeSpan.FromMinutes):
                                switch (method.Parameters, invocationExpression.TryGetArgumentsInDeclarationOrder())
                                {
                                    case ([{ Type: var minutesType }], [{ } minutesArgument]) when minutesType.IsLong():
                                        AnalyzeFromMinutes_Int64(consumer, invocationExpression, minutesArgument);
                                        break;

                                    case ([
                                            { Type: var minutesType },
                                            { Type: var secondsType },
                                            { Type: var millisecondsType },
                                            { Type: var microsecondsType },
                                        ], [{ } minutesArgument, var secondsArgument, var millisecondsArgument, var microsecondsArgument])
                                        when minutesType.IsLong() && secondsType.IsLong() && millisecondsType.IsLong() && microsecondsType.IsLong():

                                        AnalyzeFromMinutes_Int64_Int64_Int64_Int64(
                                            consumer,
                                            invocationExpression,
                                            minutesArgument,
                                            secondsArgument,
                                            millisecondsArgument,
                                            microsecondsArgument);
                                        break;
                                }
                                break;

                            case nameof(TimeSpan.FromSeconds):
                                switch (method.Parameters, invocationExpression.TryGetArgumentsInDeclarationOrder())
                                {
                                    case ([{ Type: var secondsType }], [{ } secondsArgument]) when secondsType.IsLong():
                                        AnalyzeFromSeconds_Int64(consumer, invocationExpression, secondsArgument);
                                        break;

                                    case ([{ Type: var secondsType }, { Type: var millisecondsType }, { Type: var microsecondsType }], [
                                        { } secondsArgument, var millisecondsArgument, var microsecondsArgument,
                                    ]) when secondsType.IsLong() && millisecondsType.IsLong() && microsecondsType.IsLong():
                                        AnalyzeFromSeconds_Int64_Int64_Int64(
                                            consumer,
                                            invocationExpression,
                                            secondsArgument,
                                            millisecondsArgument,
                                            microsecondsArgument);
                                        break;
                                }
                                break;

                            case nameof(TimeSpan.FromTicks):
                                switch (method.Parameters, invocationExpression.TryGetArgumentsInDeclarationOrder())
                                {
                                    case ([{ Type: var ticksType }], [{ } ticksArgument]) when ticksType.IsLong():
                                        AnalyzeFromTicks_Int64(consumer, invocationExpression, ticksArgument);
                                        break;
                                }
                                break;

                            case nameof(TimeSpan.ParseExact):
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
                                        && stylesType.IsTimeSpanStyles():

                                        AnalyzeParseExact_String_String_IFormatProvider_TimeSpanStyles(
                                            consumer,
                                            formatArgument,
                                            formatProviderArgument);
                                        break;

                                    case ([{ Type: var inputType }, { Type: var formatsType }, { Type: var formatProviderType }], [
                                        _, { } formatsArgument, _,
                                    ]) when inputType.IsString() && formatsType.IsGenericArrayOfString() && formatProviderType.IsIFormatProvider():
                                        AnalyzeParseExact_String_StringArray_IFormatProvider(consumer, invocationExpression, formatsArgument);
                                        break;

                                    case ([
                                            { Type: var inputType },
                                            { Type: var formatsType },
                                            { Type: var formatProviderType },
                                            { Type: var stylesType },
                                        ], [_, { } formatsArgument, _, _]) when inputType.IsString()
                                        && formatsType.IsGenericArrayOfString()
                                        && formatProviderType.IsIFormatProvider()
                                        && stylesType.IsTimeSpanStyles():

                                        AnalyzeParseExact_String_StringArray_IFormatProvider_TimeSpanStyles(
                                            consumer,
                                            invocationExpression,
                                            formatsArgument);
                                        break;
                                }
                                break;

                            case nameof(TimeSpan.TryParseExact):
                                switch (method.Parameters, invocationExpression.TryGetArgumentsInDeclarationOrder())
                                {
                                    case ([
                                            { Type: var inputType },
                                            { Type: var formatType },
                                            { Type: var formatProviderType },
                                            { Type: var resultType },
                                        ], [_, { } formatArgument, { } formatProviderArgument, _])
                                        when inputType.IsString()
                                        && formatType.IsString()
                                        && formatProviderType.IsIFormatProvider()
                                        && resultType.IsTimeSpan():

                                        AnalyzeTryParseExact_String_String_IFormatProvider_TimeSpan(consumer, formatArgument, formatProviderArgument);
                                        break;

                                    case ([
                                            { Type: var inputType },
                                            { Type: var formatType },
                                            { Type: var formatProviderType },
                                            { Type: var stylesType },
                                            { Type: var resultType },
                                        ], [_, { } formatArgument, { } formatProviderArgument, _, _]) when inputType.IsString()
                                        && formatType.IsString()
                                        && formatProviderType.IsIFormatProvider()
                                        && stylesType.IsTimeSpanStyles()
                                        && resultType.IsTimeSpan():

                                        AnalyzeTryParseExact_String_String_IFormatProvider_TimeSpanStyles_TimeSpan(
                                            consumer,
                                            formatArgument,
                                            formatProviderArgument);
                                        break;

                                    case ([
                                            { Type: var inputType },
                                            { Type: var formatsType },
                                            { Type: var formatProviderType },
                                            { Type: var resultType },
                                        ], [_, { } formatsArgument, _, _]) when inputType.IsString()
                                        && formatsType.IsGenericArrayOfString()
                                        && formatProviderType.IsIFormatProvider()
                                        && resultType.IsTimeSpan():

                                        AnalyzeTryParseExact_String_StringArray_IFormatProvider_TimeSpan(
                                            consumer,
                                            invocationExpression,
                                            formatsArgument);
                                        break;

                                    case ([
                                            { Type: var inputType },
                                            { Type: var formatsType },
                                            { Type: var formatProviderType },
                                            { Type: var stylesType },
                                            { Type: var resultType },
                                        ], [_, { } formatsArgument, _, _, _]) when inputType.IsString()
                                        && formatsType.IsGenericArrayOfString()
                                        && formatProviderType.IsIFormatProvider()
                                        && stylesType.IsTimeSpanStyles()
                                        && resultType.IsTimeSpan():

                                        AnalyzeTryParseExact_String_StringArray_IFormatProvider_TimeSpanStyles_TimeSpan(
                                            consumer,
                                            invocationExpression,
                                            formatsArgument);
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