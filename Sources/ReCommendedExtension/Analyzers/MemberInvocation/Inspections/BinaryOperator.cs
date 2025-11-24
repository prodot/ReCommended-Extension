using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.CSharp.Tree;
using JetBrains.ReSharper.Psi.Tree;
using JetBrains.ReSharper.Psi.Util;
using ReCommendedExtension.Analyzers.MemberInvocation.Rules;
using ReCommendedExtension.Extensions;
using ReCommendedExtension.Extensions.NumberInfos;

namespace ReCommendedExtension.Analyzers.MemberInvocation.Inspections;

internal sealed record BinaryOperator : Inspection
{
    public static BinaryOperator ArgumentZero { get; } = new()
    {
        TryGetOperands = (_, args) => args is [{ Value: { } value }] ? new BinaryOperatorOperands { Left = value.GetText(), Right = "0" } : null,
        Message = op => $"Use the '{op}' operator.",
    };

    public static BinaryOperator QualifierArgument { get; } = new()
    {
        TryGetOperands = (qualifier, args) => args is [{ Value: { } value }]
            ? new BinaryOperatorOperands { Left = qualifier.GetText(), Right = value.GetText() }
            : null,
        Message = op => $"Use the '{op}' operator.",
    };

    public static BinaryOperator QualifierArgumentNonBooleanConstants { get; } = new()
    {
        TryGetOperands = (qualifier, args)
            => qualifier.AsBooleanConstant == null && args is [{ Value: { AsBooleanConstant: null } value }]
                ? new BinaryOperatorOperands { Left = qualifier.GetText(), Right = value.GetText() }
                : null,
        Message = op => $"Use the '{op}' operator.",
    };

    public static BinaryOperator NullableQualifierDefault { get; } = new()
    {
        TryGetOperands = (qualifier, _)
            => new BinaryOperatorOperands
            {
                Left = qualifier.GetText(), Right = qualifier.Type().Unlift().TryGetDefaultValueLiteral(qualifier) ?? "default",
            },
        Message = op => $"Use the '{op}' operator.",
    };

    public static BinaryOperator Arguments { get; } = new()
    {
        TryGetOperands = (_, args) => args is [{ Value: { } left }, { Value: { } right }]
            ? new BinaryOperatorOperands { Left = left.GetText(), Right = right.GetText() }
            : null,
        Message = op => $"Use the '{op}' operator.",
    };

    public static BinaryOperator ArgumentsDecimal { get; } = new()
    {
        TryGetOperands = (_, args) =>
        {
            if (args is [{ Value: { } left }, { Value: { } right }])
            {
                if (left.Type().IsDecimal() || right.Type().IsDecimal())
                {
                    return new BinaryOperatorOperands { Left = left.GetText(), Right = right.GetText() };
                }

                if (NumberInfo.Decimal.TryGetConstant(left, out var leftImplicitlyConverted) is { } && leftImplicitlyConverted)
                {
                    Debug.Assert(NumberInfo.Decimal.CastConstant is { });

                    return new BinaryOperatorOperands { Left = NumberInfo.Decimal.CastConstant(left, true), Right = right.GetText() };
                }

                if (NumberInfo.Decimal.TryGetConstant(right, out var rightImplicitlyConverted) is { } && rightImplicitlyConverted)
                {
                    Debug.Assert(NumberInfo.Decimal.CastConstant is { });

                    return new BinaryOperatorOperands { Left = left.GetText(), Right = NumberInfo.Decimal.CastConstant(right, true) };
                }

                Debug.Assert(NumberInfo.Decimal.Cast is { });

                return new BinaryOperatorOperands { Left = NumberInfo.Decimal.Cast(left), Right = right.GetText() };
            }

            return null;
        },
        Message = op => $"Use the '{op}' operator.",
    };

    public required Func<ICSharpExpression, TreeNodeCollection<ICSharpArgument?>, BinaryOperatorOperands?> TryGetOperands
    {
        get;
        init;
    }

    public new string? Operator { get; init; }

    public bool HighlightInvokedMethodOnly { get; init; }
}