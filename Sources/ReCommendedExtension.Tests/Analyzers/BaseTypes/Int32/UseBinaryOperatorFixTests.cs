using JetBrains.ReSharper.FeaturesTestFramework.Intentions;
using JetBrains.ReSharper.TestFramework;
using NUnit.Framework;
using ReCommendedExtension.Analyzers.BaseTypes;

namespace ReCommendedExtension.Tests.Analyzers.BaseTypes.Int32;

[TestFixture]
public sealed class UseBinaryOperatorFixTests : QuickFixTestBase<UseBinaryOperatorFix>
{
    protected override string RelativeTestDataPath => @"Analyzers\BaseTypes\Int32QuickFixes";

    [Test]
    public void TestEquals_Int32() => DoNamedTest2();

    [Test]
    [TestNet70]
    public void TestIsNegative() => DoNamedTest2();

    [Test]
    [TestNet70]
    public void TestIsPositive() => DoNamedTest2();
}