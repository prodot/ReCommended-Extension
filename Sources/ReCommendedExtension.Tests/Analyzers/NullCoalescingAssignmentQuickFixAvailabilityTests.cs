using JetBrains.Application.Settings;
using JetBrains.ReSharper.Feature.Services.Daemon;
using JetBrains.ReSharper.FeaturesTestFramework.Intentions;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.CSharp;
using JetBrains.ReSharper.TestFramework;
using NUnit.Framework;
using ReCommendedExtension.Analyzers.NullCoalescingAssignment;

namespace ReCommendedExtension.Tests.Analyzers
{
    [TestFixture]
    [CSharpLanguageLevel(CSharpLanguageLevel.CSharp80)]
    public sealed class NullCoalescingAssignmentQuickFixAvailabilityTests : QuickFixAvailabilityTestBase
    {
        protected override string RelativeTestDataPath => @"Analyzers\NullCoalescingAssignmentQuickFixes";

        protected override bool HighlightingPredicate(IHighlighting highlighting, IPsiSourceFile sourceFile, IContextBoundSettingsStore settingsStore)
            => highlighting is NullCoalescingAssignmentSuggestion;

        [Test]
        public void TestAvailability() => DoNamedTest2();
    }
}