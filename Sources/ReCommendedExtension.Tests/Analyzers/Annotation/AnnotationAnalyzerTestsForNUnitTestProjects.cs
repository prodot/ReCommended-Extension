using JetBrains.ReSharper.Feature.Services.Daemon;
using JetBrains.ReSharper.TestFramework;
using NUnit.Framework;
using ReCommendedExtension.Analyzers.Annotation;

namespace ReCommendedExtension.Tests.Analyzers.Annotation;

[TestFixture]
[TestNetFramework46]
public sealed class AnnotationAnalyzerTestsForNUnitTestProjects : CSharpAnalyzerTests
{
    protected override string RelativeTestDataPath => @"Analyzers\Annotation";

    protected override bool UseHighlighting(IHighlighting highlighting) => highlighting is MissingSuppressionJustificationWarning;

    [Test]
    [TestPackages("nunit/3.13.3")]
    public void SuppressMessage_TestProject() => DoNamedTest();

    [Test]
    [TestNet50("nunit/3.13.3")]
    public void SuppressMessage_TestProject_NET_5() => DoNamedTest();
}