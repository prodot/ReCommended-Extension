using ReCommendedExtension.Analyzers.BaseTypes.NumberInfos;

namespace ReCommendedExtension.Analyzers.BaseTypes.Analyzers;

public abstract class FractionalNumberAnalyzer<N>(NumberInfo<N> numberInfo) : NumberAnalyzer<N>(numberInfo) where N : struct;