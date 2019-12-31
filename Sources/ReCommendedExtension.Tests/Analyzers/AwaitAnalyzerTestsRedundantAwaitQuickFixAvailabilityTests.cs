using JetBrains.Application.Settings;
using JetBrains.ReSharper.Feature.Services.Daemon;
using JetBrains.ReSharper.FeaturesTestFramework.Intentions;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.TestFramework;
using NUnit.Framework;
using ReCommendedExtension.Analyzers.Await;

namespace ReCommendedExtension.Tests.Analyzers
{
    [TestNetFramework45]
    [TestFixture]
    public sealed class AwaitAnalyzerTestsRedundantAwaitQuickFixAvailabilityTests : QuickFixAvailabilityTestBase
    {
        protected override string RelativeTestDataPath => @"Analyzers\AwaitQuickFixes";

        protected override bool HighlightingPredicate(IHighlighting highlighting, IPsiSourceFile sourceFile, IContextBoundSettingsStore settingsStore)
            => highlighting is RedundantAwaitSuggestion;

        [Test]
        public void TestRedundantAwaitAvailability() => DoNamedTest2();
    }
}