using JetBrains.Metadata.Reader.API;
using JetBrains.ReSharper.Feature.Services.Daemon;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.CSharp.Tree;
using ReCommendedExtension.Extensions;

namespace ReCommendedExtension.Analyzers.BaseTypes.Analyzers;

public abstract class NumberAnalyzer<N>(IClrTypeName clrTypeName) : ElementProblemAnalyzer<IInvocationExpression> where N : struct
{
    private protected IClrTypeName ClrTypeName => clrTypeName;

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
            && TryGetConstant(minArgument.Value, out var minImplicitlyConverted) is { } min
            && TryGetConstant(maxArgument.Value, out var maxImplicitlyConverted) is { } max)
        {
            if (AreEqual(min, max))
            {
                Debug.Assert(minArgument.Value is { });
                Debug.Assert(maxArgument.Value is { });

                var (replacementMin, replacementMax) = invocationExpression.TryGetTargetType().IsClrType(clrTypeName)
                    ? (minArgument.Value.GetText(), maxArgument.Value.GetText())
                    : (CastConstant(minArgument.Value, minImplicitlyConverted), CastConstant(maxArgument.Value, maxImplicitlyConverted));

                consumer.AddHighlighting(
                    new UseExpressionResultSuggestion(
                        $"The expression is always {min}.",
                        invocationExpression,
                        replacementMin,
                        replacementMax != replacementMin ? replacementMax : null));
            }

            if (AreMinMaxValues(min, max) && valueArgument.Value is { } value)
            {
                var replacement = TryGetConstant(value, out var valueImplicitlyConverted) is { }
                    && !invocationExpression.TryGetTargetType().IsClrType(clrTypeName)
                        ? CastConstant(value, valueImplicitlyConverted)
                        : value.GetText();

                consumer.AddHighlighting(
                    new UseExpressionResultSuggestion("The expression is always the same as the first argument.", invocationExpression, replacement));
            }
        }
    }

    /// <remarks>
    /// <c>number.Equals(obj)</c> → <c>number == obj</c>
    /// </remarks>
    private protected virtual void AnalyzeEquals_Number(
        IHighlightingConsumer consumer,
        IInvocationExpression invocationExpression,
        IReferenceExpression invokedExpression,
        ICSharpArgument objArgument)
    {
        Debug.Assert(invokedExpression.QualifierExpression is { });

        if (!invocationExpression.IsUsedAsStatement() && objArgument.Value is { })
        {
            consumer.AddHighlighting(
                new UseBinaryOperationSuggestion(
                    "Use the '==' operator.",
                    invocationExpression,
                    "==",
                    invokedExpression.QualifierExpression,
                    objArgument.Value));
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
        if (!invocationExpression.IsUsedAsStatement() && TryGetTypeCode() is { } typeCode)
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
            && TryGetConstant(xArgument.Value, out var xImplicitlyConverted) is { } x
            && TryGetConstant(yArgument.Value, out var yImplicitlyConverted) is { } y
            && AreEqual(x, y))
        {
            Debug.Assert(xArgument.Value is { });
            Debug.Assert(yArgument.Value is { });

            var (replacementX, replacementY) = invocationExpression.TryGetTargetType().IsClrType(clrTypeName)
                ? (xArgument.Value.GetText(), yArgument.Value.GetText())
                : (CastConstant(xArgument.Value, xImplicitlyConverted), CastConstant(yArgument.Value, yImplicitlyConverted));

            consumer.AddHighlighting(
                new UseExpressionResultSuggestion(
                    $"The expression is always {x}.",
                    invocationExpression,
                    replacementX,
                    replacementY != replacementX ? replacementY : null));
        }
    }

    /// <remarks>
    /// <c>T.Max(n, n)</c> → <c>n</c>
    /// </remarks>
    void AnalyzeMin(IHighlightingConsumer consumer, IInvocationExpression invocationExpression, ICSharpArgument xArgument, ICSharpArgument yArgument)
    {
        if (!invocationExpression.IsUsedAsStatement()
            && TryGetConstant(xArgument.Value, out var xImplicitlyConverted) is { } x
            && TryGetConstant(yArgument.Value, out var yImplicitlyConverted) is { } y
            && AreEqual(x, y))
        {
            Debug.Assert(xArgument.Value is { });
            Debug.Assert(yArgument.Value is { });

            var (replacementX, replacementY) = invocationExpression.TryGetTargetType().IsClrType(clrTypeName)
                ? (xArgument.Value.GetText(), yArgument.Value.GetText())
                : (CastConstant(xArgument.Value, xImplicitlyConverted), CastConstant(yArgument.Value, yImplicitlyConverted));

            consumer.AddHighlighting(
                new UseExpressionResultSuggestion(
                    $"The expression is always {x}.",
                    invocationExpression,
                    replacementX,
                    replacementY != replacementX ? replacementY : null));
        }
    }

    [Pure]
    private protected abstract TypeCode? TryGetTypeCode();

    [Pure]
    private protected abstract N? TryGetConstant(ICSharpExpression? expression, out bool implicitlyConverted);

    [Pure]
    private protected abstract string CastConstant(ICSharpExpression constant, bool implicitlyConverted);

    [Pure]
    private protected abstract bool AreEqual(N x, N y);

    [Pure]
    private protected abstract bool AreMinMaxValues(N min, N max);

    private protected virtual void Analyze(
        IInvocationExpression element,
        IReferenceExpression invokedExpression,
        IMethod method,
        IHighlightingConsumer consumer)
    {
        if (method.ContainingType.IsClrType(clrTypeName))
        {
            switch (invokedExpression, method)
            {
                case ({ QualifierExpression: { } }, { IsStatic: false }):
                    switch (method.ShortName)
                    {
                        case nameof(Equals):
                            switch (method.Parameters, element.Arguments)
                            {
                                case ([{ Type: var objType }], [var objArgument]) when objType.IsClrType(ClrTypeName):
                                    AnalyzeEquals_Number(consumer, element, invokedExpression, objArgument);
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
                                ]) when valueType.IsClrType(clrTypeName) && minType.IsClrType(clrTypeName) && maxType.IsClrType(clrTypeName):
                                    AnalyzeClamp(consumer, element, valueArgument, minArgument, maxArgument);
                                    break;
                            }
                            break;

                        case "Max": // todo: nameof(INumber<T>.Max) when available
                            switch (method.Parameters, element.Arguments)
                            {
                                case ([{ Type: var xType }, { Type: var yType }], [var xArgument, var yArgument])
                                    when xType.IsClrType(clrTypeName) && yType.IsClrType(clrTypeName):

                                    AnalyzeMax(consumer, element, xArgument, yArgument);
                                    break;
                            }
                            break;

                        case "Min": // todo: nameof(INumber<T>.Min) when available
                            switch (method.Parameters, element.Arguments)
                            {
                                case ([{ Type: var xType }, { Type: var yType }], [var xArgument, var yArgument])
                                    when xType.IsClrType(clrTypeName) && yType.IsClrType(clrTypeName):

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
                    switch (method.Parameters, element.Arguments)
                    {
                        case ([{ Type: var valueType }, { Type: var minType }, { Type: var maxType }], [
                            var valueArgument, var minArgument, var maxArgument,
                        ]) when valueType.IsClrType(clrTypeName) && minType.IsClrType(clrTypeName) && maxType.IsClrType(clrTypeName):
                            AnalyzeClamp(consumer, element, valueArgument, minArgument, maxArgument);
                            break;
                    }
                    break;

                case nameof(Math.Max):
                    switch (method.Parameters, element.Arguments)
                    {
                        case ([{ Type: var val1Type }, { Type: var val2Type }], [var val1Argument, var val2Argument])
                            when val1Type.IsClrType(clrTypeName) && val2Type.IsClrType(clrTypeName):

                            AnalyzeMax(consumer, element, val1Argument, val2Argument);
                            break;
                    }
                    break;

                case nameof(Math.Min):
                    switch (method.Parameters, element.Arguments)
                    {
                        case ([{ Type: var val1Type }, { Type: var val2Type }], [var val1Argument, var val2Argument])
                            when val1Type.IsClrType(clrTypeName) && val2Type.IsClrType(clrTypeName):

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