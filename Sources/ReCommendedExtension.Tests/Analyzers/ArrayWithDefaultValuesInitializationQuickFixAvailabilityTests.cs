using JetBrains.Application.Settings;
using JetBrains.ReSharper.Feature.Services.Daemon;
using JetBrains.ReSharper.FeaturesTestFramework.Intentions;
using JetBrains.ReSharper.Psi;
using NUnit.Framework;
using ReCommendedExtension.Analyzers.ArrayWithDefaultValuesInitialization;

namespace ReCommendedExtension.Tests.Analyzers
{
    [TestFixture]
    public sealed class ArrayWithDefaultValuesInitializationQuickFixAvailabilityTests : QuickFixAvailabilityTestBase
    {
        protected override string RelativeTestDataPath => @"Analyzers\ArrayWithDefaultValuesInitializationQuickFixes";

        protected override bool HighlightingPredicate(IHighlighting highlighting, IPsiSourceFile sourceFile, IContextBoundSettingsStore settingsStore)
            => highlighting is ArrayWithDefaultValuesInitializationHighlighting;

        [Test]
        public void TestArrayWithDefaultValuesInitializationAvailability() => DoNamedTest2();
    }
}