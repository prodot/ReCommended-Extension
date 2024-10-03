using JetBrains.Application.Settings;
using JetBrains.ReSharper.Daemon.CSharp.Errors;
using JetBrains.ReSharper.Feature.Services.Daemon;
using JetBrains.ReSharper.FeaturesTestFramework.Daemon;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.CSharp;
using JetBrains.ReSharper.TestFramework;
using NUnit.Framework;
using ReCommendedExtension.Analyzers.Linq;

namespace ReCommendedExtension.Tests.Analyzers.Linq;

[TestFixture]
public sealed class LinqAnalyzerTests : CSharpHighlightingTestBase
{
    protected override string RelativeTestDataPath => @"Analyzers\Linq";

    protected override bool HighlightingPredicate(IHighlighting highlighting, IPsiSourceFile sourceFile, IContextBoundSettingsStore settingsStore)
        => highlighting is UseIndexerSuggestion
            or UseListPatternSuggestion
            or UseSwitchExpressionSuggestion
            or UsePropertySuggestion
            or SuspiciousElementAccessWarning
            or UseCollectionCountPropertyWarning; // to figure out which cases are supported by R#

    [Test]
    [CSharpLanguageLevel(CSharpLanguageLevel.CSharp100)]
    [TestNet60]
    public void TestElementAt() => DoNamedTest2();

    [Test]
    [CSharpLanguageLevel(CSharpLanguageLevel.CSharp100)]
    [TestNet60]
    public void TestElementAtOrDefault() => DoNamedTest2();

    [Test]
    [CSharpLanguageLevel(CSharpLanguageLevel.CSharp90)]
    [TestNet50]
    public void TestFirst() => DoNamedTest2();

    [Test]
    [CSharpLanguageLevel(CSharpLanguageLevel.CSharp110)]
    [TestNet70]
    public void TestFirstOrDefault() => DoNamedTest2();

    [Test]
    [CSharpLanguageLevel(CSharpLanguageLevel.CSharp90)]
    [TestNet50]
    public void TestLast() => DoNamedTest2();

    [Test]
    [CSharpLanguageLevel(CSharpLanguageLevel.CSharp110)]
    [TestNet70]
    public void TestLastOrDefault() => DoNamedTest2();

    [Test]
    [CSharpLanguageLevel(CSharpLanguageLevel.CSharp90)]
    [TestNet50]
    public void TestLongCount() => DoNamedTest2();

    [Test]
    [CSharpLanguageLevel(CSharpLanguageLevel.CSharp110)]
    [TestNet70]
    public void TestSingle() => DoNamedTest2();

    [Test]
    [CSharpLanguageLevel(CSharpLanguageLevel.CSharp110)]
    [TestNet70]
    public void TestSingleOrDefault() => DoNamedTest2();
}