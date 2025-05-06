using System.Globalization;
using JetBrains.Metadata.Reader.API;
using JetBrains.ReSharper.Psi.CSharp;

namespace ReCommendedExtension.Analyzers.BaseTypes.Analyzers.NumberInfos;

public abstract class IntegerInfo<N>(IClrTypeName clrTypeName) : NumberInfo<N>(clrTypeName) where N : struct
{
    internal sealed override NumberStyles DefaultNumberStyles => NumberStyles.Integer;

    internal sealed override bool CanUseEqualityOperator => true;

    internal sealed override bool SupportsCaseInsensitiveGeneralFormatSpecifierWithoutPrecision => true;

    internal sealed override bool SupportsBinaryOrHexFormatSpecifier => true;

    internal sealed override bool SupportsDecimalFormatSpecifier => true;

    internal sealed override RoundTripFormatSpecifierSupport GetRoundTripFormatSpecifier(string precisionSpecifier, out string? replacement)
    {
        replacement = null;
        return RoundTripFormatSpecifierSupport.Unsupported;
    }

    [Pure]
    internal abstract string CastZero(CSharpLanguageLevel languageLevel);

    [Pure]
    internal abstract bool IsZero(N value);
}