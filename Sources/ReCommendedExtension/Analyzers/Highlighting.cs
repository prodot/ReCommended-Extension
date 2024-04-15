using JetBrains.DocumentModel;
using JetBrains.ReSharper.Feature.Services.Daemon;

namespace ReCommendedExtension.Analyzers;

public abstract class Highlighting(string message) : IHighlighting
{
    public string ErrorStripeToolTip => message;

    public string ToolTip => message;

    public bool IsValid() => true;

    public abstract DocumentRange CalculateRange();
}