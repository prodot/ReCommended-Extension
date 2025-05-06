using System.Globalization;
using JetBrains.Metadata.Reader.API;
using JetBrains.ReSharper.Feature.Services.Daemon;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.CSharp;
using JetBrains.ReSharper.Psi.CSharp.Tree;
using ReCommendedExtension.Extensions;

namespace ReCommendedExtension.Analyzers.BaseTypes.Analyzers;

public abstract class FloatingPointNumberAnalyzer<N>(IClrTypeName clrTypeName) : FractionalNumberAnalyzer<N>(clrTypeName) where N : struct
{
    /// <remarks>
    /// <c>T.IsNaN(value)</c> → <c>value is T.NaN</c> (C# 9)
    /// </remarks>
    void AnalyzeIsNaN(IHighlightingConsumer consumer, IInvocationExpression invocationExpression, ICSharpArgument valueArgument)
    {
        if (invocationExpression.GetLanguageVersion() >= CSharpLanguageLevel.CSharp90
            && TryGetNanConstant() is { } nanConstant
            && !invocationExpression.IsUsedAsStatement()
            && valueArgument.Value is { })
        {
            consumer.AddHighlighting(new UseFloatingPointPatternSuggestion("Use pattern.", invocationExpression, valueArgument.Value, nanConstant));
        }
    }

    private protected sealed override bool CanUseEqualityOperator() => false; // can only be checked by comparing literals

    private protected sealed override bool AreEqual(N x, N y) => false; // can only be checked by comparing literals

    private protected sealed override bool AreMinMaxValues(N min, N max) => false; // can only be checked by comparing literals

    private protected sealed override NumberStyles GetDefaultNumberStyles() => NumberStyles.Float | NumberStyles.AllowThousands;

    private protected sealed override int? TryGetMaxValueStringLength() => null;

    private protected sealed override bool SupportsCaseInsensitiveGeneralFormatSpecifierWithoutPrecision() => false;

    [Pure]
    private protected abstract string? TryGetNanConstant();

    private protected override void Analyze(
        IInvocationExpression element,
        IReferenceExpression invokedExpression,
        IMethod method,
        IHighlightingConsumer consumer)
    {
        base.Analyze(element, invokedExpression, method, consumer);

        if (method.ContainingType.IsClrType(ClrTypeName) && method.IsStatic)
        {
            switch (method.ShortName)
            {
                case "IsNaN": // todo: nameof(INumberBase<T>.IsNaN) when available
                    switch (method.Parameters, element.Arguments)
                    {
                        case ([{ Type: var valueType }], [var valueArgument]) when valueType.IsClrType(ClrTypeName):
                            AnalyzeIsNaN(consumer, element, valueArgument);
                            break;
                    }
                    break;
            }
        }
    }
}