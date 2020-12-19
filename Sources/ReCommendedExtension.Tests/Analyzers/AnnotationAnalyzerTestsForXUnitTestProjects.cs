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
    public sealed class AnnotationAnalyzerTestsForXUnitTestProjects : CSharpHighlightingTestBase
    {
        protected override string RelativeTestDataPath => @"Analyzers\Annotation";

        protected override bool HighlightingPredicate(IHighlighting highlighting, IPsiSourceFile sourceFile, IContextBoundSettingsStore settingsStore)
            => highlighting is MissingSuppressionJustificationWarning;

        [Test]
        [TestPackages("xunit.core")]
        public void TestSuppressMessage_TestProject() => DoNamedTest2();

        [Test]
        [TestNet50("xunit.core")]
        public void TestSuppressMessage_TestProject_NET_5() => DoNamedTest2();
    }
}