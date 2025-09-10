using JetBrains.ReSharper.Feature.Services.Daemon;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.CSharp.Tree;
using ReCommendedExtension.Extensions.MethodFinding;

namespace ReCommendedExtension.Analyzers.BaseTypes.Analyzers;

public abstract class NumberAnalyzer : ElementProblemAnalyzer<IInvocationExpression>
{
    [SuppressMessage("ReSharper", "InconsistentNaming", Justification = "Underscore character used intentionally as a separator.")]
    private protected static class ParameterTypes
    {
        public static IReadOnlyList<ParameterType> String { get; } = [new() { ClrTypeName = PredefinedType.STRING_FQN }];

        public static IReadOnlyList<ParameterType> ReadOnlySpanOfT { get; } =
        [
            new GenericParameterType { ClrTypeName = PredefinedType.SYSTEM_READ_ONLY_SPAN_FQN },
        ];

        public static IReadOnlyList<ParameterType> Int32 { get; } = [new() { ClrTypeName = PredefinedType.INT_FQN }];

        public static IReadOnlyList<ParameterType> IFormatProvider { get; } = [new() { ClrTypeName = PredefinedType.IFORMATPROVIDER_FQN }];

        public static IReadOnlyList<ParameterType> MidpointRounding { get; } = [new() { ClrTypeName = ClrTypeNames.MidpointRounding }];

        public static IReadOnlyList<ParameterType> String_IFormatProvider { get; } =
        [
            new() { ClrTypeName = PredefinedType.STRING_FQN }, new() { ClrTypeName = PredefinedType.IFORMATPROVIDER_FQN },
        ];

        public static IReadOnlyList<ParameterType> String_NumberStyles { get; } =
        [
            new() { ClrTypeName = PredefinedType.STRING_FQN }, new() { ClrTypeName = ClrTypeNames.NumberStyles },
        ];

        public static IReadOnlyList<ParameterType> ReadOnlySpanOfT_IFormatProvider { get; } =
        [
            new GenericParameterType { ClrTypeName = PredefinedType.SYSTEM_READ_ONLY_SPAN_FQN },
            new() { ClrTypeName = PredefinedType.IFORMATPROVIDER_FQN },
        ];

        public static IReadOnlyList<ParameterType> ReadOnlySpanOfT_NumberStyles_IFormatProvider { get; } =
        [
            new GenericParameterType { ClrTypeName = PredefinedType.SYSTEM_READ_ONLY_SPAN_FQN },
            new() { ClrTypeName = ClrTypeNames.NumberStyles },
            new() { ClrTypeName = PredefinedType.IFORMATPROVIDER_FQN },
        ];
    }
}