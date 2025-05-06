using JetBrains.Application.Settings;
using JetBrains.ReSharper.Feature.Services.Daemon;
using JetBrains.ReSharper.FeaturesTestFramework.Intentions;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.CSharp;
using JetBrains.ReSharper.TestFramework;
using NUnit.Framework;
using ReCommendedExtension.Analyzers.BaseTypes;

namespace ReCommendedExtension.Tests.Analyzers.BaseTypes.Int64;

[TestFixture]
[TestNet70]
public sealed class QuickFixAvailabilityTests : QuickFixAvailabilityTestBase
{
    protected override string RelativeTestDataPath => @"Analyzers\BaseTypes\Int64QuickFixes";

    protected override bool HighlightingPredicate(IHighlighting highlighting, IPsiSourceFile sourceFile, IContextBoundSettingsStore settingsStore)
        => highlighting is UseExpressionResultSuggestion
                or UseBinaryOperatorSuggestion
                or RedundantArgumentHint
                or RedundantFormatPrecisionSpecifierHint
            || highlighting.IsError();

    [Test]
    public void TestUseExpressionResultFixAvailability() => DoNamedTest2();

    [Test]
    public void TestUseBinaryOperatorFixAvailability() => DoNamedTest2();

    [Test]
    [TestNet80]
    public void TestRemoveArgumentFixAvailability() => DoNamedTest2();

    [Test]
    [CSharpLanguageLevel(CSharpLanguageLevel.CSharp110)]
    [TestNet80]
    public void TestRemoveFormatPrecisionSpecifierAvailability() => DoNamedTest2();
}