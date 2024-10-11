using JetBrains.Application.Settings;
using JetBrains.ReSharper.Feature.Services.Daemon;
using JetBrains.ReSharper.FeaturesTestFramework.Intentions;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.CSharp;
using JetBrains.ReSharper.TestFramework;
using NUnit.Framework;
using ReCommendedExtension.Analyzers.Linq;

namespace ReCommendedExtension.Tests.Analyzers.Linq;

[TestFixture]
public sealed class UseLinqQuickFixAvailabilityTests : QuickFixAvailabilityTestBase
{
    protected override string RelativeTestDataPath => @"Analyzers\LinqQuickFixes";

    protected override bool HighlightingPredicate(IHighlighting highlighting, IPsiSourceFile sourceFile, IContextBoundSettingsStore settingsStore)
        => highlighting is UseIndexerSuggestion or UseLinqListPatternSuggestion or UseSwitchExpressionSuggestion or UsePropertySuggestion;

    [Test]
    [TestNet60]
    [CSharpLanguageLevel(CSharpLanguageLevel.CSharp100)]
    public void TestUseIndexerAvailability() => DoNamedTest2();

    [Test]
    [TestNet70]
    [CSharpLanguageLevel(CSharpLanguageLevel.CSharp110)]
    public void TestUseIndexerWithListPatternAvailability() => DoNamedTest2();

    [Test]
    [TestNet70]
    [CSharpLanguageLevel(CSharpLanguageLevel.CSharp110)]
    public void TestUseListPatternAvailability() => DoNamedTest2();

    [Test]
    [TestNet70]
    [CSharpLanguageLevel(CSharpLanguageLevel.CSharp110)]
    public void TestUseSwitchExpressionAvailability() => DoNamedTest2();

    [Test]
    [CSharpLanguageLevel(CSharpLanguageLevel.CSharp90)]
    [TestNet50]
    public void TestUsePropertyAvailability() => DoNamedTest2();
}