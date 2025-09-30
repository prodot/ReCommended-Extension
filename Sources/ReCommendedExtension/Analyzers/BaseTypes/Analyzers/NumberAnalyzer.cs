using JetBrains.ReSharper.Feature.Services.Daemon;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.CSharp.Tree;
using ReCommendedExtension.Extensions;
using ReCommendedExtension.Extensions.MethodFinding;

namespace ReCommendedExtension.Analyzers.BaseTypes.Analyzers;

public abstract class NumberAnalyzer : ElementProblemAnalyzer<IInvocationExpression>
{
    [SuppressMessage("ReSharper", "InconsistentNaming", Justification = "Underscore character used intentionally as a separator.")]
    private protected static class Parameters
    {
        public static IReadOnlyList<Parameter> String { get; } = [new(t => t.IsString())];

        public static IReadOnlyList<Parameter> ReadOnlySpanOfChar { get; } = [new(t => t.IsReadOnlySpanOfChar())];

        public static IReadOnlyList<Parameter> ReadOnlySpanOfByte { get; } = [new(t => t.IsReadOnlySpanOfByte())];

        public static IReadOnlyList<Parameter> Int32 { get; } = [new(t => t.IsInt())];

        public static IReadOnlyList<Parameter> MidpointRounding { get; } = [new(t => t.IsMidpointRounding())];

        public static IReadOnlyList<Parameter> String_IFormatProvider { get; } = [new(t => t.IsString()), new(t => t.IsIFormatProvider())];

        public static IReadOnlyList<Parameter> String_NumberStyles { get; } = [new(t => t.IsString()), new(t => t.IsNumberStyles())];

        public static IReadOnlyList<Parameter> ReadOnlySpanOfChar_IFormatProvider { get; } =
        [
            new(t => t.IsReadOnlySpanOfChar()), new(t => t.IsIFormatProvider()),
        ];

        public static IReadOnlyList<Parameter> ReadOnlySpanOfByte_IFormatProvider { get; } =
        [
            new(t => t.IsReadOnlySpanOfByte()), new(t => t.IsIFormatProvider()),
        ];

        public static IReadOnlyList<Parameter> ReadOnlySpanOfChar_NumberStyles_IFormatProvider { get; } =
        [
            new(t => t.IsReadOnlySpanOfChar()), new(t => t.IsNumberStyles()), new(t => t.IsIFormatProvider()),
        ];

        public static IReadOnlyList<Parameter> ReadOnlySpanOfByte_NumberStyles_IFormatProvider { get; } =
        [
            new(t => t.IsReadOnlySpanOfByte()), new(t => t.IsNumberStyles()), new(t => t.IsIFormatProvider()),
        ];
    }
}