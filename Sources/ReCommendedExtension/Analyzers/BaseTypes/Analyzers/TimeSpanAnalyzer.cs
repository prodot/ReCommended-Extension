using JetBrains.ReSharper.Feature.Services.Daemon;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.CSharp;
using JetBrains.ReSharper.Psi.CSharp.Tree;
using ReCommendedExtension.Extensions;

namespace ReCommendedExtension.Analyzers.BaseTypes.Analyzers;

/// <remarks>
/// C# language version checks are only done when a quick fix would require it.
/// </remarks>
[ElementProblemAnalyzer(typeof(ICSharpInvocationInfo), HighlightingTypes = [typeof(UseExpressionResultSuggestion)])]
public sealed class TimeSpanAnalyzer : ElementProblemAnalyzer<ICSharpInvocationInfo>
{
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
                            case nameof(TimeSpan.Equals):
                                switch (method.Parameters, invocationExpression.TryGetArgumentsInDeclarationOrder())
                                {
                                    case ([{ Type: var objType }], [{ } objArgument]) when objType.IsObject():
                                        AnalyzeEquals_Object(consumer, invocationExpression, objArgument);
                                        break;
                                }
                                break;
                        }
                        break;

                    case (_, { IsStatic: true }):
                        switch (method.ShortName)
                        {
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
                        }
                        break;
                }
                break;
        }
    }
}