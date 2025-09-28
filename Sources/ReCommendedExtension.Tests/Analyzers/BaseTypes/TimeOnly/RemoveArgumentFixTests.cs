using JetBrains.ReSharper.FeaturesTestFramework.Intentions;
using JetBrains.ReSharper.TestFramework;
using NUnit.Framework;
using ReCommendedExtension.Analyzers.BaseTypes;

namespace ReCommendedExtension.Tests.Analyzers.BaseTypes.TimeOnly;

[TestFixture]
[TestNet60]
public sealed class RemoveArgumentFixTests : QuickFixTestBase<RemoveArgumentFix>
{
    protected override string RelativeTestDataPath => @"Analyzers\BaseTypes\TimeOnly\QuickFixes";

    [Test]
    public void TestAdd() => DoNamedTest2();

    [Test]
    public void TestAddHours() => DoNamedTest2();

    [Test]
    public void TestAddMinutes() => DoNamedTest2();

    [Test]
    public void TestParse() => DoNamedTest2();

    [Test]
    public void TestParseExact() => DoNamedTest2();

    [Test]
    [TestNet70]
    public void TestTryParse() => DoNamedTest2();
}