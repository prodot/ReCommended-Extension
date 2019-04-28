using JetBrains.Application.Settings;
using JetBrains.ReSharper.Feature.Services.Daemon;
using JetBrains.ReSharper.FeaturesTestFramework.Daemon;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.TestFramework;
using NUnit.Framework;
using ReCommendedExtension.Analyzers.Await;

namespace ReCommendedExtension.Tests.Analyzers
{
    [TestNetFramework45]
    [TestPackages("MSTest.TestFramework/1.3.2")]
    [TestFixture]
    public sealed class AwaitAnalyzerTestsRedundantAwaitForMsTestProjects : CSharpHighlightingTestBase
    {
        protected override string RelativeTestDataPath => @"Analyzers\Await";

        protected override bool HighlightingPredicate(IHighlighting highlighting, IPsiSourceFile sourceFile, IContextBoundSettingsStore settingsStore)
            => highlighting is RedundantAwaitHighlighting;

        [Test]
        public void TestRedundantAwait_TestProject() => DoNamedTest2();
    }
}