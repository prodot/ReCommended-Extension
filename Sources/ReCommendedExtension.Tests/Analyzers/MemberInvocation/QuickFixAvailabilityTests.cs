using JetBrains.Application.Settings;
using JetBrains.ProjectModel.Properties.CSharp;
using JetBrains.ReSharper.Feature.Services.Daemon;
using JetBrains.ReSharper.FeaturesTestFramework.Intentions;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.CSharp;
using JetBrains.ReSharper.TestFramework;
using NUnit.Framework;
using ReCommendedExtension.Analyzers.MemberInvocation;

namespace ReCommendedExtension.Tests.Analyzers.MemberInvocation;

[TestFixture]
public sealed class QuickFixAvailabilityTests : QuickFixAvailabilityTestBase
{
    protected override string RelativeTestDataPath => @"Analyzers\MemberInvocation\QuickFixes";

    protected override bool HighlightingPredicate(IHighlighting highlighting, IPsiSourceFile sourceFile, IContextBoundSettingsStore settingsStore)
        => highlighting is RedundantMethodInvocationHint
                or UseOtherMethodSuggestion
                or UseBinaryOperatorSuggestion
                or UseUnaryOperatorSuggestion
                or UsePatternSuggestion
                or UseNullableHasValueAlternativeSuggestion
                or ReplaceNullableValueWithTypeCastSuggestion
            || highlighting.IsError();

    [Test]
    [TestNetCore21]
    public void TestRemoveMethodInvocationFixAvailability() => DoNamedTest2();

    [Test]
    [CSharpLanguageLevel(CSharpLanguageLevel.CSharp120)]
    [NullableContext(NullableContextKind.Enable)]
    [TestNetCore21]
    public void TestUseOtherMethodFixAvailability() => DoNamedTest2();

    [Test]
    [CSharpLanguageLevel(CSharpLanguageLevel.CSharp130)]
    [TestNet70]
    public void TestUseBinaryOperatorFixAvailability() => DoNamedTest2();

    [Test]
    [CSharpLanguageLevel(CSharpLanguageLevel.CSharp130)]
    [TestNetCore21]
    public void TestUseUnaryOperatorFixAvailability() => DoNamedTest2();

    [Test]
    [CSharpLanguageLevel(CSharpLanguageLevel.CSharp110)]
    [NullableContext(NullableContextKind.Enable)]
    [TestNet70]
    public void TestUsePatternFixAvailability() => DoNamedTest2();

    [Test]
    [CSharpLanguageLevel(CSharpLanguageLevel.CSharp90)]
    [TestNetCore20]
    public void TestUseNullableHasValueAlternativeFixAvailability() => DoNamedTest2();

    [Test]
    public void TestReplaceNullableValueWithTypeCastFixAvailability() => DoNamedTest2();
}