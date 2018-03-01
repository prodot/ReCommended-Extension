using JetBrains.ReSharper.Feature.Services.Daemon;
using JetBrains.ReSharper.FeaturesTestFramework.Daemon;
using JetBrains.ReSharper.Psi;
using NUnit.Framework;
using ReCommendedExtension.Highlightings;

namespace ReCommendedExtension.Tests.Analyzers
{
    [TestFixture]
    public sealed class CatchClauseWithoutVariableAnalyzerTests : CSharpHighlightingTestBase
    {
        protected override string RelativeTestDataPath => @"Analyzers\CatchClauseWithoutVariable";

        protected override bool HighlightingPredicate(IHighlighting highlighting, IPsiSourceFile sourceFile)
            => highlighting is CatchClauseWithoutVariableHighlighting;

        [Test]
        public void TestCatchClauseWithoutVariable() => DoNamedTest2();
    }
}