using JetBrains.Application.Settings;
using JetBrains.ReSharper.Feature.Services.Daemon;
using JetBrains.ReSharper.FeaturesTestFramework.Daemon;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.TestFramework;
using NUnit.Framework;
using ReCommendedExtension.Analyzers.ValueTask;

namespace ReCommendedExtension.Tests.Analyzers
{
    [TestNetCore30]
    [TestFixture]
    public sealed class ValueTaskAnalyzerTests : CSharpHighlightingTestBase
    {
        protected override string RelativeTestDataPath => @"Analyzers\ValueTask";

        protected override bool HighlightingPredicate(IHighlighting highlighting, IPsiSourceFile sourceFile, IContextBoundSettingsStore settingsStore)
            => highlighting is PossibleMultipleConsumptionWarning;

        [Test]
        public void TestCodePaths() => DoNamedTest2();

        [Test]
        public void TestSingleConsumption() => DoNamedTest2();

        [Test]
        public void TestMultipleConsumption() => DoNamedTest2();
    }
}