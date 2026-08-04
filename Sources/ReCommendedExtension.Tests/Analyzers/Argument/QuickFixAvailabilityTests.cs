using JetBrains.ReSharper.Feature.Services.Daemon;
using JetBrains.ReSharper.Psi.CSharp;
using JetBrains.ReSharper.TestFramework;
using NUnit.Framework;
using ReCommendedExtension.Analyzers.Argument;

namespace ReCommendedExtension.Tests.Analyzers.Argument;

[TestFixture]
public sealed class QuickFixAvailabilityTests : ReCommendedExtension.Tests.Analyzers.QuickFixAvailabilityTests
{
    protected override string RelativeTestDataPath => @"Analyzers\Argument\QuickFixes";

    protected override bool UseHighlighting(IHighlighting highlighting)
        => highlighting is RedundantArgumentHint
            or RedundantArgumentRangeHint
            or RedundantElementHint
            or UseOtherArgumentSuggestion
            or UseOtherArgumentRangeSuggestion;

    [Test]
    public void RemoveArgumentFixAvailability() => DoNamedTest();

    [Test]
    public void RemoveArgumentRangeFixAvailability() => DoNamedTest();

    [Test]
    [CSharpLanguageLevel(CSharpLanguageLevel.CSharp120)]
    [TestNetCore21]
    public void RemoveElementFixAvailability() => DoNamedTest();

    [Test]
    [CSharpLanguageLevel(CSharpLanguageLevel.CSharp120)]
    [TestNetCore21]
    public void UseOtherArgumentFixAvailability() => DoNamedTest();

    [Test]
    [TestNetCore21]
    public void UseOtherArgumentRangeFixAvailability() => DoNamedTest();
}