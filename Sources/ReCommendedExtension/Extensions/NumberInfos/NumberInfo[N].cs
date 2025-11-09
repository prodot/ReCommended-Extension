using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.CSharp.Tree;

namespace ReCommendedExtension.Extensions.NumberInfos;

public sealed record NumberInfo<N> : NumberInfo where N : struct
{
    public required TryGetConstant<N> TryGetConstant { get; init; }

    internal override string GetReplacementFromArgument(IType? targetType, ICSharpExpression argumentValue)
    {
        if (targetType.IsClrType(ClrTypeName) || argumentValue.Type().IsClrType(ClrTypeName))
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