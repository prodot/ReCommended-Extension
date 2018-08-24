using JetBrains.Application.Settings;
using JetBrains.ReSharper.Feature.Services.Daemon;
using JetBrains.ReSharper.Psi;
using NUnit.Framework;
using ReCommendedExtension.Highlightings;

namespace ReCommendedExtension.Tests.Analyzers
{
    [TestFixture]
    public sealed class CatchClauseWithoutVariableQuickFixAvailabilityTests : QuickFixAvailabilityTestBaseWithAnnotationAssemblyReference
    {
        protected override string RelativeTestDataPath => @"Analyzers\CatchClauseWithoutVariableQuickFixes";

        protected override bool HighlightingPredicate(IHighlighting highlighting, IPsiSourceFile sourceFile, IContextBoundSettingsStore settingsStore)
            => highlighting is CatchClauseWithoutVariableHighlighting;

        [Test]
        public void TestCatchClauseWithoutVariableAvailability() => DoNamedTest2();
    }
}