using JetBrains.Application.Settings;
using JetBrains.ReSharper.Feature.Services.Daemon;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.TestFramework;
using NUnit.Framework;
using ReCommendedExtension.Highlightings;

namespace ReCommendedExtension.Tests.Analyzers
{
    [TestNetFramework45]
    [TestFixture]
    public sealed class AwaitAnalyzerTestsRedundantAwaitQuickFixAvailabilityTests : QuickFixAvailabilityTestBaseWithAnnotationAssemblyReference
    {
        protected override string RelativeTestDataPath => @"Analyzers\AwaitQuickFixes";

        protected override bool HighlightingPredicate(IHighlighting highlighting, IPsiSourceFile sourceFile, IContextBoundSettingsStore settingsStore)
            => highlighting is RedundantAwaitHighlighting;

        [Test]
        public void TestRedundantAwaitAvailability() => DoNamedTest2();
    }
}