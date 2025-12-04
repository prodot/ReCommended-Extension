using JetBrains.Application.Settings;
using JetBrains.ReSharper.Feature.Services.Daemon;
using JetBrains.ReSharper.FeaturesTestFramework.Daemon;
using JetBrains.ReSharper.Psi;
using NUnit.Framework;

namespace ReCommendedExtension.Tests.Analyzers;

public abstract class CSharpAnalyzerTests : CSharpHighlightingTestBase
{
    readonly HighlightingExtensions.HighlightingStatistics highlightingStatistics = new();

    protected sealed override bool HighlightingPredicate(
        IHighlighting highlighting,
        IPsiSourceFile sourceFile,
        IContextBoundSettingsStore settingsStore)
        => (UseHighlighting(highlighting) || highlighting is { IsError: true }) && highlightingStatistics.TryAdd(highlighting);

    [Pure]
    protected abstract bool UseHighlighting(IHighlighting highlighting);

    public override void TestFixtureTearDown()
    {
        try
        {
            TestContext.WriteLine(highlightingStatistics.GetSummary());
        }
        finally
        {
            base.TestFixtureTearDown();
        }
    }
}