namespace ReCommendedExtension.Analyzers.ControlFlow;

public abstract class RedundantAssertionSuggestion(string message) : Highlighting(message)
{
    internal abstract Assertion Assertion { get; }
}