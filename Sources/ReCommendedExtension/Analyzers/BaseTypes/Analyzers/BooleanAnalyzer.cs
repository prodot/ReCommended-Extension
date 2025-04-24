using JetBrains.ReSharper.Feature.Services.Daemon;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.CSharp.Tree;
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
        typeof(UseExpressionResultSuggestion),
        typeof(UseBinaryOperationSuggestion),
        typeof(RedundantMethodInvocationHint),
        typeof(RedundantArgumentHint),
    ])]
public sealed class BooleanAnalyzer : ElementProblemAnalyzer<IInvocationExpression>
{
    /// <remarks>
    /// <c>flag.Equals(true)</c> → <c>flag</c><para/>
    /// <c>flag.Equals(false)</c> → <c>!flag</c><para/>
    /// <c>true.Equals(obj)</c> → <c>obj</c><para/>
    /// <c>false.Equals(obj)</c> → <c>!obj</c><para/>
    /// <c>flag.Equals(obj)</c> → <c>flag == obj</c>
    /// </remarks>
    static void AnalyzeEquals_Boolean(
        IHighlightingConsumer consumer,
        IInvocationExpression invocationExpression,
        IReferenceExpression invokedExpression,
        ICSharpArgument objArgument)
    {
        Debug.Assert(invokedExpression.QualifierExpression is { });

        if (!invocationExpression.IsUsedAsStatement())
        {
            switch (invokedExpression.QualifierExpression.TryGetBooleanConstant(), objArgument.Value.TryGetBooleanConstant())
            {
                case (null, true):
                    consumer.AddHighlighting(
                        new RedundantMethodInvocationHint(
                            $"Calling '{nameof(bool.Equals)}' with true is redundant.",
                            invocationExpression,
                            invokedExpression));
                    break;

                case (null, false):
                    consumer.AddHighlighting(
                        new UseExpressionResultSuggestion(
                            "The expression is always the same as the inverted qualifier.",
                            invocationExpression,
                            $"!{invokedExpression.QualifierExpression.GetText()}"));
                    break;

                case (true, null) when objArgument.Value is { }:
                    consumer.AddHighlighting(
                        new RedundantMethodInvocationHint(
                            $"Calling 'true.{nameof(bool.Equals)}' is redundant.",
                            invocationExpression,
                            invokedExpression,
                            objArgument.Value));
                    break;

                case (false, null) when objArgument.Value is { }:
                    consumer.AddHighlighting(
                        new UseExpressionResultSuggestion(
                            "The expression is always the same as the inverted argument.",
                            invocationExpression,
                            $"!({objArgument.Value.GetText()})"));
                    break;

                default:
                    if (objArgument.Value is { })
                    {
                        consumer.AddHighlighting(
                            new UseBinaryOperationSuggestion(
                                "Use the '==' operator.",
                                invocationExpression,
                                "==",
                                invokedExpression.QualifierExpression.GetText(),
                                objArgument.Value.GetText()));
                    }
                    break;
            }
        }
    }

    /// <remarks>
    /// <c>flag.Equals(null)</c> → <c>false</c>
    /// </remarks>
    static void AnalyzeEquals_Object(IHighlightingConsumer consumer, IInvocationExpression invocationExpression, ICSharpArgument objArgument)
    {
        if (!invocationExpression.IsUsedAsStatement() && objArgument.Value.IsDefaultValue())
        {
            consumer.AddHighlighting(new UseExpressionResultSuggestion("The expression is always false.", invocationExpression, "false"));
        }
    }

    /// <remarks>
    /// <c>flag.GetTypeCode()</c> → <c>TypeCode.Boolean</c>
    /// </remarks>
    static void AnalyzeGetTypeCode(IHighlightingConsumer consumer, IInvocationExpression invocationExpression)
    {
        if (!invocationExpression.IsUsedAsStatement())
        {
            consumer.AddHighlighting(
                new UseExpressionResultSuggestion(
                    $"The expression is always {nameof(TypeCode)}.{nameof(TypeCode.Boolean)}.",
                    invocationExpression,
                    $"{nameof(TypeCode)}.{nameof(TypeCode.Boolean)}"));
        }
    }

    /// <remarks>
    /// <c>flag.ToString(provider)</c> → <c>flag.ToString()</c>
    /// </remarks>
    static void AnalyzeToString_IFormatProvider(
        IHighlightingConsumer consumer,
        IInvocationExpression invocationExpression,
        ICSharpArgument providerArgument)
    {
        if (PredefinedType.BOOLEAN_FQN.HasMethod(
            new MethodSignature { Name = nameof(bool.ToString), ParameterTypes = [] },
            invocationExpression.PsiModule))
        {
            consumer.AddHighlighting(new RedundantArgumentHint("Passing a format provider is redundant.", providerArgument));
        }
    }

    protected override void Run(IInvocationExpression element, ElementProblemAnalyzerData data, IHighlightingConsumer consumer)
    {
        if (element is { InvokedExpression: IReferenceExpression { Reference: var reference, QualifierExpression: { } } invokedExpression }
            && reference.Resolve().DeclaredElement is IMethod
            {
                AccessibilityDomain.DomainType: AccessibilityDomain.AccessibilityDomainType.PUBLIC, IsStatic: false, TypeParameters: [],
            } method
            && method.ContainingType.IsClrType(PredefinedType.BOOLEAN_FQN))
        {
            switch (method.ShortName)
            {
                case nameof(bool.Equals):
                    switch (method.Parameters, element.Arguments)
                    {
                        case ([{ Type: var objType }], [var objArgument]) when objType.IsBool():
                            AnalyzeEquals_Boolean(consumer, element, invokedExpression, objArgument);
                            break;

                        case ([{ Type: var objType }], [var objArgument]) when objType.IsObject():
                            AnalyzeEquals_Object(consumer, element, objArgument);
                            break;
                    }
                    break;

                case nameof(bool.GetTypeCode):
                    switch (method.Parameters, element.Arguments)
                    {
                        case ([], []): AnalyzeGetTypeCode(consumer, element); break;
                    }
                    break;

                case nameof(bool.ToString):
                    switch (method.Parameters, element.Arguments)
                    {
                        case ([{ Type: var providerType }], [var providerArgument]) when providerType.IsIFormatProvider():
                            AnalyzeToString_IFormatProvider(consumer, element, providerArgument);
                            break;
                    }
                    break;
            }
        }
    }
}