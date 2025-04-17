using JetBrains.ReSharper.FeaturesTestFramework.Intentions;
using JetBrains.ReSharper.TestFramework;
using NUnit.Framework;
using ReCommendedExtension.Analyzers.BaseTypes;

namespace ReCommendedExtension.Tests.Analyzers.BaseTypes.UInt16;

[TestFixture]
public sealed class UseExpressionResultQuickFixTests : QuickFixTestBase<UseExpressionResultFix>
{
    protected override string RelativeTestDataPath => @"Analyzers\BaseTypes\UInt16QuickFixes";

    [Test]
    [TestNet70]
    public void TestClamp() => DoNamedTest2();

    [Test]
    [TestNet70]
    public void TestDivRem() => DoNamedTest2();

    [Test]
    public void TestEquals_Object() => DoNamedTest2();
}