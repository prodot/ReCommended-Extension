using JetBrains.ReSharper.FeaturesTestFramework.Intentions;
using JetBrains.ReSharper.Psi.CSharp;
using JetBrains.ReSharper.TestFramework;
using NUnit.Framework;
using ReCommendedExtension.Analyzers.Formatter;

namespace ReCommendedExtension.Tests.Analyzers.Formatter;

[TestFixture]
public sealed class PassOtherFormatSpecifierFixTests : QuickFixTestBase<PassOtherFormatSpecifierSuggestion.Fix>
{
    protected override string RelativeTestDataPath => @"Analyzers\Formatter\QuickFixes";

    [Test]
    [CSharpLanguageLevel(CSharpLanguageLevel.CSharp110)]
    public void TestPassOtherFormatSpecifier_StringInterpolation() => DoNamedTest2();

    [Test]
    public void TestPassOtherFormatSpecifier_StringFormat() => DoNamedTest2();

    [Test]
    public void TestPassOtherFormatSpecifier_ToString() => DoNamedTest2();
}