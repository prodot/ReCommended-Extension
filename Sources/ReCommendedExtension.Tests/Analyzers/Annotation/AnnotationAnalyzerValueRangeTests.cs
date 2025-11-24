using JetBrains.Application.Settings;
using JetBrains.ProjectModel.Properties.CSharp;
using JetBrains.ReSharper.Feature.Services.Daemon;
using JetBrains.ReSharper.FeaturesTestFramework.Daemon;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.TestFramework;
using NUnit.Framework;
using ReCommendedExtension.Analyzers.Annotation;

namespace ReCommendedExtension.Tests.Analyzers.Annotation;

[TestFixture]
[NullableContext(NullableContextKind.Enable)]
[TestNetCore30(ANNOTATIONS_PACKAGE)]
public sealed class AnnotationAnalyzerValueRangeTests : CSharpHighlightingTestBase
{
    protected override string RelativeTestDataPath => @"Analyzers\Annotation";

    protected override bool HighlightingPredicate(IHighlighting highlighting, IPsiSourceFile sourceFile, IContextBoundSettingsStore settingsStore)
        => highlighting is RedundantAnnotationSuggestion or NotAllowedAnnotationWarning or InvalidValueRangeBoundaryWarning or { IsError: true };

    [Test]
    public void TestValueRange() => DoNamedTest2();
}