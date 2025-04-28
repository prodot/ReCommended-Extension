using JetBrains.Application.Settings;
using JetBrains.ReSharper.Feature.Services.Daemon;
using JetBrains.ReSharper.FeaturesTestFramework.Intentions;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.TestFramework;
using NUnit.Framework;
using ReCommendedExtension.Analyzers.BaseTypes;

namespace ReCommendedExtension.Tests.Analyzers.BaseTypes.Decimal;

[TestFixture]
public sealed class QuickFixAvailabilityTests : QuickFixAvailabilityTestBase
{
    protected override string RelativeTestDataPath => @"Analyzers\BaseTypes\DecimalQuickFixes";

    protected override bool HighlightingPredicate(IHighlighting highlighting, IPsiSourceFile sourceFile, IContextBoundSettingsStore settingsStore)
        => highlighting is UseExpressionResultSuggestion or UseBinaryOperatorSuggestion or RedundantArgumentHint or UseUnaryOperatorSuggestion
            || highlighting.IsError();

    [Test]
    [TestNet70]
    public void TestUseExpressionResultFixAvailability() => DoNamedTest2();

    [Test]
    [TestNet70]
    public void TestUseBinaryOperatorFixAvailability() => DoNamedTest2();

    [Test]
    [TestNet80]
    public void TestRemoveArgumentFixAvailability() => DoNamedTest2();

    [Test]
    public void TestUseUnaryOperatorFixAvailability() => DoNamedTest2();
}