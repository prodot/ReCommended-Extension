using JetBrains.Application.Settings;
using JetBrains.ReSharper.Daemon.CSharp.Errors;
using JetBrains.ReSharper.Feature.Services.Daemon;
using JetBrains.ReSharper.FeaturesTestFramework.Intentions;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.CSharp;
using JetBrains.ReSharper.TestFramework;
using NUnit.Framework;
using ReCommendedExtension.Analyzers.BaseTypes;

namespace ReCommendedExtension.Tests.Analyzers.BaseTypes.Char;

[TestFixture]
public sealed class QuickFixAvailabilityTests : QuickFixAvailabilityTestBase
{
    protected override string RelativeTestDataPath => @"Analyzers\BaseTypes\CharQuickFixes";

    protected override bool HighlightingPredicate(IHighlighting highlighting, IPsiSourceFile sourceFile, IContextBoundSettingsStore settingsStore)
        => highlighting is UseBinaryOperationSuggestion
            or UseExpressionResultSuggestion
            or UseCharRangePatternSuggestion
            or RedundantArgumentHint
            or NotResolvedError;

    [Test]
    public void TestUseExpressionResultFixAvailability() => DoNamedTest2();

    [Test]
    public void TestUseBinaryOperatorFixAvailability() => DoNamedTest2();

    [Test]
    [CSharpLanguageLevel(CSharpLanguageLevel.CSharp90)]
    [TestNet70]
    public void TestUseCharRangePatternFixAvailability() => DoNamedTest2();

    [Test]
    public void TestRemoveArgumentFixAvailability() => DoNamedTest2();
}