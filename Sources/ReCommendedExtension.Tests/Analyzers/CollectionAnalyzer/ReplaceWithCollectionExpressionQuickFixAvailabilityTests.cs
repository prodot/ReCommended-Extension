using JetBrains.Application.Settings;
using JetBrains.ReSharper.Feature.Services.Daemon;
using JetBrains.ReSharper.FeaturesTestFramework.Intentions;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.CSharp;
using JetBrains.ReSharper.TestFramework;
using NUnit.Framework;
using ReCommendedExtension.Analyzers.Collection;

namespace ReCommendedExtension.Tests.Analyzers.CollectionAnalyzer;

[CSharpLanguageLevel(CSharpLanguageLevel.CSharp120)]
[TestNet80]
[TestFixture]
public sealed class ReplaceWithCollectionExpressionQuickFixAvailabilityTests : QuickFixAvailabilityTestBase
{
    protected override string RelativeTestDataPath => @"Analyzers\CollectionQuickFixes";

    protected override bool HighlightingPredicate(IHighlighting highlighting, IPsiSourceFile sourceFile, IContextBoundSettingsStore settingsStore)
        => highlighting is UseTargetTypedCollectionExpressionSuggestion;

    [Test]
    public void TestCollectionInitialization_Array_Target_Array_Availability() => DoNamedTest2();

    [Test]
    public void TestCollectionInitialization_Array_Target_IEnumerable_Availability() => DoNamedTest2();
}