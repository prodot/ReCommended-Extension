using JetBrains.Application.Settings;
using JetBrains.ReSharper.Feature.Services.Daemon;
using JetBrains.ReSharper.FeaturesTestFramework.Intentions;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.CSharp;
using JetBrains.ReSharper.TestFramework;
using NUnit.Framework;
using ReCommendedExtension.Analyzers.Argument;

namespace ReCommendedExtension.Tests.Analyzers.Argument;

[TestFixture]
public sealed class QuickFixAvailabilityTests : QuickFixAvailabilityTestBase
{
    protected override string RelativeTestDataPath => @"Analyzers\Argument\QuickFixes";

    protected override bool HighlightingPredicate(IHighlighting highlighting, IPsiSourceFile sourceFile, IContextBoundSettingsStore settingsStore)
        => highlighting is RedundantArgumentHint
            or RedundantArgumentRangeHint
            or RedundantElementHint
            or UseOtherArgumentSuggestion
            or UseOtherArgumentRangeSuggestion
            or { IsError: true };

    [Test]
    public void TestRemoveArgumentFixAvailability() => DoNamedTest2();

    [Test]
    public void TestRemoveArgumentRangeFixAvailability() => DoNamedTest2();

    [Test]
    [CSharpLanguageLevel(CSharpLanguageLevel.CSharp120)]
    [TestNetCore21]
    public void TestRemoveElementFixAvailability() => DoNamedTest2();

    [Test]
    [CSharpLanguageLevel(CSharpLanguageLevel.CSharp120)]
    [TestNetCore21]
    public void TestUseOtherArgumentFixAvailability() => DoNamedTest2();

    [Test]
    [TestNetCore21]
    public void TestUseOtherArgumentRangeFixAvailability() => DoNamedTest2();
}