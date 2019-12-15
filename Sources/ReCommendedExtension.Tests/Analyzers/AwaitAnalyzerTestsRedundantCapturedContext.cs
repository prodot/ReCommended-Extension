using JetBrains.Application.Settings;
using JetBrains.ReSharper.Feature.Services.Daemon;
using JetBrains.ReSharper.FeaturesTestFramework.Daemon;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.TestFramework;
using NUnit.Framework;
using ReCommendedExtension.Analyzers.Await;

namespace ReCommendedExtension.Tests.Analyzers
{
    [TestFixture]
    public sealed class AwaitAnalyzerTestsRedundantCapturedContext : CSharpHighlightingTestBase
    {
        protected override string RelativeTestDataPath => @"Analyzers\Await";

        protected override bool HighlightingPredicate(IHighlighting highlighting, IPsiSourceFile sourceFile, IContextBoundSettingsStore settingsStore)
            => highlighting is RedundantCapturedContextHighlighting;

        [TestNetFramework45]
        [Test]
        public void TestRedundantCapturedContext() => DoNamedTest2();

        [TestNetCore21]
        [Test]
        public void TestRedundantCapturedContext_ValueTask() => DoNamedTest2();

        [TestNetCore21]
        [Test]
        public void TestRedundantCapturedContext_ReturnValueTask() => DoNamedTest2();

        [TestNetCore21]
        [Test]
        public void TestRedundantCapturedContext_ValueTask_ReturnTask() => DoNamedTest2();

        [TestNetCore30]
        [Test]
        public void TestRedundantCapturedContext_IAsyncTypes() => DoNamedTest2();
    }
}