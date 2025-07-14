using JetBrains.ReSharper.FeaturesTestFramework.Intentions;
using JetBrains.ReSharper.TestFramework;
using NUnit.Framework;
using ReCommendedExtension.Analyzers.BaseTypes;

namespace ReCommendedExtension.Tests.Analyzers.BaseTypes.TimeSpan;

[TestFixture]
public sealed class RemoveArgumentFixTests : QuickFixTestBase<RemoveArgumentFix>
{
    protected override string RelativeTestDataPath => @"Analyzers\BaseTypes\TimeSpan\QuickFixes";

    [Test]
    public void Test_Constructors_RedundantArgument() => DoNamedTest2();

    [Test]
    [TestNetCore21]
    public void TestParse() => DoNamedTest2();

    [Test]
    [TestNetCore21]
    public void TestParseExact_None() => DoNamedTest2();

    [Test]
    [TestNetCore21]
    public void TestToString() => DoNamedTest2();

    [Test]
    [TestNetCore21]
    public void TestToString_FormatProvider() => DoNamedTest2();

    [Test]
    [TestNetCore21]
    public void TestTryParse() => DoNamedTest2();
}