using JetBrains.Application.Settings;
using JetBrains.ReSharper.Feature.Services.Daemon;
using JetBrains.ReSharper.FeaturesTestFramework.Daemon;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.CSharp;
using JetBrains.ReSharper.TestFramework;
using NUnit.Framework;
using ReCommendedExtension.Analyzers.NullCoalescingAssignment;

namespace ReCommendedExtension.Tests.Analyzers
{
    [TestFixture]
    public sealed class NullCoalescingAssignmentAnalyzerTests : CSharpHighlightingTestBase
    {
        protected override string RelativeTestDataPath => @"Analyzers\NullCoalescingAssignment";

        protected override bool HighlightingPredicate(IHighlighting highlighting, IPsiSourceFile sourceFile, IContextBoundSettingsStore settingsStore)
            => highlighting is NullCoalescingAssignmentSuggestion;

        [Test]
        [CSharpLanguageLevel(CSharpLanguageLevel.CSharp80)]
        public void TestNullCoalescingAssignment() => DoNamedTest2();

        [Test]
        [CSharpLanguageLevel(CSharpLanguageLevel.CSharp73)]
        public void TestNullCoalescingAssignmentUnavailable() => DoNamedTest2();
    }
}