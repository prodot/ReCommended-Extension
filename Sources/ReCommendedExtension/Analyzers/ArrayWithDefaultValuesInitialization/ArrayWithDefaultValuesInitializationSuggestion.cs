using JetBrains.Annotations;
using JetBrains.DocumentModel;
using JetBrains.ReSharper.Feature.Services.Daemon;
using JetBrains.ReSharper.Psi.CSharp;
using JetBrains.ReSharper.Psi.CSharp.Tree;
using JetBrains.ReSharper.Psi.Tree;

namespace ReCommendedExtension.Analyzers.ArrayWithDefaultValuesInitialization
{
    [RegisterConfigurableSeverity(
        SeverityId,
        null,
        HighlightingGroupIds.LanguageUsage,
        "Use 'new T[n]' for arrays with default values" + ZoneMarker.Suffix,
        "",
        Severity.SUGGESTION)]
    [ConfigurableSeverityHighlighting(SeverityId, CSharpLanguage.Name)]
    public sealed class ArrayWithDefaultValuesInitializationSuggestion : Highlighting
    {
        const string SeverityId = "ArrayWithDefaultValuesInitialization";

        internal ArrayWithDefaultValuesInitializationSuggestion(
            [NotNull] string message,
            [CanBeNull] string suggestedCode,
            [NotNull] IArrayInitializer arrayInitializer) : base(message)
        {
            SuggestedCode = suggestedCode;
            ArrayInitializer = arrayInitializer;
        }

        [CanBeNull]
        internal string SuggestedCode { get; }

        [NotNull]
        internal IArrayInitializer ArrayInitializer { get; }

        public override DocumentRange CalculateRange() => ArrayInitializer.GetDocumentRange();
    }
}