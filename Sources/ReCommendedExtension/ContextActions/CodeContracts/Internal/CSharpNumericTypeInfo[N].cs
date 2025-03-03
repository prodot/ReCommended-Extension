using JetBrains.ReSharper.Psi;

namespace ReCommendedExtension.ContextActions.CodeContracts.Internal;

internal sealed class CSharpNumericTypeInfo<N>(
    bool isSigned,
    N one,
    string literalSuffix,
    string? epsilonLiteral,
    Func<N, N, bool> lessOrEquals,
    Func<N, bool> zero,
    Func<N, N>? next,
    Func<N, N> multipliedWithTwo,
    Func<N, double> toDouble,
    Func<ConstantValue, N> constantValue) : CSharpNumericTypeInfo(isSigned, epsilonLiteral, literalSuffix) where N : struct
{
    public override EnumBetweenFirstAndLast.EnumContractInfo? TryCreateEnumContractInfoForEnumBetweenFirstAndLast(IField[] members)
    {
        Debug.Assert(next is { });

        return EnumBetweenFirstAndLast.EnumContractInfo<N>.TryCreate(members, lessOrEquals, next, constantValue);
    }

    public override EnumFlags.EnumContractInfo? TryCreateEnumFlags(IField[] members)
        => EnumFlags.EnumContractInfo<N>.TryCreate(
            members,
            one,
            toDouble,
            zero,
            lessOrEquals,
            multipliedWithTwo,
            LiteralSuffix,
            constantValue);
}