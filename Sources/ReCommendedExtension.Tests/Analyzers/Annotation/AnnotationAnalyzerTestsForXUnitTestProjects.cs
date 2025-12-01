using JetBrains.ReSharper.Feature.Services.Daemon;
using JetBrains.ReSharper.TestFramework;
using NUnit.Framework;
using ReCommendedExtension.Analyzers.Annotation;

namespace ReCommendedExtension.Tests.Analyzers.Annotation;

[TestFixture]
public sealed class AnnotationAnalyzerTestsForXUnitTestProjects : CSharpAnalyzerTests
{
    protected override string RelativeTestDataPath => @"Analyzers\Annotation";

    protected override bool UseHighlighting(IHighlighting highlighting) => highlighting is MissingSuppressionJustificationWarning;

    [Test]
    [TestPackages("xunit.core/2.4.1")]
    public void TestSuppressMessage_TestProject() => DoNamedTest2();

    [Test]
    [TestNet50("xunit.core/2.4.1")]
    public void TestSuppressMessage_TestProject_NET_5() => DoNamedTest2();
}