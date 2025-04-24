using JetBrains.Metadata.Reader.API;

namespace ReCommendedExtension.Analyzers.BaseTypes.Analyzers;

public abstract class NativeIntegerAnalyzer<N>(IClrTypeName clrTypeName) : IntegerAnalyzer<N>(clrTypeName) where N : struct
{
    private protected sealed override TypeCode? TryGetTypeCode() => null;

    private protected sealed override bool AreMinMaxValues(N min, N max) => false; // N.MinValue and N.MaxValue are platform-dependent
}