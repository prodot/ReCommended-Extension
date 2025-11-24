using JetBrains.ReSharper.Feature.Services.Daemon;
using JetBrains.ReSharper.Psi.ControlFlow;

namespace ReCommendedExtension.Extensions;

internal static class ElementProblemAnalyzerDataExtensions
{
    extension(ElementProblemAnalyzerData data)
    {
        [MustUseReturnValue]
        public ValueAnalysisMode GetValueAnalysisMode()
            => data.SettingsStore.GetValue<HighlightingSettings, ValueAnalysisMode>(s => s.ValueAnalysisMode);
    }
}