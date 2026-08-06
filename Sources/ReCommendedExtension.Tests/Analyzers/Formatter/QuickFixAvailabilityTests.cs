using JetBrains.ReSharper.Feature.Services.Daemon;
using JetBrains.ReSharper.Psi.CSharp;
using JetBrains.ReSharper.TestFramework;
using NUnit.Framework;
using ReCommendedExtension.Analyzers.Formatter;

namespace ReCommendedExtension.Tests.Analyzers.Formatter;

[TestFixture]
[CSharpLanguageLevel(CSharpLanguageLevel.CSharp110)]
public sealed class QuickFixAvailabilityTests : ReCommendedExtension.Tests.Analyzers.QuickFixAvailabilityTests
{
    protected override string RelativeTestDataPath => @"Analyzers\Formatter\QuickFixes";

    protected override bool UseHighlighting(IHighlighting highlighting)
        => highlighting is RedundantFormatSpecifierHint
            or RedundantFormatProviderHint
            or RedundantFormatPrecisionSpecifierHint
            or PassOtherFormatSpecifierSuggestion
            or SuspiciousFormatSpecifierWarning
            or ReplaceTypeCastWithFormatSpecifierSuggestion;

    [Test]
    public void RemoveFormatSpecifierAvailability() => DoNamedTest();

    [Test]
    public void RemoveFormatPrecisionSpecifierAvailability() => DoNamedTest();

    [Test]
    public void PassOtherFormatSpecifierAvailability() => DoNamedTest();

    [Test]
    public void ReplaceTypeCastWithFormatSpecifierAvailability() => DoNamedTest();

    [Test]
    public void RemoveFormatProviderFixAvailability() => DoNamedTest();
}