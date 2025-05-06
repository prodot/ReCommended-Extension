using JetBrains.Application.Settings;
using JetBrains.ReSharper.Feature.Services.Daemon;
using JetBrains.ReSharper.FeaturesTestFramework.Intentions;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.CSharp;
using JetBrains.ReSharper.TestFramework;
using NUnit.Framework;
using ReCommendedExtension.Analyzers.BaseTypes;

namespace ReCommendedExtension.Tests.Analyzers.BaseTypes.Double;

[TestFixture]
public sealed class QuickFixAvailabilityTests : QuickFixAvailabilityTestBase
{
    protected override string RelativeTestDataPath => @"Analyzers\BaseTypes\DoubleQuickFixes";

    protected override bool HighlightingPredicate(IHighlighting highlighting, IPsiSourceFile sourceFile, IContextBoundSettingsStore settingsStore)
        => highlighting is UseExpressionResultSuggestion
                or RedundantArgumentHint
                or UseFloatingPointPatternSuggestion
                or PassOtherFormatSpecifierSuggestion
                or RedundantFormatPrecisionSpecifierHint
            || highlighting.IsError();

    [Test]
    public void TestUseExpressionResultFixAvailability() => DoNamedTest2();

    [Test]
    [TestNet80]
    public void TestRemoveArgumentFixAvailability() => DoNamedTest2();

    [Test]
    [CSharpLanguageLevel(CSharpLanguageLevel.CSharp90)]
    public void TestUseFloatingPointPatternFixAvailability() => DoNamedTest2();

    [Test]
    [CSharpLanguageLevel(CSharpLanguageLevel.CSharp110)]
    public void TestRemoveFormatPrecisionSpecifierAvailability() => DoNamedTest2();

    [Test]
    public void TestPassOtherFormatSpecifierAvailability() => DoNamedTest2();
}