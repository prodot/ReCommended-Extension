using JetBrains.ReSharper.FeaturesTestFramework.Intentions;
using JetBrains.ReSharper.TestFramework;
using NUnit.Framework;
using ReCommendedExtension.Analyzers.BaseTypes;

namespace ReCommendedExtension.Tests.Analyzers.BaseTypes.SByte;

[TestFixture]
public sealed class UseBinaryOperatorFixTests : QuickFixTestBase<UseBinaryOperatorFix>
{
    protected override string RelativeTestDataPath => @"Analyzers\BaseTypes\SByteQuickFixes";

    [Test]
    public void TestEquals_SByte() => DoNamedTest2();

    [Test]
    [TestNet70]
    public void TestIsNegative() => DoNamedTest2();

    [Test]
    [TestNet70]
    public void TestIsPositive() => DoNamedTest2();
}