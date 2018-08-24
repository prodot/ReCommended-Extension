using JetBrains.Application.Settings;
using JetBrains.ReSharper.Feature.Services.Daemon;
using JetBrains.ReSharper.Psi;
using NUnit.Framework;
using ReCommendedExtension.Highlightings;

namespace ReCommendedExtension.Tests.Analyzers
{
    [TestFixture]
    public sealed class DelegateInvokeAnalyzerTests : HighlightingTestBaseWithAnnotationAssemblyReference
    {
        protected override string RelativeTestDataPath => @"Analyzers\DelegateInvoke";

        protected override bool HighlightingPredicate(IHighlighting highlighting, IPsiSourceFile sourceFile, IContextBoundSettingsStore settingsStore)
            => highlighting is RedundantDelegateInvokeHighlighting;

        [Test]
        public void TestDelegateInvoke() => DoNamedTest2();
    }
}