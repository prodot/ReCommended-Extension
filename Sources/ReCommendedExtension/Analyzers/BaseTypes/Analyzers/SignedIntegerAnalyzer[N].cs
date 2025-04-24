using JetBrains.Metadata.Reader.API;

namespace ReCommendedExtension.Analyzers.BaseTypes.Analyzers;

public abstract class SignedIntegerAnalyzer<N>(IClrTypeName clrTypeName) : IntegerAnalyzer<N>(clrTypeName) where N : struct
{
    private protected sealed override bool CanUseComparisonOperatorWithZero() => true;
}