using JetBrains.ProjectModel.Properties.CSharp;
using JetBrains.ReSharper.Feature.Services.Daemon;
using JetBrains.ReSharper.Psi.CSharp;
using JetBrains.ReSharper.TestFramework;
using NUnit.Framework;
using ReCommendedExtension.Analyzers.MemberInvocation;

namespace ReCommendedExtension.Tests.Analyzers.MemberInvocation;

[TestFixture]
public sealed class QuickFixAvailabilityTests : ReCommendedExtension.Tests.Analyzers.QuickFixAvailabilityTests
{
    protected override string RelativeTestDataPath => @"Analyzers\MemberInvocation\QuickFixes";

    protected override bool UseHighlighting(IHighlighting highlighting)
        => highlighting is RedundantMethodInvocationHint
            or UseOtherMethodSuggestion
            or UseBinaryOperatorSuggestion
            or UseUnaryOperatorSuggestion
            or UsePatternSuggestion
            or UseNullableHasValueAlternativeSuggestion
            or ReplaceNullableValueWithTypeCastSuggestion
            or UseRangeIndexerSuggestion
            or UsePropertySuggestion
            or UseStaticPropertySuggestion;

    [Test]
    [TestNetCore21]
    public void RemoveMethodInvocationFixAvailability() => DoNamedTest();

    [Test]
    [CSharpLanguageLevel(CSharpLanguageLevel.CSharp120)]
    [NullableContext(NullableContextKind.Enable)]
    [TestNetCore21]
    public void UseOtherMethodFixAvailability() => DoNamedTest();

    [Test]
    [CSharpLanguageLevel(CSharpLanguageLevel.CSharp130)]
    [TestNet70]
    public void UseBinaryOperatorFixAvailability() => DoNamedTest();

    [Test]
    [CSharpLanguageLevel(CSharpLanguageLevel.CSharp130)]
    [TestNetCore21]
    public void UseUnaryOperatorFixAvailability() => DoNamedTest();

    [Test]
    [CSharpLanguageLevel(CSharpLanguageLevel.CSharp110)]
    [NullableContext(NullableContextKind.Enable)]
    [TestNet70]
    public void UsePatternFixAvailability() => DoNamedTest();

    [Test]
    [CSharpLanguageLevel(CSharpLanguageLevel.CSharp90)]
    [TestNetCore20]
    public void UseNullableHasValueAlternativeFixAvailability() => DoNamedTest();

    [Test]
    public void ReplaceNullableValueWithTypeCastFixAvailability() => DoNamedTest();

    [Test]
    [CSharpLanguageLevel(CSharpLanguageLevel.CSharp80)]
    [NullableContext(NullableContextKind.Enable)]
    [TestNetCore30]
    public void UseRangeIndexerAvailability() => DoNamedTest();

    [Test]
    [CSharpLanguageLevel(CSharpLanguageLevel.CSharp90)]
    [TestNet50]
    public void UsePropertyAvailability() => DoNamedTest();

    [Test]
    [CSharpLanguageLevel(CSharpLanguageLevel.CSharp60)]
    public void UseStaticPropertyAvailability() => DoNamedTest();
}