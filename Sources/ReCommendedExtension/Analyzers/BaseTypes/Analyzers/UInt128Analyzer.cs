using JetBrains.ReSharper.Feature.Services.Daemon;
using JetBrains.ReSharper.Psi.CSharp.Tree;
using ReCommendedExtension.Analyzers.BaseTypes.NumberInfos;

namespace ReCommendedExtension.Analyzers.BaseTypes.Analyzers;

/// <remarks>
/// C# language version checks are only done when a quick fix would require it.
/// </remarks>
[ElementProblemAnalyzer(
    typeof(IInvocationExpression),
    HighlightingTypes =
    [
        typeof(UseExpressionResultSuggestion),
        typeof(UseBinaryOperatorSuggestion),
        typeof(RedundantArgumentHint),
        typeof(SuspiciousFormatSpecifierWarning),
        typeof(RedundantFormatPrecisionSpecifierHint),
    ])]
public sealed class UInt128Analyzer() : UnsignedIntegerAnalyzer<UInt128>(BaseTypes.NumberInfos.NumberInfo.UInt128);