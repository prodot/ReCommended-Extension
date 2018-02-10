using JetBrains.ReSharper.Feature.Services.Daemon;
using JetBrains.ReSharper.Psi;
using NUnit.Framework;
using ReCommendedExtension.Highlightings;

namespace ReCommendedExtension.Tests.Analyzers
{
    [TestFixture]
    public sealed class ArrayWithDefaultValuesInitializationQuickFixAvailabilityTests : QuickFixAvailabilityTestBaseWithAnnotationAssemblyReference
    {
        protected override string RelativeTestDataPath => @"Analyzers\ArrayWithDefaultValuesInitializationQuickFixes";

        protected override bool HighlightingPredicate(IHighlighting highlighting, IPsiSourceFile sourceFile)
            => highlighting is ArrayWithDefaultValuesInitializationHighlighting;

        [Test]
        public void TestArrayWithDefaultValuesInitializationAvailability() => DoNamedTest2();
    }
}