using JetBrains.Application.Settings;
using JetBrains.ReSharper.Feature.Services.Daemon;
using JetBrains.ReSharper.FeaturesTestFramework.Intentions;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.CSharp;
using JetBrains.ReSharper.TestFramework;
using NUnit.Framework;
using ReCommendedExtension.Analyzers.BaseTypes;

namespace ReCommendedExtension.Tests.Analyzers.BaseTypes.InterpolatedStringExpression;

[TestFixture]
[CSharpLanguageLevel(CSharpLanguageLevel.CSharp110)]
public sealed class QuickFixAvailabilityTests : QuickFixAvailabilityTestBase
{
    protected override string RelativeTestDataPath => @"Analyzers\BaseTypes\InterpolatedStringExpression\QuickFixes";

    protected override bool HighlightingPredicate(IHighlighting highlighting, IPsiSourceFile sourceFile, IContextBoundSettingsStore settingsStore)
        => highlighting is RedundantFormatSpecifierHint
                or RedundantFormatPrecisionSpecifierHint
                or PassOtherFormatSpecifierSuggestion
                or SuspiciousFormatSpecifierWarning
            || highlighting.IsError();

    [Test]
    [TestNet70]
    public void TestRemoveFormatSpecifierAvailability() => DoNamedTest2();

    [Test]
    [TestNet80]
    public void TestRemoveFormatPrecisionSpecifierAvailability() => DoNamedTest2();

    [Test]
    public void TestPassOtherFormatSpecifierAvailability() => DoNamedTest2();
}