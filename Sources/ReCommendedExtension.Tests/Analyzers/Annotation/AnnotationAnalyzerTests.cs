using JetBrains.Application.Settings;
using JetBrains.ProjectModel.Properties.CSharp;
using JetBrains.ReSharper.Daemon.CSharp.Errors;
using JetBrains.ReSharper.Feature.Services.Daemon;
using JetBrains.ReSharper.Psi.ControlFlow;
using JetBrains.ReSharper.Psi.CSharp;
using JetBrains.ReSharper.TestFramework;
using JetBrains.TestFramework.Projects;
using NUnit.Framework;
using ReCommendedExtension.Analyzers.Annotation;

namespace ReCommendedExtension.Tests.Analyzers.Annotation;

[TestFixture]
public sealed class AnnotationAnalyzerTests : CSharpAnalyzerTests
{
    protected override string RelativeTestDataPath => @"Analyzers\Annotation";

    protected override bool UseHighlighting(IHighlighting highlighting)
        => highlighting is RedundantNullableAnnotationHint
            or RedundantAnnotationSuggestion
            or NotAllowedAnnotationWarning
            or MissingAnnotationWarning
            or MissingSuppressionJustificationWarning
            or ConflictingAnnotationWarning
            or InParameterWithMustDisposeResourceAttributeWarning // to figure out which cases are supported by R#
            or ReturnTypeCanBeNotNullableWarning; // to figure out which cases are supported by R#

    [Test]
    [TestNetFramework45]
    public void TestAsyncMethod() => DoNamedTest2();

    [Test]
    [TestNetFramework45]
    public void TestIteratorMethod() => DoNamedTest2();

    [Test]
    [TestNetCore30(ANNOTATIONS_PACKAGE)]
    [NullableContext(NullableContextKind.Disable)]
    public void TestAsyncIteratorMethod() => DoNamedTest2();

    [Test]
    [TestNetFramework45]
    public void TestSuppressMessage() => DoNamedTest2();

    [Test]
    [TestNet50]
    public void TestSuppressMessage_NET_5() => DoNamedTest2();

    [TestCase("Other_Pessimistic.cs", ValueAnalysisMode.PESSIMISTIC)]
    [TestCase("Other_Optimistic.cs", ValueAnalysisMode.OPTIMISTIC)]
    [TestCase("Override.cs", ValueAnalysisMode.PESSIMISTIC)]
    [TestCase("ItemNotNull.cs", ValueAnalysisMode.PESSIMISTIC)]
    [CSharpLanguageLevel(CSharpLanguageLevel.CSharp73)]
    [TestNetFramework45]
    public void TestFileWithValueAnalysisMode(string file, ValueAnalysisMode valueAnalysisMode)
        => ExecuteWithinSettingsTransaction(store =>
        {
            RunGuarded(() => store.SetValue<HighlightingSettings, ValueAnalysisMode>(s => s.ValueAnalysisMode, valueAnalysisMode));

            DoTestSolution(file);
        });

    [Test]
    [NullableContext(NullableContextKind.Enable)]
    [TestNetCore30(ANNOTATIONS_PACKAGE)]
    public void TestNullableAnnotationContext() => DoNamedTest2();

    [Test]
    [NullableContext(NullableContextKind.Enable)]
    [TestNetCore30(ANNOTATIONS_PACKAGE)]
    public void TestNonNegativeValue() => DoNamedTest2();

    [Test]
    public void TestAttributeUsage() => DoNamedTest2();

    [Test]
    [NullableContext(NullableContextKind.Enable)]
    public void TestEditorBrowsable() => DoNamedTest2();

    [Test]
    [CSharpLanguageLevel(CSharpLanguageLevel.CSharp120)]
    [NullableContext(NullableContextKind.Enable)]
    [TestNet80("JetBrains.Annotations/2023.3.0")] // structs cannot be annotated with [MustDisposeResource]
    [ReuseSolution(false)] // prevents reusing cached packages
    public void TestPurityAndDisposability_Types_Legacy() => DoNamedTest2();

    [Test]
    [CSharpLanguageLevel(CSharpLanguageLevel.CSharp120)]
    [NullableContext(NullableContextKind.Enable)]
    [TestNet80(ANNOTATIONS_PACKAGE)]
    public void TestPurityAndDisposability_Types() => DoNamedTest2();

