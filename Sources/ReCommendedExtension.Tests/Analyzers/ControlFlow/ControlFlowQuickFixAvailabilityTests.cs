using JetBrains.ReSharper.Feature.Services.Daemon;
using JetBrains.ReSharper.TestFramework;
using NUnit.Framework;
using ReCommendedExtension.Analyzers.ControlFlow;

namespace ReCommendedExtension.Tests.Analyzers.ControlFlow;

[TestFixture]
public sealed class ControlFlowQuickFixAvailabilityTests : QuickFixAvailabilityTests
{
    protected override string RelativeTestDataPath => @"Analyzers\ControlFlow\QuickFixes";

    protected override bool UseHighlighting(IHighlighting highlighting) => highlighting is RedundantAssertionStatementSuggestion;

    [Test]
    [TestNetCore30(ANNOTATIONS_PACKAGE)]
    public void TestControlFlowAvailability() => DoNamedTest2();
}