using JetBrains.DocumentModel;
using JetBrains.ReSharper.Feature.Services.Daemon;

namespace ReCommendedExtension.Analyzers;

public abstract record Highlighting : IHighlighting
{
    readonly string message;

    private protected Highlighting(string message) => this.message = message;

    public string ErrorStripeToolTip => message;

    public string ToolTip => message;

    public bool IsValid() => true;

    public abstract DocumentRange CalculateRange();
}