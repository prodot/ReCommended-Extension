using JetBrains.Application.Settings;
using JetBrains.ProjectModel.Properties.CSharp;
using JetBrains.ReSharper.Daemon.CSharp.Errors;
using JetBrains.ReSharper.Feature.Services.Daemon;
using JetBrains.ReSharper.FeaturesTestFramework.Intentions;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.CSharp;
using JetBrains.ReSharper.TestFramework;
using NUnit.Framework;
using ReCommendedExtension.Analyzers.Collection;

namespace ReCommendedExtension.Tests.Analyzers.Collection;

[TestFixture]
[CSharpLanguageLevel(CSharpLanguageLevel.CSharp80)]
public sealed class ArrayWithDefaultValuesInitializationQuickFixAvailabilityTests : QuickFixAvailabilityTestBase
{
    protected override string RelativeTestDataPath => @"Analyzers\CollectionQuickFixes";

    protected override bool HighlightingPredicate(IHighlighting highlighting, IPsiSourceFile sourceFile, IContextBoundSettingsStore settingsStore)
        => highlighting is ArrayWithDefaultValuesInitializationSuggestion or NotResolvedError;

    [Test]
    [CSharpLanguageLevel(CSharpLanguageLevel.CSharp110)]
    public void TestArrayWithDefaultValuesInitializationAvailability() => DoNamedTest2();

    [Test]
    [CSharpLanguageLevel(CSharpLanguageLevel.CSharp120)]
    public void TestArrayWithDefaultValuesInitializationAvailability_CS12() => DoNamedTest2();

    [Test]
    [NullableContext(NullableContextKind.Enable)]
    public void TestArrayWithDefaultValuesInitializationAvailabilityWithNullableAnnotations() => DoNamedTest2();

    [Test]
    [CSharpLanguageLevel(CSharpLanguageLevel.CSharp120)]
    [NullableContext(NullableContextKind.Enable)]
    public void TestArrayWithDefaultValuesInitializationAvailabilityWithNullableAnnotations_CS12() => DoNamedTest2();
}