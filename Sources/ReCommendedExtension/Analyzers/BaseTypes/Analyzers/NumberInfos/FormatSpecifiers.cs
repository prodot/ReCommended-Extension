namespace ReCommendedExtension.Analyzers.BaseTypes.Analyzers.NumberInfos;

[Flags]
public enum FormatSpecifiers
{
    GeneralCaseInsensitiveWithoutPrecision = 1 << 0,
    Binary = 1 << 1,
    Hexadecimal = 1 << 2,
    Decimal = 1 << 3,
    RoundtripToBeReplaced = 1 << 4,
    RoundtripPrecisionRedundant = 1 << 5,
}