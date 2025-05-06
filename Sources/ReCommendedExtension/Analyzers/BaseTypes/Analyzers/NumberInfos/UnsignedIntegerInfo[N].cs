using JetBrains.Metadata.Reader.API;

namespace ReCommendedExtension.Analyzers.BaseTypes.Analyzers.NumberInfos;

public abstract class UnsignedIntegerInfo<N>(IClrTypeName clrTypeName) : IntegerInfo<N>(clrTypeName) where N : struct;