using JetBrains.DocumentModel;
using JetBrains.ReSharper.Psi.CSharp.Tree;
using JetBrains.ReSharper.Psi.Tree;

namespace ReCommendedExtension.Analyzers.Region;

public abstract class RegionHighlighting(string message, IStartRegion startRegion) : Highlighting(message)
{
    public sealed override DocumentRange CalculateRange() => startRegion.Directive.GetDocumentRange();
}