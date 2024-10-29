using JetBrains.DocumentModel;

namespace ReCommendedExtension.Analyzers;

public abstract class MultipleHighlightings(string message) : Highlighting(message)
{
    public sealed override DocumentRange CalculateRange() => throw new NotSupportedException();
}