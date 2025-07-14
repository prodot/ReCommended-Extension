using JetBrains.ReSharper.FeaturesTestFramework.Intentions;
using JetBrains.ReSharper.Psi.CSharp;
using JetBrains.ReSharper.TestFramework;
using NUnit.Framework;
using ReCommendedExtension.Analyzers.BaseTypes;

namespace ReCommendedExtension.Tests.Analyzers.BaseTypes.TimeSpan;

[TestFixture]
[TestNetCore21]
public sealed class UseOtherArgumentFixTests : QuickFixTestBase<UseOtherArgumentFix>
{
    protected override string RelativeTestDataPath => @"Analyzers\BaseTypes\TimeSpan\QuickFixes";

    [Test]
    public void TestParseExact() => DoNamedTest2();

    [Test]
    public void TestParseExact_Named() => DoNamedTest2();

    [Test]
    [CSharpLanguageLevel(CSharpLanguageLevel.CSharp120)]
    public void TestParseExact_Single() => DoNamedTest2();

    [Test]
    [CSharpLanguageLevel(CSharpLanguageLevel.CSharp120)]
    public void TestParseExact_Single_Named() => DoNamedTest2();

    [Test]
    public void TestTryParseExact() => DoNamedTest2();

    [Test]
    public void TestTryParseExact_Named() => DoNamedTest2();

    [Test]
    [CSharpLanguageLevel(CSharpLanguageLevel.CSharp120)]
    public void TestTryParseExact_Single() => DoNamedTest2();

    [Test]
    [CSharpLanguageLevel(CSharpLanguageLevel.CSharp120)]
    public void TestTryParseExact_Single_Named() => DoNamedTest2();
}