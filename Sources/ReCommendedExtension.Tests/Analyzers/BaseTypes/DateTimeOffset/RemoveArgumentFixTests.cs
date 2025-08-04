using JetBrains.ReSharper.FeaturesTestFramework.Intentions;
using JetBrains.ReSharper.TestFramework;
using NUnit.Framework;
using ReCommendedExtension.Analyzers.BaseTypes;

namespace ReCommendedExtension.Tests.Analyzers.BaseTypes.DateTimeOffset;

[TestFixture]
public sealed class RemoveArgumentFixTests : QuickFixTestBase<RemoveArgumentFix>
{
    protected override string RelativeTestDataPath => @"Analyzers\BaseTypes\DateTimeOffset\QuickFixes";

    [Test]
    public void Test_Constructors_RedundantArgument() => DoNamedTest2();

    [Test]
    public void TestParse() => DoNamedTest2();

    [Test]
    public void TestParseExact() => DoNamedTest2();

    [Test]
    public void TestToString() => DoNamedTest2();

    [Test]
    [TestNet70]
    public void TestTryParse() => DoNamedTest2();
}