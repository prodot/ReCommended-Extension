using JetBrains.Application.Settings;
using JetBrains.ReSharper.Feature.Services.Daemon;
using JetBrains.ReSharper.FeaturesTestFramework.Intentions;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.CSharp;
using JetBrains.ReSharper.TestFramework;
using NUnit.Framework;
using ReCommendedExtension.Analyzers.BaseTypes;

namespace ReCommendedExtension.Tests.Analyzers.BaseTypes.DateTimeOffset;

[TestFixture]
public sealed class QuickFixAvailabilityTests : QuickFixAvailabilityTestBase
{
    protected override string RelativeTestDataPath => @"Analyzers\BaseTypes\DateTimeOffset\QuickFixes";

    protected override bool HighlightingPredicate(IHighlighting highlighting, IPsiSourceFile sourceFile, IContextBoundSettingsStore settingsStore)
        => highlighting is RedundantMethodInvocationHint
                or UseBinaryOperatorSuggestion
                or UseExpressionResultSuggestion
                or UseOtherArgumentSuggestion
                or RedundantElementHint
            || highlighting.IsError();

    [Test]
    public void TestRemoveMethodInvocationFixAvailability() => DoNamedTest2();

    [Test]
    public void TestUseBinaryOperatorFixAvailability() => DoNamedTest2();

    [Test]
    public void TestUseExpressionResultFixAvailability() => DoNamedTest2();

    [Test]
    [CSharpLanguageLevel(CSharpLanguageLevel.CSharp120)]
    [TestNetCore21]
    public void TestUseOtherArgumentFixAvailability() => DoNamedTest2();

    [Test]
    [CSharpLanguageLevel(CSharpLanguageLevel.CSharp120)]
    [TestNetCore21]
    public void TestRemoveElementFixAvailability() => DoNamedTest2();
}