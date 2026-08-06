using JetBrains.ReSharper.FeaturesTestFramework.Intentions;
using NUnit.Framework;
using ReCommendedExtension.Analyzers.CatchClauseWithoutVariable;

namespace ReCommendedExtension.Tests.Analyzers.CatchClauseWithoutVariable;

[TestFixture]
public sealed class CatchClauseWithoutVariableQuickFixTests : QuickFixTestBase<CatchClauseWithoutVariableHint.Fix>
{
    protected override string RelativeTestDataPath => @"Analyzers\CatchClauseWithoutVariable\QuickFixes";

    [Test]
    public void CatchClauseWithoutVariable() => DoNamedTest();

    [Test]
    public void CatchClauseWithoutVariable2() => DoNamedTest();
}