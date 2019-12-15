using JetBrains.Application.Settings;
using JetBrains.ProjectModel.Properties.CSharp;
using JetBrains.ReSharper.Feature.Services.Daemon;
using JetBrains.ReSharper.FeaturesTestFramework.Daemon;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.CSharp;
using JetBrains.ReSharper.TestFramework;
using NUnit.Framework;
using ReCommendedExtension.Analyzers.ControlFlow;

namespace ReCommendedExtension.Tests.Analyzers
{
    [TestFixture]
    public sealed class ControlFlowAnalyzerTests : CSharpHighlightingTestBase
    {
        protected override string RelativeTestDataPath => @"Analyzers\ControlFlow";

        protected override bool HighlightingPredicate(IHighlighting highlighting, IPsiSourceFile sourceFile, IContextBoundSettingsStore settingsStore)
            => highlighting is RedundantAssertionHighlighting;

        [Test]
        public void TestControlFlow() => DoNamedTest2();

        [NullableContext(NullableContextKind.Enable)]
        [CSharpLanguageLevel(CSharpLanguageLevel.CSharp80)]
        [TestNetCore30("JetBrains.Annotations")]
        [Test]
        public void TestControlFlow_NullableContext() => DoNamedTest2();
    }
}