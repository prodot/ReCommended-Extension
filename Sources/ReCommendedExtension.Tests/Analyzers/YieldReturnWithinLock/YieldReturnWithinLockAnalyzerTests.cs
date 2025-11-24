using JetBrains.Application.Settings;
using JetBrains.ReSharper.Daemon.CSharp.Errors;
using JetBrains.ReSharper.Feature.Services.Daemon;
using JetBrains.ReSharper.FeaturesTestFramework.Daemon;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.CSharp;
using JetBrains.ReSharper.TestFramework;
using NUnit.Framework;
using ReCommendedExtension.Analyzers.YieldReturnWithinLock;

namespace ReCommendedExtension.Tests.Analyzers.YieldReturnWithinLock;

[TestFixture]
public sealed class YieldReturnWithinLockAnalyzerTests : CSharpHighlightingTestBase
{
    protected override string RelativeTestDataPath => @"Analyzers\YieldReturnWithinLock";

    protected override bool HighlightingPredicate(IHighlighting highlighting, IPsiSourceFile sourceFile, IContextBoundSettingsStore settingsStore)
        => highlighting is YieldReturnWithinLockWarning or { IsError: true } and not ByRefTypeAndAwaitError;

    [Test]
    public void TestYieldReturnWithinLock() => DoNamedTest2();

    [Test]
    [CSharpLanguageLevel(CSharpLanguageLevel.CSharp130)]
    [TestNet90]
    public void TestYieldReturnWithinLockAroundLockObject() => DoNamedTest2();
}