using JetBrains.ReSharper.FeaturesTestFramework.Intentions;
using JetBrains.ReSharper.TestFramework;
using NUnit.Framework;
using ReCommendedExtension.Analyzers.BaseTypes;

namespace ReCommendedExtension.Tests.Analyzers.BaseTypes.Int64;

[TestFixture]
public sealed class UseBinaryOperationFixTests : QuickFixTestBase<UseBinaryOperationFix>
{
    protected override string RelativeTestDataPath => @"Analyzers\BaseTypes\Int64QuickFixes";

    [Test]
    public void TestEquals_Int64() => DoNamedTest2();

    [Test]
    [TestNet70]
    public void TestIsNegative() => DoNamedTest2();

    [Test]
    [TestNet70]
    public void TestIsPositive() => DoNamedTest2();
}