using JetBrains.ReSharper.FeaturesTestFramework.Intentions;
using JetBrains.ReSharper.TestFramework;
using NUnit.Framework;
using ReCommendedExtension.Analyzers.BaseTypes;

namespace ReCommendedExtension.Tests.Analyzers.BaseTypes.TimeSpan;

[TestFixture]
public sealed class UseExpressionResultQuickFixTests : QuickFixTestBase<UseExpressionResultFix>
{
    protected override string RelativeTestDataPath => @"Analyzers\BaseTypes\TimeSpan\QuickFixes";

    [Test]
    public void Test_Constructors_ExpressionResult() => DoNamedTest2();

    [Test]
    public void TestEquals_Null() => DoNamedTest2();

    [Test]
    [TestNet90]
    public void TestFromDays() => DoNamedTest2();

    [Test]
    [TestNet90]
    public void TestFromHours() => DoNamedTest2();

    [Test]
    [TestNet90]
    public void TestFromMicroseconds() => DoNamedTest2();

    [Test]
    [TestNet90]
    public void TestFromMilliseconds() => DoNamedTest2();

    [Test]
    [TestNet90]
    public void TestFromMinutes() => DoNamedTest2();

    [Test]
    [TestNet90]
    public void TestFromSeconds() => DoNamedTest2();

    [Test]
    public void TestFromTicks() => DoNamedTest2();
}