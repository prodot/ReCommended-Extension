using JetBrains.Application.Settings;
using JetBrains.ReSharper.Feature.Services.Daemon;
using JetBrains.ReSharper.FeaturesTestFramework.Intentions;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.TestFramework;
using NUnit.Framework;
using ReCommendedExtension.Analyzers.ControlFlow;

namespace ReCommendedExtension.Tests.Analyzers.ControlFlow;

[TestFixture]
public sealed class ControlFlowQuickFixAvailabilityTests : QuickFixAvailabilityTestBase
{
    protected override string RelativeTestDataPath => @"Analyzers\ControlFlow\QuickFixes";

    protected override bool HighlightingPredicate(IHighlighting highlighting, IPsiSourceFile sourceFile, IContextBoundSettingsStore settingsStore)
        => highlighting is RedundantAssertionStatementSuggestion or { IsError: true };

    [Test]
    [TestNetCore30(ANNOTATIONS_PACKAGE)]
    public void TestControlFlowAvailability() => DoNamedTest2();
}