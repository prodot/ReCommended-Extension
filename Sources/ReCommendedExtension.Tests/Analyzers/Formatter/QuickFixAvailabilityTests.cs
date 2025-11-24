using JetBrains.Application.Settings;
using JetBrains.ReSharper.Feature.Services.Daemon;
using JetBrains.ReSharper.FeaturesTestFramework.Intentions;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.CSharp;
using JetBrains.ReSharper.TestFramework;
using NUnit.Framework;
using ReCommendedExtension.Analyzers.Formatter;

namespace ReCommendedExtension.Tests.Analyzers.Formatter;

[TestFixture]
[CSharpLanguageLevel(CSharpLanguageLevel.CSharp110)]
public sealed class QuickFixAvailabilityTests : QuickFixAvailabilityTestBase
{
    protected override string RelativeTestDataPath => @"Analyzers\Formatter\QuickFixes";

    protected override bool HighlightingPredicate(IHighlighting highlighting, IPsiSourceFile sourceFile, IContextBoundSettingsStore settingsStore)
        => highlighting is RedundantFormatSpecifierHint
            or RedundantFormatProviderHint
            or RedundantFormatPrecisionSpecifierHint
            or PassOtherFormatSpecifierSuggestion
            or SuspiciousFormatSpecifierWarning
            or ReplaceTypeCastWithFormatSpecifierSuggestion
            or { IsError: true };

    [Test]
    public void TestRemoveFormatSpecifierAvailability() => DoNamedTest2();

    [Test]
    public void TestRemoveFormatPrecisionSpecifierAvailability() => DoNamedTest2();

    [Test]
    public void TestPassOtherFormatSpecifierAvailability() => DoNamedTest2();

    [Test]
    public void TestReplaceTypeCastWithFormatSpecifierAvailability() => DoNamedTest2();

    [Test]
    public void TestRemoveFormatProviderFixAvailability() => DoNamedTest2();
}