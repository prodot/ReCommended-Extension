using JetBrains.Application.Settings;
using JetBrains.ReSharper.Feature.Services.Daemon;
using JetBrains.ReSharper.FeaturesTestFramework.Intentions;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.CSharp;
using JetBrains.ReSharper.TestFramework;
using NUnit.Framework;
using ReCommendedExtension.Analyzers.BaseTypes;

namespace ReCommendedExtension.Tests.Analyzers.BaseTypes.TimeSpan;

[TestFixture]
public sealed class QuickFixAvailabilityTests : QuickFixAvailabilityTestBase
{
    protected override string RelativeTestDataPath => @"Analyzers\BaseTypes\TimeSpan\QuickFixes";

    protected override bool HighlightingPredicate(IHighlighting highlighting, IPsiSourceFile sourceFile, IContextBoundSettingsStore settingsStore)
        => highlighting is UseExpressionResultSuggestion or UseBinaryOperatorSuggestion or UseUnaryOperatorSuggestion or UseOtherArgumentSuggestion
            || highlighting.IsError();

    [Test]
    [CSharpLanguageLevel(CSharpLanguageLevel.CSharp90)]
    [TestNet90]
    public void TestUseExpressionResultFixAvailability() => DoNamedTest2();

    [Test]
    [TestNetCore20]
    public void TestUseBinaryOperatorFixAvailability() => DoNamedTest2();

    [Test]
    public void TestUseUnaryOperatorFixAvailability() => DoNamedTest2();

    [Test]
    [CSharpLanguageLevel(CSharpLanguageLevel.CSharp120)]
    [TestNetCore21]
    public void TestUseOtherArgumentFixAvailability() => DoNamedTest2();
}