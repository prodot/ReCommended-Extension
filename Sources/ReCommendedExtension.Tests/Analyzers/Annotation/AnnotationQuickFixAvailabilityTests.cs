using JetBrains.Application.Settings;
using JetBrains.ProjectModel.Properties.CSharp;
using JetBrains.ReSharper.Feature.Services.Daemon;
using JetBrains.ReSharper.FeaturesTestFramework.Intentions;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.CSharp;
using JetBrains.ReSharper.TestFramework;
using NUnit.Framework;
using ReCommendedExtension.Analyzers.Annotation;

namespace ReCommendedExtension.Tests.Analyzers.Annotation;

[TestFixture]
[TestNetCore30(ANNOTATIONS_PACKAGE)]
public sealed class AnnotationQuickFixAvailabilityTests : QuickFixAvailabilityTestBase
{
    protected override string RelativeTestDataPath => @"Analyzers\Annotation\QuickFixes";

    protected override bool HighlightingPredicate(IHighlighting highlighting, IPsiSourceFile sourceFile, IContextBoundSettingsStore settingsStore)
        => highlighting is RedundantNullableAnnotationHint
                or NotAllowedAnnotationWarning
                or ConflictingAnnotationWarning
                or RedundantAnnotationSuggestion
                or InvalidValueRangeBoundaryWarning
            || highlighting.IsError();

    [Test]
    public void TestAnnotationAvailability() => DoNamedTest2();

    [Test]
    public void TestCoerceValueRangeBoundaryAvailability() => DoNamedTest2();

    [Test]
    [CSharpLanguageLevel(CSharpLanguageLevel.CSharp80)]
    [NullableContext(NullableContextKind.Enable)]
    public void TestRedundantNullableAnnotationAvailability() => DoNamedTest2();
}