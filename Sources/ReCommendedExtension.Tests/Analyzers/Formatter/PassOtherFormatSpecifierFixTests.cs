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
    public void PassOtherFormatSpecifier_StringInterpolation() => DoNamedTest();

    [Test]
    public void PassOtherFormatSpecifier_StringFormat() => DoNamedTest();

    [Test]
    public void PassOtherFormatSpecifier_ToString() => DoNamedTest();
}