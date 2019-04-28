using JetBrains.Annotations;
using JetBrains.DocumentModel;
using JetBrains.ReSharper.Psi.CSharp.Tree;
using JetBrains.ReSharper.Psi.Tree;

namespace ReCommendedExtension.Analyzers.Region
{
    public abstract class RegionHighlighting : Highlighting
    {
        [NotNull]
        readonly IStartRegion startRegion;

        private protected RegionHighlighting([NotNull] string message, [NotNull] IStartRegion startRegion) : base(message)
            => this.startRegion = startRegion;

        public sealed override DocumentRange CalculateRange() => startRegion.Directive.GetDocumentRange();
    }
}