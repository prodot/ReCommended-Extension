using JetBrains.ReSharper.Psi.CSharp.Tree;
using ReCommendedExtension.Extensions;

namespace ReCommendedExtension.Analyzers.BaseTypes.NumberInfos;

public sealed record NumberInfo<N> : NumberInfo where N : struct
{
    public required TryGetConstant<N> TryGetConstant { get; init; }

    public required Func<N, N, bool> AreEqual { get; init; }

    public required Func<N, N, bool> AreMinMaxValues { get; init; }

    internal Func<N, bool>? IsZero { get; init; }

    [Pure]
    internal string GetReplacementFromArgument(IInvocationExpression invocationExpression, ICSharpExpression argumentValue)
    {
        if (invocationExpression.TryGetTargetType(false).IsClrType(ClrTypeName) || argumentValue.Type().IsClrType(ClrTypeName))
        {
            return argumentValue.GetText();
        }

        if (TryGetConstant(argumentValue, out var valueImplicitlyConverted) is { } && valueImplicitlyConverted)
        {
            Debug.Assert(CastConstant is { });

            return CastConstant(argumentValue, valueImplicitlyConverted);
        }

        Debug.Assert(Cast is { });

        return Cast(argumentValue);
    }
}