using JetBrains.Annotations;
using ReCommendedExtension.Assertions;

namespace ReCommendedExtension.Highlightings
{
    public abstract class RedundantAssertionHighlighting : Highlighting
    {
        private protected RedundantAssertionHighlighting([NotNull] string message) : base(message) { }

        [NotNull]
        internal abstract Assertion Assertion { get; }
    }
}