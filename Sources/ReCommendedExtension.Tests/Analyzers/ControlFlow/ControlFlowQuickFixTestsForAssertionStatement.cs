using JetBrains.ReSharper.FeaturesTestFramework.Intentions;
using JetBrains.ReSharper.TestFramework;
using NUnit.Framework;
using ReCommendedExtension.Analyzers.ControlFlow;

namespace ReCommendedExtension.Tests.Analyzers.ControlFlow;

[TestFixture]
public sealed class ControlFlowQuickFixTestsForAssertionStatement : QuickFixTestBase<RedundantAssertionStatementSuggestion.Fix>
{
    protected override string RelativeTestDataPath => @"Analyzers\ControlFlow\QuickFixes";

    [Test]
    [TestNetCore30(ANNOTATIONS_PACKAGE)]
    public void TestRemoveAssertionStatement() => DoNamedTest2();
}