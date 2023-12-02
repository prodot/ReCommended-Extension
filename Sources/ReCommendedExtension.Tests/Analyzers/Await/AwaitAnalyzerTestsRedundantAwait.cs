using JetBrains.Application.Settings;
using JetBrains.ReSharper.Feature.Services.Daemon;
using JetBrains.ReSharper.FeaturesTestFramework.Daemon;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.TestFramework;
using NUnit.Framework;
using ReCommendedExtension.Analyzers.Await;

namespace ReCommendedExtension.Tests.Analyzers.Await;

[TestFixture]
public sealed class AwaitAnalyzerTestsRedundantAwait : CSharpHighlightingTestBase
{
    protected override string RelativeTestDataPath => @"Analyzers\Await";

    protected override bool HighlightingPredicate(IHighlighting highlighting, IPsiSourceFile sourceFile, IContextBoundSettingsStore settingsStore)
        => highlighting is RedundantAwaitSuggestion;

    [Test]
    [TestNetCore30]
    public void TestRedundantAwait() => DoNamedTest2();

    [Test]
    [TestNetCore30]
    public void TestRedundantAwait_ValueTask() => DoNamedTest2();

    [Test]
    [TestNet80]
    public void TestRedundantAwait_NET_8() => DoNamedTest2();
}