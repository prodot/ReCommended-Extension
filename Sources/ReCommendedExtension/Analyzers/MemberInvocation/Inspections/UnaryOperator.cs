using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.CSharp.Tree;
using JetBrains.ReSharper.Psi.Tree;
using ReCommendedExtension.Analyzers.BaseTypes.NumberInfos;

namespace ReCommendedExtension.Analyzers.MemberInvocation.Inspections;

internal sealed record UnaryOperator : Inspection
{
    public static UnaryOperator Qualifier { get; } = new()
    {
        TryGetOperand = (qualifier, _) => qualifier.GetText(), Message = op => $"Use the '{op}' operator.",
    };

    public static UnaryOperator ArgumentDecimal { get; } = new()
    {
        TryGetOperand = (_, args) =>
        {
            Debug.Assert(NumberInfo.Decimal is { CastConstant: { }, Cast: { } });

            return args is [{ Value: { } value }]
                ? value.Type().IsDecimal()
                    ? value.GetText()
                    : NumberInfo.Decimal.TryGetConstant(value, out var implicitlyConverted) is { } && implicitlyConverted
                        ? NumberInfo.Decimal.CastConstant(value, implicitlyConverted)
                        : NumberInfo.Decimal.Cast(value)
                : null;
        },
        Message = op => $"Use the '{op}' operator.",
    };

    public required Func<ICSharpExpression, TreeNodeCollection<ICSharpArgument?>, string?> TryGetOperand { get; init; }

    public new string? Operator { get; init; }
}