    [Test]
    [CSharpLanguageLevel(CSharpLanguageLevel.CSharp120)]
    [NullableContext(NullableContextKind.Enable)]
    [TestNet80("JetBrains.Annotations/2023.3.0")] // structs cannot be annotated with [MustDisposeResource]
    [ReuseSolution(false)] // prevents reusing cached packages
    public void TestPurityAndDisposability_Constructors_Legacy() => DoNamedTest2();

    [Test]
    [CSharpLanguageLevel(CSharpLanguageLevel.CSharp120)]
    [NullableContext(NullableContextKind.Enable)]
    [TestNet80(ANNOTATIONS_PACKAGE)]
    public void TestPurityAndDisposability_Constructors() => DoNamedTest2();

    [Test]
    [CSharpLanguageLevel(CSharpLanguageLevel.CSharp120)]
    [NullableContext(NullableContextKind.Enable)]
    [TestNet80("JetBrains.Annotations/2023.3.0")] // structs cannot be annotated with [MustDisposeResource]
    [ReuseSolution(false)] // prevents reusing cached packages
    public void TestPurityAndDisposability_PrimaryConstructors_Legacy() => DoNamedTest2();

    [Test]
    [CSharpLanguageLevel(CSharpLanguageLevel.CSharp120)]
    [NullableContext(NullableContextKind.Enable)]
    [TestNet80(ANNOTATIONS_PACKAGE)]
    public void TestPurityAndDisposability_PrimaryConstructors() => DoNamedTest2();

    [Test]
    [CSharpLanguageLevel(CSharpLanguageLevel.CSharp120)]
    [NullableContext(NullableContextKind.Enable)]
    [TestNet80(ANNOTATIONS_PACKAGE)]
    public void TestPurityAndDisposability_Methods() => DoNamedTest2();

    [Test]
    [CSharpLanguageLevel(CSharpLanguageLevel.CSharp120)]
    [NullableContext(NullableContextKind.Enable)]
    [TestNet80(ANNOTATIONS_PACKAGE)]
    public void TestPurityAndDisposability_LocalFunctions() => DoNamedTest2();

    [Test]
    [CSharpLanguageLevel(CSharpLanguageLevel.CSharp120)]
    [NullableContext(NullableContextKind.Enable)]
    [TestNet80(ANNOTATIONS_PACKAGE)]
    public void TestPurityAndDisposability_Parameters() => DoNamedTest2();

    [Test]
    [CSharpLanguageLevel(CSharpLanguageLevel.CSharp120)]
    [NullableContext(NullableContextKind.Enable)]
    [TestNet80(ANNOTATIONS_PACKAGE)]
    public void TestPurityAndDisposability_DisposableOverride() => DoNamedTest2();

    [Test]
    [CSharpLanguageLevel(CSharpLanguageLevel.CSharp120)]
    [NullableContext(NullableContextKind.Enable)]
    [TestNet80(ANNOTATIONS_PACKAGE)]
    public void TestDisposalHandling_Methods() => DoNamedTest2();

    [Test]
    [CSharpLanguageLevel(CSharpLanguageLevel.CSharp120)]
    [NullableContext(NullableContextKind.Enable)]
    [TestNet80(ANNOTATIONS_PACKAGE)]
    public void TestDisposalHandling_Parameters() => DoNamedTest2();

    [Test]
    [CSharpLanguageLevel(CSharpLanguageLevel.CSharp120)]
    [NullableContext(NullableContextKind.Enable)]
    [TestNet80(ANNOTATIONS_PACKAGE)]
    public void TestDisposalHandling_Properties() => DoNamedTest2();

    [Test]
    [CSharpLanguageLevel(CSharpLanguageLevel.CSharp120)]
    [NullableContext(NullableContextKind.Enable)]
    [TestNet80(ANNOTATIONS_PACKAGE)]
    public void TestDisposalHandling_Fields() => DoNamedTest2();

    [Test]
    [CSharpLanguageLevel(CSharpLanguageLevel.CSharp90)]
    [NullableContext(NullableContextKind.Enable)]
    [TestNetCore30(ANNOTATIONS_PACKAGE)]
    public void TestRedundantNullableAnnotations() => DoNamedTest2();
}