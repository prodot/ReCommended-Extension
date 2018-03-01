using JetBrains.ReSharper.Feature.Services.Daemon;
using JetBrains.ReSharper.FeaturesTestFramework.Daemon;
using JetBrains.ReSharper.Psi;
using NUnit.Framework;
using ReCommendedExtension.Highlightings;

namespace ReCommendedExtension.Tests.Analyzers
{
    [TestFixture]
    public sealed class XamlBindingWithoutMode : XamlHighlightingTestBase
    {
        protected override string RelativeTestDataPath => @"Analyzers\XamlBindingWithoutMode";

        protected override bool HighlightingPredicate(IHighlighting highlighting, IPsiSourceFile sourceFile)
            => highlighting is XamlBindingWithoutModeHighlighting;

        [Test]
        public void TestXamlBindingWithoutMode() => DoNamedTest2();
    }
}