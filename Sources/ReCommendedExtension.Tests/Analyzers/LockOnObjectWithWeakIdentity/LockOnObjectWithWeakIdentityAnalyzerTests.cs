using JetBrains.Application.Settings;
using JetBrains.ReSharper.Feature.Services.Daemon;
using JetBrains.ReSharper.FeaturesTestFramework.Daemon;
using JetBrains.ReSharper.Psi;
using NUnit.Framework;
using ReCommendedExtension.Analyzers.LockOnObjectWithWeakIdentity;

namespace ReCommendedExtension.Tests.Analyzers.LockOnObjectWithWeakIdentity;

[TestFixture]
public sealed class LockOnObjectWithWeakIdentityAnalyzerTests : CSharpHighlightingTestBase
{
    protected override string RelativeTestDataPath => @"Analyzers\LockOnObjectWithWeakIdentity";

    protected override bool HighlightingPredicate(IHighlighting highlighting, IPsiSourceFile sourceFile, IContextBoundSettingsStore settingsStore)
        => highlighting is LockOnObjectWithWeakIdentityWarning or { IsError: true };

    [Test]
    public void TestLockOnObjectWithWeakIdentity() => DoNamedTest2();
}