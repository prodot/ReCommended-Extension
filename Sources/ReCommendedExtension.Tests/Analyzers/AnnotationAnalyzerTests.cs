using System.Diagnostics.CodeAnalysis;
using JetBrains.Application.Settings;
using JetBrains.ProjectModel.Properties.CSharp;
using JetBrains.ReSharper.Feature.Services.Daemon;
using JetBrains.ReSharper.FeaturesTestFramework.Daemon;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.ControlFlow;
using JetBrains.ReSharper.TestFramework;
using NUnit.Framework;
using ReCommendedExtension.Analyzers.Annotation;

namespace ReCommendedExtension.Tests.Analyzers
{
    [TestNetFramework45]
    [TestFixture]
    public sealed class AnnotationAnalyzerTests : CSharpHighlightingTestBase
    {
        protected override string RelativeTestDataPath => @"Analyzers\Annotation";

        protected override bool HighlightingPredicate(IHighlighting highlighting, IPsiSourceFile sourceFile, IContextBoundSettingsStore settingsStore)
            => highlighting is RedundantAnnotationHighlighting ||
                highlighting is NotAllowedAnnotationHighlighting ||
                highlighting is MissingAnnotationHighlighting ||
                highlighting is MissingSuppressionJustificationHighlighting ||
                highlighting is ConflictingAnnotationHighlighting;

        [Test]
        public void TestAsyncMethod() => DoNamedTest2();

        [Test]
        public void TestIteratorMethod() => DoNamedTest2();

        [Test]
        [NullableContext(NullableContextKind.Enable)]
        public void TestIteratorMethod_NullableContext() => DoNamedTest2();

        [Test]
        public void TestSuppressMessage() => DoNamedTest2();

        [Test]
        public void TestPureWithMustUseReturnValue() => DoNamedTest2();

        [TestCase("Other_Pessimistic.cs", ValueAnalysisMode.PESSIMISTIC)]
        [TestCase("Other_Optimistic.cs", ValueAnalysisMode.OPTIMISTIC)]
        [TestCase("Override.cs", ValueAnalysisMode.PESSIMISTIC)]
        [TestCase("ItemNotNull.cs", ValueAnalysisMode.PESSIMISTIC)]
        [SuppressMessage("ReSharper", "AssignNullToNotNullAttribute")]
        public void TestFileWithValueAnalysisMode(string file, ValueAnalysisMode valueAnalysisMode)
            => ExecuteWithinSettingsTransaction(
                store =>
                {
                    RunGuarded(() => store.SetValue<HighlightingSettings, ValueAnalysisMode>(s => s.ValueAnalysisMode, valueAnalysisMode));

                    DoTestSolution(file);
                });

        [TestCase("Other_Optimistic_NullableContext.cs", ValueAnalysisMode.OPTIMISTIC)]
        [NullableContext(NullableContextKind.Enable)]
        [SuppressMessage("ReSharper", "AssignNullToNotNullAttribute")]
        public void TestFileWithValueAnalysisMode_NullableContext(string file, ValueAnalysisMode valueAnalysisMode)
            => ExecuteWithinSettingsTransaction(
                store =>
                {
                    RunGuarded(() => store.SetValue<HighlightingSettings, ValueAnalysisMode>(s => s.ValueAnalysisMode, valueAnalysisMode));

                    DoTestSolution(file);
                });
    }
}