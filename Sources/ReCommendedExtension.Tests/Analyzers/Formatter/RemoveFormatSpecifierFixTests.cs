using JetBrains.ReSharper.FeaturesTestFramework.Intentions;
using JetBrains.ReSharper.Psi.CSharp;
using JetBrains.ReSharper.TestFramework;
using NUnit.Framework;
using ReCommendedExtension.Analyzers.Formatter;

namespace ReCommendedExtension.Tests.Analyzers.Formatter;

[TestFixture]
public sealed class RemoveFormatSpecifierFixTests : QuickFixTestBase<RedundantFormatSpecifierHint.Fix>
{
    protected override string RelativeTestDataPath => @"Analyzers\Formatter\QuickFixes";

    [Test]
    [CSharpLanguageLevel(CSharpLanguageLevel.CSharp110)]
    public void RemoveFormatSpecifier_StringInterpolation() => DoNamedTest();

    [Test]
    public void RemoveFormatSpecifier_StringFormat() => DoNamedTest();

    [Test]
    public void RemoveFormatSpecifier_ToString() => DoNamedTest();
}