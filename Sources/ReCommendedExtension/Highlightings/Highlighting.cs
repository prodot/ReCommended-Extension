using JetBrains.Annotations;
using JetBrains.DocumentModel;
using JetBrains.ReSharper.Feature.Services.Daemon;

namespace ReCommendedExtension.Highlightings
{
    public abstract class Highlighting : IHighlighting
    {
        [NotNull]
        readonly string message;

        internal Highlighting([NotNull] string message) => this.message = message;

        public string ErrorStripeToolTip => message;

        public string ToolTip => message;

        public bool IsValid() => true;

        public abstract DocumentRange CalculateRange();
    }
}