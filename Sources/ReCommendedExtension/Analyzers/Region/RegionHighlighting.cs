using JetBrains.DocumentModel;
using JetBrains.ReSharper.Psi.CSharp.Tree;
using JetBrains.ReSharper.Psi.Tree;

namespace ReCommendedExtension.Analyzers.Region;

public abstract record RegionHighlighting : Highlighting
{
    readonly IStartRegion startRegion;

    private protected RegionHighlighting(string message, IStartRegion startRegion) : base(message) => this.startRegion = startRegion;

    public sealed override DocumentRange CalculateRange() => startRegion.Directive.GetDocumentRange();
}