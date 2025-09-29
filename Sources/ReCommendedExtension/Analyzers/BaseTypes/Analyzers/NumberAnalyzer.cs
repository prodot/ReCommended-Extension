using JetBrains.ReSharper.Feature.Services.Daemon;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.CSharp.Tree;
using ReCommendedExtension.Extensions;

namespace ReCommendedExtension.Analyzers.BaseTypes.Analyzers;

public abstract class NumberAnalyzer : ElementProblemAnalyzer<IInvocationExpression>
{
    [SuppressMessage("ReSharper", "InconsistentNaming", Justification = "Underscore character used intentionally as a separator.")]
    private protected static class ParameterTypes
    {
        public static IReadOnlyList<Func<IType, bool>> String { get; } = [t => t.IsString()];

        public static IReadOnlyList<Func<IType, bool>> ReadOnlySpanOfChar { get; } = [t => t.IsReadOnlySpanOfChar()];

        public static IReadOnlyList<Func<IType, bool>> ReadOnlySpanOfByte { get; } = [t => t.IsReadOnlySpanOfByte()];

        public static IReadOnlyList<Func<IType, bool>> Int32 { get; } = [t => t.IsInt()];

        public static IReadOnlyList<Func<IType, bool>> MidpointRounding { get; } = [t => t.IsMidpointRounding()];

        public static IReadOnlyList<Func<IType, bool>> String_IFormatProvider { get; } = [t => t.IsString(), t => t.IsIFormatProvider()];

        public static IReadOnlyList<Func<IType, bool>> String_NumberStyles { get; } = [t => t.IsString(), t => t.IsNumberStyles()];

        public static IReadOnlyList<Func<IType, bool>> ReadOnlySpanOfChar_IFormatProvider { get; } =
        [
            t => t.IsReadOnlySpanOfChar(), t => t.IsIFormatProvider(),
        ];

        public static IReadOnlyList<Func<IType, bool>> ReadOnlySpanOfByte_IFormatProvider { get; } =
        [
            t => t.IsReadOnlySpanOfByte(), t => t.IsIFormatProvider(),
        ];

        public static IReadOnlyList<Func<IType, bool>> ReadOnlySpanOfChar_NumberStyles_IFormatProvider { get; } =
        [
            t => t.IsReadOnlySpanOfChar(), t => t.IsNumberStyles(), t => t.IsIFormatProvider(),
        ];

        public static IReadOnlyList<Func<IType, bool>> ReadOnlySpanOfByte_NumberStyles_IFormatProvider { get; } =
        [
            t => t.IsReadOnlySpanOfByte(), t => t.IsNumberStyles(), t => t.IsIFormatProvider(),
        ];
    }
}