namespace ReCommendedExtension.Extensions.NumberInfos;

[Flags]
public enum FormatSpecifiers
{
    GeneralCaseInsensitiveWithoutPrecision = 1 << 0,
    GeneralZeroPrecisionRedundant = 1 << 1,
    Binary = 1 << 2,
    Hexadecimal = 1 << 3,
    Decimal = 1 << 4,
    RoundtripToBeReplaced = 1 << 5,
    RoundtripPrecisionRedundant = 1 << 6,
}