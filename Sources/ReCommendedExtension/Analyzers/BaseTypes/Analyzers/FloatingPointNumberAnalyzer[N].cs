using JetBrains.ReSharper.Feature.Services.Daemon;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.CSharp;
using JetBrains.ReSharper.Psi.CSharp.Tree;
using ReCommendedExtension.Analyzers.BaseTypes.NumberInfos;
using ReCommendedExtension.Extensions;

namespace ReCommendedExtension.Analyzers.BaseTypes.Analyzers;

public abstract class FloatingPointNumberAnalyzer<N>(NumberInfo<N> numberInfo) : FractionalNumberAnalyzer<N>(numberInfo) where N : struct
{
    /// <remarks>
    /// <c>T.IsNaN(value)</c> → <c>value is T.NaN</c> (C# 9)
    /// </remarks>
    void AnalyzeIsNaN(IHighlightingConsumer consumer, IInvocationExpression invocationExpression, ICSharpArgument valueArgument)
    {
        if (invocationExpression.GetLanguageVersion() >= CSharpLanguageLevel.CSharp90
            && NumberInfo.NanConstant is { } nanConstant
            && !invocationExpression.IsUsedAsStatement()
            && valueArgument.Value is { })
        {
            consumer.AddHighlighting(new UseFloatingPointPatternSuggestion("Use pattern.", invocationExpression, valueArgument.Value, nanConstant));
        }
    }

    private protected override void Analyze(
        IInvocationExpression element,
        IReferenceExpression invokedExpression,
        IMethod method,
        IHighlightingConsumer consumer)
    {
        base.Analyze(element, invokedExpression, method, consumer);

        if (method.ContainingType.IsClrType(NumberInfo.ClrTypeName) && method.IsStatic)
        {
            switch (method.ShortName)
            {
                case "IsNaN": // todo: nameof(INumberBase<T>.IsNaN) when available
                    switch (method.Parameters, element.TryGetArgumentsInDeclarationOrder())
                    {
                        case ([{ Type: var valueType }], [{ } valueArgument]) when valueType.IsClrType(NumberInfo.ClrTypeName):
                            AnalyzeIsNaN(consumer, element, valueArgument);
                            break;
                    }
                    break;
            }
        }
    }
}