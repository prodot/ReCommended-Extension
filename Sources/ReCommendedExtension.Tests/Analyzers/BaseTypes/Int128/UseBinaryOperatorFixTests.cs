using JetBrains.ReSharper.FeaturesTestFramework.Intentions;
using JetBrains.ReSharper.TestFramework;
using NUnit.Framework;
using ReCommendedExtension.Analyzers.BaseTypes;

namespace ReCommendedExtension.Tests.Analyzers.BaseTypes.Int128;

[TestFixture]
[TestNet70]
public sealed class UseBinaryOperatorFixTests : QuickFixTestBase<UseBinaryOperatorFix>
{
    protected override string RelativeTestDataPath => @"Analyzers\BaseTypes\Int128QuickFixes";

    [Test]
    public void TestEquals_Int128() => DoNamedTest2();

    [Test]
    public void TestIsNegative() => DoNamedTest2();

    [Test]
    public void TestIsPositive() => DoNamedTest2();
}