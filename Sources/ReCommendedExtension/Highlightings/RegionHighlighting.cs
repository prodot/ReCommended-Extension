using JetBrains.Annotations;
using JetBrains.DocumentModel;
using JetBrains.ReSharper.Psi.CSharp.Tree;
using JetBrains.ReSharper.Psi.Tree;

namespace ReCommendedExtension.Highlightings
{
    public abstract class RegionHighlighting : Highlighting
    {
        [NotNull]
        readonly IStartRegion startRegion;

        internal RegionHighlighting([NotNull] string message, [NotNull] IStartRegion startRegion) : base(message) => this.startRegion = startRegion;

        public sealed override DocumentRange CalculateRange() => startRegion.Directive.GetDocumentRange();
    }
}