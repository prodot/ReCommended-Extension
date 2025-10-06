using JetBrains.ReSharper.Feature.Services.Daemon;
using JetBrains.ReSharper.Psi.CSharp.Tree;

namespace ReCommendedExtension.Analyzers.BaseTypes.Analyzers;

/// <remarks>
/// C# language version checks are only done when a quick fix would require it.
/// </remarks>
[ElementProblemAnalyzer(
    typeof(IInvocationExpression),
    HighlightingTypes = [typeof(UseExpressionResultSuggestion), typeof(UseBinaryOperatorSuggestion)])]
public sealed class SByteAnalyzer() : SignedIntegerAnalyzer<sbyte>(NumberInfos.NumberInfo.SByte);