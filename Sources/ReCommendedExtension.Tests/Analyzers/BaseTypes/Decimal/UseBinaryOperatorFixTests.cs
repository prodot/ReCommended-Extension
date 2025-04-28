using JetBrains.ReSharper.FeaturesTestFramework.Intentions;
using NUnit.Framework;
using ReCommendedExtension.Analyzers.BaseTypes;

namespace ReCommendedExtension.Tests.Analyzers.BaseTypes.Decimal;

[TestFixture]
public sealed class UseBinaryOperatorFixTests : QuickFixTestBase<UseBinaryOperatorFix>
{
    protected override string RelativeTestDataPath => @"Analyzers\BaseTypes\DecimalQuickFixes";

    [Test]
    public void TestEquals_Decimal() => DoNamedTest2();

    [Test]
    public void TestAdd() => DoNamedTest2();

    [Test]
    public void TestDivide() => DoNamedTest2();

    [Test]
    public void TestMultiply() => DoNamedTest2();

    [Test]
    public void TestRemainder() => DoNamedTest2();

    [Test]
    public void TestSubtract() => DoNamedTest2();
}