using JetBrains.Annotations;

namespace ReCommendedExtension.Analyzers.ControlFlow
{
    public abstract class RedundantAssertionSuggestion : Highlighting
    {
        private protected RedundantAssertionSuggestion([NotNull] string message) : base(message) { }

        [NotNull]
        internal abstract Assertion Assertion { get; }
    }
}