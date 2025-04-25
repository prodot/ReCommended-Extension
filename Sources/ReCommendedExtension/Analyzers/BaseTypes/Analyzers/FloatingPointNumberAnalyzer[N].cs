using System.Globalization;
using JetBrains.Metadata.Reader.API;

namespace ReCommendedExtension.Analyzers.BaseTypes.Analyzers;

public abstract class FloatingPointNumberAnalyzer<N>(IClrTypeName clrTypeName) : NumberAnalyzer<N>(clrTypeName) where N : struct
{
    private protected sealed override bool CanUseEqualityOperator() => false; // can only be checked by comparing literals

    private protected sealed override bool AreEqual(N x, N y) => false; // can only be checked by comparing literals

    private protected sealed override bool AreMinMaxValues(N min, N max) => false; // can only be checked by comparing literals

    private protected sealed override NumberStyles GetDefaultNumberStyles() => NumberStyles.Float | NumberStyles.AllowThousands;
}