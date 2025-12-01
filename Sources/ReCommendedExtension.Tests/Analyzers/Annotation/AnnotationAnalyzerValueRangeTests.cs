using JetBrains.ProjectModel.Properties.CSharp;
using JetBrains.ReSharper.Feature.Services.Daemon;
using JetBrains.ReSharper.TestFramework;
using NUnit.Framework;
using ReCommendedExtension.Analyzers.Annotation;

namespace ReCommendedExtension.Tests.Analyzers.Annotation;

[TestFixture]
[NullableContext(NullableContextKind.Enable)]
[TestNetCore30(ANNOTATIONS_PACKAGE)]
public sealed class AnnotationAnalyzerValueRangeTests : CSharpAnalyzerTests
{
    protected override string RelativeTestDataPath => @"Analyzers\Annotation";

    protected override bool UseHighlighting(IHighlighting highlighting)
        => highlighting is RedundantAnnotationSuggestion or NotAllowedAnnotationWarning or InvalidValueRangeBoundaryWarning;

    [Test]
    public void TestValueRange() => DoNamedTest2();
}