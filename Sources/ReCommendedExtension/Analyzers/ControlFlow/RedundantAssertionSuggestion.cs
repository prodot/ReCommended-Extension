namespace ReCommendedExtension.Analyzers.ControlFlow;

public abstract record RedundantAssertionSuggestion : Highlighting
{
    private protected RedundantAssertionSuggestion(string message) : base(message) { }

    internal abstract Assertion Assertion { get; }
}