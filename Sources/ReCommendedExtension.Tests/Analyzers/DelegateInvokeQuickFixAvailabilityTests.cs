using JetBrains.ReSharper.Feature.Services.Daemon;
using JetBrains.ReSharper.Psi;
using NUnit.Framework;
using ReCommendedExtension.Highlightings;

namespace ReCommendedExtension.Tests.Analyzers
{
    [TestFixture]
    public sealed class DelegateInvokeQuickFixAvailabilityTests : QuickFixAvailabilityTestBaseWithAnnotationAssemblyReference
    {
        protected override string RelativeTestDataPath => @"Analyzers\DelegateInvokeQuickFixes";

        protected override bool HighlightingPredicate(IHighlighting highlighting, IPsiSourceFile sourceFile)
            => highlighting is RedundantDelegateInvokeHighlighting;

        [Test]
        public void TestDelegateInvokeAvailability() => DoNamedTest2();
    }
}