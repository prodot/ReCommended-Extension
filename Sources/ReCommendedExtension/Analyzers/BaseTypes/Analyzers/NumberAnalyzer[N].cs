using JetBrains.ReSharper.Feature.Services.Daemon;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.CSharp.Tree;
using ReCommendedExtension.Analyzers.BaseTypes.NumberInfos;
using ReCommendedExtension.Extensions;

namespace ReCommendedExtension.Analyzers.BaseTypes.Analyzers;

public abstract class NumberAnalyzer<N>(NumberInfo<N> numberInfo) : ElementProblemAnalyzer<IInvocationExpression> where N : struct
{
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

                var replacementMin = numberInfo.GetReplacementFromArgument(invocationExpression, minArgument.Value);
                var replacementMax = numberInfo.GetReplacementFromArgument(invocationExpression, maxArgument.Value);

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
                        numberInfo.GetReplacementFromArgument(invocationExpression, value)));
            }
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
            var replacement = $"{nameof(TypeCode)}.{typeCode}";

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

            var replacementX = numberInfo.GetReplacementFromArgument(invocationExpression, xArgument.Value);
            var replacementY = numberInfo.GetReplacementFromArgument(invocationExpression, yArgument.Value);

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

            var replacementX = numberInfo.GetReplacementFromArgument(invocationExpression, xArgument.Value);
            var replacementY = numberInfo.GetReplacementFromArgument(invocationExpression, yArgument.Value);

            consumer.AddHighlighting(
                new UseExpressionResultSuggestion(
                    $"The expression is always {x}.",
                    invocationExpression,
                    replacementX,
                    replacementY != replacementX ? replacementY : null));
        }
    }

    private protected NumberInfo<N> NumberInfo => numberInfo;

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
                            switch (method.Parameters, element.TryGetArgumentsInDeclarationOrder())
                            {
                                case ([{ Type: var objType }], [{ } objArgument]) when objType.IsObject():
                                    AnalyzeEquals_Object(consumer, element, objArgument);
                                    break;
                            }
                            break;

                        case nameof(IConvertible.GetTypeCode):
                            switch (method.Parameters, element.TryGetArgumentsInDeclarationOrder())
                            {
                                case ([], []): AnalyzeGetTypeCode(consumer, element); break;
                            }
                            break;
                    }
                    break;

                case (_, { IsStatic: true }):
                    switch (method.ShortName)
                    {
                        case "Clamp": // todo: nameof(INumber<T>.Clamp) when available
                            switch (method.Parameters, element.TryGetArgumentsInDeclarationOrder())
                            {
                                case ([{ Type: var valueType }, { Type: var minType }, { Type: var maxType }], [
                                        { } valueArgument, { } minArgument, { } maxArgument,
                                    ]) when valueType.IsClrType(numberInfo.ClrTypeName)
                                    && minType.IsClrType(numberInfo.ClrTypeName)
                                    && maxType.IsClrType(numberInfo.ClrTypeName):

                                    AnalyzeClamp(consumer, element, valueArgument, minArgument, maxArgument);
                                    break;
                            }
                            break;

                        case "Max": // todo: nameof(INumber<T>.Max) when available
                            switch (method.Parameters, element.TryGetArgumentsInDeclarationOrder())
                            {
                                case ([{ Type: var xType }, { Type: var yType }], [{ } xArgument, { } yArgument])
                                    when xType.IsClrType(numberInfo.ClrTypeName) && yType.IsClrType(numberInfo.ClrTypeName):

                                    AnalyzeMax(consumer, element, xArgument, yArgument);
                                    break;
                            }
                            break;

                        case "Min": // todo: nameof(INumber<T>.Min) when available
                            switch (method.Parameters, element.TryGetArgumentsInDeclarationOrder())
                            {
                                case ([{ Type: var xType }, { Type: var yType }], [{ } xArgument, { } yArgument])
                                    when xType.IsClrType(numberInfo.ClrTypeName) && yType.IsClrType(numberInfo.ClrTypeName):

                                    AnalyzeMin(consumer, element, xArgument, yArgument);
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
                    switch (method.Parameters, element.TryGetArgumentsInDeclarationOrder())
                    {
                        case ([{ Type: var valueType }, { Type: var minType }, { Type: var maxType }], [
                                { } valueArgument, { } minArgument, { } maxArgument,
                            ]) when valueType.IsClrType(numberInfo.ClrTypeName)
                            && minType.IsClrType(numberInfo.ClrTypeName)
                            && maxType.IsClrType(numberInfo.ClrTypeName):

                            AnalyzeClamp(consumer, element, valueArgument, minArgument, maxArgument);
                            break;
                    }
                    break;

                case nameof(Math.Max):
                    switch (method.Parameters, element.TryGetArgumentsInDeclarationOrder())
                    {
                        case ([{ Type: var val1Type }, { Type: var val2Type }], [{ } val1Argument, { } val2Argument])
                            when val1Type.IsClrType(numberInfo.ClrTypeName) && val2Type.IsClrType(numberInfo.ClrTypeName):

                            AnalyzeMax(consumer, element, val1Argument, val2Argument);
                            break;
                    }
                    break;

                case nameof(Math.Min):
                    switch (method.Parameters, element.TryGetArgumentsInDeclarationOrder())
                    {
                        case ([{ Type: var val1Type }, { Type: var val2Type }], [{ } val1Argument, { } val2Argument])
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