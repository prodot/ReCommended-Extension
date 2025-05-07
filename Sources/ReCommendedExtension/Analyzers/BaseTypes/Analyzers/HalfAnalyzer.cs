using JetBrains.ReSharper.Feature.Services.Daemon;
using JetBrains.ReSharper.Psi.CSharp.Tree;
using ReCommendedExtension.Analyzers.BaseTypes.Analyzers.NumberInfos;

namespace ReCommendedExtension.Analyzers.BaseTypes.Analyzers;

/// <remarks>
/// C# language version checks are only done when a quick fix would require it.
/// </remarks>
[ElementProblemAnalyzer(
    typeof(IInvocationExpression),
    HighlightingTypes = [typeof(UseExpressionResultSuggestion), typeof(RedundantArgumentHint), typeof(RedundantFormatPrecisionSpecifierHint)])]
public sealed class HalfAnalyzer() : FloatingPointNumberAnalyzer<Half>(NumberInfos.NumberInfo.Half);