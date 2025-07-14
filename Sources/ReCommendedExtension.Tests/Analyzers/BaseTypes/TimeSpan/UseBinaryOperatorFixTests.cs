using JetBrains.ReSharper.FeaturesTestFramework.Intentions;
using JetBrains.ReSharper.TestFramework;
using NUnit.Framework;
using ReCommendedExtension.Analyzers.BaseTypes;

namespace ReCommendedExtension.Tests.Analyzers.BaseTypes.TimeSpan;

[TestFixture]
public sealed class UseBinaryOperatorFixTests : QuickFixTestBase<UseBinaryOperatorFix>
{
    protected override string RelativeTestDataPath => @"Analyzers\BaseTypes\TimeSpan\QuickFixes";

    [Test]
    public void TestAdd() => DoNamedTest2();

    [Test]
    [TestNetCore20]
    public void TestDivide_Double() => DoNamedTest2();

    [Test]
    [TestNetCore20]
    public void TestDivide_TimeSpan() => DoNamedTest2();

    [Test]
    public void TestEquals_TimeSpan() => DoNamedTest2();

    [Test]
    public void TestEquals_Static() => DoNamedTest2();

    [Test]
    [TestNetCore20]
    public void TestMultiply() => DoNamedTest2();

    [Test]
    public void TestSubtract() => DoNamedTest2();
}