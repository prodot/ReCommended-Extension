using System.Globalization;
using JetBrains.Metadata.Reader.API;
using JetBrains.ReSharper.Feature.Services.Daemon;
using JetBrains.ReSharper.Psi.CSharp.Tree;

namespace ReCommendedExtension.Analyzers.BaseTypes.Analyzers;

public abstract class FloatingPointNumberAnalyzer<N>(IClrTypeName clrTypeName) : NumberAnalyzer<N>(clrTypeName) where N : struct
{
    private protected sealed override void AnalyzeEquals_Number(
        IHighlightingConsumer consumer,
        IInvocationExpression invocationExpression,
        IReferenceExpression invokedExpression,
        ICSharpArgument objArgument)
    {
        // avoid the "==" operator
    }

    private protected sealed override bool AreEqual(N x, N y) => false; // can only be checked by comparing literals

    private protected sealed override bool AreMinMaxValues(N min, N max) => false; // can only be checked by comparing literals

    private protected sealed override NumberStyles GetDefaultNumberStyles() => NumberStyles.Float | NumberStyles.AllowThousands;
}