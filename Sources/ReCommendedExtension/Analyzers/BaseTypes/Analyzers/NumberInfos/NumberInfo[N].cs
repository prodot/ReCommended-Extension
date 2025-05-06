using System.Globalization;
using JetBrains.Metadata.Reader.API;
using JetBrains.ReSharper.Psi.CSharp.Tree;

namespace ReCommendedExtension.Analyzers.BaseTypes.Analyzers.NumberInfos;

public abstract class NumberInfo<N>(IClrTypeName clrTypeName) : NumberInfo where N : struct
{
    internal IClrTypeName ClrTypeName => clrTypeName;

    internal abstract TypeCode? TypeCode { get; }

    internal abstract NumberStyles DefaultNumberStyles { get; }

    internal abstract bool CanUseEqualityOperator { get; }

    internal abstract bool SupportsCaseInsensitiveGeneralFormatSpecifierWithoutPrecision { get; }

    internal abstract bool SupportsBinaryOrHexFormatSpecifier { get; }

    internal abstract bool SupportsDecimalFormatSpecifier { get; }

    [Pure]
    internal abstract N? TryGetConstant(ICSharpExpression? expression, out bool implicitlyConverted);

    [Pure]
    internal abstract string CastConstant(ICSharpExpression constant, bool implicitlyConverted);

    [Pure]
    internal abstract string Cast(ICSharpExpression expression);

    [Pure]
    internal abstract bool AreEqual(N x, N y);

    [Pure]
    internal abstract bool AreMinMaxValues(N min, N max);

    [Pure]
    internal abstract RoundTripFormatSpecifierSupport GetRoundTripFormatSpecifier(string precisionSpecifier, out string? replacement);
}