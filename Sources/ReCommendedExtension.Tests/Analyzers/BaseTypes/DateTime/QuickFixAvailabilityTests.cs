using JetBrains.Application.Settings;
using JetBrains.ReSharper.Feature.Services.Daemon;
using JetBrains.ReSharper.FeaturesTestFramework.Intentions;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.CSharp;
using JetBrains.ReSharper.TestFramework;
using NUnit.Framework;
using ReCommendedExtension.Analyzers.BaseTypes;

namespace ReCommendedExtension.Tests.Analyzers.BaseTypes.DateTime;

[TestFixture]
public sealed class QuickFixAvailabilityTests : QuickFixAvailabilityTestBase
{
    protected override string RelativeTestDataPath => @"Analyzers\BaseTypes\DateTime\QuickFixes";

    protected override bool HighlightingPredicate(IHighlighting highlighting, IPsiSourceFile sourceFile, IContextBoundSettingsStore settingsStore)
        => highlighting is UseExpressionResultSuggestion
                or RedundantArgumentHint
                or RedundantArgumentRangeHint
                or UseDateTimePropertySuggestion
                or UseBinaryOperatorSuggestion
                or UseOtherArgumentSuggestion
                or RedundantElementHint
                or RedundantMethodInvocationHint
            || highlighting.IsError();

    [Test]
    [CSharpLanguageLevel(CSharpLanguageLevel.CSharp90)]
    public void TestUseExpressionResultFixAvailability() => DoNamedTest2();

    [Test]
    [CSharpLanguageLevel(CSharpLanguageLevel.CSharp90)]
    [TestNet80]
    public void TestRemoveArgumentFixAvailability() => DoNamedTest2();

    [Test]
    [CSharpLanguageLevel(CSharpLanguageLevel.CSharp90)]
    public void TestRemoveArgumentRangeFixAvailability() => DoNamedTest2();

    [Test]
    [CSharpLanguageLevel(CSharpLanguageLevel.CSharp60)]
    public void TestUseDateTimePropertyFixAvailability() => DoNamedTest2();

    [Test]
    public void TestUseBinaryOperatorFixAvailability() => DoNamedTest2();

    [Test]
    [CSharpLanguageLevel(CSharpLanguageLevel.CSharp120)]
    [TestNetCore21]
    public void TestUseOtherArgumentFixAvailability() => DoNamedTest2();

    [Test]
    [CSharpLanguageLevel(CSharpLanguageLevel.CSharp120)]
    [TestNetCore21]
    public void TestRemoveElementFixAvailability() => DoNamedTest2();

    [Test]
    public void TestRemoveMethodInvocationFixAvailability() => DoNamedTest2();
}