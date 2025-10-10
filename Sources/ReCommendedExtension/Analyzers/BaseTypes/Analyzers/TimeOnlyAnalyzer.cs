using JetBrains.ReSharper.Feature.Services.Daemon;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.CSharp;
using JetBrains.ReSharper.Psi.CSharp.Tree;
using ReCommendedExtension.Extensions;

namespace ReCommendedExtension.Analyzers.BaseTypes.Analyzers;

/// <remarks>
/// C# language version checks are only done when a quick fix would require it.
/// </remarks>
[ElementProblemAnalyzer(
    typeof(ICSharpInvocationInfo),
    HighlightingTypes = [typeof(UseExpressionResultSuggestion), typeof(UseBinaryOperatorSuggestion)])]
public sealed class TimeOnlyAnalyzer : ElementProblemAnalyzer<ICSharpInvocationInfo>
{
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
                }
                break;
        }
    }
}