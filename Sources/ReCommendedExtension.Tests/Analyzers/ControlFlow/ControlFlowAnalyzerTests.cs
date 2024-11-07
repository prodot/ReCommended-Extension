using JetBrains.Application.Settings;
using JetBrains.ProjectModel.Properties.CSharp;
using JetBrains.ReSharper.Feature.Services.Daemon;
using JetBrains.ReSharper.FeaturesTestFramework.Daemon;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.TestFramework;
using NUnit.Framework;
using ReCommendedExtension.Analyzers.ControlFlow;

namespace ReCommendedExtension.Tests.Analyzers.ControlFlow;

[TestFixture]
public sealed class ControlFlowAnalyzerTests : CSharpHighlightingTestBase
{
    protected override string RelativeTestDataPath => @"Analyzers\ControlFlow";

    protected override bool HighlightingPredicate(IHighlighting highlighting, IPsiSourceFile sourceFile, IContextBoundSettingsStore settingsStore)
        => highlighting is RedundantAssertionSuggestion;

    [Test]
    public void TestControlFlow() => DoNamedTest2();

    [Test]
    [NullableContext(NullableContextKind.Enable)]
    [TestNetCore30(ANNOTATIONS_PACKAGE)]
    [TestCompilationSymbols("DEBUG")]
    public void TestControlFlow_NullableContext() => DoNamedTest2();
}