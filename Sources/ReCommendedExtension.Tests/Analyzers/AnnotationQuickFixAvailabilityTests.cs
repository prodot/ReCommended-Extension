using JetBrains.Application.Settings;
using JetBrains.ReSharper.Feature.Services.Daemon;
using JetBrains.ReSharper.FeaturesTestFramework.Intentions;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.TestFramework;
using NUnit.Framework;
using ReCommendedExtension.Analyzers.Annotation;

namespace ReCommendedExtension.Tests.Analyzers
{
    [TestNetFramework45]
    [TestPackagesWithAnnotations]
    [TestFixture]
    public sealed class AnnotationQuickFixAvailabilityTests : QuickFixAvailabilityTestBase
    {
        protected override string RelativeTestDataPath => @"Analyzers\AnnotationQuickFixes";

        protected override bool HighlightingPredicate(IHighlighting highlighting, IPsiSourceFile sourceFile, IContextBoundSettingsStore settingsStore)
            => highlighting is NotAllowedAnnotationWarning ||
                highlighting is ConflictingAnnotationWarning ||
                highlighting is RedundantAnnotationSuggestion;

        [Test]
        public void TestAnnotationAvailability() => DoNamedTest2();
    }
}