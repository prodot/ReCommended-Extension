using JetBrains.ReSharper.Feature.Services.Daemon;
using JetBrains.ReSharper.Psi;
using NUnit.Framework;
using ReCommendedExtension.Highlightings;

namespace ReCommendedExtension.Tests.Analyzers
{
    [TestFixture]
    public sealed class LocalSuppressionAnalyzerTests : HighlightingTestBaseWithAnnotationAssemblyReference
    {
        protected override string RelativeTestDataPath => @"Analyzers\LocalSuppression";

        protected override bool HighlightingPredicate(IHighlighting highlighting, IPsiSourceFile sourceFile)
            => highlighting is LocalSuppressionHighlighting;

        [Test]
        public void TestLocalSuppression() => DoNamedTest2();
    }
}