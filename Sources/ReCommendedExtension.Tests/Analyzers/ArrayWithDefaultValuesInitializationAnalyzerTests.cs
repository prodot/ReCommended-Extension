using JetBrains.Application.Settings;
using JetBrains.ReSharper.Feature.Services.Daemon;
using JetBrains.ReSharper.FeaturesTestFramework.Daemon;
using JetBrains.ReSharper.Psi;
using NUnit.Framework;
using ReCommendedExtension.Analyzers.ArrayWithDefaultValuesInitialization;

namespace ReCommendedExtension.Tests.Analyzers
{
    [TestFixture]
    public sealed class ArrayWithDefaultValuesInitializationAnalyzerTests : CSharpHighlightingTestBase
    {
        protected override string RelativeTestDataPath => @"Analyzers\ArrayWithDefaultValuesInitialization";

        protected override bool HighlightingPredicate(IHighlighting highlighting, IPsiSourceFile sourceFile, IContextBoundSettingsStore settingsStore)
            => highlighting is ArrayWithDefaultValuesInitializationHighlighting;

        [Test]
        public void TestArrayWithDefaultValuesInitialization() => DoNamedTest2();
    }
}