using JetBrains.ProjectModel.Properties.CSharp;
using JetBrains.ReSharper.Feature.Services.Daemon;
using JetBrains.ReSharper.Psi.CSharp;
using JetBrains.ReSharper.TestFramework;
using NUnit.Framework;
using ReCommendedExtension.Analyzers.Collection;

namespace ReCommendedExtension.Tests.Analyzers.Collection;

[TestFixture]
[CSharpLanguageLevel(CSharpLanguageLevel.CSharp80)]
public sealed class ArrayWithDefaultValuesInitializationQuickFixAvailabilityTests : QuickFixAvailabilityTests
{
    protected override string RelativeTestDataPath => @"Analyzers\Collection\QuickFixes";

    protected override bool UseHighlighting(IHighlighting highlighting) => highlighting is ArrayWithDefaultValuesInitializationSuggestion;

    [Test]
    [CSharpLanguageLevel(CSharpLanguageLevel.CSharp110)]
    public void ArrayWithDefaultValuesInitializationAvailability() => DoNamedTest();

    [Test]
    [CSharpLanguageLevel(CSharpLanguageLevel.CSharp120)]
    public void ArrayWithDefaultValuesInitializationAvailability_CS12() => DoNamedTest();

    [Test]
    [NullableContext(NullableContextKind.Enable)]
    public void ArrayWithDefaultValuesInitializationAvailabilityWithNullableAnnotations() => DoNamedTest();

    [Test]
    [CSharpLanguageLevel(CSharpLanguageLevel.CSharp120)]
    [NullableContext(NullableContextKind.Enable)]
    public void ArrayWithDefaultValuesInitializationAvailabilityWithNullableAnnotations_CS12() => DoNamedTest();
}