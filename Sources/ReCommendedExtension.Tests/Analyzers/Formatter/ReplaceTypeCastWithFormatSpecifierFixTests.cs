using JetBrains.ReSharper.FeaturesTestFramework.Intentions;
using JetBrains.ReSharper.Psi.CSharp;
using JetBrains.ReSharper.TestFramework;
using NUnit.Framework;
using ReCommendedExtension.Analyzers.Formatter;

namespace ReCommendedExtension.Tests.Analyzers.Formatter;

[TestFixture]
[CSharpLanguageLevel(CSharpLanguageLevel.CSharp110)]
public sealed class ReplaceTypeCastWithFormatSpecifierFixTests : QuickFixTestBase<ReplaceTypeCastWithFormatSpecifierSuggestion.Fix>
{
    protected override string RelativeTestDataPath => @"Analyzers\Formatter\QuickFixes";

    [Test]
    public void TestReplaceTypeCastWithFormatSpecifier() => DoNamedTest2();
}