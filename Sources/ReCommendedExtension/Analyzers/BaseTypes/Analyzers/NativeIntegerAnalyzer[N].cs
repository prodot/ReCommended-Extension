using JetBrains.Metadata.Reader.API;
using JetBrains.ReSharper.Feature.Services.Daemon;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.CSharp;
using JetBrains.ReSharper.Psi.CSharp.Tree;

namespace ReCommendedExtension.Analyzers.BaseTypes.Analyzers;

public abstract class NativeIntegerAnalyzer<N>(IClrTypeName clrTypeName) : IntegerAnalyzer<N>(clrTypeName) where N : struct
{
    private protected sealed override TypeCode? TryGetTypeCode() => null;

    private protected sealed override bool AreMinMaxValues(N min, N max) => false; // N.MinValue and N.MaxValue are platform-dependent
}

/// <remarks>
/// C# language version checks are only done when a quick fix would require it.
/// </remarks>
[ElementProblemAnalyzer(
    typeof(IInvocationExpression),
    HighlightingTypes = [typeof(UseExpressionResultSuggestion), typeof(UseBinaryOperationSuggestion)])]
public sealed class IntPtrAnalyzer() : NativeIntegerAnalyzer<nint>(PredefinedType.INTPTR_FQN)
{
    private protected override nint? TryGetConstant(ICSharpExpression? expression, out bool implicitlyConverted)
    {
        if (expression is IConstantValueOwner constantValueOwner)
        {
            switch (constantValueOwner.ConstantValue)
            {
                case { Kind: ConstantValueKind.Nint, IntValue: var value }:
                    implicitlyConverted = false;
                    return value;

                case { Kind: ConstantValueKind.Int, IntValue: var value }:
                    implicitlyConverted = true;
                    return value;

                case { Kind: ConstantValueKind.Byte, ByteValue: var value }:
                    implicitlyConverted = true;
                    return value;

                case { Kind: ConstantValueKind.Sbyte, SbyteValue: var value }:
                    implicitlyConverted = true;
                    return value;

                case { Kind: ConstantValueKind.Short, ShortValue: var value }:
                    implicitlyConverted = true;
                    return value;

                case { Kind: ConstantValueKind.Ushort, UshortValue: var value }:
                    implicitlyConverted = true;
                    return value;

                case { Kind: ConstantValueKind.Char, CharValue: var value }:
                    implicitlyConverted = true;
                    return value;
            }
        }

        implicitlyConverted = false;
        return null;
    }

    private protected override string CastConstant(ICSharpExpression constant, bool implicitlyConverted)
    {
        var result = constant.GetText();

        if (implicitlyConverted)
        {
            return constant.GetCSharpLanguageLevel() >= CSharpLanguageLevel.CSharp90 ? $"(nint){result}" : $"(IntPtr){result}";
        }

        return result;
    }

    private protected override string CastZero() => "(nint)0";

    private protected override bool AreEqual(nint x, nint y) => x == y;

    private protected override bool IsZero(nint value) => value == 0;

    private protected override bool IsOne(nint value) => value == 1;
}

/// <remarks>
/// C# language version checks are only done when a quick fix would require it.
/// </remarks>
[ElementProblemAnalyzer(
    typeof(IInvocationExpression),
    HighlightingTypes = [typeof(UseExpressionResultSuggestion), typeof(UseBinaryOperationSuggestion)])]
public sealed class UIntPtrAnalyzer() : NativeIntegerAnalyzer<nuint>(PredefinedType.UINTPTR_FQN)
{
    private protected override nuint? TryGetConstant(ICSharpExpression? expression, out bool implicitlyConverted)
    {
        if (expression is IConstantValueOwner constantValueOwner)
        {
            switch (constantValueOwner.ConstantValue)
            {
                case { Kind: ConstantValueKind.Nuint, UintValue: var value }:
                    implicitlyConverted = false;
                    return value;

                case { Kind: ConstantValueKind.Int, IntValue: >= 0 and var value }:
                    implicitlyConverted = true;
                    return (nuint)value;

                case { Kind: ConstantValueKind.Uint, UintValue: var value }:
                    implicitlyConverted = true;
                    return value;

                case { Kind: ConstantValueKind.Byte, ByteValue: var value }:
                    implicitlyConverted = true;
                    return value;

                case { Kind: ConstantValueKind.Ushort, UshortValue: var value }:
                    implicitlyConverted = true;
                    return value;

                case { Kind: ConstantValueKind.Char, CharValue: var value }:
                    implicitlyConverted = true;
                    return value;
            }
        }

        implicitlyConverted = false;
        return null;
    }

    private protected override string CastConstant(ICSharpExpression constant, bool implicitlyConverted)
    {
        var result = constant.GetText();

        if (implicitlyConverted)
        {
            return constant.GetCSharpLanguageLevel() >= CSharpLanguageLevel.CSharp90 ? $"(nuint){result}" : $"(UIntPtr){result}";
        }

        return result;
    }

    private protected override string CastZero() => "(nuint)0";

    private protected override bool AreEqual(nuint x, nuint y) => x == y;

    private protected override bool IsZero(nuint value) => value == 0;

    private protected override bool IsOne(nuint value) => value == 1;
}