using JetBrains.ReSharper.FeaturesTestFramework.Intentions;
using JetBrains.ReSharper.TestFramework;
using NUnit.Framework;
using ReCommendedExtension.Analyzers.BaseTypes;

namespace ReCommendedExtension.Tests.Analyzers.BaseTypes.Decimal;

[TestFixture]
public sealed class UseExpressionResultQuickFixTests : QuickFixTestBase<UseExpressionResultFix>
{
    protected override string RelativeTestDataPath => @"Analyzers\BaseTypes\DecimalQuickFixes";

    [Test]
    [TestNet70]
    public void TestClamp() => DoNamedTest2();

    [Test]
    public void TestEquals_Object() => DoNamedTest2();

    [Test]
    public void TestGetTypeCode() => DoNamedTest2();
}