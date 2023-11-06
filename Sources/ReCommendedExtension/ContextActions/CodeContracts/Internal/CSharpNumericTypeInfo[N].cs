using JetBrains.ReSharper.Psi;

namespace ReCommendedExtension.ContextActions.CodeContracts.Internal;

internal sealed record CSharpNumericTypeInfo<N> : CSharpNumericTypeInfo where N : struct
{
    readonly N one;
    readonly Func<N, N, bool> isLessOrEquals;
    readonly Func<N, bool> isZero;
    readonly Func<N, N>? getNext;
    readonly Func<N, N> getMultipliedWithTwo;
    readonly Func<N, double> convertToDouble;
    readonly Func<ConstantValue, N> extractConstantValue;

    public CSharpNumericTypeInfo(
        bool isSigned,
        N one,
        string literalSuffix,
        string? epsilonLiteral,
        Func<N, N, bool> isLessOrEquals,
        Func<N, bool> isZero,
        Func<N, N>? getNext,
        Func<N, N> getMultipliedWithTwo,
        Func<N, double> convertToDouble,
        Func<ConstantValue, N> extractConstantValue) : base(isSigned, epsilonLiteral, literalSuffix)
    {
        this.one = one;
        this.isLessOrEquals = isLessOrEquals;
        this.isZero = isZero;
        this.getNext = getNext;
        this.getMultipliedWithTwo = getMultipliedWithTwo;
        this.convertToDouble = convertToDouble;
        this.extractConstantValue = extractConstantValue;
    }

    public override EnumBetweenFirstAndLast.EnumContractInfo? TryCreateEnumContractInfoForEnumBetweenFirstAndLast(IList<IField> members)
    {
        Debug.Assert(getNext is { });

        return EnumBetweenFirstAndLast.EnumContractInfo<N>.TryCreate(members, isLessOrEquals, getNext, extractConstantValue);
    }

    public override EnumFlags.EnumContractInfo? TryCreateEnumFlags(IList<IField> members)
        => EnumFlags.EnumContractInfo<N>.TryCreate(
            members,
            one,
            convertToDouble,
            isZero,
            isLessOrEquals,
            getMultipliedWithTwo,
            LiteralSuffix,
            extractConstantValue);
}