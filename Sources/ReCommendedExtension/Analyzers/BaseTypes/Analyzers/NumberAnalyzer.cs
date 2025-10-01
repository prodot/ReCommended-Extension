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
        public static IReadOnlyList<Parameter> String { get; } = [Parameter.String];

        public static IReadOnlyList<Parameter> ReadOnlySpanOfChar { get; } = [Parameter.ReadOnlySpanOfChar];

        public static IReadOnlyList<Parameter> ReadOnlySpanOfByte { get; } = [Parameter.ReadOnlySpanOfByte];

        public static IReadOnlyList<Parameter> Int32 { get; } = [Parameter.Int32];

        public static IReadOnlyList<Parameter> MidpointRounding { get; } = [Parameter.MidpointRounding];

        public static IReadOnlyList<Parameter> String_IFormatProvider { get; } = [Parameter.String, Parameter.IFormatProvider];

        public static IReadOnlyList<Parameter> String_NumberStyles { get; } = [Parameter.String, Parameter.NumberStyles];

        public static IReadOnlyList<Parameter> ReadOnlySpanOfChar_IFormatProvider { get; } =
        [
            Parameter.ReadOnlySpanOfChar, Parameter.IFormatProvider,
        ];

        public static IReadOnlyList<Parameter> ReadOnlySpanOfByte_IFormatProvider { get; } =
        [
            Parameter.ReadOnlySpanOfByte, Parameter.IFormatProvider,
        ];

        public static IReadOnlyList<Parameter> ReadOnlySpanOfChar_NumberStyles_IFormatProvider { get; } =
        [
            Parameter.ReadOnlySpanOfChar, Parameter.NumberStyles, Parameter.IFormatProvider,
        ];

        public static IReadOnlyList<Parameter> ReadOnlySpanOfByte_NumberStyles_IFormatProvider { get; } =
        [
            Parameter.ReadOnlySpanOfByte, Parameter.NumberStyles, Parameter.IFormatProvider,
        ];
    }
}