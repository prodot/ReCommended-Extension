using System.Globalization;
using JetBrains.Metadata.Reader.API;
using JetBrains.ReSharper.Psi.CSharp.Tree;

namespace ReCommendedExtension.Analyzers.BaseTypes.Analyzers.NumberInfos;

public abstract class FloatingPointNumberInfo<N>(IClrTypeName clrTypeName) : FractionalNumberInfo<N>(clrTypeName) where N : struct
{
    internal sealed override NumberStyles DefaultNumberStyles => NumberStyles.Float | NumberStyles.AllowThousands;

    internal sealed override bool CanUseEqualityOperator => false; // can only be checked by comparing literals

    internal sealed override int? MaxValueStringLength => null;

    internal sealed override bool SupportsCaseInsensitiveGeneralFormatSpecifierWithoutPrecision => false;

    internal abstract string? NanConstant { get; }

    internal sealed override string CastConstant(ICSharpExpression constant, bool implicitlyConverted) => throw new NotSupportedException();

    internal sealed override bool AreEqual(N x, N y) => false; // can only be checked by comparing literals

    internal sealed override bool AreMinMaxValues(N min, N max) => false; // can only be checked by comparing literals
}