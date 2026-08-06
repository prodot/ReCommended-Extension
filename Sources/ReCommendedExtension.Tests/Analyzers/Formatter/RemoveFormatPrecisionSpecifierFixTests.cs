using JetBrains.ReSharper.FeaturesTestFramework.Intentions;
using JetBrains.ReSharper.Psi.CSharp;
using JetBrains.ReSharper.TestFramework;
using NUnit.Framework;
using ReCommendedExtension.Analyzers.Formatter;

namespace ReCommendedExtension.Tests.Analyzers.Formatter;

[TestFixture]
public sealed class RemoveFormatPrecisionSpecifierFixTests : QuickFixTestBase<RedundantFormatPrecisionSpecifierHint.Fix>
{
    protected override string RelativeTestDataPath => @"Analyzers\Formatter\QuickFixes";

    [Test]
    [CSharpLanguageLevel(CSharpLanguageLevel.CSharp110)]
    public void RemoveFormatPrecisionSpecifier_StringInterpolation() => DoNamedTest();

    [Test]
    public void RemoveFormatPrecisionSpecifier_StringFormat() => DoNamedTest();

    [Test]
    public void RemoveFormatPrecisionSpecifier_ToString() => DoNamedTest();
}