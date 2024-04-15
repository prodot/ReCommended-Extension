using JetBrains.Application.Settings;
using JetBrains.ReSharper.Feature.Services.Daemon;
using JetBrains.ReSharper.FeaturesTestFramework.Daemon;
using JetBrains.ReSharper.Psi;
using NUnit.Framework;
using ReCommendedExtension.Analyzers.YieldReturnWithinLock;

namespace ReCommendedExtension.Tests.Analyzers.YieldReturnWithinLock;

[TestFixture]
public sealed class YieldReturnWithinLockAnalyzerTests : CSharpHighlightingTestBase
{
    protected override string RelativeTestDataPath => @"Analyzers\YieldReturnWithinLock";

    protected override bool HighlightingPredicate(IHighlighting highlighting, IPsiSourceFile sourceFile, IContextBoundSettingsStore settingsStore)
        => highlighting is YieldReturnWithinLockWarning;

    [Test]
    public void TestYieldReturnWithinLock() => DoNamedTest2();
}