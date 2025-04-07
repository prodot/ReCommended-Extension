using JetBrains.ReSharper.FeaturesTestFramework.Intentions;
using JetBrains.ReSharper.Psi.CSharp;
using JetBrains.ReSharper.TestFramework;
using NUnit.Framework;
using ReCommendedExtension.Analyzers.BaseTypes;

namespace ReCommendedExtension.Tests.Analyzers.BaseTypes.Char;

[TestFixture]
[CSharpLanguageLevel(CSharpLanguageLevel.CSharp90)]
[TestNet70]
public sealed class UseCharRangePatternFixTests : QuickFixTestBase<UseCharRangePatternFix>
{
    protected override string RelativeTestDataPath => @"Analyzers\BaseTypes\CharQuickFixes";

    [Test]
    public void TestIsAsciiDigit() => DoNamedTest2();

    [Test]
    public void TestIsAsciiHexDigit() => DoNamedTest2();

    [Test]
    public void TestIsAsciiHexDigitLower() => DoNamedTest2();

    [Test]
    public void TestIsAsciiHexDigitUpper() => DoNamedTest2();

    [Test]
    public void TestIsAsciiLetter() => DoNamedTest2();

    [Test]
    public void TestIsAsciiLetterLower() => DoNamedTest2();

    [Test]
    public void TestIsAsciiLetterOrDigit() => DoNamedTest2();

    [Test]
    public void TestIsAsciiLetterUpper() => DoNamedTest2();

    [Test]
    public void TestIsBetween() => DoNamedTest2();
}