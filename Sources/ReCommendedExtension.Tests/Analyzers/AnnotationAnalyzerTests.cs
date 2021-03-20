using JetBrains.Annotations;
using JetBrains.Application.Settings;
using JetBrains.ProjectModel.Properties.CSharp;
using JetBrains.ReSharper.Feature.Services.Daemon;
using JetBrains.ReSharper.FeaturesTestFramework.Daemon;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.ControlFlow;
using JetBrains.ReSharper.Psi.CSharp;
using JetBrains.ReSharper.TestFramework;
using NUnit.Framework;
using ReCommendedExtension.Analyzers.Annotation;

namespace ReCommendedExtension.Tests.Analyzers
{
    [TestFixture]
    public sealed class AnnotationAnalyzerTests : CSharpHighlightingTestBase
    {
        protected override string RelativeTestDataPath => @"Analyzers\Annotation";

        protected override bool HighlightingPredicate(IHighlighting highlighting, IPsiSourceFile sourceFile, IContextBoundSettingsStore settingsStore)
            => highlighting is RedundantAnnotationSuggestion ||
                highlighting is NotAllowedAnnotationWarning ||
                highlighting is MissingAnnotationWarning ||
                highlighting is MissingSuppressionJustificationWarning ||
                highlighting is ConflictingAnnotationWarning ||
                highlighting is InvalidValueRangeBoundaryWarning;

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

        [Test]
        [TestNetFramework45]
        public void TestPureWithMustUseReturnValue() => DoNamedTest2();

        [TestCase("Other_Pessimistic.cs", ValueAnalysisMode.PESSIMISTIC)]
        [TestCase("Other_Optimistic.cs", ValueAnalysisMode.OPTIMISTIC)]
        [TestCase("Override.cs", ValueAnalysisMode.PESSIMISTIC)]
        [TestCase("ItemNotNull.cs", ValueAnalysisMode.PESSIMISTIC)]
        [TestNetFramework45]
        [CSharpLanguageLevel(CSharpLanguageLevel.CSharp73)]
        public void TestFileWithValueAnalysisMode([NotNull] string file, ValueAnalysisMode valueAnalysisMode)
            => ExecuteWithinSettingsTransaction(
                store =>
                {
                    RunGuarded(() => store.SetValue<HighlightingSettings, ValueAnalysisMode>(s => s.ValueAnalysisMode, valueAnalysisMode));

                    DoTestSolution(file);
                });

        [Test]
        [TestNetCore30(ANNOTATIONS_PACKAGE)]
        [NullableContext(NullableContextKind.Enable)]
        public void TestNullableAnnotationContext() => DoNamedTest2();

        [Test]
        [TestNetCore30(ANNOTATIONS_PACKAGE)]
        [NullableContext(NullableContextKind.Enable)]
        public void TestNonNegativeValue() => DoNamedTest2();

        [Test]
        [TestNetCore30(ANNOTATIONS_PACKAGE)]
        [NullableContext(NullableContextKind.Enable)]
        public void TestValueRange() => DoNamedTest2();
    }
}