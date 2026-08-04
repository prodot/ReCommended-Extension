using JetBrains.ProjectModel.Properties.CSharp;
using JetBrains.ReSharper.Feature.Services.Daemon;
using JetBrains.ReSharper.TestFramework;
using NUnit.Framework;
using ReCommendedExtension.Analyzers.ControlFlow;

namespace ReCommendedExtension.Tests.Analyzers.ControlFlow;

[TestFixture]
public sealed class ControlFlowAnalyzerTests : CSharpAnalyzerTests
{
    protected override string RelativeTestDataPath => @"Analyzers\ControlFlow";

    protected override bool UseHighlighting(IHighlighting highlighting)
        => highlighting is RedundantAssertionStatementSuggestion or RedundantInlineAssertionSuggestion;

    [Test]
    public void ControlFlow() => DoNamedTest();

    [Test]
    [NullableContext(NullableContextKind.Enable)]
    [TestNetCore30(ANNOTATIONS_PACKAGE)]
    [TestCompilationSymbols("DEBUG")]
    public void ControlFlow_NullableContext() => DoNamedTest();
}