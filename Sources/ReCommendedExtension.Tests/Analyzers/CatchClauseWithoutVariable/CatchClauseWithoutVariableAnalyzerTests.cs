using JetBrains.ReSharper.Feature.Services.Daemon;
using NUnit.Framework;
using ReCommendedExtension.Analyzers.CatchClauseWithoutVariable;

namespace ReCommendedExtension.Tests.Analyzers.CatchClauseWithoutVariable;

[TestFixture]
public sealed class CatchClauseWithoutVariableAnalyzerTests : CSharpAnalyzerTests
{
    protected override string RelativeTestDataPath => @"Analyzers\CatchClauseWithoutVariable";

    protected override bool UseHighlighting(IHighlighting highlighting) => highlighting is CatchClauseWithoutVariableHint;

    [Test]
    public void TestCatchClauseWithoutVariable() => DoNamedTest2();
}