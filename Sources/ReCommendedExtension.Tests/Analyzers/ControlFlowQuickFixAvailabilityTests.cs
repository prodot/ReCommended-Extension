using JetBrains.Application.Settings;
using JetBrains.ReSharper.Feature.Services.Daemon;
using JetBrains.ReSharper.FeaturesTestFramework.Intentions;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.TestFramework;
using NUnit.Framework;
using ReCommendedExtension.Analyzers.ControlFlow;

namespace ReCommendedExtension.Tests.Analyzers
{
    [TestFixture]
    [TestPackagesWithAnnotations]
    public sealed class ControlFlowQuickFixAvailabilityTests : QuickFixAvailabilityTestBase
    {
        protected override string RelativeTestDataPath => @"Analyzers\ControlFlowQuickFixes";

        protected override bool HighlightingPredicate(IHighlighting highlighting, IPsiSourceFile sourceFile, IContextBoundSettingsStore settingsStore)
            => highlighting is RedundantAssertionStatementSuggestion;

        [Test]
        public void TestControlFlowAvailability() => DoNamedTest2();
    }
}