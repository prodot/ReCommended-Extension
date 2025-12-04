using JetBrains.ReSharper.Feature.Services.Daemon;
using JetBrains.ReSharper.Psi.CSharp;
using JetBrains.ReSharper.TestFramework;
using NUnit.Framework;
using ReCommendedExtension.Analyzers.Collection;

namespace ReCommendedExtension.Tests.Analyzers.Collection;

[TestFixture]
[CSharpLanguageLevel(CSharpLanguageLevel.CSharp120)]
[TestNet80]
public sealed class ReplaceWithCollectionExpressionQuickFixAvailabilityTests : QuickFixAvailabilityTests
{
    protected override string RelativeTestDataPath => @"Analyzers\Collection\QuickFixes";

    protected override bool UseHighlighting(IHighlighting highlighting) => highlighting is UseTargetTypedCollectionExpressionSuggestion;

    [Test]
    public void TestCollectionInitialization_Array_Target_Array_Availability() => DoNamedTest2();

    [Test]
    public void TestCollectionInitialization_Array_Target_IEnumerable_Availability() => DoNamedTest2();
}