using JetBrains.ReSharper.Feature.Services.Daemon;
using JetBrains.ReSharper.TestFramework;
using NUnit.Framework;
using ReCommendedExtension.Analyzers.Annotation;

namespace ReCommendedExtension.Tests.Analyzers.Annotation;

[TestFixture]
[TestNetCore31("MSTest.TestFramework")]
public sealed class AnnotationAnalyzerTestsForMsTestProjects : CSharpAnalyzerTests
{
    protected override string RelativeTestDataPath => @"Analyzers\Annotation";

    protected override bool UseHighlighting(IHighlighting highlighting) => highlighting is MissingSuppressionJustificationWarning;

    [Test]
    public void TestSuppressMessage_TestProject() => DoNamedTest2();

    [Test]
    [TestNet50("MSTest.TestFramework")]
    public void TestSuppressMessage_TestProject_NET_5() => DoNamedTest2();
}