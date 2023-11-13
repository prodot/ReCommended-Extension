using JetBrains.Application.Settings;
using JetBrains.ReSharper.Feature.Services.Daemon;
using JetBrains.ReSharper.FeaturesTestFramework.Intentions;
using JetBrains.ReSharper.Psi;
using NUnit.Framework;
using ReCommendedExtension.Analyzers.Annotation;

namespace ReCommendedExtension.Tests.Analyzers.Annotation;

[TestFixture]
public sealed class AttributeUsageAnnotationQuickFixAvailabilityTests : QuickFixAvailabilityTestBase
{
    protected override string RelativeTestDataPath => @"Analyzers\AnnotationQuickFixes";

    protected override bool HighlightingPredicate(IHighlighting highlighting, IPsiSourceFile sourceFile, IContextBoundSettingsStore settingsStore)
        => highlighting is MissingAttributeUsageAnnotationWarning;

    [Test]
    public void TestAnnotateWithAttributeUsageAvailability() => DoNamedTest2();
}