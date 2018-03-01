using JetBrains.ReSharper.Feature.Services.Daemon;
using JetBrains.ReSharper.Psi;
using NUnit.Framework;
using ReCommendedExtension.Highlightings;

namespace ReCommendedExtension.Tests.Analyzers
{
    [TestFixture]
    public sealed class
        NotifyPropertyChangedInvocatorFromConstructorQuickFixAvailabilityTests : QuickFixAvailabilityTestBaseWithAnnotationAssemblyReference
    {
        protected override string RelativeTestDataPath => @"Analyzers\NotifyPropertyChangedInvocatorFromConstructorQuickFixes";

        protected override bool HighlightingPredicate(IHighlighting highlighting, IPsiSourceFile sourceFile)
            => highlighting is NotifyPropertyChangedInvocatorFromConstructorHighlighting;

        [Test]
        public void TestNotifyPropertyChangedInvocatorFromConstructorAvailability() => DoNamedTest2();
    }
}