using JetBrains.Annotations;

namespace ReCommendedExtension.Analyzers.ControlFlow
{
    public abstract class RedundantAssertionHighlighting : Highlighting
    {
        private protected RedundantAssertionHighlighting([NotNull] string message) : base(message) { }

        [NotNull]
        internal abstract Assertion Assertion { get; }
    }
}