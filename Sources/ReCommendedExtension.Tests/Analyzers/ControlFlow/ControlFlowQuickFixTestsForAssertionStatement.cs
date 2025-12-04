using JetBrains.ReSharper.FeaturesTestFramework.Intentions;
using NUnit.Framework;
using ReCommendedExtension.Analyzers.ControlFlow;

namespace ReCommendedExtension.Tests.Analyzers.ControlFlow;

[TestFixture]
public sealed class ControlFlowQuickFixTestsForAssertionStatement : QuickFixTestBase<RedundantAssertionStatementSuggestion.Fix>
{
    protected override string RelativeTestDataPath => @"Analyzers\ControlFlow\QuickFixes";

    [Test]
    public void TestRemoveAssertionStatement() => DoNamedTest2();
}