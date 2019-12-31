using JetBrains.Annotations;
using JetBrains.DocumentModel;
using JetBrains.ReSharper.Feature.Services.Daemon;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.CSharp;
using JetBrains.ReSharper.Psi.CSharp.Tree;
using JetBrains.ReSharper.Psi.Tree;
using ReCommendedExtension.Analyzers.ArrayWithDefaultValuesInitialization;
using ZoneMarker = ReCommendedExtension.ZoneMarker;

[assembly:
    RegisterConfigurableSeverity(
        ArrayWithDefaultValuesInitializationSuggestion.SeverityId,
        null,
        HighlightingGroupIds.LanguageUsage,
        "Use 'new T[n]' for arrays with default values" + ZoneMarker.Suffix,
        "",
        Severity.SUGGESTION)]

namespace ReCommendedExtension.Analyzers.ArrayWithDefaultValuesInitialization
{
    [ConfigurableSeverityHighlighting(SeverityId, CSharpLanguage.Name)]
    public sealed class ArrayWithDefaultValuesInitializationSuggestion : Highlighting
    {
        internal const string SeverityId = "ArrayWithDefaultValuesInitialization";

        internal ArrayWithDefaultValuesInitializationSuggestion(
            [NotNull] string message,
            [NotNull] IArrayInitializer arrayInitializer,
            [NotNull] IType arrayElementType,
            int elementCount) : base(message)
        {
            ArrayInitializer = arrayInitializer;
            ArrayElementType = arrayElementType;
            ElementCount = elementCount;
        }

        [NotNull]
        internal IArrayInitializer ArrayInitializer { get; }

        [NotNull]
        internal IType ArrayElementType { get; }

        internal int ElementCount { get; }

        public override DocumentRange CalculateRange() => ArrayInitializer.GetDocumentRange();
    }
}