using JetBrains.ReSharper.Feature.Services.Daemon;
using JetBrains.ReSharper.Psi.CSharp;
using JetBrains.ReSharper.TestFramework;
using JetBrains.TestFramework.Projects;
using NUnit.Framework;
using ReCommendedExtension.Analyzers.Annotation;

namespace ReCommendedExtension.Tests.Analyzers.Annotation;

[TestFixture]
[CSharpLanguageLevel(CSharpLanguageLevel.CSharp120)]
public sealed class AnnotationAnalyzerArgumentTests : CSharpAnalyzerTests
{
    protected override string RelativeTestDataPath => @"Analyzers\Annotation";

    protected override bool UseHighlighting(IHighlighting highlighting) => highlighting is RedundantAnnotationArgumentSuggestion;

    [Test]
    [TestNet80("JetBrains.Annotations/2023.3.0")] // structs cannot be annotated with [MustDisposeResource]
    [ReuseSolution(false)] // prevents reusing cached packages
    public void TestRedundantAnnotationArgument_Legacy() => DoNamedTest2();

    [Test]
    [TestNet80(ANNOTATIONS_PACKAGE)]
    public void TestRedundantAnnotationArgument() => DoNamedTest2();
}