using JetBrains.ReSharper.FeaturesTestFramework.Intentions;
using JetBrains.ReSharper.TestFramework;
using NUnit.Framework;
using ReCommendedExtension.Analyzers.BaseTypes;

namespace ReCommendedExtension.Tests.Analyzers.BaseTypes.Decimal;

[TestFixture]
public sealed class UseBinaryOperationFixTests : QuickFixTestBase<UseBinaryOperationFix>
{
    protected override string RelativeTestDataPath => @"Analyzers\BaseTypes\DecimalQuickFixes";

    [Test]
    public void TestEquals_Decimal() => DoNamedTest2();

    [Test]
    [TestNet70]
    public void TestIsNegative() => DoNamedTest2();

    [Test]
    [TestNet70]
    public void TestIsPositive() => DoNamedTest2();
}