using JetBrains.ReSharper.Feature.Services.Daemon;
using JetBrains.ReSharper.Psi.ControlFlow;

namespace ReCommendedExtension.Extensions;

internal static class ElementProblemAnalyzerDataExtensions
{
    [MustUseReturnValue]
    public static ValueAnalysisMode GetValueAnalysisMode(this ElementProblemAnalyzerData data)
        => data.SettingsStore.GetValue<HighlightingSettings, ValueAnalysisMode>(s => s.ValueAnalysisMode);
}