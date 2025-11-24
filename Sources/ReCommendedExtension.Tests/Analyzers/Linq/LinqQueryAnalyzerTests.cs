using JetBrains.Application.Settings;
using JetBrains.ReSharper.Feature.Services.Daemon;
using JetBrains.ReSharper.FeaturesTestFramework.Daemon;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.CSharp;
using JetBrains.ReSharper.TestFramework;
using NUnit.Framework;
using ReCommendedExtension.Analyzers.Linq;

namespace ReCommendedExtension.Tests.Analyzers.Linq;

[TestFixture]
[CSharpLanguageLevel(CSharpLanguageLevel.CSharp120)]
public sealed class LinqQueryAnalyzerTests : CSharpHighlightingTestBase
{
    protected override string RelativeTestDataPath => @"Analyzers\Linq";

    protected override bool HighlightingPredicate(IHighlighting highlighting, IPsiSourceFile sourceFile, IContextBoundSettingsStore settingsStore)
        => highlighting is RedundantLinqQueryHint or { IsError: true };

    [Test]
    public void TestNoOpQuery() => DoNamedTest2();
}