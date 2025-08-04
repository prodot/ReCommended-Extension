using JetBrains.ReSharper.FeaturesTestFramework.Intentions;
using NUnit.Framework;
using ReCommendedExtension.Analyzers.BaseTypes;

namespace ReCommendedExtension.Tests.Analyzers.BaseTypes.DateTimeOffset;

[TestFixture]
public sealed class UseBinaryOperatorFixTests : QuickFixTestBase<UseBinaryOperatorFix>
{
    protected override string RelativeTestDataPath => @"Analyzers\BaseTypes\DateTimeOffset\QuickFixes";

    [Test]
    public void TestAdd() => DoNamedTest2();

    [Test]
    public void TestEquals_Static() => DoNamedTest2();

    [Test]
    public void TestEquals_DateTimeOffset() => DoNamedTest2();

    [Test]
    public void TestSubtract() => DoNamedTest2();
}