using JetBrains.Application.Settings;
using JetBrains.ReSharper.Feature.Services.Daemon;
using JetBrains.ReSharper.FeaturesTestFramework.Daemon;
using JetBrains.ReSharper.Psi;
using NUnit.Framework;
using ReCommendedExtension.Analyzers.CatchClauseWithoutVariable;

namespace ReCommendedExtension.Tests.Analyzers.CatchClauseWithoutVariable;

[TestFixture]
public sealed class CatchClauseWithoutVariableAnalyzerTests : CSharpHighlightingTestBase
{
    protected override string RelativeTestDataPath => @"Analyzers\CatchClauseWithoutVariable";

    protected override bool HighlightingPredicate(IHighlighting highlighting, IPsiSourceFile sourceFile, IContextBoundSettingsStore settingsStore)
        => highlighting is CatchClauseWithoutVariableHint || highlighting.IsError();

    [Test]
    public void TestCatchClauseWithoutVariable() => DoNamedTest2();
}