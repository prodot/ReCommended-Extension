using JetBrains.Application.Settings;
using JetBrains.ReSharper.Feature.Services.Daemon;
using JetBrains.ReSharper.FeaturesTestFramework.Daemon;
using JetBrains.ReSharper.Psi;
using NUnit.Framework;
using ReCommendedExtension.Analyzers.Region;

namespace ReCommendedExtension.Tests.Analyzers
{
    [TestFixture]
    public sealed class RegionAnalyzerTests : CSharpHighlightingTestBase
    {
        protected override string RelativeTestDataPath => @"Analyzers\Region";

        protected override bool HighlightingPredicate(IHighlighting highlighting, IPsiSourceFile sourceFile, IContextBoundSettingsStore settingsStore)
            => highlighting is RegionHighlighting;

        [Test]
        public void TestEmptyRegion() => DoNamedTest2();

        [Test]
        public void TestRegionWithSingleElement() => DoNamedTest2();

        [Test]
        public void TestRegionWithinTypeMemberBody() => DoNamedTest2();
    }
}