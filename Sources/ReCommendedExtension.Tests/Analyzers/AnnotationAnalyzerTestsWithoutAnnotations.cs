using System.Diagnostics.CodeAnalysis;
using JetBrains.Application.Settings;
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
    public sealed class AnnotationAnalyzerTestsWithoutAnnotations : CSharpHighlightingTestBase
    {
        protected override string RelativeTestDataPath => @"Analyzers\Annotation";

        protected override bool HighlightingPredicate(IHighlighting highlighting, IPsiSourceFile sourceFile, IContextBoundSettingsStore settingsStore)
            => highlighting is MissingAnnotationHighlighting;

        [TestCase("WithoutAnnotations_Pessimistic.cs", ValueAnalysisMode.PESSIMISTIC)]
        [TestCase("WithoutAnnotations_Optimistic.cs", ValueAnalysisMode.OPTIMISTIC)]
        [TestCase("WithoutAnnotations_Pessimistic.cs", ValueAnalysisMode.OFF)]
        [SuppressMessage("ReSharper", "AssignNullToNotNullAttribute")]
        public void TestFileWithValueAnalysisMode(string file, ValueAnalysisMode valueAnalysisMode)
            => ExecuteWithinSettingsTransaction(
                store =>
                {
                    RunGuarded(() => store.SetValue<HighlightingSettings, ValueAnalysisMode>(s => s.ValueAnalysisMode, valueAnalysisMode));

                    DoTestSolution(file);
                });
    }
}