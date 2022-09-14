using JetBrains.Application.Settings;
using JetBrains.ReSharper.Feature.Services.Daemon;
using JetBrains.ReSharper.FeaturesTestFramework.Daemon;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.TestFramework;
using NUnit.Framework;
using ReCommendedExtension.Analyzers.AsyncVoid;

namespace ReCommendedExtension.Tests.Analyzers
{
    [TestFixture]
    public sealed class AsyncVoidAnalyzerTests : CSharpHighlightingTestBase
    {
        protected override string RelativeTestDataPath => @"Analyzers\AsyncVoid";

        protected override bool HighlightingPredicate(IHighlighting highlighting, IPsiSourceFile sourceFile, IContextBoundSettingsStore settingsStore)
            => highlighting is AsyncVoidFunctionExpressionWarning || highlighting is AvoidAsyncVoidWarning;

        [Test]
        [TestNetFramework45]
        public void TestAnonymousMethod() => DoNamedTest2();

        [Test]
        [TestNetFramework45]
        public void TestLambdaExpression() => DoNamedTest2();

        [Test]
        [TestNetFramework45]
        public void TestAsyncVoidMethod() => DoNamedTest2();

        // [Test]
        // [TestNet60("Microsoft.WindowsAppSDK/1.1.4")]
        // public void TestAsyncVoidMethodXBind() => DoNamedTest2();
    }
}