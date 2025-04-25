using JetBrains.ReSharper.FeaturesTestFramework.Intentions;
using JetBrains.ReSharper.TestFramework;
using NUnit.Framework;
using ReCommendedExtension.Analyzers.BaseTypes;

namespace ReCommendedExtension.Tests.Analyzers.BaseTypes.Int128;

[TestFixture]
[TestNet70]
public sealed class UseExpressionResultQuickFixTests : QuickFixTestBase<UseExpressionResultFix>
{
    protected override string RelativeTestDataPath => @"Analyzers\BaseTypes\Int128QuickFixes";

    [Test]
    public void TestDivRem() => DoNamedTest2();

    [Test]
    public void TestEquals_Object() => DoNamedTest2();

    [Test]
    public void TestMax() => DoNamedTest2();

    [Test]
    public void TestMaxMagnitude() => DoNamedTest2();

    [Test]
    public void TestMin() => DoNamedTest2();

    [Test]
    public void TestMinMagnitude() => DoNamedTest2();

    [Test]
    public void TestRotateLeft() => DoNamedTest2();

    [Test]
    public void TestRotateRight() => DoNamedTest2();
}