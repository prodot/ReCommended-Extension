using JetBrains.Metadata.Reader.API;

namespace ReCommendedExtension.Analyzers.BaseTypes.Analyzers.NumberInfos;

public abstract class FractionalNumberInfo<N>(IClrTypeName clrTypeName) : NumberInfo<N>(clrTypeName) where N : struct
{
    internal sealed override bool SupportsBinaryOrHexFormatSpecifier => false;

    internal sealed override bool SupportsDecimalFormatSpecifier => false;
}