using ReCommendedExtension.Analyzers.BaseTypes.Analyzers.NumberInfos;

namespace ReCommendedExtension.Analyzers.BaseTypes.Analyzers;

public abstract class UnsignedIntegerAnalyzer<N>(NumberInfo<N> numberInfo) : IntegerAnalyzer<N>(numberInfo) where N : struct;