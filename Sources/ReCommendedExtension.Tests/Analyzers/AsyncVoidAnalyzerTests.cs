using JetBrains.ReSharper.Feature.Services.Daemon;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.TestFramework;
using NUnit.Framework;
using ReCommendedExtension.Highlightings;

namespace ReCommendedExtension.Tests.Analyzers
{
    [TestNetFramework45]
    [TestFixture]
    public sealed class AsyncVoidAnalyzerTests : HighlightingTestBaseWithAnnotationAssemblyReference
    {
        protected override string RelativeTestDataPath => @"Analyzers\AsyncVoid";

        protected override bool HighlightingPredicate(IHighlighting highlighting, IPsiSourceFile sourceFile)
            => highlighting is AsyncVoidFunctionExpressionHighlighting || highlighting is AvoidAsyncVoidHighlighting;

        [Test]
        public void TestAnonymousMethod() => DoNamedTest2();

        [Test]
        public void TestLambdaExpression() => DoNamedTest2();

        [Test]
        public void TestAsyncVoidMethod() => DoNamedTest2();
    }
}