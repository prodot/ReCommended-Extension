namespace ReCommendedExtension.Analyzers.BaseTypes.Analyzers.NumberInfos;

internal enum RoundTripFormatSpecifierSupport
{
    Unsupported,
    ToBeReplaced,
    RedundantPrecisionSpecifier,
    Ignore,
}