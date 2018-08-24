using JetBrains.Application.Settings;
using JetBrains.ReSharper.Feature.Services.Daemon;
using JetBrains.ReSharper.Psi;
using NUnit.Framework;
using ReCommendedExtension.Highlightings;

namespace ReCommendedExtension.Tests.Analyzers
{
    [TestFixture]
    public sealed class LockOnObjectWithWeakIdentityAnalyzerTests : HighlightingTestBaseWithAnnotationAssemblyReference
    {
        protected override string RelativeTestDataPath => @"Analyzers\LockOnObjectWithWeakIdentity";

        protected override bool HighlightingPredicate(IHighlighting highlighting, IPsiSourceFile sourceFile, IContextBoundSettingsStore settingsStore)
            => highlighting is LockOnObjectWithWeakIdentityHighlighting;

        [Test]
        public void TestLockOnObjectWithWeakIdentity() => DoNamedTest2();
    }
}