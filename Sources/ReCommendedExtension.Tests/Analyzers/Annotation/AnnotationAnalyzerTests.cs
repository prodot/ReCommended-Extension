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
    public void AsyncMethod() => DoNamedTest();

    [Test]
    [TestNetFramework45]
    public void IteratorMethod() => DoNamedTest();

    [Test]
    [TestNetCore30(ANNOTATIONS_PACKAGE)]
    [NullableContext(NullableContextKind.Disable)]
    public void AsyncIteratorMethod() => DoNamedTest();

    [Test]
    [TestNetFramework45]
    public void SuppressMessage() => DoNamedTest();

    [Test]
    [TestNet50]
    public void SuppressMessage_NET_5() => DoNamedTest();

    [TestCase("Other_Pessimistic.cs", ValueAnalysisMode.PESSIMISTIC)]
    [TestCase("Other_Optimistic.cs", ValueAnalysisMode.OPTIMISTIC)]
    [TestCase("Override.cs", ValueAnalysisMode.PESSIMISTIC)]
    [TestCase("ItemNotNull.cs", ValueAnalysisMode.PESSIMISTIC)]
    [CSharpLanguageLevel(CSharpLanguageLevel.CSharp73)]
    [TestNetFramework45]
    public void FileWithValueAnalysisMode(string file, ValueAnalysisMode valueAnalysisMode)
        => ExecuteWithinSettingsTransaction(store =>
        {
            RunGuarded(() => store.SetValue<HighlightingSettings, ValueAnalysisMode>(s => s.ValueAnalysisMode, valueAnalysisMode));

            DoTestSolution(file);
        });

    [Test]
    [NullableContext(NullableContextKind.Enable)]
    [TestNetCore30(ANNOTATIONS_PACKAGE)]
    public void NullableAnnotationContext() => DoNamedTest();

    [Test]
    [NullableContext(NullableContextKind.Enable)]
    [TestNetCore30(ANNOTATIONS_PACKAGE)]
    public void NonNegativeValue() => DoNamedTest();

    [Test]
    public void AttributeUsage() => DoNamedTest();

    [Test]
    [NullableContext(NullableContextKind.Enable)]
    public void EditorBrowsable() => DoNamedTest();

    [Test]
    [CSharpLanguageLevel(CSharpLanguageLevel.CSharp120)]
    [NullableContext(NullableContextKind.Enable)]
    [TestNet80("JetBrains.Annotations/2023.3.0")] // structs cannot be annotated with [MustDisposeResource]
    [ReuseSolution(false)] // prevents reusing cached packages
    public void PurityAndDisposability_Types_Legacy() => DoNamedTest();

    [Test]
    [CSharpLanguageLevel(CSharpLanguageLevel.CSharp120)]
    [NullableContext(NullableContextKind.Enable)]
    [TestNet80(ANNOTATIONS_PACKAGE)]
    public void PurityAndDisposability_Types() => DoNamedTest();

    [Test]
    [CSharpLanguageLevel(CSharpLanguageLevel.CSharp120)]
    [NullableContext(NullableContextKind.Enable)]
    [TestNet80("JetBrains.Annotations/2023.3.0")] // structs cannot be annotated with [MustDisposeResource]
    [ReuseSolution(false)] // prevents reusing cached packages
    public void PurityAndDisposability_Constructors_Legacy() => DoNamedTest();

    [Test]
    [CSharpLanguageLevel(CSharpLanguageLevel.CSharp120)]
    [NullableContext(NullableContextKind.Enable)]
    [TestNet80(ANNOTATIONS_PACKAGE)]
    public void PurityAndDisposability_Constructors() => DoNamedTest();

    [Test]
    [CSharpLanguageLevel(CSharpLanguageLevel.CSharp120)]
    [NullableContext(NullableContextKind.Enable)]
    [TestNet80("JetBrains.Annotations/2023.3.0")] // structs cannot be annotated with [MustDisposeResource]
    [ReuseSolution(false)] // prevents reusing cached packages
    public void PurityAndDisposability_PrimaryConstructors_Legacy() => DoNamedTest();

    [Test]
    [CSharpLanguageLevel(CSharpLanguageLevel.CSharp120)]
    [NullableContext(NullableContextKind.Enable)]
    [TestNet80(ANNOTATIONS_PACKAGE)]
    public void PurityAndDisposability_PrimaryConstructors() => DoNamedTest();

    [Test]
    [CSharpLanguageLevel(CSharpLanguageLevel.CSharp120)]
    [NullableContext(NullableContextKind.Enable)]
    [TestNet80(ANNOTATIONS_PACKAGE)]
    public void PurityAndDisposability_Methods() => DoNamedTest();

    [Test]
    [CSharpLanguageLevel(CSharpLanguageLevel.CSharp120)]
    [NullableContext(NullableContextKind.Enable)]
    [TestNet80(ANNOTATIONS_PACKAGE)]
    public void PurityAndDisposability_LocalFunctions() => DoNamedTest();

    [Test]
    [CSharpLanguageLevel(CSharpLanguageLevel.CSharp120)]
    [NullableContext(NullableContextKind.Enable)]
    [TestNet80(ANNOTATIONS_PACKAGE)]
    public void PurityAndDisposability_Parameters() => DoNamedTest();

    [Test]
    [CSharpLanguageLevel(CSharpLanguageLevel.CSharp120)]
    [NullableContext(NullableContextKind.Enable)]
    [TestNet80(ANNOTATIONS_PACKAGE)]
    public void PurityAndDisposability_DisposableOverride() => DoNamedTest();

    [Test]
    [CSharpLanguageLevel(CSharpLanguageLevel.CSharp120)]
    [NullableContext(NullableContextKind.Enable)]
    [TestNet80(ANNOTATIONS_PACKAGE)]
    public void DisposalHandling_Methods() => DoNamedTest();

    [Test]
    [CSharpLanguageLevel(CSharpLanguageLevel.CSharp120)]
    [NullableContext(NullableContextKind.Enable)]
    [TestNet80(ANNOTATIONS_PACKAGE)]
    public void DisposalHandling_Parameters() => DoNamedTest();

    [Test]
    [CSharpLanguageLevel(CSharpLanguageLevel.CSharp120)]
    [NullableContext(NullableContextKind.Enable)]
    [TestNet80(ANNOTATIONS_PACKAGE)]
    public void DisposalHandling_Properties() => DoNamedTest();

    [Test]
    [CSharpLanguageLevel(CSharpLanguageLevel.CSharp120)]
    [NullableContext(NullableContextKind.Enable)]
    [TestNet80(ANNOTATIONS_PACKAGE)]
    public void DisposalHandling_Fields() => DoNamedTest();

    [Test]
    [CSharpLanguageLevel(CSharpLanguageLevel.CSharp90)]
    [NullableContext(NullableContextKind.Enable)]
    [TestNetCore30(ANNOTATIONS_PACKAGE)]
    public void RedundantNullableAnnotations() => DoNamedTest();
}