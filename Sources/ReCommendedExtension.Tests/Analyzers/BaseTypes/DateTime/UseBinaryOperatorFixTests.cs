using JetBrains.ReSharper.FeaturesTestFramework.Intentions;
using NUnit.Framework;
using ReCommendedExtension.Analyzers.BaseTypes;

namespace ReCommendedExtension.Tests.Analyzers.BaseTypes.DateTime;

[TestFixture]
public sealed class UseBinaryOperatorFixTests : QuickFixTestBase<UseBinaryOperatorFix>
{
    protected override string RelativeTestDataPath => @"Analyzers\BaseTypes\DateTime\QuickFixes";

    [Test]
    public void TestAdd() => DoNamedTest2();

    [Test]
    public void TestEquals_Static() => DoNamedTest2();

    [Test]
    public void TestEquals_DateTime() => DoNamedTest2();

    [Test]
    public void TestSubtract() => DoNamedTest2();
}