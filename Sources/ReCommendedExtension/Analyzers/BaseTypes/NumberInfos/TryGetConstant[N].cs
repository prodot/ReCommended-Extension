using JetBrains.ReSharper.Psi.CSharp.Tree;

namespace ReCommendedExtension.Analyzers.BaseTypes.NumberInfos;

public delegate N? TryGetConstant<N>(ICSharpExpression? expression, out bool implicitlyConverted) where N : struct;