using JetBrains.ReSharper.FeaturesTestFramework.Intentions;
using NUnit.Framework;
using ReCommendedExtension.Analyzers.CatchClauseWithoutVariable;

namespace ReCommendedExtension.Tests.Analyzers.CatchClauseWithoutVariable;

[TestFixture]
public sealed class CatchClauseWithoutVariableQuickFixTests : QuickFixTestBase<CatchClauseWithoutVariableHint.Fix>
{
    protected override string RelativeTestDataPath => @"Analyzers\CatchClauseWithoutVariable\QuickFixes";

    [Test]
    public void TestCatchClauseWithoutVariable() => DoNamedTest2();

    [Test]
    public void TestCatchClauseWithoutVariable2() => DoNamedTest2();
}