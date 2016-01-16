using JetBrains.ReSharper.Feature.Services.Daemon;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.TestFramework;
using NUnit.Framework;
using ReCommendedExtension.Highlightings;

namespace ReCommendedExtension.Tests.Analyzers
{
    [TestNetFramework45]
    [TestFixture]
    public sealed class AnnotationQuickFixAvailabilityTests : QuickFixAvailabilityTestBaseWithAnnotationAssemblyReference
    {
        protected override string RelativeTestDataPath => @"Analyzers\AnnotationQuickFixes";

        protected override bool HighlightingPredicate(IHighlighting highlighting, IPsiSourceFile sourceFile)
            =>
                highlighting is NotAllowedAnnotationHighlighting || highlighting is ConflictingAnnotationHighlighting ||
                highlighting is RedundantAnnotationHighlighting;

        [Test]
        public void TestAnnotationAvailability() => DoNamedTest2();
    }
}