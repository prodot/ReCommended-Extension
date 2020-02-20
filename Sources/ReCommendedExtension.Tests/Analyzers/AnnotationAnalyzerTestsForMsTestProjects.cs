using JetBrains.Application.Settings;
using JetBrains.ReSharper.Feature.Services.Daemon;
using JetBrains.ReSharper.FeaturesTestFramework.Daemon;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.TestFramework;
using NUnit.Framework;
using ReCommendedExtension.Analyzers.Annotation;

namespace ReCommendedExtension.Tests.Analyzers
{
    [TestFixture]
    [TestPackages("MSTest.TestFramework")]
    public sealed class AnnotationAnalyzerTestsForMsTestProjects : CSharpHighlightingTestBase
    {
        protected override string RelativeTestDataPath => @"Analyzers\Annotation";

        protected override bool HighlightingPredicate(IHighlighting highlighting, IPsiSourceFile sourceFile, IContextBoundSettingsStore settingsStore)
            => highlighting is MissingSuppressionJustificationWarning;

        [Test]
        public void TestSuppressMessage_TestProject() => DoNamedTest2();
    }
}